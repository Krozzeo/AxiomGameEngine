using System.Text.Json;
using L=Axiom.Gameplay;
using U=UnityEngine;
public static class NativeCloneReferenceCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(1501);var native=JsonDocument.Parse(File.ReadAllText("docs/compatibility/evidence/m20-unity-6000.3-play-observations.json")).RootElement;
  var facts=native.GetProperty("facts").EnumerateArray().ToDictionary(value=>value.GetProperty("key").GetString()!,value=>value.GetProperty("value").GetString()!);
  var parent=new U.GameObject("Parent");parent.transform.position=new(3,0,0);parent.transform.rotation=U.Quaternion.Euler(0,30,15);parent.transform.localScale=new(-2,1,1);var child=new U.GameObject("Child");child.transform.SetParent(parent.transform,false);child.transform.localPosition=U.Vector3.right;child.transform.localRotation=U.Quaternion.Euler(20,40,45);child.transform.localScale=new(.5f,2,3);
  var destination=new U.GameObject("Destination");destination.transform.position=new(10,0,0);destination.transform.localScale=new(3,2,4);
  var cases=new (string Key,Func<U.GameObject> Clone)[]{
   ("generic",()=>U.Object.Instantiate(child)),("untyped",()=>(U.GameObject)U.Object.Instantiate((U.Object)child)),
   ("parameters-default",()=>U.Object.Instantiate(child,new U.InstantiateParameters())),("parameters-world",()=>U.Object.Instantiate(child,new U.InstantiateParameters{worldSpace=true})),
   ("parent-null-world",()=>U.Object.Instantiate(child,(U.Transform?)null,true)),("explicit-pose",()=>U.Object.Instantiate(child,new U.Vector3(8,2,1),U.Quaternion.identity)),
   ("parameters-explicit-world",()=>U.Object.Instantiate(child,new U.Vector3(8,2,1),U.Quaternion.identity,new U.InstantiateParameters{worldSpace=true})),("parameters-explicit-local",()=>U.Object.Instantiate(child,new U.Vector3(8,2,1),U.Quaternion.identity,new U.InstantiateParameters())),
   ("destination-scene",()=>(U.GameObject)U.Object.Instantiate((U.Object)child,child.scene)),("parent-world",()=>U.Object.Instantiate(child,destination.transform,true)),("parent-local",()=>U.Object.Instantiate(child,destination.transform,false)),("pose-parent",()=>U.Object.Instantiate(child,new U.Vector3(8,2,1),U.Quaternion.identity,destination.transform)),
   ("generic-parent",()=>U.Object.Instantiate(child,destination.transform)),("untyped-parent",()=>(U.GameObject)U.Object.Instantiate((U.Object)child,destination.transform)),("untyped-parent-world",()=>(U.GameObject)U.Object.Instantiate((U.Object)child,destination.transform,true)),("untyped-pose",()=>(U.GameObject)U.Object.Instantiate((U.Object)child,new U.Vector3(8,2,1),U.Quaternion.identity)),("untyped-pose-parent",()=>(U.GameObject)U.Object.Instantiate((U.Object)child,new U.Vector3(8,2,1),U.Quaternion.identity,destination.transform))};
  foreach(var entry in cases){L.Context.Operations.Clear();var clone=entry.Clone();float[] p=Read(entry.Key+".position"),q=Read(entry.Key+".rotation"),s=Read(entry.Key+".scale");var expected=new U.Quaternion(q[0],q[1],q[2],q[3]);check("native Play clone overload "+entry.Key,Near(clone.transform.position,new(p[0],p[1],p[2]))&&Near(clone.transform.localScale,new(s[0],s[1],s[2]))&&Near(clone.transform.right,expected*U.Vector3.right)&&Near(clone.transform.up,expected*U.Vector3.up));U.Object.DestroyImmediate(clone);}
  float[] Read(string key)=>facts[key].Split(',').Select(value=>float.Parse(value,System.Globalization.CultureInfo.InvariantCulture)).ToArray();
  static bool Near(U.Vector3 a,U.Vector3 b)=>(a-b).magnitude<2e-5f;
 }
}
