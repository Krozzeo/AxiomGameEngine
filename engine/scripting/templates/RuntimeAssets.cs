using System.Text.Json;
using System.Text.Json.Nodes;
namespace Axiom.Gameplay {
internal sealed class RuntimeAssetState {
 internal readonly Object Value;internal readonly bool Owned;internal readonly int Generation=Context.Generation;internal bool Live=true;internal string Name;
 internal RuntimeAssetState(Object value,bool owned,string name){Value=value;Owned=owned;Name=name;}
}
internal static class RuntimeAssets {
 internal const string Prefix="axiom://runtime/assets/";
 private static readonly Dictionary<int,RuntimeAssetState> Objects=new();
 private static readonly Dictionary<(Type,string),Object> Loaded=new();
 private static readonly Dictionary<Entity,Mesh?> MeshBindings=new(),ColliderBindings=new();
 internal static bool IsAsset(Object value)=>value is ProjectAsset or TextAsset;
 internal static RuntimeAssetState Register(Object value,bool owned=false,string? name=null){
  if(!IsAsset(value))throw new ArgumentException("A supported asset is required");
  if(value.AssetState is {} existing){if(!value)throw new UnityEngine.MissingReferenceException("The runtime asset is no longer alive");return existing;}
  if(Objects.Count>=512)throw new InvalidOperationException("Runtime asset capacity exceeded");
  var state=new RuntimeAssetState(value,owned,name??(value is ProjectAsset asset?asset.name:((TextAsset)value).name));value.AssetState=state;Objects.Add(value.GetInstanceID(),state);return state;
 }
 internal static string Name(Object value){if(!value)throw new UnityEngine.MissingReferenceException("Asset has been destroyed");return value.AssetState?.Name??(value is ProjectAsset asset?asset.name:((TextAsset)value).name);}
 internal static void Name(Object value,string name){ArgumentNullException.ThrowIfNull(name);if(name.Length>4096)throw new ArgumentException("Runtime asset name exceeds4096 characters");Register(value).Name=name;}
 internal static string Reference(Object value){Register(value);return Prefix+value.GetInstanceID().ToString(System.Globalization.CultureInfo.InvariantCulture);}
 internal static Object? Resolve(string path)=>path.StartsWith(Prefix,StringComparison.Ordinal)&&int.TryParse(path.AsSpan(Prefix.Length),out var id)&&Objects.TryGetValue(id,out var state)&&state.Value?state.Value:null;
 internal static Object[] Owned(Type type)=>Objects.Values.Where(state=>state.Owned&&state.Value&&type.IsInstanceOfType(UnityEngine.Object.Wrap(state.Value))).Select(state=>state.Value).ToArray();
 internal static void Reset(){foreach(var state in Objects.Values)state.Live=false;Objects.Clear();Loaded.Clear();MeshBindings.Clear();ColliderBindings.Clear();}
 internal static void BindPrimitive(Entity entity,Mesh mesh,bool collider){Register(mesh);MeshBindings[entity]=mesh;if(collider)ColliderBindings[entity]=mesh;}
 internal static Mesh? MeshOf(Entity entity,bool collider=false){var bindings=collider?ColliderBindings:MeshBindings;if(bindings.TryGetValue(entity,out var value))return value?value:null;var data=Context.EntityData[entity.Id];if(!collider&&data.TryGetProperty("runtimeMeshObjectId",out var id)){value=Resolve(Prefix+id.GetInt32()) as Mesh;bindings[entity]=value;return value;}if(data.TryGetProperty("runtimeMeshAssetId",out var source)){value=PrimitiveMeshes.Get(source.GetString()!);Register(value);bindings[entity]=value;return value;}return null;}
 internal sealed record Bindings(bool HasMesh,Mesh? Mesh,bool HasCollider,Mesh? Collider){internal void Restore(Entity entity){if(HasMesh)MeshBindings[entity]=Mesh;if(HasCollider)ColliderBindings[entity]=Collider;}}
 internal static Bindings CaptureBindings(Entity entity){var data=Context.EntityData[entity.Id];if(data.TryGetProperty("runtimeMeshFilter",out var flag)&&flag.GetBoolean())_=MeshOf(entity);return new(MeshBindings.TryGetValue(entity,out var mesh),mesh,ColliderBindings.TryGetValue(entity,out var collider),collider);}
 internal static void Forget(Entity entity){MeshBindings.Remove(entity);ColliderBindings.Remove(entity);}
 internal static Object? Load(Type type,string path){if(Loaded.TryGetValue((type,path),out var cached)&&cached)return cached;Object? value=type==typeof(TextAsset)?Resources.Load<TextAsset>(path):type==typeof(Mesh)?Resources.Load<Mesh>(path):type==typeof(Texture2D)?Resources.Load<Texture2D>(path):type==typeof(AudioClip)?Resources.Load<AudioClip>(path):null;if(value!=null){Register(value);Loaded[(type,path)]=value;}return value;}
 internal static void Retire(Object value){
  var bound=Context.EntityData.Where(pair=>pair.Value.TryGetProperty("runtimeMeshObjectId",out var id)?id.GetInt32()==value.GetInstanceID():MeshBindings.TryGetValue(new(pair.Key,Context.Generation),out var mesh)&&ReferenceEquals(mesh,value)).Select(pair=>new Entity(pair.Key,Context.Generation)).ToArray();
  var colliders=ColliderBindings.Where(pair=>ReferenceEquals(pair.Value,value)&&Context.Entities.ContainsKey(pair.Key.Id)).Select(pair=>pair.Key).ToArray();
  if(Context.Operations.Count+bound.Length+colliders.Length>Limits.Operations)throw new InvalidOperationException("Asset destruction exceeds transport capacity");
  foreach(var entity in bound)AssignMesh(entity,null);
  foreach(var entity in colliders){ColliderBindings[entity]=null;Context.Property(entity,"runtimeColliderEnabled",JsonValue.Create(false));}
  Register(value).Live=false;Objects.Remove(value.GetInstanceID());UnityEngine.Object.ForgetNative(value.GetInstanceID());
 }
 internal sealed record Snapshot(Type Type,string Path,string SourceId,string Text,string Name,UnityEngine.HideFlags Flags){
  internal Object Create(){Object value=Type==typeof(Mesh)?new Mesh(Path,SourceId):Type==typeof(Texture2D)?new Texture2D(Path,SourceId):Type==typeof(AudioClip)?new AudioClip(Path,SourceId):new TextAsset(Path,Text);Register(value,true,Name+"(Clone)");UnityEngine.NativeObjects.Flags[value.GetInstanceID()]=Flags;return value;}
 }
 internal static Snapshot Capture(Object value){
  if(!value)throw new UnityEngine.MissingReferenceException("Cannot clone a destroyed asset");
  if(value is ProjectAsset asset)return new(value.GetType(),asset.Path,asset.assetId,"",Name(value),UnityEngine.Object.Wrap(value).hideFlags);
  if(value is TextAsset text)return new(typeof(TextAsset),text.Path,"",text.text,Name(value),UnityEngine.Object.Wrap(value).hideFlags);
  throw new NotSupportedException("Asset type requires its own snapshot dependency");
 }
 internal static Object[] Clone(Snapshot snapshot,int count){
  if(count<1||count>512-Objects.Count)throw new ArgumentOutOfRangeException(nameof(count));var created=new List<Object>();
  try{for(var i=0;i<count;i++)created.Add(snapshot.Create());return created.ToArray();}catch{foreach(var value in created){value.AssetState!.Live=false;Objects.Remove(value.GetInstanceID());}throw;}
 }
 internal static void Write(Utf8JsonWriter writer){
  writer.WriteStartArray("runtimeAssets");foreach(var (id,state) in Objects){if(!state.Value)continue;writer.WriteStartObject();writer.WriteNumber("id",id);writer.WriteString("path",Prefix+id.ToString(System.Globalization.CultureInfo.InvariantCulture));writer.WriteString("type",state.Value.GetType().Name);writer.WriteString("name",state.Name);writer.WriteBoolean("owned",state.Owned);writer.WriteString("sourceAssetId",state.Value is ProjectAsset asset?asset.assetId:"");writer.WriteEndObject();}writer.WriteEndArray();
 }
 internal static void AssignMesh(Entity entity,Mesh? mesh){
  if(!Context.EntityData[entity.Id].TryGetProperty("runtimeMeshFilter",out var filter)||!filter.GetBoolean())throw new InvalidOperationException("MeshFilter is missing");
  if(!ReferenceEquals(mesh,null)&&!mesh)throw new UnityEngine.MissingReferenceException("Mesh is destroyed");
  if(mesh!=null)Register(mesh);var id=mesh?.GetInstanceID()??0;var source=mesh?.assetId??"";
  var data=new JsonObject{["objectId"]=id,["sourceAssetId"]=source};Context.Add(new("mesh",entity.Id,null,default,null,Data:data.ToJsonString()));
  var node=JsonNode.Parse(Context.EntityData[entity.Id].GetRawText())!.AsObject();node["runtimeMeshObjectId"]=id;node["runtimeMeshAssigned"]=source!="";
  if(source!=""){node["runtimeMeshAssetId"]=source;node["renderable"]=new JsonObject{["kind"]="mesh",["assetId"]=source};}
  Context.EntityData[entity.Id]=Context.ParseData(node.ToJsonString());
  MeshBindings[entity]=mesh;
 }
}
public partial class Object {internal RuntimeAssetState? AssetState;}
}
namespace UnityEngine {
public class TextAsset:Object {
 public TextAsset():this(""){}public TextAsset(string text){Wrapped=new Axiom.Gameplay.TextAsset("",text??"");Axiom.Gameplay.RuntimeAssets.Register(Wrapped,true,"");CacheFacade();}
 internal TextAsset(Axiom.Gameplay.TextAsset value){Wrapped=value;}
 public string text=>this?((Axiom.Gameplay.TextAsset)Native).text:throw new MissingReferenceException("TextAsset has been destroyed");public byte[] bytes=>System.Text.Encoding.UTF8.GetBytes(text);
}
public class Texture2D:Object {internal Texture2D(Axiom.Gameplay.Texture2D value){Wrapped=value;}}
public class AudioClip:Object {internal AudioClip(Axiom.Gameplay.AudioClip value){Wrapped=value;}}
public static class Resources {
 public static T[] FindObjectsOfTypeAll<T>() where T:Object=>NativeObjects.All(typeof(T)).Cast<T>().ToArray();public static Object[] FindObjectsOfTypeAll(Type type)=>NativeObjects.All(type);
 public static T? Load<T>(string path) where T:Object {var type=typeof(T)==typeof(TextAsset)?typeof(Axiom.Gameplay.TextAsset):typeof(T)==typeof(Mesh)?typeof(Axiom.Gameplay.Mesh):typeof(T)==typeof(Texture2D)?typeof(Axiom.Gameplay.Texture2D):typeof(T)==typeof(AudioClip)?typeof(Axiom.Gameplay.AudioClip):typeof(Axiom.Gameplay.Object);var native=Axiom.Gameplay.RuntimeAssets.Load(type,path);return native==null?null:(T)Object.Wrap(native);}
}
}
