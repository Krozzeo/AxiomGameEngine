using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad] public static class M20ComponentCreationOracle {
 [Serializable] public sealed class Fact {public string key,value;}
 [Serializable] public sealed class Report {public string unityVersion,coreAssemblySha256,sourceSha256,error;public List<Fact> receiverSourceHashes=new List<Fact>();public string profile="Native Play component creation probes";public List<Game.ComponentCreationCorpus.Observation> rows=new List<Game.ComponentCreationCorpus.Observation>();}
 static M20ComponentCreationOracle(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){SessionState.SetBool("Axiom.M20.BulkOracle",true);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.EnterPlaymode();}
 private static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("Axiom.M20.BulkOracle",false))return;SessionState.SetBool("Axiom.M20.BulkOracle",false);var report=new Report{unityVersion=Application.unityVersion};try{report.coreAssemblySha256=Hash(typeof(Transform).Assembly.Location);report.sourceSha256=Hash(Path.Combine(Application.dataPath,"ComponentCreationCorpus.cs"));foreach(var name in new[]{"RepeatedScriptProbe.cs","UniqueScriptProbe.cs","RequiredBoxProbe.cs"})report.receiverSourceHashes.Add(new Fact{key=name,value=Hash(Path.Combine(Application.dataPath,name))});foreach(var row in Game.ComponentCreationCorpus.Run())report.rows.Add(row);}catch(Exception error){report.error=error.ToString();}File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"oracle-component-creation-facts.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(report.error==null?0:2);}
 private static string Hash(string path){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();}
}
