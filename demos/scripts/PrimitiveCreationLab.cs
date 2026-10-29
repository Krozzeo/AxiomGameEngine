using UnityEngine;
namespace Game {
public sealed class PrimitiveCreationLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="",Diagnostics="";
 private void Update(){if(Complete)return;PrimitiveCreationCorpus.Run((key,value)=>{Count++;var failure=PrimitiveCreationReference.Compare(key,value);if(failure=="")Passed++;else{Failed++;if(Diagnostics.Length<3000)Diagnostics+=key+": "+failure+";";}Report+=key+":"+(failure==""?"pass":"fail")+";";});Complete=true;}
}
}
