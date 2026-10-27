using System;
using UnityEngine;
namespace Game {
public sealed class HierarchyPathLab:MonoBehaviour {
 public int Passed,Failed;public string Report="";private bool complete;
 private void Update(){if(complete)return;complete=true;RunCases((key,value)=>{if(value)Passed++;else Failed++;Report+=key+":"+(value?"pass":"fail")+";";});}
 public static void RunCases(Action<string,bool> check){
  var root=new GameObject("PathRoot");var child=new GameObject("PathChild");child.transform.SetParent(root.transform,false);var leaf=new GameObject("PathLeaf");leaf.transform.SetParent(child.transform,false);var other=new GameObject("OtherRoot");
  check("find.leaf",GameObject.Find("PathLeaf")==leaf);
  check("find.relative",GameObject.Find("PathRoot/PathChild/PathLeaf")==leaf);
  check("find.absolute",GameObject.Find("/PathRoot/PathChild/PathLeaf")==leaf);
  check("find.suffix",GameObject.Find("PathChild/PathLeaf")==leaf);
  check("find.wrong-root",GameObject.Find("/PathChild/PathLeaf")==null);
  check("find.double-slash",GameObject.Find("PathRoot//PathChild/PathLeaf")==null);
  check("find.empty",GameObject.Find("")==null);
  check("find.null",GameObject.Find(null)==null);
  check("find.missing",GameObject.Find("MissingPath")==null);
  check("transform.find-path",root.transform.Find("PathChild/PathLeaf")==leaf.transform);
  check("transform.find-missing",root.transform.Find("MissingPath")==null);
  check("transform.find-null",Throws(()=>root.transform.Find(null),typeof(ArgumentNullException)));
  check("child.valid",root.transform.GetChild(0)==child.transform);
  check("child.negative",Throws(()=>root.transform.GetChild(-1),typeof(UnityException)));
  check("child.outside",Throws(()=>root.transform.GetChild(1),typeof(UnityException)));
  check("is-child.self",root.transform.IsChildOf(root.transform));
  check("is-child.descendant",leaf.transform.IsChildOf(root.transform));
  check("is-child.foreign",!leaf.transform.IsChildOf(other.transform));
  check("is-child.null",Throws(()=>leaf.transform.IsChildOf(null),typeof(ArgumentNullException)));
  root.SetActive(false);check("find.inactive",GameObject.Find("PathLeaf")==null);check("transform.find-inactive",root.transform.Find("PathChild/PathLeaf")==leaf.transform);root.SetActive(true);
  check("add-transform.duplicate",root.AddComponent<Transform>()==null&&root.GetComponents<Transform>().Length==1);
  check("query.type-null",Throws(()=>root.GetComponent((Type)null),typeof(ArgumentException)));
  check("query.types-null",Throws(()=>root.GetComponents((Type)null),typeof(ArgumentException)));
  check("query.type-invalid",Throws(()=>root.GetComponent(typeof(string)),typeof(ArgumentException)));
  check("query.types-invalid",Throws(()=>root.GetComponents(typeof(string)),typeof(ArgumentException)));
  check("query.generic-invalid",Throws(()=>root.GetComponent<string>(),typeof(ArgumentException)));
  check("query.generics-invalid",Throws(()=>root.GetComponents<string>(),typeof(ArgumentException)));
  check("query.list-null",NoThrow(()=>root.GetComponents<Transform>(null)));
  check("query.children-list-null",NoThrow(()=>root.GetComponentsInChildren<Transform>(null)));
  check("query.parent-list-null",NoThrow(()=>root.GetComponentsInParent<Transform>(false,null)));
  check("query.string-null",root.GetComponent((string)null)==null);
  var stale=root.transform;UnityEngine.Object.DestroyImmediate(root);
  check("child.destroyed",Throws(()=>stale.GetChild(0),typeof(MissingReferenceException)));
  check("transform.find-destroyed",Throws(()=>stale.Find("missing"),typeof(MissingReferenceException)));
  check("is-child.destroyed",Throws(()=>stale.IsChildOf(other.transform),typeof(MissingReferenceException)));
  check("find.destroyed",GameObject.Find("PathLeaf")==null&&GameObject.Find("/PathRoot/PathChild/PathLeaf")==null);
  UnityEngine.Object.DestroyImmediate(other);
 }
 private static bool NoThrow(Action action){try{action();return true;}catch{return false;}}
 private static bool Throws(Action action,Type expected){try{action();return false;}catch(Exception error){return error.GetType()==expected;}}
}
}
