using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Game {
// Owned direct public calls. Identical source executes in native Unity, Axiom .NET and Wasm.
public static class VectorTransformCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){
  for(var index=0;index<24;index++){
   foreach(var profile in new[]{"regular","mirrored","collapsed","local-mirror","double-mirror","deep-mirror","deep-collapse","collapsed-child","destroyed"})yield return Evaluate(index,profile);
   if(IsSpan(index))yield return Evaluate(index,"empty");
   if(IsSeparate(index)){yield return Evaluate(index,"short");yield return Evaluate(index,"long");}
  }
 }
 private static bool IsSpan(int index)=>new[]{1,2,4,6,8,9,12,13,15,17,18,19}.Contains(index);
 private static bool IsSeparate(int index)=>new[]{2,9,12,13,15,18}.Contains(index);
 private static Observation Evaluate(int index,string profile){
  var row=new Observation{index=index,profile=profile};var root=new GameObject("Vector reference parent");var middle=new GameObject("Vector reference middle");middle.transform.SetParent(root.transform,false);var child=new GameObject("Vector reference child");child.transform.SetParent(middle.transform,false);
  root.transform.position=new Vector3(3,-2,5);root.transform.rotation=Quaternion.Euler(0,30,15);root.transform.localScale=(profile=="mirrored"||profile=="double-mirror"||profile=="deep-mirror")?new Vector3(-2,1,3):profile=="collapsed"?new Vector3(0,2,1):new Vector3(2,1,3);
  if(profile=="deep-mirror"||profile=="deep-collapse"){middle.transform.localPosition=new Vector3(0,1,0);middle.transform.localRotation=Quaternion.Euler(10,15,20);middle.transform.localScale=profile=="deep-mirror"?new Vector3(1,-2,3):new Vector3(0,2,1);}
  var target=child.transform;target.localPosition=new Vector3(1,2,3);target.localRotation=Quaternion.Euler(20,40,45);target.localScale=profile=="local-mirror"||profile=="double-mirror"?new Vector3(-.5f,2,3):profile=="collapsed-child"?new Vector3(0,2,3):new Vector3(.5f,2,3);
  if(profile=="destroyed")UnityEngine.Object.DestroyImmediate(root);
  try{var result=Invoke(index,target,profile=="empty"?1:profile=="short"?2:profile=="long"?3:0);row.data=Encode(profile=="destroyed"?result:State(target).Concat(result).ToArray());}catch(Exception error){row.error=error.GetType().FullName;}finally{if(root)UnityEngine.Object.DestroyImmediate(root);}
  return row;
 }
 private static float[] Invoke(int index,Transform t,int size){switch(index){case 0:{return V(t.TransformPoint(1.25f,-2,3.5f));}
case 1:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.InverseTransformDirections(new Span<Vector3>(input));return Flatten(input);}
case 2:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.InverseTransformDirections(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 3:{return V(t.InverseTransformPoint(1.25f,-2,3.5f));}
case 4:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.TransformDirections(new Span<Vector3>(input));return Flatten(input);}
case 5:{return V(t.TransformDirection(new Vector3(1.25f,-2,3.5f)));}
case 6:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.InverseTransformPoints(new Span<Vector3>(input));return Flatten(input);}
case 7:{return V(t.InverseTransformVector(1.25f,-2,3.5f));}
case 8:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.TransformPoints(new Span<Vector3>(input));return Flatten(input);}
case 9:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.InverseTransformPoints(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 10:{return V(t.InverseTransformDirection(new Vector3(1.25f,-2,3.5f)));}
case 11:{return V(t.TransformPoint(new Vector3(1.25f,-2,3.5f)));}
case 12:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.TransformVectors(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 13:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.TransformDirections(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 14:{return V(t.TransformDirection(1.25f,-2,3.5f));}
case 15:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.InverseTransformVectors(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 16:{return V(t.InverseTransformDirection(1.25f,-2,3.5f));}
case 17:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.TransformVectors(new Span<Vector3>(input));return Flatten(input);}
case 18:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.TransformPoints(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();}
case 19:{var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};t.InverseTransformVectors(new Span<Vector3>(input));return Flatten(input);}
case 20:{return V(t.InverseTransformVector(new Vector3(1.25f,-2,3.5f)));}
case 21:{return V(t.TransformVector(1.25f,-2,3.5f));}
case 22:{return V(t.InverseTransformPoint(new Vector3(1.25f,-2,3.5f)));}
case 23:{return V(t.TransformVector(new Vector3(1.25f,-2,3.5f)));} default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static float[] V(Vector3 v)=>new[]{v.x,v.y,v.z};private static float[] Q(Quaternion q)=>new[]{q.x,q.y,q.z,q.w};
 private static float[] Flatten(Vector3[] values)=>values.SelectMany(V).ToArray();
 private static float[] State(Transform t)=>V(t.position).Concat(Q(t.rotation)).Concat(V(t.localPosition)).Concat(Q(t.localRotation)).Concat(V(t.localScale)).ToArray();
 private static string Encode(float[] values){var bytes=new byte[values.Length*4];Buffer.BlockCopy(values,0,bytes,0,bytes.Length);if(!BitConverter.IsLittleEndian)for(var i=0;i<bytes.Length;i+=4)Array.Reverse(bytes,i,4);return Convert.ToBase64String(bytes);}
}
}
