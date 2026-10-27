using UnityEngine;
using System.Collections.Generic;
namespace Game {
// The same source executes in Unity, native Axiom and the browser: no platform test branches.
public sealed class AsyncLifecycleLab:MonoBehaviour {
 public int Frames,Passed,Failed;
 public bool Complete;
 public string Report="";
 private GameObject source,parent,destination;
 private List<AsyncOverloadCatalog.Observation> gated,cancelled;
 private int[] callbacks,cancelCallbacks;
 private int released=-1;
 private void Check(string key,bool passed){if(passed)Passed++;else Failed++;Report+=key.Replace("async.overload.","a")+":"+(passed?"pass":"fail")+";";}
 private void Update(){
  Frames++;
  if(Frames==20){
   AsyncLifecycleProbe.ResetCounters();parent=new GameObject("Async lifecycle source parent");parent.transform.position=new Vector3(100,0,0);source=new GameObject("Async lifecycle source");source.transform.SetParent(parent.transform,false);source.transform.localPosition=new Vector3(3,0,0);source.AddComponent<AsyncLifecycleProbe>();destination=new GameObject("Async lifecycle destination");destination.transform.position=new Vector3(10,0,0);
   cancelled=AsyncOverloadCatalog.Begin(source,destination.transform);cancelCallbacks=new int[cancelled.Count];for(var i=0;i<cancelled.Count;i++){var index=i;cancelled[i].Operation.completed+=_=>cancelCallbacks[index]++;cancelled[i].Operation.Cancel();}gated=AsyncOverloadCatalog.Begin(source,destination.transform);callbacks=new int[gated.Count];for(var i=0;i<gated.Count;i++){var index=i;gated[i].Operation.allowSceneActivation=false;gated[i].Operation.completed+=_=>callbacks[index]++;}
  }
  if(Frames==23){
   for(var i=0;i<gated.Count;i++){Check(gated[i].Label+".held",!gated[i].Operation.isDone&&AsyncLifecycleProbe.GlobalAwakes==1);Check(cancelled[i].Label+".cancelled",cancelled[i].Operation.isDone&&cancelled[i].Operation.Result==null);Check(cancelled[i].Label+".cancelled-completed",cancelCallbacks[i]==1);}foreach(var value in gated)value.Operation.allowSceneActivation=true;
  }
  if(Complete||gated==null||Frames<24||!gated.TrueForAll(value=>value.Operation.isDone))return;
  if(released<0){released=Frames;return;}if(Frames<=released)return;
  for(var i=0;i<gated.Count;i++){
   var value=gated[i];var result=value.Operation.Result;bool pose=result!=null&&result.Length==value.ExpectedX.Length,initialized=pose;
   if(pose)foreach(var pair in Enumerate(result)){pose&=System.Math.Abs(pair.Value.transform.position.x-value.ExpectedX[pair.Key])<.00001f&&pair.Value!=source;var probe=pair.Value.GetComponent<AsyncLifecycleProbe>();initialized&=probe!=null&&probe.AwakeCalls==1&&probe.EnableCalls==1;}
   Check(value.Label+".positive",pose);Check(value.Label+".null",value.RejectedNull);Check(value.Label+".initialize",initialized);Check(value.Label+".completed",callbacks[i]==1);
   var disabled=AsyncLifecycleProbe.GlobalDisables;var destroyed=AsyncLifecycleProbe.GlobalDestroys;if(result!=null)foreach(var go in result)UnityEngine.Object.DestroyImmediate(go);var dead=result!=null;if(result!=null)foreach(var go in result)dead&=!go;Check(value.Label+".destroyed",dead&&AsyncLifecycleProbe.GlobalDisables-disabled==result.Length&&AsyncLifecycleProbe.GlobalDestroys-destroyed==result.Length);
  }
  UnityEngine.Object.DestroyImmediate(source);UnityEngine.Object.DestroyImmediate(parent);UnityEngine.Object.DestroyImmediate(destination);Check("async.lifecycle.cleanup",!source&&!parent&&!destination);Complete=true;
 }
 private static IEnumerable<KeyValuePair<int,GameObject>> Enumerate(GameObject[] values){for(var i=0;i<values.Length;i++)yield return new KeyValuePair<int,GameObject>(i,values[i]);}
}
}
