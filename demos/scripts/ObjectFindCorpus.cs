using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
public static class ObjectFindCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){for(var index=0;index<23;index++)foreach(var profile in Profiles(index))yield return Evaluate(index,profile);}
 private static string[] Profiles(int index){switch(index){case 0:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 1:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","unsorted","null-type","invalid-type","assets"};case 2:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 3:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 4:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 5:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","unsorted"};case 6:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","unsorted"};case 7:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type","assets"};case 8:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type","assets"};case 9:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type","assets"};case 10:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type","assets"};case 11:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 12:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type","assets"};case 13:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 14:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 15:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 16:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","unsorted","null-type","invalid-type","assets"};case 17:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired","null-type","invalid-type"};case 18:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 19:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 20:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 21:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};case 22:return new[]{"mixed","include-inactive","all-inactive","dont-save","retired"};default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static Observation Evaluate(int index,string profile){
  var row=new Observation{index=index,profile=profile};TextAsset text=null,textFlags=null;if(profile=="assets"){text=new TextAsset("find text");text.name="F text";textFlags=new TextAsset("find hidden text");textFlags.name="F text flags";textFlags.hideFlags=HideFlags.DontSave;}var root=new GameObject("F active");var off=new GameObject("F off");var nested=new GameObject("F nested");nested.transform.SetParent(off.transform,false);var hidden=new GameObject("F flags");
  var known=new[]{root.AddComponent<SphereCollider>(),off.AddComponent<SphereCollider>(),nested.AddComponent<SphereCollider>(),hidden.AddComponent<SphereCollider>()};foreach(var value in known){var id=value.GetInstanceID();}off.SetActive(false);
  if(profile=="all-inactive"){root.SetActive(false);hidden.SetActive(false);}if(profile=="dont-save")hidden.hideFlags=HideFlags.DontSave;
  if(profile=="retired"){UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(off);UnityEngine.Object.DestroyImmediate(hidden);}
  try{row.data=Invoke(index,profile,known);}catch(Exception error){row.error=error.GetType().FullName;}finally{if(root)UnityEngine.Object.DestroyImmediate(root);if(off)UnityEngine.Object.DestroyImmediate(off);if(hidden)UnityEngine.Object.DestroyImmediate(hidden);if(text)UnityEngine.Object.DestroyImmediate(text);if(textFlags)UnityEngine.Object.DestroyImmediate(textFlags);}
  return row;
 }
 private static string Invoke(int index,string profile,SphereCollider[] known){var type=profile=="null-type"?null:profile=="invalid-type"?typeof(string):profile=="assets"?typeof(TextAsset):typeof(SphereCollider);var include=profile=="include-inactive";switch(index){case 0:{return ScalarResult(UnityEngine.Object.FindFirstObjectByType(type),known,include,false);}
case 1:{return ArrayResult(UnityEngine.Object.FindObjectsByType(type,include?FindObjectsInactive.Include:FindObjectsInactive.Exclude,profile=="unsorted"?FindObjectsSortMode.None:FindObjectsSortMode.InstanceID),profile!="unsorted");}
case 2:{return ScalarResult(UnityEngine.Object.FindAnyObjectByType<SphereCollider>(),known,include,true);}
case 3:{return ArrayResult(UnityEngine.Object.FindObjectsOfType<SphereCollider>(),true);}
case 4:{return ScalarResult(UnityEngine.Object.FindFirstObjectByType(type,include?FindObjectsInactive.Include:FindObjectsInactive.Exclude),known,include,false);}
case 5:{return ArrayResult(UnityEngine.Object.FindObjectsByType<SphereCollider>(profile=="unsorted"?FindObjectsSortMode.None:FindObjectsSortMode.InstanceID),profile!="unsorted");}
case 6:{return ArrayResult(UnityEngine.Object.FindObjectsByType<SphereCollider>(include?FindObjectsInactive.Include:FindObjectsInactive.Exclude,profile=="unsorted"?FindObjectsSortMode.None:FindObjectsSortMode.InstanceID),profile!="unsorted");}
case 7:{return ArrayResult(UnityEngine.Object.FindObjectsOfType(type,include),true);}
case 8:{return ArrayResult(UnityEngine.Object.FindObjectsOfType(type),true);}
case 9:{return ArrayResult(UnityEngine.Object.FindObjectsOfTypeIncludingAssets(type),false);}
case 10:{return ArrayResult(UnityEngine.Object.FindObjectsOfTypeAll(type),false);}
case 11:{return ScalarResult(UnityEngine.Object.FindObjectOfType<SphereCollider>(include),known,include,false);}
case 12:{return ArrayResult(UnityEngine.Object.FindSceneObjectsOfType(type),true);}
case 13:{return ScalarResult(UnityEngine.Object.FindAnyObjectByType(type),known,include,true);}
case 14:{return ScalarResult(UnityEngine.Object.FindAnyObjectByType(type,include?FindObjectsInactive.Include:FindObjectsInactive.Exclude),known,include,true);}
case 15:{return ScalarResult(UnityEngine.Object.FindObjectOfType(type,include),known,include,false);}
case 16:{return ArrayResult(UnityEngine.Object.FindObjectsByType(type,profile=="unsorted"?FindObjectsSortMode.None:FindObjectsSortMode.InstanceID),profile!="unsorted");}
case 17:{return ScalarResult(UnityEngine.Object.FindObjectOfType(type),known,include,false);}
case 18:{return ScalarResult(UnityEngine.Object.FindObjectOfType<SphereCollider>(),known,include,false);}
case 19:{return ArrayResult(UnityEngine.Object.FindObjectsOfType<SphereCollider>(include),true);}
case 20:{return ScalarResult(UnityEngine.Object.FindFirstObjectByType<SphereCollider>(),known,include,false);}
case 21:{return ScalarResult(UnityEngine.Object.FindFirstObjectByType<SphereCollider>(include?FindObjectsInactive.Include:FindObjectsInactive.Exclude),known,include,false);}
case 22:{return ScalarResult(UnityEngine.Object.FindAnyObjectByType<SphereCollider>(include?FindObjectsInactive.Include:FindObjectsInactive.Exclude),known,include,true);} default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static string ArrayResult(UnityEngine.Object[] values,bool sorted){if(values==null)return "null-array";var names=values.Select(value=>value is Component component?component.gameObject.name:value?value.name:"null").Where(value=>value.StartsWith("F ",StringComparison.Ordinal)).OrderBy(value=>value,StringComparer.Ordinal);var ordered=true;if(sorted)for(var i=1;i<values.Length;i++)ordered&=values[i-1].GetInstanceID()<=values[i].GetInstanceID();return string.Join(",",names)+"/ordered:"+ordered;}
 private static string ScalarResult(UnityEngine.Object value,SphereCollider[] known,bool include,bool any){if(!value)return "null";var eligible=known.Where(item=>item&&(include||item.gameObject.activeInHierarchy)&&(item.hideFlags&HideFlags.DontSaveInEditor)==0).ToArray();return "member:"+known.Any(item=>item==value)+",eligible:"+eligible.Any(item=>item==value)+(any?"":",lowest:"+(eligible.Length>0&&value==eligible.OrderBy(item=>item.GetInstanceID()).First()));}
}
}
