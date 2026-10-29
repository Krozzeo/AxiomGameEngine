using UnityEngine;
namespace Game;
public sealed class PrimitiveContractsLab : MonoBehaviour {
 public int Frames,Passed,Failed;public string Report="";
 private readonly System.Collections.Generic.List<GameObject> objects=new System.Collections.Generic.List<GameObject>();
 private GameObject falling;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private void BuildPrimitives(){
  for(var i=0;i<6;i++){
   var type=(PrimitiveType)i;var go=GameObject.CreatePrimitive(type);objects.Add(go);go.transform.position=new Vector3(i*2-5,2,4);
   var collider=go.GetComponent<Collider>();var filter=go.GetComponent<MeshFilter>();var renderer=go.GetComponent<MeshRenderer>();
   var shape=type==PrimitiveType.Cube?collider is BoxCollider:type==PrimitiveType.Sphere?collider is SphereCollider:type==PrimitiveType.Capsule||type==PrimitiveType.Cylinder?collider is CapsuleCollider:collider is MeshCollider;
   Check("create."+type,go&&go.name==type.ToString()&&filter&&filter.sharedMesh&&filter.sharedMesh.name==type.ToString()&&renderer&&shape&&go.GetComponentCount()==4);
  }
  var clone=UnityEngine.Object.Instantiate(objects[1]);objects.Add(clone);clone.transform.position=new Vector3(7,2,4);Check("primitive.clone",clone!=objects[1]&&clone.GetComponent<MeshFilter>().sharedMesh==objects[1].GetComponent<MeshFilter>().sharedMesh&&clone.GetComponent<CapsuleCollider>()!=null);
  var filterHandle=objects[5].GetComponent<MeshFilter>();UnityEngine.Object.DestroyImmediate(filterHandle);Check("primitive.remove-filter",filterHandle==null&&objects[5].GetComponent<MeshFilter>()==null&&objects[5].GetComponent<MeshCollider>()!=null);
  var ground=GameObject.CreatePrimitive(PrimitiveType.Plane);objects.Add(ground);ground.transform.position=new Vector3(0,0,-5);
  falling=GameObject.CreatePrimitive(PrimitiveType.Capsule);objects.Add(falling);falling.transform.position=new Vector3(0,4,-5);
  // The owned physics facade is retained while the broader Unity physics port continues in M23.
  falling.AddComponent(typeof(Axiom.Gameplay.Rigidbody));falling.GetComponent<Axiom.Gameplay.Rigidbody>().freezeRotation=true;
  Check("primitive.scene",falling.scene==gameObject.scene&&falling.sceneCullingMask==16140901064495857664UL);
 }
 private void Update(){
  Frames++;if(Frames==20)BuildPrimitives();
  if(Frames==140){Check("primitive.physical-capsule",System.Math.Abs(falling.transform.position.y-1)<.06f);foreach(var go in objects)UnityEngine.Object.Destroy(go);}
  if(Frames==141)Check("primitive.cleanup",objects.TrueForAll(go=>!go));
 }
}
