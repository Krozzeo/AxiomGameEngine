using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Game {
// Owned direct public calls. Identical source executes in native Unity, Axiom .NET and Wasm.
public static class PoseTransformCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){
  for(var index=0;index<36;index++){
   foreach(var profile in new[]{"regular","mirrored","collapsed","local-mirror","double-mirror","deep-mirror","deep-collapse","collapsed-child","destroyed"})yield return Evaluate(index,profile);
   if(IsNullable(index))yield return Evaluate(index,"null");
   if(IsSpace(index)){yield return Evaluate(index,"world");yield return Evaluate(index,"world-mirrored");}if(IsZero(index))yield return Evaluate(index,"zero-input");
  }
 }
 private static bool IsNullable(int index)=>new[]{4,9,11,26}.Contains(index);
 private static bool IsSpace(int index)=>new[]{8,20,23,28,35}.Contains(index);private static bool IsZero(int index)=>new[]{0,1,5,10,12,13,16,23,24,32}.Contains(index);
 private static Observation Evaluate(int index,string profile){
  var row=new Observation{index=index,profile=profile};var root=new GameObject("Vector reference parent");var middle=new GameObject("Vector reference middle");middle.transform.SetParent(root.transform,false);var child=new GameObject("Vector reference child");child.transform.SetParent(middle.transform,false);
  root.transform.position=new Vector3(3,-2,5);root.transform.rotation=Quaternion.Euler(0,30,15);root.transform.localScale=(profile=="mirrored"||profile=="world-mirrored"||profile=="double-mirror"||profile=="deep-mirror")?new Vector3(-2,1,3):profile=="collapsed"?new Vector3(0,2,1):new Vector3(2,1,3);
  if(profile=="deep-mirror"||profile=="deep-collapse"){middle.transform.localPosition=new Vector3(0,1,0);middle.transform.localRotation=Quaternion.Euler(10,15,20);middle.transform.localScale=profile=="deep-mirror"?new Vector3(1,-2,3):new Vector3(0,2,1);}
  var target=child.transform;target.localPosition=new Vector3(1,2,3);target.localRotation=Quaternion.Euler(20,40,45);target.localScale=profile=="local-mirror"||profile=="double-mirror"?new Vector3(-.5f,2,3):profile=="collapsed-child"?new Vector3(0,2,3):new Vector3(.5f,2,3);
  var relative=profile=="null"?null:middle.transform;if(profile=="destroyed")UnityEngine.Object.DestroyImmediate(root);
  try{var result=Invoke(index,target,profile,relative);row.data=Encode(profile=="destroyed"?result:State(target).Concat(result).ToArray());}catch(Exception error){row.error=error.GetType().FullName;}finally{if(root)UnityEngine.Object.DestroyImmediate(root);}
  return row;
 }
 private static float[] Invoke(int index,Transform t,string profile,Transform relative){var space=profile.StartsWith("world")?Space.World:Space.Self;var axis=profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1);switch(index){case 0:{t.LookAt((profile=="zero-input"?t.position:new Vector3(8,2,-3)));return new float[0];}
case 1:{t.right=(profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1));return V(t.right);}
case 2:{t.Rotate(new Vector3(10,20,30));return new float[0];}
case 3:{t.rotation=Quaternion.Euler(35,15,20);return Q(t.rotation);}
case 4:{t.LookAt(relative,Vector3.up);return new float[0];}
case 5:{t.RotateAround(new Vector3(1,0,2),axis,35);return new float[0];}
case 6:{return M(t.localToWorldMatrix);}
case 7:{t.SetLocalPositionAndRotation(new Vector3(8,2,-3),Quaternion.Euler(35,15,20));return new float[0];}
case 8:{t.Rotate(new Vector3(10,20,30),space);return new float[0];}
case 9:{t.LookAt(relative);return new float[0];}
case 10:{t.LookAt((profile=="zero-input"?t.position:new Vector3(8,2,-3)),Vector3.up);return new float[0];}
case 11:{t.Translate(1.25f,-2,3.5f,relative);return new float[0];}
case 12:{t.up=(profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1));return V(t.up);}
case 13:{t.RotateAroundLocal(axis,.35f);return new float[0];}
case 14:{t.localEulerAngles=new Vector3(25,15,35);return V(t.localEulerAngles);}
case 15:{return V(t.lossyScale);}
case 16:{t.Rotate(axis,35);return new float[0];}
case 17:{t.GetPositionAndRotation(out var position,out var rotation);return V(position).Concat(Q(rotation)).ToArray();}
case 18:{t.localRotation=Quaternion.Euler(35,15,20);return Q(t.localRotation);}
case 19:{t.position=new Vector3(8,2,-3);return V(t.position);}
case 20:{t.Rotate(10,20,30,space);return new float[0];}
case 21:{t.Rotate(10,20,30);return new float[0];}
case 22:{return M(t.worldToLocalMatrix);}
case 23:{t.Rotate(axis,35,space);return new float[0];}
case 24:{t.RotateAround(axis,.35f);return new float[0];}
case 25:{t.GetLocalPositionAndRotation(out var position,out var rotation);return V(position).Concat(Q(rotation)).ToArray();}
case 26:{t.Translate(new Vector3(1.25f,-2,3.5f),relative);return new float[0];}
case 27:{t.localPosition=new Vector3(8,2,-3);return V(t.localPosition);}
case 28:{t.Translate(new Vector3(1.25f,-2,3.5f),space);return new float[0];}
case 29:{t.localScale=new Vector3(-1,.75f,2);return V(t.localScale);}
case 30:{t.Translate(new Vector3(1.25f,-2,3.5f));return new float[0];}
case 31:{t.eulerAngles=new Vector3(25,15,35);return V(t.eulerAngles);}
case 32:{t.forward=(profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1));return V(t.forward);}
case 33:{t.SetPositionAndRotation(new Vector3(8,2,-3),Quaternion.Euler(35,15,20));return new float[0];}
case 34:{t.Translate(1.25f,-2,3.5f);return new float[0];}
case 35:{t.Translate(1.25f,-2,3.5f,space);return new float[0];} default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static float[] V(Vector3 v)=>new[]{v.x,v.y,v.z};private static float[] Q(Quaternion q)=>new[]{q.x,q.y,q.z,q.w};
 private static float[] M(Matrix4x4 value){var data=new float[16];for(var i=0;i<16;i++)data[i]=value[i];return data;}
 private static float[] Flatten(Vector3[] values)=>values.SelectMany(V).ToArray();
 private static float[] State(Transform t)=>V(t.position).Concat(Q(t.rotation)).Concat(V(t.localPosition)).Concat(Q(t.localRotation)).Concat(V(t.localScale)).ToArray();
 private static string Encode(float[] values){var bytes=new byte[values.Length*4];Buffer.BlockCopy(values,0,bytes,0,bytes.Length);if(!BitConverter.IsLittleEndian)for(var i=0;i<bytes.Length;i+=4)Array.Reverse(bytes,i,4);return Convert.ToBase64String(bytes);}
}
}
