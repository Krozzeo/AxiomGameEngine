using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
public static class TagSearchCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){for(var index=0;index<4;index++)foreach(var profile in new[]{"Player","Finish","Untagged","unknown","null","empty","inactive","retired","explicit-untagged","null-results","null-tag-and-results","unknown-tag-and-results"}){if(profile.Contains("results")&&index!=3)continue;yield return Evaluate(index,profile);}}
 private static Observation Evaluate(int index,string profile){var row=new Observation{index=index,profile=profile};var root=new GameObject("T root");var child=new GameObject("T child");child.transform.SetParent(root.transform,false);var off=new GameObject("T off");var untagged=new GameObject("T untagged");root.tag="Player";child.tag="Player";off.tag="Player";off.SetActive(false);if(profile=="inactive")root.SetActive(false);if(profile=="retired")UnityEngine.Object.DestroyImmediate(root);if(profile=="explicit-untagged")untagged.tag="Untagged";
  var tag=profile=="Finish"?"Finish":profile=="Untagged"||profile=="explicit-untagged"?"Untagged":profile=="unknown"||profile=="unknown-tag-and-results"?"Unknown M20 tag":profile=="null"||profile=="null-tag-and-results"?null:profile=="empty"?"":"Player";var list=profile.Contains("results")?null:new List<GameObject>{off};
  try{switch(index){case 0:row.data=Single(GameObject.FindWithTag(tag),tag,root,child);break;case 1:row.data=Single(GameObject.FindGameObjectWithTag(tag),tag,root,child);break;case 2:row.data=Many(GameObject.FindGameObjectsWithTag(tag));break;case 3:GameObject.FindGameObjectsWithTag(tag,list);break;default:throw new ArgumentOutOfRangeException(nameof(index));}}catch(Exception error){row.error=error.GetType().FullName;}if(index==3)row.data=list==null?"null-list":Many(list.ToArray());
  if(root)UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(off);UnityEngine.Object.DestroyImmediate(untagged);return row;
 }
 private static string Single(GameObject found,string tag,GameObject root,GameObject child)=>!found?"null":tag=="Untagged"?found.CompareTag("Untagged")?"valid-tag":"wrong-tag":found==root||found==child?"member":found.name.StartsWith("T ",StringComparison.Ordinal)?"inactive":"foreign";
 private static string Many(GameObject[] values)=>values==null?"null-array":string.Join(",",values.Where(value=>value.name.StartsWith("T ",StringComparison.Ordinal)).Select(value=>value.name).OrderBy(value=>value,StringComparer.Ordinal));
}
}
