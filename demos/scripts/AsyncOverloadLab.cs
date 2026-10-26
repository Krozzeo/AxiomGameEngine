using UnityEngine;
namespace Game;
public sealed class AsyncOverloadLab : MonoBehaviour {
 public int Frames,Passed,Failed;public string Report="";
 private System.Collections.Generic.List<AsyncOverloadCatalog.Observation> observations;
 private GameObject original,destination;private bool finished;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private void Update(){
  Frames++;if(Frames==160){original=new GameObject("Overload source");original.transform.position=new Vector3(3,0,0);destination=new GameObject("Overload destination");destination.transform.position=new Vector3(10,0,0);observations=AsyncOverloadCatalog.Begin(original,destination.transform);}
  if(finished||observations==null||!observations.TrueForAll(value=>value.Operation.isDone))return;finished=true;
  foreach(var value in observations){var result=value.Operation.Result;var valid=result!=null&&result.Length==value.ExpectedX.Length;if(valid)for(var i=0;i<result.Length;i++)valid&=System.Math.Abs(result[i].transform.position.x-value.ExpectedX[i])<.00001f&&result[i]!=original;Check(value.Label,valid);Check(value.Label+".null",value.RejectedNull);if(result!=null)foreach(var go in result)UnityEngine.Object.Destroy(go);}
  UnityEngine.Object.Destroy(original);UnityEngine.Object.Destroy(destination);Check("async.overloads.complete",observations.Count==17);
 }
}
