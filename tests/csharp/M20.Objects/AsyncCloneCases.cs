using L=Axiom.Gameplay;
using U=UnityEngine;
public static class AsyncCloneCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(60);Register();var source=new U.GameObject("Snapshot");var child=new U.GameObject("Snapshot child");child.transform.SetParent(source.transform,false);child.transform.localPosition=new(2,0,0);
  var probe=source.AddComponent<AsyncCloneProbe>();probe.Number=7;probe.Peer=child;source.transform.position=new(5,0,0);var awakeBefore=AsyncCloneProbe.Awakes;
  var operation=U.Object.InstantiateAsync(source,2);operation.allowSceneActivation=false;var completions=0;operation.completed+=_=>completions++;
  var entityCount=L.Context.Entities.Count;check("async snapshot is invisible before integration",!operation.isDone&&operation.Result is null&&L.Context.Entities.Count==entityCount);
  source.transform.position=new(40,0,0);probe.Number=99;U.Object.DestroyImmediate(source);L.Context.Operations.Clear();Frame();
  check("async activation gate preserves invisible snapshot",operation.IsWaitingForSceneActivation()&&operation.progress==.9f&&L.Context.Entities.Count==0&&AsyncCloneProbe.Awakes==awakeBefore&&completions==0);
  operation.allowSceneActivation=true;Frame();var result=operation.Result;
  check("async source destruction does not lose snapshot",operation.isDone&&result.Length==2&&result.All(go=>go.transform.position==new U.Vector3(5,0,0)));
  check("async serialized state is captured before source mutation",result.All(go=>go.GetComponent<AsyncCloneProbe>()!.Number==7));
  check("async hierarchy references remap after source destruction",result.All(go=>go.GetComponent<AsyncCloneProbe>()!.Peer==go.transform.GetChild(0).gameObject));
  check("async activation invokes Awake only on integration",AsyncCloneProbe.Awakes==awakeBefore+2&&completions==1);
  var late=0;operation.completed+=_=>late++;check("completed late subscription invokes synchronously",late==1);
  check("async batch emits a single hierarchy import",L.Context.Operations.Count(op=>op.Kind=="import")==1);
  check("async generated names and independent identities",result.All(go=>go.name=="Snapshot(Clone)")&&result[0]!=result[1]);
  check("async awaiter returns actual typed objects",operation.GetAwaiter().IsCompleted&&ReferenceEquals(operation.GetAwaiter().GetResult(),result));
  L.Context.Operations.Clear();var parent=new U.GameObject("Destination");parent.transform.position=new(100,0,0);var scene=U.SceneManagement.SceneManager.CreateScene("Target scene");
  var local=U.Object.Instantiate(result[0],new U.Vector3(3,0,0),U.Quaternion.identity,new U.InstantiateParameters{parent=parent.transform,scene=scene});
  check("InstantiateParameters local pose and parent scene",local.transform.localPosition==new U.Vector3(3,0,0)&&local.transform.position==new U.Vector3(103,0,0)&&local.LegacyGameObject.scene.name==parent.LegacyGameObject.scene.name);
  var relocated=U.Object.Instantiate(result[0],new U.InstantiateParameters{scene=scene});check("InstantiateParameters explicit destination scene",relocated.LegacyGameObject.scene.name==scene.name);
  var worlds=U.Object.InstantiateAsync(result[0],3,parent.transform,new U.Vector3[]{new(10,0,0),new(20,0,0)},new U.Quaternion[]{U.Quaternion.identity});worlds.WaitForCompletion();
  check("async span poses repeat independently to requested count",worlds.Result.Select(go=>go.transform.position.x).SequenceEqual(new[]{10f,20f,10f}));
  check("async WaitForCompletion integrates actual hierarchy",worlds.isDone&&worlds.Result.All(go=>go.transform.parent==parent.transform));
  var sameFrame=0;worlds.completed+=_=>sameFrame++;check("completion event waits beyond creation frame",sameFrame==0);Frame();check("completion event fires next frame after synchronous completion",sameFrame==1);
  L.Context.Operations.Clear();var before=L.Context.Entities.Count;var cancel=U.Object.InstantiateAsync(result[0],3);cancel.allowSceneActivation=false;cancel.Cancel();Frame();check("cancelled snapshot leaves no entities or wire intents",cancel.isDone&&cancel.Result is null&&L.Context.Entities.Count==before&&L.Context.Operations.Count==0);
  using(var token=new System.Threading.CancellationTokenSource()){var cancelled=U.Object.InstantiateAsync(result[0],new U.InstantiateParameters(),token.Token);token.Cancel();Frame();check("cancellation token stops pending integration",cancelled.isDone&&cancelled.Result is null&&L.Context.Entities.Count==before);try{cancelled.GetAwaiter().GetResult();check("cancelled await throws",false);}catch(OperationCanceledException){check("cancelled await throws",true);}}
  var failure=U.Object.InstantiateAsync(result[0],new U.Vector3(2000000,0,0),U.Quaternion.identity);try{failure.WaitForCompletion();check("failed batch rolls back native state",false);}catch(ArgumentException){check("failed batch rolls back native state",L.Context.Entities.Count==before&&L.Context.Operations.Count==0);}Frame();L.Context.Operations.Clear();
  var lostParent=new U.GameObject("Lost parent");var pending=U.Object.InstantiateAsync(result[0],lostParent.transform);U.Object.DestroyImmediate(lostParent);L.Context.Operations.Clear();try{pending.WaitForCompletion();check("destroyed destination parent fails without partial objects",false);}catch(U.MissingReferenceException){check("destroyed destination parent fails without partial objects",L.Context.Entities.Count==before&&L.Context.Operations.Count==0);}Frame();
  check("async invalid count is rejected before snapshot publication",Throws(()=>U.Object.InstantiateAsync(result[0],0)));
  var unfinished=U.Object.InstantiateAsync(result[0]);reset(61);check("runtime reset abandons pending snapshots",unfinished.isDone&&unfinished.Result is null);
  static bool Throws(Action action){try{action();return false;}catch(ArgumentException){return true;}}
 }
 private static void Frame(){L.Time.Step(.016);L.RuntimeLifecycle.Update(.016);}
 private static void Register(){AsyncCloneProbe.Awakes=0;L.RuntimeLifecycle.Register<AsyncCloneProbe>("Scripts/AsyncCloneProbe.cs",()=>new AsyncCloneProbe(),(s,v)=>L.ScriptValues.Apply(typeof(AsyncCloneProbe),s,v),(s,w)=>L.ScriptValues.Write(typeof(AsyncCloneProbe),s,w,new[]{"Number","Peer"}));}
}
public sealed class AsyncCloneProbe : U.MonoBehaviour {
 public static int Awakes;public int Number;public U.GameObject? Peer;
 private void Awake(){Awakes++;}
}
