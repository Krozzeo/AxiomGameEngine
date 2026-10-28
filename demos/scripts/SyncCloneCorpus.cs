using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
public static class SyncCloneCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="",meta="";}
 public static IEnumerable<Observation> Run(){for(var index=0;index<13;index++)foreach(var profile in new[]{"regular","world","null-parent","component","null-source","retired-source","retired-parent"})yield return Evaluate(index,profile);}
 private static Observation Evaluate(int index,string profile){var row=new Observation{index=index,profile=profile};var ancestor=new GameObject("S ancestor");ancestor.transform.position=new Vector3(3,-2,5);ancestor.transform.rotation=Quaternion.Euler(0,30,15);ancestor.transform.localScale=new Vector3(-2,1,3);var source=new GameObject("S source");source.transform.SetParent(ancestor.transform,false);source.transform.localPosition=new Vector3(1,2,3);source.transform.localRotation=Quaternion.Euler(20,40,45);source.transform.localScale=new Vector3(.5f,2,3);source.AddComponent<BoxCollider>();source.tag="Player";source.layer=7;source.isStatic=true;var nested=new GameObject("S nested");nested.transform.SetParent(source.transform,false);nested.SetActive(false);var destination=new GameObject("S destination");destination.transform.position=new Vector3(10,2,-1);destination.transform.rotation=Quaternion.Euler(10,15,20);destination.transform.localScale=new Vector3(3,2,4);var parent=destination.transform;var originalTransform=source.transform;
  if(profile=="retired-source")UnityEngine.Object.DestroyImmediate(ancestor);if(profile=="retired-parent")UnityEngine.Object.DestroyImmediate(destination);if(profile=="null-parent")parent=null;UnityEngine.Object copy=null;
  try{copy=profile=="component"?Invoke<Transform>(index,originalTransform,parent,profile=="world"):Invoke<GameObject>(index,profile=="null-source"?null:source,parent,profile=="world");var go=copy is GameObject game?game:((Component)copy).gameObject;var t=go.transform;row.data=Encode(V(t.position).Concat(Q(t.rotation)).Concat(V(t.localPosition)).Concat(Q(t.localRotation)).Concat(V(t.localScale)).ToArray());row.meta=(copy!=null&&copy!=(profile=="component"?(UnityEngine.Object)originalTransform:source))+":"+(profile=="component"?copy is Transform:copy is GameObject)+":"+(go.name=="S source(Clone)")+":"+(t.parent==parent)+":"+(t.parent==null)+":"+go.layer+":"+go.tag+":"+go.isStatic+":"+t.childCount+":"+(t.GetChild(0).name=="S nested")+":"+t.GetChild(0).gameObject.activeSelf;
  }catch(Exception error){row.error=error.GetType().FullName;}finally{if(copy){var go=copy is GameObject game?game:((Component)copy).gameObject;UnityEngine.Object.DestroyImmediate(go);}if(ancestor)UnityEngine.Object.DestroyImmediate(ancestor);if(destination)UnityEngine.Object.DestroyImmediate(destination);}return row;
 }
 private static UnityEngine.Object Invoke<T>(int index,T original,Transform parent,bool world)where T:UnityEngine.Object{switch(index){case 0:return UnityEngine.Object.Instantiate<T>(original,parent);
case 1:return UnityEngine.Object.Instantiate<T>(original,new Vector3(8,2,1),Quaternion.Euler(5,15,25),parent);
case 2:return UnityEngine.Object.Instantiate(original,parent,world);
case 3:return UnityEngine.Object.Instantiate(original,new Vector3(8,2,1),Quaternion.Euler(5,15,25),parent);
case 4:return UnityEngine.Object.Instantiate<T>(original,parent,world);
case 5:return UnityEngine.Object.Instantiate(original,parent);
case 6:return UnityEngine.Object.Instantiate(original,UnityEngine.SceneManagement.SceneManager.GetActiveScene());
case 7:return UnityEngine.Object.Instantiate(original,new Vector3(8,2,1),Quaternion.Euler(5,15,25));
case 8:return UnityEngine.Object.Instantiate<T>(original,new InstantiateParameters{parent=parent,worldSpace=world});
case 9:return UnityEngine.Object.Instantiate(original);
case 10:return UnityEngine.Object.Instantiate<T>(original);
case 11:return UnityEngine.Object.Instantiate<T>(original,new Vector3(8,2,1),Quaternion.Euler(5,15,25),new InstantiateParameters{parent=parent,worldSpace=world});
case 12:return UnityEngine.Object.Instantiate<T>(original,new Vector3(8,2,1),Quaternion.Euler(5,15,25)); default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static float[] V(Vector3 v)=>new[]{v.x,v.y,v.z};private static float[] Q(Quaternion q)=>new[]{q.x,q.y,q.z,q.w};private static string Encode(float[] values){var bytes=new byte[values.Length*4];Buffer.BlockCopy(values,0,bytes,0,bytes.Length);return Convert.ToBase64String(bytes);}
}
}
