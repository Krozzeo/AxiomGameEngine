using Legacy=Axiom.Gameplay;
using System.Text.Json.Nodes;
namespace UnityEngine;
public enum PrimitiveType {Sphere=0,Capsule=1,Cylinder=2,Cube=3,Plane=4,Quad=5}
internal static class PrimitiveCatalog {
 internal static readonly string[] Names={"Sphere","Capsule","Cylinder","Cube","Plane","Quad"};
 internal static readonly string[] Ids={"asset://a4e7bf07e6b0af06a37bff887e47cd30ccaa82c1dc61ee4600b2f77568a6dbb8","asset://454a343948c0661a09f1b6dbbf2f42b5f33897420ac1f80dc7036ccc20cf168a","asset://3b0a942875d3bf88ad8589b2c4f310aa4d59792d119f093d3f54fe3eb73735b6","asset://797106b5b49558fb07ad54a058005627cdcf201d0afc4bc88f294d0296ba6378","asset://af4460fe5fa74a628838c5635391163d7f080ef3fc9e8063f9af9f84936b3de4","asset://8c831db589568c0dd95db5ae845e53bf329de0d40d83b19aba21ecba58fd5579"};
}
public partial class GameObject {
 public ulong sceneCullingMask {get{_=LegacyGameObject;return 16140901064495857664UL;}}
 public static GameObject CreatePrimitive(PrimitiveType type){
  if(!Enum.IsDefined(type))throw new ArgumentOutOfRangeException(nameof(type));
  var index=(int)type;var entity=Legacy.Context.CreateObject(PrimitiveCatalog.Names[index]);
  var node=JsonNode.Parse(Legacy.Context.EntityData[entity.Id].GetRawText())!.AsObject();
  node["renderable"]=new JsonObject{["kind"]="mesh",["assetId"]=PrimitiveCatalog.Ids[index]};node["runtimeMeshFilter"]=true;node["runtimeMeshAssetId"]=PrimitiveCatalog.Ids[index];node["runtimeColliderLocal"]=true;
  var shape=index switch{0=>"sphere",1 or 2=>"capsule",3=>"box",4=>"plane",_=>"quad"};var half=index switch{1 or 2=>new[]{.5,1,.5},4=>new[]{5d,0,5},5=>new[]{.5,.5,0},_=>new[]{.5,.5,.5}};
  node["collider"]=new JsonObject{["dimension"]=3,["shape"]=shape,["halfExtents"]=new JsonArray(half.Select(v=>(JsonNode?)JsonValue.Create(v)).ToArray()),["trigger"]=false,["layer"]=1,["mask"]=4294967295u};
  Legacy.Context.EntityData[entity.Id]=Legacy.Context.ParseData(node.ToJsonString());Legacy.Context.Operations[^1]=new("primitive",entity.Id,null,default,null,Data:index.ToString(System.Globalization.CultureInfo.InvariantCulture));Legacy.Context.Refresh(entity);
  var native=new Legacy.GameObject(entity);Legacy.Context.ComponentOrders[entity]=new[]{native.transform.GetInstanceID(),new Legacy.MeshFilter(entity).GetInstanceID(),new Legacy.MeshRenderer(entity).GetInstanceID(),native.GetComponent<Legacy.Collider>()!.GetInstanceID()}.ToList();return native;
 }
}
public class Collider : Component {
 internal Collider(Legacy.Collider value){Wrapped=value;}
 private Legacy.Collider Body=>LegacyComponent as Legacy.Collider??throw new MissingReferenceException("Collider is missing");
 public bool enabled {get=>Body.enabled;set=>Body.enabled=value;}public bool isTrigger {get=>Body.isTrigger;set=>Body.isTrigger=value;}
 public Vector3 ClosestPoint(Vector3 position)=>Body.ClosestPoint(position);
}
public class BoxCollider : Collider {internal BoxCollider(Legacy.BoxCollider value):base(value){}public Vector3 size {get=>((Legacy.BoxCollider)LegacyComponent).size;set=>((Legacy.BoxCollider)LegacyComponent).size=value;}}
public class SphereCollider : Collider {internal SphereCollider(Legacy.SphereCollider value):base(value){}public float radius {get=>(float)((Legacy.SphereCollider)LegacyComponent).radius;set=>((Legacy.SphereCollider)LegacyComponent).radius=value;}}
public class CapsuleCollider : Collider {internal CapsuleCollider(Legacy.CapsuleCollider value):base(value){}public float radius {get=>(float)((Legacy.CapsuleCollider)LegacyComponent).radius;set=>((Legacy.CapsuleCollider)LegacyComponent).radius=value;}public float height {get=>(float)((Legacy.CapsuleCollider)LegacyComponent).height;set=>((Legacy.CapsuleCollider)LegacyComponent).height=value;}}
public class MeshCollider : Collider {internal MeshCollider(Legacy.MeshCollider value):base(value){}public Mesh? sharedMesh=>((Legacy.MeshCollider)LegacyComponent).sharedMesh is {} mesh?(Mesh)Wrap(mesh):null;}
public class MeshFilter : Component {internal MeshFilter(Legacy.MeshFilter value){Wrapped=value;}public Mesh? sharedMesh=>((Legacy.MeshFilter)LegacyComponent).sharedMesh is {} mesh?(Mesh)Wrap(mesh):null;}
public class Renderer : Component {internal Renderer(Legacy.Renderer value){Wrapped=value;}public bool enabled {get=>((Legacy.Renderer)LegacyComponent).enabled;set=>((Legacy.Renderer)LegacyComponent).enabled=value;}}
public class MeshRenderer : Renderer {internal MeshRenderer(Legacy.MeshRenderer value):base(value){}}
public class Mesh : Object {internal Mesh(Legacy.Mesh value){Wrapped=value;}}
