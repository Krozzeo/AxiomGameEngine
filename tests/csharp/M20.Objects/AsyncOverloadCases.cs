using L=Axiom.Gameplay;
using U=UnityEngine;
public static class AsyncOverloadCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(90);var original=new U.GameObject("Overload source");original.transform.position=new(3,0,0);var parent=new U.GameObject("Overload destination");parent.transform.position=new(10,0,0);var observations=Game.AsyncOverloadCatalog.Begin(original,parent.transform);
  for(var i=0;i<180&&observations.Any(value=>!value.Operation.isDone);i++){L.Context.Operations.Clear();L.Time.Step(.016);L.RuntimeLifecycle.Update(.016);}
  foreach(var observation in observations){var result=observation.Operation.Result;check(observation.Label+".native."+observation.Id,observation.Operation.isDone&&result!=null&&result.Length==observation.ExpectedX.Length&&result.Select(go=>go.transform.position.x).Zip(observation.ExpectedX).All(pair=>Math.Abs(pair.First-pair.Second)<1e-5f)&&result.All(go=>go!=original));check(observation.Label+".null.native."+observation.Id,observation.RejectedNull);}
 }
}
