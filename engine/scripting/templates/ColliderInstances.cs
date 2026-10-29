using System.Text.Json;
using System.Text.Json.Nodes;
namespace Axiom.Gameplay;

// Additional colliders keep their real owner. They are not hidden entities or
// independently integrated bodies. Logical slots survive a hierarchy clone;
// native identities remain specific to the new owner/generation.
internal static class ColliderInstances {
 internal static string Key(Collider value)=>value.RuntimeSlot??"collider";
 internal static IEnumerable<JsonElement> Records(Entity entity)=>Context.EntityData[entity.Id].TryGetProperty("runtimeColliders",out var values)?values.EnumerateArray():Enumerable.Empty<JsonElement>();
 private static JsonElement Record(Collider value)=>Records(value.Entity).First(record=>"collider:"+record.GetProperty("slot").GetInt32()==value.RuntimeSlot);
 internal static bool Exists(Collider value)=>value.RuntimeSlot==null?Context.EntityData[value.Entity.Id].TryGetProperty("collider",out _):Records(value.Entity).Any(record=>"collider:"+record.GetProperty("slot").GetInt32()==value.RuntimeSlot);
 internal static JsonElement Data(Collider value)=>value.RuntimeSlot==null?Context.EntityData[value.Entity.Id].GetProperty("collider"):Record(value).GetProperty("collider");
 internal static bool HasGeometry(Collider value){if(value is MeshCollider mesh&&MeshOf(mesh)==null)return false;return value.RuntimeSlot==null?!Context.EntityData[value.Entity.Id].TryGetProperty("runtimeColliderHasGeometry",out var enabled)||enabled.GetBoolean():Record(value).GetProperty("hasGeometry").GetBoolean();}
 internal static bool Enabled(Collider value)=>ComponentActivation.Read(value.Entity,Key(value))&&(value.RuntimeSlot==null?!Context.EntityData[value.Entity.Id].TryGetProperty("runtimeColliderEnabled",out var enabled)||enabled.GetBoolean():Record(value).GetProperty("enabled").GetBoolean());
 internal static void Enabled(Collider value,bool enabled){ComponentActivation.Write(value.Entity,Key(value),enabled);if(value.RuntimeSlot==null)Context.Property(value.Entity,"runtimeColliderEnabled",JsonValue.Create(enabled));else Update(value,record=>record["enabled"]=enabled);}
 internal static void Set(Collider value,string property,JsonNode node){if(value.RuntimeSlot==null)PhysicsData.Set(value.Entity,"collider",property,node);else Update(value,record=>record["collider"]![property]=node.DeepClone());}
 internal static Vector3 Half(Collider value)=>Vector3.Read(Data(value).GetProperty("halfExtents"));
 internal static void Half(Collider value,Vector3 half){PhysicsData.Finite(half);if(half.X<=0||half.Y<=0||half.Z<=0||Math.Max(half.X,Math.Max(half.Y,half.Z))>10000)throw new ArgumentOutOfRangeException(nameof(half));Set(value,"halfExtents",new JsonArray(half.X,half.Y,half.Z));}
 private static void Update(Collider value,Action<JsonObject> update){_=Context.Read(value.Entity);var records=JsonNode.Parse(Context.EntityData[value.Entity.Id].GetProperty("runtimeColliders").GetRawText())!.AsArray();var record=records.Select(node=>node!.AsObject()).Single(record=>"collider:"+record["slot"]!.GetValue<int>()==value.RuntimeSlot);update(record);Context.Property(value.Entity,"runtimeColliders",records);}
 internal static void Remove(Collider value){if(value.RuntimeSlot==null){RuntimeAssets.UnbindCollider(value.Entity);Context.InvalidateIdentity(value.Entity,"collider");Context.Property(value.Entity,"collider",null);return;}var records=JsonNode.Parse(Context.EntityData[value.Entity.Id].GetProperty("runtimeColliders").GetRawText())!.AsArray();for(var index=records.Count-1;index>=0;index--)if("collider:"+records[index]!["slot"]!.GetValue<int>()==value.RuntimeSlot)records.RemoveAt(index);Context.ForgetIdentity(value.Entity,value.RuntimeSlot);Context.Property(value.Entity,"runtimeColliders",records.Count==0?null:records);}
 internal static Mesh? MeshOf(MeshCollider value){if(value.RuntimeSlot==null)return RuntimeAssets.MeshOf(value.Entity,true);var record=Record(value);return record.TryGetProperty("meshObjectId",out var id)&&RuntimeAssets.Resolve(RuntimeAssets.Prefix+id.GetInt32()) is Mesh mesh&&mesh?mesh:null;}
 internal static Collider Create(Entity entity,string type,string? slot=null)=>type switch {
  nameof(BoxCollider)=>new BoxCollider(entity,slot),nameof(SphereCollider)=>new SphereCollider(entity,slot),nameof(CapsuleCollider)=>new CapsuleCollider(entity,slot),nameof(MeshCollider)=>new MeshCollider(entity,slot),nameof(BoxCollider2D)=>new BoxCollider2D(entity,slot),nameof(CircleCollider2D)=>new CircleCollider2D(entity,slot),_=>throw new NotSupportedException("Collider class is not implemented: "+type)
 };
 internal static Collider Add(Entity entity,Type type){
  _=Context.Read(entity);if(type==typeof(Collider))type=typeof(BoxCollider);if(type==typeof(Collider2D))type=typeof(BoxCollider2D);
  if(!new[]{typeof(BoxCollider),typeof(SphereCollider),typeof(CapsuleCollider),typeof(MeshCollider),typeof(BoxCollider2D),typeof(CircleCollider2D)}.Contains(type))throw new NotSupportedException("Collider class is not implemented: "+type.Name);
  var dimension=typeof(Collider2D).IsAssignableFrom(type)?2:3;var data=Context.EntityData[entity.Id];var existing=new GameObject(entity).AllComponents().OfType<Collider>().ToArray();
  if(existing.Length>=8)throw new InvalidOperationException("Collider attachment limit exceeded");if(existing.Any(value=>value.Data.GetProperty("dimension").GetInt32()!=dimension)||data.TryGetProperty("runtimePhysicsDimension",out var bodyDimension)&&bodyDimension.GetInt32()!=dimension)throw new ArgumentException("2D and 3D physics components cannot be mixed");
  if(Context.Operations.Count+4>=Limits.Operations)throw new InvalidOperationException("AX_SCRIPT_0003: operation limit exceeded");
  var mesh=type==typeof(MeshCollider)?RuntimeAssets.MeshOf(entity):null;var meshIndex=mesh==null?-1:Array.IndexOf(UnityEngine.PrimitiveCatalog.Ids,mesh.assetId);var hasGeometry=type!=typeof(MeshCollider)||meshIndex>=0;
  var shape=type==typeof(SphereCollider)||type==typeof(CircleCollider2D)?"sphere":type==typeof(CapsuleCollider)?"capsule":meshIndex switch{0=>"sphere",1 or 2=>"capsule",4=>"plane",5=>"quad",_=>"box"};
  var half=shape switch{"capsule"=>type==typeof(CapsuleCollider)?new[]{.5,.5,.5}:new[]{.5,1,.5},"plane"=>new[]{5d,0,5},"quad"=>new[]{.5,.5,0},_=>new[]{.5,.5,.5}};
  if(shape is "plane" or "quad"&&Context.Bodies.ContainsKey(entity.Id))throw new InvalidOperationException("Nonconvex mesh colliders require a static body");
  var layer=data.TryGetProperty("runtimeLayer",out var collisionLayer)?1u<<collisionLayer.GetInt32():existing.FirstOrDefault()?.Data.GetProperty("layer").GetUInt32()??1u;
  var collider=new JsonObject{["dimension"]=dimension,["shape"]=shape,["halfExtents"]=new JsonArray(half.Select(value=>(JsonNode?)JsonValue.Create(value)).ToArray()),["trigger"]=false,["layer"]=layer,["mask"]=uint.MaxValue};
  if(!data.TryGetProperty("collider",out _)){
   Context.Property(entity,"runtimeColliderEnabled",JsonValue.Create(true));Context.Property(entity,"runtimeMeshCollider",JsonValue.Create(type==typeof(MeshCollider)));Context.Property(entity,"runtimeColliderHasGeometry",JsonValue.Create(hasGeometry));Context.Property(entity,"collider",collider);Context.Property(entity,"runtimeColliderLocal",JsonValue.Create(true));
   if(mesh!=null)RuntimeAssets.BindCollider(entity,mesh);return Create(entity,type.Name);
  }
  var slot=Object.AllocateIdentity();var result=Create(entity,type.Name,"collider:"+slot);var records=data.TryGetProperty("runtimeColliders",out var collection)?JsonNode.Parse(collection.GetRawText())!.AsArray():new JsonArray();
  records.Add(new JsonObject{["slot"]=slot,["type"]=type.Name,["collider"]=collider,["enabled"]=true,["hasGeometry"]=hasGeometry,["meshObjectId"]=mesh?.GetInstanceID()??0});Context.Property(entity,"runtimeColliders",records);return result;
 }
}
