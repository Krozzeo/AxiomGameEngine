using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
// Same owned direct public calls execute without source adaptation in Unity, .NET and Wasm.
public static class ComponentQueryCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){for(var index=0;index<56;index++)foreach(var profile in Profiles(index))yield return Evaluate(index,profile);}
 private static string[] Profiles(int index){switch(index){case 0:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 1:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 2:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 3:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","child-only","inactive-target","disabled-component"};case 4:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","parent-only","disabled-component"};case 5:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 6:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 7:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 8:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-list","child-only","inactive-target","disabled-component"};case 9:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","parent-only","disabled-component"};case 10:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","parent-only","disabled-component"};case 11:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 12:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 13:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","child-only","inactive-target","disabled-component"};case 14:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-list","child-only","inactive-target","disabled-component"};case 15:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","parent-only","disabled-component"};case 16:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","parent-only","disabled-component"};case 17:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","child-only","inactive-target","disabled-component"};case 18:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","null-list","disabled-component"};case 19:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 20:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-list","disabled-component"};case 21:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","parent-only","disabled-component"};case 22:return new[]{"regular","inactive-self","inactive-parent","missing","retired","child-only","inactive-target","disabled-component"};case 23:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 24:return new[]{"regular","inactive-self","inactive-parent","missing","retired","child-only","inactive-target","disabled-component"};case 25:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 26:return new[]{"regular","inactive-self","inactive-parent","missing","retired","parent-only","disabled-component"};case 27:return new[]{"regular","inactive-self","inactive-parent","missing","retired","parent-only","disabled-component"};case 28:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 29:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-list","disabled-component"};case 30:return new[]{"regular","inactive-self","inactive-parent","missing","retired","parent-only","disabled-component"};case 31:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-string","empty-string","disabled-component"};case 32:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 33:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 34:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-list","parent-only","disabled-component"};case 35:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-list","parent-only","disabled-component"};case 36:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","parent-only","disabled-component"};case 37:return new[]{"regular","inactive-self","inactive-parent","missing","retired","parent-only","disabled-component"};case 38:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 39:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","parent-only","disabled-component"};case 40:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","null-list","disabled-component"};case 41:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 42:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","child-only","inactive-target","disabled-component"};case 43:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","disabled-component"};case 44:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","parent-only","disabled-component"};case 45:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-list","child-only","inactive-target","disabled-component"};case 46:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","parent-only","disabled-component"};case 47:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","parent-only","disabled-component"};case 48:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-type","invalid-type","child-only","inactive-target","disabled-component"};case 49:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-string","empty-string","disabled-component"};case 50:return new[]{"regular","inactive-self","inactive-parent","missing","retired","child-only","inactive-target","disabled-component"};case 51:return new[]{"regular","inactive-self","inactive-parent","missing","retired","disabled-component"};case 52:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","parent-only","disabled-component"};case 53:return new[]{"regular","inactive-self","inactive-parent","missing","retired","child-only","inactive-target","disabled-component"};case 54:return new[]{"regular","inactive-self","inactive-parent","missing","retired","include-inactive","null-list","child-only","inactive-target","disabled-component"};case 55:return new[]{"regular","inactive-self","inactive-parent","missing","retired","null-type","invalid-type","child-only","inactive-target","disabled-component"};default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static Observation Evaluate(int index,string profile){
  var row=new Observation{index=index,profile=profile};var ancestor=new GameObject("Q ancestor");var parent=new GameObject("Q parent");var owner=new GameObject("Q owner");var child=new GameObject("Q child");var inactive=new GameObject("Q inactive");
  parent.transform.SetParent(ancestor.transform,false);owner.transform.SetParent(parent.transform,false);child.transform.SetParent(owner.transform,false);inactive.transform.SetParent(child.transform,false);
  ancestor.AddComponent<SphereCollider>();parent.AddComponent<BoxCollider>();owner.AddComponent<SphereCollider>();child.AddComponent<BoxCollider>();inactive.AddComponent<SphereCollider>();inactive.SetActive(false);var proxy=owner.transform;
  if(profile=="disabled-component")owner.GetComponent<SphereCollider>().enabled=false;
  if(profile=="inactive-target"){UnityEngine.Object.DestroyImmediate(owner.GetComponent<SphereCollider>());}
  if(profile=="inactive-self")owner.SetActive(false);if(profile=="inactive-parent")parent.SetActive(false);
  if(profile=="retired")UnityEngine.Object.DestroyImmediate(ancestor);
  try{row.data=profile=="missing"?Invoke<MeshCollider>(index,owner,proxy,profile):profile=="child-only"||profile=="parent-only"?Invoke<BoxCollider>(index,owner,proxy,profile):profile=="inactive-target"?Invoke<SphereCollider>(index,owner,proxy,profile):Invoke<Collider>(index,owner,proxy,profile);}catch(Exception error){row.error=error.GetType().FullName;}finally{if(ancestor)UnityEngine.Object.DestroyImmediate(ancestor);}
  return row;
 }
 private static string Invoke<T>(int index,GameObject owner,Component proxy,string profile) where T:Component {
  var include=profile=="include-inactive"||profile=="inactive-target";var type=profile=="null-type"?null:profile=="invalid-type"?typeof(string):typeof(T);var text=profile=="null-string"?null:profile=="empty-string"?"":typeof(T).Name;
  switch(index){case 0:{return Describe(proxy.GetComponent(type));}
case 1:{var foundValue=proxy.TryGetComponent<T>(out var found);return foundValue+":"+Describe(found);}
case 2:{return DescribeList(owner.GetComponentsInChildren(type));}
case 3:{return DescribeList(proxy.GetComponentsInChildren<T>(include));}
case 4:{return Describe(owner.GetComponentInParent(type,include));}
case 5:{return DescribeList(owner.GetComponents(type));}
case 6:{return DescribeList(owner.GetComponentsInChildren(type,include));}
case 7:{return DescribeList(proxy.GetComponents(type));}
case 8:{var values=profile=="null-list"?null:new List<T>{default(T)};owner.GetComponentsInChildren<T>(include,values);return DescribeList(values);}
case 9:{return DescribeList(owner.GetComponentsInParent<T>(include));}
case 10:{return Describe(proxy.GetComponentInParent(type,include));}
case 11:{var foundValue=owner.TryGetComponent(type,out var found);return foundValue+":"+Describe(found);}
case 12:{var foundValue=owner.TryGetComponent<T>(out var found);return foundValue+":"+Describe(found);}
case 13:{return DescribeList(owner.GetComponentsInChildren<T>(include));}
case 14:{var values=profile=="null-list"?null:new List<T>{default(T)};proxy.GetComponentsInChildren<T>(values);return DescribeList(values);}
case 15:{return DescribeList(proxy.GetComponentsInParent(type));}
case 16:{return Describe(owner.GetComponentInParent<T>(include));}
case 17:{return Describe(owner.GetComponentInChildren<T>(include));}
case 18:{var values=profile=="null-list"?null:new List<Component>{owner.transform};proxy.GetComponents(type,values);return DescribeList(values);}
case 19:{return Describe(owner.GetComponent<T>());}
case 20:{var values=profile=="null-list"?null:new List<T>{default(T)};owner.GetComponents<T>(values);return DescribeList(values);}
case 21:{return Describe(proxy.GetComponentInParent(type));}
case 22:{return Describe(owner.GetComponentInChildren<T>());}
case 23:{return DescribeList(proxy.GetComponentsInChildren(type,include));}
case 24:{return DescribeList(owner.GetComponentsInChildren<T>());}
case 25:{return DescribeList(owner.GetComponents<T>());}
case 26:{return Describe(proxy.GetComponentInParent<T>());}
case 27:{return DescribeList(proxy.GetComponentsInParent<T>());}
case 28:{return Describe(proxy.GetComponentInChildren(type,include));}
case 29:{var values=profile=="null-list"?null:new List<T>{default(T)};proxy.GetComponents<T>(values);return DescribeList(values);}
case 30:{return Describe(owner.GetComponentInParent<T>());}
case 31:{return Describe(owner.GetComponent(text));}
case 32:{return DescribeList(proxy.GetComponents<T>());}
case 33:{return Describe(proxy.GetComponentInChildren(type));}
case 34:{var values=profile=="null-list"?null:new List<T>{default(T)};proxy.GetComponentsInParent<T>(include,values);return DescribeList(values);}
case 35:{var values=profile=="null-list"?null:new List<T>{default(T)};owner.GetComponentsInParent<T>(include,values);return DescribeList(values);}
case 36:{return DescribeList(owner.GetComponentsInParent(type));}
case 37:{return DescribeList(owner.GetComponentsInParent<T>());}
case 38:{return Describe(owner.GetComponentInChildren(type));}
case 39:{return DescribeList(proxy.GetComponentsInParent<T>(include));}
case 40:{var values=profile=="null-list"?null:new List<Component>{owner.transform};owner.GetComponents(type,values);return DescribeList(values);}
case 41:{return Describe(owner.GetComponent(type));}
case 42:{return Describe(proxy.GetComponentInChildren<T>(include));}
case 43:{var foundValue=proxy.TryGetComponent(type,out var found);return foundValue+":"+Describe(found);}
case 44:{return Describe(proxy.GetComponentInParent<T>(include));}
case 45:{var values=profile=="null-list"?null:new List<T>{default(T)};owner.GetComponentsInChildren<T>(values);return DescribeList(values);}
case 46:{return DescribeList(owner.GetComponentsInParent(type,include));}
case 47:{return DescribeList(proxy.GetComponentsInParent(type,include));}
case 48:{return Describe(owner.GetComponentInChildren(type,include));}
case 49:{return Describe(proxy.GetComponent(text));}
case 50:{return DescribeList(proxy.GetComponentsInChildren<T>());}
case 51:{return Describe(proxy.GetComponent<T>());}
case 52:{return Describe(owner.GetComponentInParent(type));}
case 53:{return Describe(proxy.GetComponentInChildren<T>());}
case 54:{var values=profile=="null-list"?null:new List<T>{default(T)};proxy.GetComponentsInChildren<T>(include,values);return DescribeList(values);}
case 55:{return DescribeList(proxy.GetComponentsInChildren(type));} default:throw new ArgumentOutOfRangeException(nameof(index));}
 }
 private static string Describe(object value){if(ReferenceEquals(value,null))return "null";var component=value as Component;if(component)return component.gameObject.name+":"+component.GetType().Name;return value.ToString();}
 private static string DescribeList(IEnumerable values){if(values==null)return "null-list";var result=new List<string>();foreach(var value in values)result.Add(Describe(value));return string.Join(",",result);}
}
}
