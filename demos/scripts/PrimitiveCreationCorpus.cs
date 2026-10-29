using System;
using System.Linq;
using UnityEngine;
namespace Game {
public static class PrimitiveCreationCorpus {
 public static void Run(Action<string,string> record){
  Action<string,Func<object>> observe=(key,call)=>{try{var value=call();record(key,value==null?"null":Convert.ToString(value,System.Globalization.CultureInfo.InvariantCulture));}catch(Exception error){record(key,"error:"+error.GetType().FullName);}};
  foreach(var type in new[]{PrimitiveType.Sphere,PrimitiveType.Capsule,PrimitiveType.Cylinder,PrimitiveType.Cube,PrimitiveType.Plane,PrimitiveType.Quad}){
   GameObject go=null;observe("create."+type,()=>{go=GameObject.CreatePrimitive(type);return go.name+":"+string.Join(",",go.GetComponents<Component>().Select(component=>component.GetType().Name))+":"+(go.transform.position==Vector3.zero&&go.transform.rotation==Quaternion.identity&&go.transform.localScale==Vector3.one)+":"+go.layer+":"+go.tag+":"+go.activeSelf;});
   observe("mesh."+type,()=>{var filter=go.GetComponent<MeshFilter>();var renderer=go.GetComponent<MeshRenderer>();var collider=go.GetComponent<Collider>();return filter&&filter.sharedMesh&&renderer&&renderer.enabled&&collider&&collider.enabled&&!collider.isTrigger&&(!(collider is MeshCollider mesh)||mesh.sharedMesh==filter.sharedMesh);});
   if(go)UnityEngine.Object.DestroyImmediate(go);
  }
  observe("create.invalid",()=>{var go=GameObject.CreatePrimitive((PrimitiveType)99);var wasNull=go==null;if(go)UnityEngine.Object.DestroyImmediate(go);return wasNull;});
 }
}
}
