using L=Axiom.Gameplay;
using U=UnityEngine;
public static class ChangeFlagInputCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(3401);const string id="entity://00000000-0000-4000-8000-000000003401";
  void Frame(double x){var pose=new {position=new[]{x,0d,0d},rotation=new[]{0d,0d,0d,1d},scale=new[]{1d,1d,1d}};L.Context.Load(System.Text.Json.JsonSerializer.SerializeToElement(new {action="frame",entities=new[]{new {id,name="Input flag root",transform=pose,runtimeLocalTransform=pose}},keys=Array.Empty<string>(),mouseDelta=new[]{0d,0d}}));}
  Frame(0);var root=(U.GameObject)new L.GameObject(new L.Entity(id,L.Context.Generation));check("new host input initializes change flag",root.transform.hasChanged);
  root.transform.hasChanged=false;Frame(0);check("identical next-frame host input retains cleared flag",!root.transform.hasChanged);
  Frame(1);check("external physics or human pose input sets change flag",root.transform.hasChanged);
  root.transform.hasChanged=false;Frame(1);check("unchanged external pose does not keep reflagging",!root.transform.hasChanged);
  root.transform.localPosition=new(2,0,0);root.transform.hasChanged=false;Frame(2);check("acknowledged script pose does not reflag on reload",!root.transform.hasChanged);
 }
}
