using L=Axiom.Gameplay;
using U=UnityEngine;
public static class AsyncLifecycleCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(3001);L.RuntimeLifecycle.Register<Game.AsyncLifecycleProbe>("Scripts/AsyncLifecycleProbe.cs",()=>new Game.AsyncLifecycleProbe(),(_,_)=>{},(_,writer)=>{writer.WriteStartObject();writer.WriteEndObject();});
  L.RuntimeLifecycle.Register<Game.AsyncLifecycleLab>("Scripts/AsyncLifecycleLab.cs",()=>new Game.AsyncLifecycleLab(),(_,_)=>{},(_,writer)=>{writer.WriteStartObject();writer.WriteEndObject();});
  var owner=new U.GameObject("Shared async lifecycle corpus");var driver=owner.AddComponent<Game.AsyncLifecycleLab>();
  for(var i=0;i<600&&!driver.Complete;i++){L.Context.Operations.Clear();L.Time.Step(.016);L.RuntimeLifecycle.Update(.016);L.RuntimeLifecycle.Late();L.RuntimeLifecycle.End();}
  check("shared async lifecycle corpus reaches completion",driver.Complete);
  foreach(var result in driver.Report.Split(';',StringSplitOptions.RemoveEmptyEntries)){var parts=result.Split(':');check(parts[0]+".shared-lifecycle",parts[1]=="pass");}
 }
}
