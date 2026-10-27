using L=Axiom.Gameplay;
using U=UnityEngine;

public static class HierarchyActivationCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(38);L.RuntimeLifecycle.Register<HierarchyActivationProbe>("Scripts/HierarchyActivationProbe.cs",()=>new HierarchyActivationProbe(),(_,_)=>{},(_,writer)=>{writer.WriteStartObject();writer.WriteEndObject();});
  var active=new U.GameObject("Active parent");var inactive=new U.GameObject("Inactive parent");inactive.SetActive(false);
  var child=new U.GameObject("Child");child.transform.SetParent(active.transform,false);var probe=child.AddComponent<HierarchyActivationProbe>();L.RuntimeLifecycle.StartAll();
  check("canonical active attachment enables once",probe.Awakes==1&&probe.Enables==1&&probe.Disables==0&&probe.isActiveAndEnabled);
  child.transform.SetParent(inactive.transform,false);
  check("reparent into inactive hierarchy disables synchronously",!child.activeInHierarchy&&!probe.isActiveAndEnabled&&probe.Disables==1);
  inactive.SetActive(true);check("reactivating parent enables reparented script once",probe.Enables==2&&probe.Disables==1&&probe.isActiveAndEnabled);
  child.transform.SetParent(null,true);check("detaching active child does not repeat enable",probe.Enables==2&&probe.Disables==1);
  active.SetActive(false);child.transform.SetParent(active.transform,true);
  check("world preserving reparent also disables synchronously",probe.Disables==2&&!probe.isActiveAndEnabled);
  child.transform.SetParent(inactive.transform,true);check("reparent into active hierarchy enables synchronously",probe.Enables==3&&probe.Disables==2&&probe.isActiveAndEnabled);
  probe.enabled=false;child.transform.SetParent(active.transform);child.transform.SetParent(null);check("disabled script stays disabled across parent changes",probe.Enables==3&&probe.Disables==3&&!probe.isActiveAndEnabled);
 }
}
public sealed class HierarchyActivationProbe : U.MonoBehaviour {
 public int Awakes,Enables,Disables;
 private void Awake(){Awakes++;}private void OnEnable(){Enables++;}private void OnDisable(){Disables++;}
}
