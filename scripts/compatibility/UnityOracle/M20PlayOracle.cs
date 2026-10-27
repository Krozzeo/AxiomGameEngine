using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using U=UnityEngine.Object;

// Our own public-API observations. Play mode is separate from the edit-mode sample.
[InitializeOnLoad] public static class M20PlayOracle {
 [Serializable] public class Fact {public string key,value;}
 [Serializable] public class Report {public string unityVersion,coreAssemblySha256,profile="Native Play mode: direct synchronous clone and world-setter observations; not complete lifecycle acceptance",error;public List<Fact> facts=new List<Fact>();}
 static M20PlayOracle(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){SessionState.SetBool("Axiom.M20.PlayOracle",true);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.EnterPlaymode();}
 static string V(Vector3 v)=>v.x.ToString("R",CultureInfo.InvariantCulture)+","+v.y.ToString("R",CultureInfo.InvariantCulture)+","+v.z.ToString("R",CultureInfo.InvariantCulture);
 static string Q(Quaternion q)=>q.x.ToString("R",CultureInfo.InvariantCulture)+","+q.y.ToString("R",CultureInfo.InvariantCulture)+","+q.z.ToString("R",CultureInfo.InvariantCulture)+","+q.w.ToString("R",CultureInfo.InvariantCulture);
 static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("Axiom.M20.PlayOracle",false))return;SessionState.SetBool("Axiom.M20.PlayOracle",false);var report=new Report{unityVersion=Application.unityVersion};
  try {
   using(var hash=System.Security.Cryptography.SHA256.Create())report.coreAssemblySha256=BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(typeof(U).Assembly.Location))).Replace("-","").ToLowerInvariant();
   Action<string,object> add=(key,value)=>report.facts.Add(new Fact{key=key,value=Convert.ToString(value,CultureInfo.InvariantCulture)});
   add("isPlaying",Application.isPlaying);var parent=new GameObject("Parent");parent.transform.position=new Vector3(3,0,0);parent.transform.rotation=Quaternion.Euler(0,30,15);parent.transform.localScale=new Vector3(-2,1,1);var child=new GameObject("Child");child.transform.SetParent(parent.transform,false);child.transform.localPosition=Vector3.right;child.transform.localRotation=Quaternion.Euler(20,40,45);child.transform.localScale=new Vector3(.5f,2,3);
   add("source.position",V(child.transform.position));add("source.rotation",Q(child.transform.rotation));add("source.local-rotation",Q(child.transform.localRotation));add("source.lossy-scale",V(child.transform.lossyScale));
   Action<string,GameObject> record=(key,copy)=>{add(key+".position",V(copy.transform.position));add(key+".rotation",Q(copy.transform.rotation));add(key+".scale",V(copy.transform.localScale));U.Destroy(copy);};
   record("generic",U.Instantiate(child));record("untyped",(GameObject)U.Instantiate((U)child));record("parameters-default",U.Instantiate(child,new InstantiateParameters()));record("parameters-world",U.Instantiate(child,new InstantiateParameters{worldSpace=true}));record("parent-null-world",U.Instantiate(child,(Transform)null,true));record("explicit-pose",U.Instantiate(child,new Vector3(8,2,1),Quaternion.identity));
   record("parameters-explicit-world",U.Instantiate(child,new Vector3(8,2,1),Quaternion.identity,new InstantiateParameters{worldSpace=true}));record("parameters-explicit-local",U.Instantiate(child,new Vector3(8,2,1),Quaternion.identity,new InstantiateParameters()));record("destination-scene",(GameObject)U.Instantiate((U)child,child.scene));
   var destination=new GameObject("Destination");destination.transform.position=new Vector3(10,0,0);destination.transform.localScale=new Vector3(3,2,4);
   record("parent-world",U.Instantiate(child,destination.transform,true));record("parent-local",U.Instantiate(child,destination.transform,false));record("pose-parent",U.Instantiate(child,new Vector3(8,2,1),Quaternion.identity,destination.transform));
   record("generic-parent",U.Instantiate(child,destination.transform));record("untyped-parent",(GameObject)U.Instantiate((U)child,destination.transform));record("untyped-parent-world",(GameObject)U.Instantiate((U)child,destination.transform,true));record("untyped-pose",(GameObject)U.Instantiate((U)child,new Vector3(8,2,1),Quaternion.identity));record("untyped-pose-parent",(GameObject)U.Instantiate((U)child,new Vector3(8,2,1),Quaternion.identity,destination.transform));
   child.transform.rotation=Quaternion.identity;add("setter.rotation-local-scale",V(child.transform.localScale));child.transform.SetPositionAndRotation(new Vector3(8,2,1),Quaternion.identity);add("setter.pose-local-scale",V(child.transform.localScale));
  }catch(Exception error){report.error=error.GetType().FullName+": "+error.Message;}
  File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"oracle-play-facts.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(report.error==null?0:2);
 }
}
