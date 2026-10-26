using L=Axiom.Gameplay;
using U=UnityEngine;
public static class PrimitiveCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(80);
  foreach(var type in Enum.GetValues<U.PrimitiveType>()){
   var primitive=U.GameObject.CreatePrimitive(type);var native=primitive.LegacyGameObject;var filter=primitive.GetComponent<U.MeshFilter>()!;var renderer=primitive.GetComponent<U.MeshRenderer>();var collider=primitive.GetComponent<U.Collider>();
   check("primitive "+type+" creates real scene geometry and components",primitive.name==type.ToString()&&filter&&filter.sharedMesh&&filter.sharedMesh!.name==type.ToString()&&renderer&&collider&&native.GetComponent<L.MeshFilter>()!=null);
   check("primitive "+type+" preserves source component order",primitive.GetComponents<U.Component>().Select(c=>c.GetType()).SequenceEqual(new[]{typeof(U.Transform),typeof(U.MeshFilter),typeof(U.MeshRenderer),collider!.GetType()}));
   check("primitive "+type+" uses correct collider class",type switch{U.PrimitiveType.Cube=>collider is U.BoxCollider,U.PrimitiveType.Sphere=>collider is U.SphereCollider,U.PrimitiveType.Capsule or U.PrimitiveType.Cylinder=>collider is U.CapsuleCollider,_=>collider is U.MeshCollider});
   if(collider is U.MeshCollider mesh)check("primitive "+type+" mesh collider shares actual render mesh",mesh.sharedMesh==filter.sharedMesh);
   check("primitive "+type+" has no implicit rigid body",native.GetComponent<L.Rigidbody>()==null);
   var clone=U.Object.Instantiate(primitive);check("primitive "+type+" clone preserves mesh identity and component types",clone.GetComponent<U.MeshFilter>()!.sharedMesh==filter.sharedMesh&&clone.GetComponent<U.Collider>()!.GetType()==collider!.GetType());
   U.Object.DestroyImmediate(primitive);U.Object.DestroyImmediate(clone);L.Context.Operations.Clear();
  }
  var sphere=U.GameObject.CreatePrimitive(U.PrimitiveType.Sphere);sphere.transform.localScale=new(2,3,1);check("scaled primitive sphere has world-space physical radius",Math.Abs(sphere.GetComponent<U.SphereCollider>()!.ClosestPoint(new U.Vector3(4,0,0)).x-1.5f)<1e-6);
  check("canonical string queries resolve component base names",sphere.GetComponent("Collider")==sphere.GetComponent<U.SphereCollider>()&&sphere.GetComponent("UnityEngine.Component")==sphere.transform);
  check("component string queries preserve source namespace and missing-type behavior",sphere.transform.GetComponent("UnityEngine.SphereCollider")==sphere.GetComponent<U.SphereCollider>()&&sphere.GetComponent("MissingComponent")==null);
  var capsule=U.GameObject.CreatePrimitive(U.PrimitiveType.Capsule);var nativeCapsule=capsule.LegacyGameObject.GetComponent<L.CapsuleCollider>()!;capsule.transform.rotation=U.Quaternion.Euler(0,0,90);
  check("capsule query hits rounded end cap",nativeCapsule.Raycast(new L.Ray(new(3,0,0),new(-1,0,0)),out var hit,10)&&Math.Abs(hit.distance-2)<1e-6);
  check("capsule query rejects bounding box corner",!nativeCapsule.Raycast(new L.Ray(new(3,.49,.49),new(-1,0,0)),out _,10));
  check("capsule closest point uses rounded geometry",Math.Abs(nativeCapsule.ClosestPoint(new(3,0,0)).X-1)<1e-6);
  var plane=U.GameObject.CreatePrimitive(U.PrimitiveType.Plane);var surface=plane.LegacyGameObject.GetComponent<L.MeshCollider>()!;
  check("plane query hits finite zero thickness surface",surface.Raycast(new L.Ray(new(0,2,0),new(0,-1,0)),out hit,3)&&Math.Abs(hit.distance-2)<1e-6);
  check("plane query rejects back face and outside mesh",!surface.Raycast(new L.Ray(new(0,-2,0),new(0,1,0)),out _,3)&&!surface.Raycast(new L.Ray(new(6,2,0),new(0,-1,0)),out _,3));
  var filterHandle=plane.GetComponent<U.MeshFilter>()!;U.Object.DestroyImmediate(filterHandle);check("removing MeshFilter retains collider and invalidates only filter",!filterHandle&&plane.GetComponent<U.MeshFilter>()==null&&plane.GetComponent<U.MeshCollider>()!=null&&plane.GetComponentCount()==3);
  var before=L.Context.Entities.Count;try{_=U.GameObject.CreatePrimitive((U.PrimitiveType)90);check("invalid primitive type does not create objects",false);}catch(ArgumentException){check("invalid primitive type does not create objects",L.Context.Entities.Count==before);}
  check("normal scenes use the pinned default scene culling mask",capsule.sceneCullingMask==16140901064495857664UL&&capsule.scene.IsValid()&&capsule.scene.isLoaded);
  L.Context.Operations.Clear();for(var i=0;i<L.Limits.Operations;i++)L.Context.Operations.Add(new("active",capsule.LegacyGameObject.Entity.Id,null,default,null,"true"));
  before=L.Context.Entities.Count;var spawned=L.Context.Spawned;
  try{_=U.GameObject.CreatePrimitive(U.PrimitiveType.Cube);check("primitive operation budget rejects without native allocation",false);}catch(InvalidOperationException){check("primitive operation budget rejects without native allocation",L.Context.Entities.Count==before&&L.Context.Spawned==spawned);}
  var oldName=capsule.name;try{capsule.name="Unpublished";check("property operation budget preserves native metadata",false);}catch(InvalidOperationException){check("property operation budget preserves native metadata",capsule.name==oldName);}
  try{capsule.SetActive(false);check("activation operation budget preserves active state",false);}catch(InvalidOperationException){check("activation operation budget preserves active state",capsule.activeSelf);}
  L.Context.Operations.Clear();
 }
}
