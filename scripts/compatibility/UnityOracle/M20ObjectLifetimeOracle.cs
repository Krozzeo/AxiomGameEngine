using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad] public static class M20ObjectLifetimeOracle {
 [Serializable] public sealed class Report {public string unityVersion,coreAssemblySha256,sourceSha256,error;public string profile="Native Play frame-stepped object lifetime";public List<Game.ObjectLifetimeCorpus.Observation> rows=new List<Game.ObjectLifetimeCorpus.Observation>();}
 static M20ObjectLifetimeOracle(){EditorApplication.playModeStateChanged+=Changed;EditorApplication.update+=()=>{if(!SessionState.GetBool("Axiom.M20.ObjectLifetimeOracle.Active",false))return;if(EditorApplication.isPaused)EditorApplication.isPaused=false;var start=SessionState.GetFloat("Axiom.M20.ObjectLifetimeOracle.Start",0);if(EditorApplication.timeSinceStartup-start>90){SessionState.SetBool("Axiom.M20.ObjectLifetimeOracle.Active",false);Finish(new Report{error="Native lifetime watchdog expired"});}};}
 public static void Run(){SessionState.SetBool("Axiom.M20.ObjectLifetimeOracle.Active",true);SessionState.SetFloat("Axiom.M20.ObjectLifetimeOracle.Start",(float)EditorApplication.timeSinceStartup);SessionState.SetBool("Axiom.M20.ObjectLifetimeOracle",true);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.EnterPlaymode();}
 private static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("Axiom.M20.ObjectLifetimeOracle",false))return;SessionState.SetBool("Axiom.M20.ObjectLifetimeOracle",false);Time.timeScale=1;Time.captureDeltaTime=1f/60;var driver=new GameObject("Native lifetime driver").AddComponent<M20ObjectLifetimeDriver>();driver.report=new Report{unityVersion=Application.unityVersion,coreAssemblySha256=Hash(typeof(Transform).Assembly.Location),sourceSha256=Hash(Path.Combine(Application.dataPath,"ObjectLifetimeCorpus.cs"))};driver.iterator=Game.ObjectLifetimeCorpus.Run().GetEnumerator();}
 private static string Hash(string path){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();}
 public static void Finish(Report report){SessionState.SetBool("Axiom.M20.ObjectLifetimeOracle.Active",false);File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"oracle-object-lifetime-facts.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(string.IsNullOrEmpty(report.error)?0:2);}
}
public sealed class M20ObjectLifetimeDriver:MonoBehaviour {
 public M20ObjectLifetimeOracle.Report report;public IEnumerator<Game.ObjectLifetimeCorpus.Observation> iterator;private int frames;
 private void Update(){try{if(++frames>3000)throw new TimeoutException("Lifetime corpus watchdog");if(!iterator.MoveNext()){M20ObjectLifetimeOracle.Finish(report);enabled=false;return;}if(iterator.Current!=null)report.rows.Add(iterator.Current);}catch(Exception error){report.error=error.ToString();M20ObjectLifetimeOracle.Finish(report);enabled=false;}}
}
