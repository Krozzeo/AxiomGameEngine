using UnityEngine;
namespace Game {
public sealed class ObjectMetaLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="";
 private void Update(){if(Complete)return;ObjectMetaCorpus.RunCases((key,value)=>{Count++;if(value)Passed++;else Failed++;Report+=key+":"+(value?"pass":"fail")+";";});Complete=true;}
}
}
