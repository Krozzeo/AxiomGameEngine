using System.Text.Json;
using L=Axiom.Gameplay;
using U=UnityEngine;
public static class AssetCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(2001);var owner=U.GameObject.CreatePrimitive(U.PrimitiveType.Cube);var filter=owner.GetComponent<U.MeshFilter>()!;var source=filter.sharedMesh!;var copy=U.Object.Instantiate(source);
  check("asset clone has independent native identity and name",copy&&copy!=source&&copy.name==source.name+"(Clone)"&&copy.GetInstanceID()!=source.GetInstanceID());copy.name="Independent mesh";check("asset clone rename leaves borrowed mesh unchanged",source.name=="Cube"&&copy.name=="Independent mesh");
  check("asset clone retains immutable render geometry dependency",((L.Mesh)copy.Native).assetId==((L.Mesh)source.Native).assetId);
  check("asset find includes owned runtime mesh and excludes borrowed source",U.Object.FindObjectsByType<U.Mesh>(U.FindObjectsSortMode.InstanceID).Contains(copy)&&!U.Object.FindObjectsByType<U.Mesh>(U.FindObjectsSortMode.None).Contains(source));
  check("resource all lookup includes borrowed and owned assets",U.Resources.FindObjectsOfTypeAll<U.Mesh>().Contains(copy)&&U.Resources.FindObjectsOfTypeAll<U.Mesh>().Contains(source));
  filter.sharedMesh=copy;check("asset assignment retains MeshFilter and Renderer identity",filter.sharedMesh==copy&&owner.GetComponent<U.MeshFilter>()==filter&&owner.GetComponent<U.MeshRenderer>()!=null);
  var text=new U.TextAsset("Asset text");text.name="Original text";var textCopy=U.Object.Instantiate(text);check("text asset clone copies content and native name",textCopy.text=="Asset text"&&textCopy.name=="Original text(Clone)"&&textCopy!=text);
  var fields=new AssetFields{Mesh=copy,Text=textCopy};var stream=new MemoryStream();using(var writer=new Utf8JsonWriter(stream))L.ScriptValues.Write(typeof(AssetFields),fields,writer,new[]{"Mesh","Text"});var values=JsonDocument.Parse(stream.ToArray()).RootElement.Clone();L.ScriptValues.Apply(typeof(AssetFields),fields,values);
  check("canonical asset fields retain identity through serialized host roundtrip",fields.Mesh==copy&&fields.Text==textCopy);
  U.Object.Destroy(copy);check("asset destroy is deferred through current frame",copy&&filter.sharedMesh==copy);L.RuntimeLifecycle.End();check("asset destruction invalidates handle and clears live mesh binding",!copy&&filter.sharedMesh==null&&owner.GetComponent<U.MeshFilter>()==filter&&owner.GetComponent<U.MeshRenderer>()!=null&&!L.Context.EntityData[owner.LegacyGameObject.Entity.Id].GetProperty("runtimeMeshAssigned").GetBoolean());
  filter.sharedMesh=source;check("borrowed mesh can be restored after clone destruction",filter.sharedMesh==source&&L.Context.EntityData[owner.LegacyGameObject.Entity.Id].GetProperty("runtimeMeshAssigned").GetBoolean());
  U.Object.DestroyImmediate(textCopy);check("owned TextAsset immediate destroy retains source content",!textCopy&&text&&text.text=="Asset text");
  check("destroyed runtime reference resolves null instead of aliasing a new asset",L.RuntimeAssets.Resolve(values.GetProperty("Mesh").GetProperty("path").GetString()!)==null);
  var invalid=U.Object.InstantiateAsync(source,2);check("native unsupported async mesh returns unavailable operation",Unavailable(invalid));check("native unsupported async TextAsset returns unavailable operation",Unavailable(U.Object.InstantiateAsync(text)));
  var plane=U.GameObject.CreatePrimitive(U.PrimitiveType.Plane);var colliderMesh=plane.GetComponent<U.MeshCollider>()!.sharedMesh;var renderCopy=U.Object.Instantiate(source);plane.GetComponent<U.MeshFilter>()!.sharedMesh=renderCopy;check("mesh collider binding stays independent of MeshFilter assignment",plane.GetComponent<U.MeshCollider>()!.sharedMesh==colliderMesh);
  var async=U.Object.InstantiateAsync(plane,2);async.allowSceneActivation=false;check("graph asset snapshot defers integration",!async.isDone&&async.Result==null);plane.GetComponent<U.MeshFilter>()!.sharedMesh=source;U.Object.DestroyImmediate(plane);async.allowSceneActivation=true;async.WaitForCompletion();check("graph snapshot preserves shared asset after source graph changes",async.Result.Length==2&&async.Result.All(value=>value.GetComponent<U.MeshFilter>()!.sharedMesh==renderCopy));check("graph snapshot retains original collider asset after source destruction",async.Result.All(value=>value.GetComponent<U.MeshCollider>()!.sharedMesh==colliderMesh));
  var pending=U.Object.InstantiateAsync(owner,2);pending.Cancel();L.Time.Step(.016);L.RuntimeLifecycle.Update(.016);check("graph async cancellation publishes no asset users",pending.isDone&&pending.Result==null);
  U.Object.DestroyImmediate(source);check("borrowed asset default destruction is rejected without data loss",source&&filter.sharedMesh==source);U.Object.DestroyImmediate(source,true);check("explicit borrowed asset retirement clears existing bindings",!source&&filter.sharedMesh==null);
  var retained=renderCopy;reset(2002);check("asset handles become invalid when runtime session resets",!retained&&!text);
  static bool Unavailable(U.AsyncInstantiateOperation operation){try{_=operation.isDone;return false;}catch(NullReferenceException){return true;}}
 }
 public sealed class AssetFields {public U.Mesh? Mesh;public U.TextAsset? Text;}
}
