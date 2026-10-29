using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game {
public static class ObjectLifetimeCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 private static string Snapshot(UnityEngine.Object target,GameObject root,GameObject child)=>((bool)target)+":"+((bool)root)+":"+((bool)child);
 public static IEnumerable<Observation> Run(){
  for(var index=0;index<6;index++)foreach(var profile in new[]{"game-object","component","transform","text","mesh","null","retired","unbound","positive-delay","negative-delay","deny-assets"}){
   if((profile=="positive-delay"||profile=="negative-delay")&&index!=1&&index!=5)continue;if(profile=="deny-assets"&&index!=3)continue;
   var root=new GameObject("L root");var child=new GameObject("L child");child.transform.SetParent(root.transform,false);UnityEngine.Object target=root;
   if(profile=="component")target=root.AddComponent<BoxCollider>();if(profile=="transform")target=root.transform;if(profile=="text"||profile=="deny-assets")target=new TextAsset("owned lifetime");if(profile=="mesh")target=new Mesh();if(profile=="null")target=null;if(profile=="unbound")target=new UnityEngine.Object();if(profile=="retired")UnityEngine.Object.DestroyImmediate(root);
   var due=Time.time+.15f;var row=new Observation{index=index,profile=profile+".call"};
   try{Invoke(index,target,profile=="positive-delay"?.08f:profile=="negative-delay"?-.5f:0,profile!="deny-assets");row.data=Snapshot(target,root,child);}catch(Exception error){row.error=error.GetType().FullName;row.data=Snapshot(target,root,child);}yield return row;
   if(profile!="positive-delay")yield return new Observation{index=index,profile=profile+".after-frame",data=Snapshot(target,root,child)};
   else{while(Time.time<due)yield return null;yield return new Observation{index=index,profile=profile+".after-delay",data=Snapshot(target,root,child)};}
   if(target&&target!=root&&!(target is Component))UnityEngine.Object.DestroyImmediate(target);if(root)UnityEngine.Object.DestroyImmediate(root);
  }
  foreach(var profile in new[]{"root","root-component","child","child-component","text","null","retired","unbound"}){
   var scene=SceneManager.CreateScene("Lifetime "+profile);var root=new GameObject("L persistent root");SceneManager.MoveGameObjectToScene(root,scene);var child=new GameObject("L persistent child");child.transform.SetParent(root.transform,false);UnityEngine.Object target=profile=="root-component"?root.transform:profile=="child"?child:profile=="child-component"?child.transform:profile=="text"?new TextAsset("persistent asset"):profile=="null"?null:profile=="unbound"?new UnityEngine.Object():root;
   if(profile=="retired")UnityEngine.Object.DestroyImmediate(root);var row=new Observation{index=6,profile=profile+".call"};try{UnityEngine.Object.DontDestroyOnLoad(target);row.data=Snapshot(target,root,child)+":"+(root?root.scene.name=="DontDestroyOnLoad":false)+":"+(child?child.scene.name=="DontDestroyOnLoad":false);}catch(Exception error){row.error=error.GetType().FullName;row.data=Snapshot(target,root,child);}yield return row;
   var unload=SceneManager.UnloadSceneAsync(scene);for(var i=0;unload!=null&&!unload.isDone&&i<120;i++)yield return null;yield return new Observation{index=6,profile=profile+".unloaded",data=Snapshot(target,root,child)+":"+(unload==null||unload.isDone)};
   if(target&&target!=root&&!(target is Component))UnityEngine.Object.DestroyImmediate(target);if(root)UnityEngine.Object.DestroyImmediate(root);
  }
 }
 private static void Invoke(int index,UnityEngine.Object target,float delay,bool allow){switch(index){case 0:UnityEngine.Object.Destroy(target);break;case 1:UnityEngine.Object.Destroy(target,delay);break;case 2:UnityEngine.Object.DestroyImmediate(target);break;case 3:UnityEngine.Object.DestroyImmediate(target,allow);break;case 4:UnityEngine.Object.DestroyObject(target);break;case 5:UnityEngine.Object.DestroyObject(target,delay);break;default:throw new ArgumentOutOfRangeException(nameof(index));}}
}
}
