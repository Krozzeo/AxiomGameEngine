using UnityEngine;
namespace Game {
public sealed class ScriptInstanceLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="",Diagnostics="";
 private void Update(){if(Complete)return;ScriptInstanceCorpus.Run((key,value)=>{Count++;var failure=ScriptInstanceReference.Compare(key,value);if(failure=="")Passed++;else{Failed++;if(Diagnostics.Length<3000)Diagnostics+=key+": "+failure+";";}Report+=key+":"+(failure==""?"pass":"fail")+";";});Complete=true;}
}
}
