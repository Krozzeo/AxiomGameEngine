using UnityEngine;
namespace Game;
public sealed class AsyncInstantiationLab : MonoBehaviour {
 public int Frames,Passed,Failed,Number=73;public string Report="";public bool IsClone;public GameObject Peer;
 public AsyncCloneData Data=new AsyncCloneData();public System.Collections.Generic.List<AsyncCloneData> Items=new System.Collections.Generic.List<AsyncCloneData>();[SerializeReference] public AsyncCloneData Reference=new AsyncCloneData();
 private AsyncInstantiateOperation<AsyncInstantiationLab> operation;
 private AsyncInstantiateOperation<GameObject> cancellation;
 private AsyncInstantiateOperation<GameObject> awaited;
 private GameObject destination;
 private int completed;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private void Start(){
  if(IsClone)return;
  Peer=new GameObject("Async child");Peer.transform.SetParent(transform,false);Peer.transform.localPosition=Vector3.right;
  Data.Value=12;Data.Peer=Peer;Items.Add(Data);Reference.Peer=Peer;Reference.Next=Reference;
  IsClone=true;operation=UnityEngine.Object.InstantiateAsync(this,2);IsClone=false;Number=99;
  operation.allowSceneActivation=false;operation.completed+=_=>completed++;
  Check("async.deferred",!operation.isDone&&operation.Result==null);
  destination=new GameObject("Async destination");destination.transform.position=new Vector3(20,0,0);
  cancellation=UnityEngine.Object.InstantiateAsync(destination);cancellation.Cancel();
  awaited=UnityEngine.Object.InstantiateAsync(destination,new Vector3(30,0,0),Quaternion.identity,new InstantiateParameters());
  ObserveAwait();
 }
 private async void ObserveAwait(){var objects=await awaited;Check("async.await",objects.Length==1&&objects[0].transform.position.x==30);}
 private void Update(){
  if(IsClone)return;Frames++;
  if(Frames==2){Check("async.activation-gate",operation.IsWaitingForSceneActivation()&&operation.progress==.9f&&completed==0);Check("async.cancel",cancellation.isDone&&cancellation.Result==null);operation.allowSceneActivation=true;}
  if(Frames==4){
   Check("async.complete",operation.isDone&&operation.Result.Length==2&&completed==1);
   Check("async.snapshot-fields",operation.Result[0].Number==73&&operation.Result[1].Number==73);
   Check("async.reference-remap",operation.Result[0].Peer==operation.Result[0].transform.GetChild(0).gameObject&&operation.Result[1].Peer==operation.Result[1].transform.GetChild(0).gameObject);
   var copy=operation.Result[0];Check("async.managed-inline",!ReferenceEquals(copy.Data,Data)&&copy.Data.Value==12&&copy.Data.Peer==copy.Peer);Check("async.managed-list",!ReferenceEquals(copy.Items,Items)&&!ReferenceEquals(copy.Items[0],Data)&&copy.Items[0].Peer==copy.Peer);Check("async.managed-cycle",!ReferenceEquals(copy.Reference,Reference)&&ReferenceEquals(copy.Reference,copy.Reference.Next)&&copy.Reference.Peer==copy.Peer);
   Check("async.distinct-identities",operation.Result[0]!=operation.Result[1]&&operation.Result[0].gameObject!=gameObject);
   var local=UnityEngine.Object.Instantiate(destination,new Vector3(2,0,0),Quaternion.identity,new InstantiateParameters{parent=destination.transform});
   Check("instantiate.parameters-local",local.transform.localPosition.x==2&&local.transform.position.x==22);
   var batch=UnityEngine.Object.InstantiateAsync(destination,3,new[]{new Vector3(1,0,0),new Vector3(2,0,0)},new[]{Quaternion.identity});batch.WaitForCompletion();
   Check("async.span-count",batch.Result.Length==3&&batch.Result[0].transform.position.x==1&&batch.Result[1].transform.position.x==2&&batch.Result[2].transform.position.x==1);
   using(var ids=new Unity.Collections.NativeArray<int>(2,Unity.Collections.Allocator.Temp))using(var transforms=new Unity.Collections.NativeArray<int>(2,Unity.Collections.Allocator.Temp)){
    GameObject.InstantiateGameObjects(destination.GetInstanceID(),2,ids,transforms);Check("bulk.identities",ids[0]!=ids[1]&&ids[0]!=transforms[0]);GameObject.SetGameObjectsActive(ids,false);GameObject.SetGameObjectsActive(ids,true);Check("bulk.activation",GameObject.Find("Async destination(Clone)")!=null);
    var alias=ids;Check("bulk.native-storage",alias[0]==ids[0]);
    foreach(var clone in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID))if(clone.name=="Async destination(Clone)")UnityEngine.Object.Destroy(clone);
   }
   foreach(var instance in batch.Result)UnityEngine.Object.Destroy(instance);UnityEngine.Object.Destroy(local);
   foreach(var instance in operation.Result)UnityEngine.Object.Destroy(instance.gameObject);foreach(var instance in awaited.Result)UnityEngine.Object.Destroy(instance);UnityEngine.Object.Destroy(destination);UnityEngine.Object.Destroy(Peer);
  }
  if(Frames==5)Check("async.cleanup",!operation.Result[0]&&!operation.Result[1]&&!destination&&!Peer&&!awaited.Result[0]);
 }
}
[System.Serializable] public sealed class AsyncCloneData {public int Value;public GameObject Peer;public AsyncCloneData Next;}
