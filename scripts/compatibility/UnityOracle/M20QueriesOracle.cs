using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class M20QueriesOracle {
 [Serializable] public sealed class Report {public string unityVersion,coreAssemblySha256,sourceSha256,error;public string profile="Native Editor exact Component/GameObject query overloads";public List<Game.ComponentQueryCorpus.Observation> rows=new List<Game.ComponentQueryCorpus.Observation>();}
 public static void Run(){var report=new Report{unityVersion=Application.unityVersion};try{EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);report.coreAssemblySha256=Hash(typeof(Transform).Assembly.Location);report.sourceSha256=Hash(Path.Combine(Application.dataPath,"ComponentQueryCorpus.cs"));foreach(var row in Game.ComponentQueryCorpus.Run())report.rows.Add(row);}catch(Exception error){report.error=error.ToString();}File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"oracle-query-facts.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(report.error==null?0:2);}
 private static string Hash(string path){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();}
}
