using Axiom.Gameplay;
using Object=Axiom.Gameplay.Object;
namespace Game;

// Editable runtime/bridge observations; this deliberately uses the legacy SDK.
public sealed class TransformContractsLab : MonoBehaviour {
 [ReadOnly] public int Frames;
 [ReadOnly] public int Passed;
 [ReadOnly] public int Failed;
 [ReadOnly] public string Report="";
 private GameObject? root;
 private GameObject? child;
 private GameObject? leaf;
 private static bool Near(Vector3 a,Vector3 b)=>(a-b).magnitude<1e-6;
 private void Check(string id,bool value){if(value)Passed++;else Failed++;Report+=id+":"+(value?"pass":"fail")+";";}
 public override void Update(){
  Frames++;
  if(Frames==3){
   root=new GameObject("M20 Transform Root");child=new GameObject("M20 Transform Child");leaf=new GameObject("M20 Transform Leaf");
   child.transform.position=Vector3.right;child.transform.SetParent(root.transform,false);leaf.transform.position=new(0,0,2);leaf.transform.SetParent(child.transform,false);
   root.transform.position=new(10,2,3);
   Check("hierarchy.same-frame",Near(child.transform.position,new(11,2,3))&&Near(leaf.transform.position,new(11,2,5)));
  }
  if(Frames==4){
   Check("hierarchy.next-frame",Near(child!.transform.position,new(11,2,3))&&Near(leaf!.transform.position,new(11,2,5)));
   root!.transform.rotation=Quaternion.AngleAxis(90,Vector3.up);
   Check("rotation.same-frame",Near(child!.transform.position,new(10,2,2))&&Near(leaf!.transform.position,new(12,2,2)));
  }
  if(Frames==5){
   Debug.Log("M20_TRANSFORM_OBSERVATION rotation child="+child!.transform.position+" leaf="+leaf!.transform.position+" forward="+leaf.transform.forward+" local="+child.transform.localPosition+" parent="+root!.transform.rotation);
   Check("rotation.next-frame",Near(child!.transform.position,new(10,2,2))&&Near(leaf!.transform.position,new(12,2,2))&&Near(leaf.transform.forward,Vector3.right));
   root!.transform.localScale=new(2,3,4);
  }
  if(Frames==6){
   Debug.Log("M20_TRANSFORM_OBSERVATION scale child="+child!.transform.position+" leaf="+leaf!.transform.position+" scale="+leaf.transform.lossyScale);
   Check("scale.next-frame",Near(child!.transform.position,new(10,2,1))&&Near(leaf!.transform.position,new(18,2,1))&&Near(leaf.transform.lossyScale,new(2,3,4)));
   var t=root!.transform;var v=new Vector3(1,2,3);
   Check("point.roundtrip",Near(t.InverseTransformPoint(t.TransformPoint(v)),v));
   Check("vector.roundtrip",Near(t.InverseTransformVector(t.TransformVector(v)),v));
   Check("direction.scale-independent",Near(t.TransformDirection(Vector3.forward),Vector3.right));
   var batch=new[]{v,Vector3.one};t.TransformPoints(batch.AsSpan());t.InverseTransformPoints(batch.AsSpan());
   Check("span.in-place",Near(batch[0],v)&&Near(batch[1],Vector3.one));
   var overlap=new[]{Vector3.right,Vector3.up,Vector3.one};var expected=t.TransformPoint(Vector3.up);t.TransformPoints(overlap.AsSpan(0,2),overlap.AsSpan(1,2));
   Check("span.overlap",Near(overlap[2],expected));
   var before=t.position;try{t.SetPositionAndRotation(new(99,0,0),new(0,0,0,0));Check("pose.invalid-atomic",false);}catch(System.ArgumentException){Check("pose.invalid-atomic",Near(t.position,before));}
   t.SetPositionAndRotation(new(1,2,3),Quaternion.identity);t.Rotate(90,90,0);
   Check("Euler.ZXY",Near(t.forward,Vector3.down)&&Near(t.up,Vector3.right));
   t.eulerAngles=new(20,30,40);var forward=t.forward;t.eulerAngles=t.eulerAngles;Check("Euler.roundtrip",Near(t.forward,forward));
  }
  if(Frames==7){
   var t=root!.transform;t.SetPositionAndRotation(Vector3.right,Quaternion.identity);t.RotateAround(Vector3.zero,Vector3.up,90);
   Check("orbit.pose",Near(t.position,Vector3.back)&&Near(t.forward,Vector3.right));
   t.LookAt(t.position+Vector3.up,Vector3.forward);Check("look.custom-up",Near(t.forward,Vector3.up)&&Near(t.up,Vector3.forward));
   t.forward=Vector3.back;Check("axis.setter",Near(t.forward,Vector3.back));
   child!.transform.SetParent(null,true);var before=child.transform.position;t.position=new(20,0,0);Check("detach.preserves-world",Near(child.transform.position,before));
  }
  if(Frames==8){
   Check("detach.next-frame",child!.transform.parent==null&&root!.transform.childCount==0);
   Object.DestroyImmediate(root);Object.DestroyImmediate(child);
   Check("cleanup.hierarchy",!root&&!child&&!leaf);
  }
 }
}
