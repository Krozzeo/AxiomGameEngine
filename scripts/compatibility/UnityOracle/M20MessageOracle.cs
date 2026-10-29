using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad] public static class M20MessageOracle {
 [Serializable] public sealed class Fact {public string key,value;}
 [Serializable] public sealed class Report {public string unityVersion,coreAssemblySha256,sourceSha256,receiverSourceSha256,error;public string profile="Native Play exact message dispatch probes";public List<Game.MessageDispatchCorpus.Observation> rows=new List<Game.MessageDispatchCorpus.Observation>();}
 static M20MessageOracle(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){SessionState.SetBool("Axiom.M20.MessageOracle",true);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.EnterPlaymode();}
 private static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("Axiom.M20.MessageOracle",false))return;SessionState.SetBool("Axiom.M20.MessageOracle",false);var report=new Report{unityVersion=Application.unityVersion};try{report.coreAssemblySha256=Hash(typeof(Transform).Assembly.Location);report.sourceSha256=Hash(Path.Combine(Application.dataPath,"MessageDispatchCorpus.cs"));report.receiverSourceSha256=Hash(Path.Combine(Application.dataPath,"MessageReceiver.cs"));foreach(var row in Game.MessageDispatchCorpus.Run())report.rows.Add(row);}catch(Exception error){report.error=error.ToString();}File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"oracle-message-facts.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(report.error==null?0:2);}
 private static string Hash(string path){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();}
}
