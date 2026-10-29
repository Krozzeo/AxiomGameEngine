using UnityEngine;
public sealed class NativeTransformLab:MonoBehaviour {
 public int Frames,Passed,Failed;
 public string Report="";
 void Update(){
  Frames++;
  // Other labs exercise initial lifecycle phases first; one hierarchy per frame keeps transport bounded.
  var index=Frames-40;if(index<0||index>=NativeTransformFixtures.Count)return;
  var passed=NativeTransformFixtures.Check(index);if(passed)Passed++;else Failed++;
  Report+="native.transform."+index+":"+(passed?"pass":"fail")+";";
 }
}
