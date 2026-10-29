using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Game {
public static class ComponentCreationCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){for(var index=0;index<2;index++)foreach(var kind in Kinds)foreach(var profile in new[]{"fresh","duplicate","retired","null-receiver","mixed","rendered"}){if(index==1&&(kind=="string"||kind=="null"))continue;if(profile=="mixed"&&kind!="BoxCollider"&&kind!="SphereCollider"&&kind!="CapsuleCollider")continue;if(profile=="rendered"&&kind!="MeshCollider"&&kind!="MeshFilter"&&kind!="MeshRenderer")continue;yield return Evaluate(index,kind,profile);}}
 private static readonly string[] Kinds={"Transform","Component","Behaviour","MonoBehaviour","Collider","BoxCollider","SphereCollider","CapsuleCollider","MeshCollider","MeshFilter","Renderer","MeshRenderer","RepeatedScriptProbe","UniqueScriptProbe","RequiredBoxProbe","string","null"};
 private static Observation Evaluate(int index,string kind,string profile){var row=new Observation{index=index,profile=kind+"."+profile};var go=profile=="rendered"?GameObject.CreatePrimitive(PrimitiveType.Cube):new GameObject("C "+kind);if(profile=="mixed")go.AddComponent<BoxCollider>();if(profile=="retired")UnityEngine.Object.DestroyImmediate(go);var target=profile=="null-receiver"?null:go;Component first=null,second=null;
  try{first=Invoke(index,kind,target);if(profile=="duplicate"||profile=="mixed")second=Invoke(index,kind,target);row.data=(first?first.GetType().Name:"null")+":"+(second?second.GetType().Name:"null")+":"+(first&&second?first!=second:false)+":"+go.GetComponentCount()+":"+string.Join(",",go.GetComponents<Component>().Select(value=>value.GetType().Name));
   if(first is BoxCollider box)row.data+=":"+(box.size==Vector3.one);if(first is SphereCollider sphere)row.data+=":"+sphere.radius.ToString("R",System.Globalization.CultureInfo.InvariantCulture);if(first is CapsuleCollider capsule)row.data+=":"+capsule.radius.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+":"+capsule.height.ToString("R",System.Globalization.CultureInfo.InvariantCulture);if(first is MeshCollider mesh)row.data+=":"+(mesh.sharedMesh==null)+":"+mesh.enabled+":"+mesh.isTrigger;if(first is MeshFilter filter)row.data+=":"+(filter.sharedMesh==null);if(first is MeshRenderer renderer)row.data+=":"+renderer.enabled;
   if(first is BoxCollider a&&second is BoxCollider b){a.size=new Vector3(2,3,4);a.enabled=false;a.isTrigger=true;row.data+=":"+(a.size==new Vector3(2,3,4))+":"+(b.size==Vector3.one)+":"+a.enabled+":"+b.enabled+":"+a.isTrigger+":"+b.isTrigger;}
  }catch(Exception error){row.error=error.GetType().FullName;}finally{if(go)UnityEngine.Object.DestroyImmediate(go);}return row;
 }
 private static Component Invoke(int index,string kind,GameObject go){if(index==0)return go.AddComponent(Resolve(kind));switch(kind){case "Transform":return go.AddComponent<Transform>();
case "Component":return go.AddComponent<Component>();
case "Behaviour":return go.AddComponent<Behaviour>();
case "MonoBehaviour":return go.AddComponent<MonoBehaviour>();
case "Collider":return go.AddComponent<Collider>();
case "BoxCollider":return go.AddComponent<BoxCollider>();
case "SphereCollider":return go.AddComponent<SphereCollider>();
case "CapsuleCollider":return go.AddComponent<CapsuleCollider>();
case "MeshCollider":return go.AddComponent<MeshCollider>();
case "MeshFilter":return go.AddComponent<MeshFilter>();
case "Renderer":return go.AddComponent<Renderer>();
case "MeshRenderer":return go.AddComponent<MeshRenderer>();
case "RepeatedScriptProbe":return go.AddComponent<RepeatedScriptProbe>();
case "UniqueScriptProbe":return go.AddComponent<UniqueScriptProbe>();
case "RequiredBoxProbe":return go.AddComponent<RequiredBoxProbe>(); default:throw new ArgumentOutOfRangeException(nameof(kind));}}
 private static Type Resolve(string kind){switch(kind){case "Transform":return typeof(Transform);
case "Component":return typeof(Component);
case "Behaviour":return typeof(Behaviour);
case "MonoBehaviour":return typeof(MonoBehaviour);
case "Collider":return typeof(Collider);
case "BoxCollider":return typeof(BoxCollider);
case "SphereCollider":return typeof(SphereCollider);
case "CapsuleCollider":return typeof(CapsuleCollider);
case "MeshCollider":return typeof(MeshCollider);
case "MeshFilter":return typeof(MeshFilter);
case "Renderer":return typeof(Renderer);
case "MeshRenderer":return typeof(MeshRenderer);
case "RepeatedScriptProbe":return typeof(RepeatedScriptProbe);
case "UniqueScriptProbe":return typeof(UniqueScriptProbe);
case "RequiredBoxProbe":return typeof(RequiredBoxProbe);
case "string":return typeof(string); case "null":return null;default:throw new ArgumentOutOfRangeException(nameof(kind));}}
}
}
