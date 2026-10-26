using UnityEngine;
namespace Game;

// This source uses public UnityEngine names and normal private Unity messages.
public sealed class UnityMigrationLab : MonoBehaviour {
 public int Frames;
 public int Passed;
 public int Failed;
 public string Report="";
 [SerializeField] private Vector3 initialPosition=new Vector3(3,0,0);
 private GameObject root;
 private GameObject child;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private bool Near(Vector3 a,Vector3 b)=>(a-b).magnitude<0.0001f;
 private void Start(){
  root=new GameObject("Unity Migration Root");child=new GameObject("Unity Migration Child");root.transform.position=initialPosition;root.transform.localScale=new Vector3(2,1,1);child.transform.SetParent(root.transform,false);child.transform.localPosition=Vector3.right;child.transform.localRotation=Quaternion.Euler(0,0,45);
  float x=child.transform.position.x;Check("source.float-field",x==5f);Component component=child.GetComponent<Transform>();UnityEngine.Object handle=component;Check("source.type-hierarchy",handle&&component.gameObject==child);
  var expected=new Vector3(6.41421356f,0.70710678f,0);Check("matrix.shear",Near(child.transform.TransformPoint(Vector3.right),expected));Check("matrix.inverse",Near(child.transform.worldToLocalMatrix.MultiplyPoint3x4(expected),Vector3.right));
  var positions=new[]{Vector3.right,Vector3.up};child.transform.TransformPoints(positions.AsSpan());child.transform.InverseTransformPoints(positions.AsSpan());Check("source.spans",Near(positions[0],Vector3.right)&&Near(positions[1],Vector3.up));
  child.transform.hasChanged=false;root.transform.position=new Vector3(4,0,0);Check("source.change-flag",child.transform.hasChanged);
 }
 private void Update(){
  Frames++;
  if(Frames==2){
   Check("matrix.next-frame",Near(child.transform.TransformPoint(Vector3.right),new Vector3(7.41421356f,0.70710678f,0)));
   Check("query.canonical-component",root.GetComponentsInChildren<Transform>(true).Length==2);
   root.transform.localScale=Vector3.zero;child.transform.localPosition=new Vector3(2,3,4);root.transform.localScale=Vector3.one;
   Check("zero-scale.local-state",Near(child.transform.position,new Vector3(6,3,4)));
   UnityEngine.Object.Destroy(root);Check("destroy.deferred",root&&child);
  }
  if(Frames==3)Check("destroy.null",!root&&!child&&root==null&&child==null);
 }
}
