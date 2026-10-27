using L=Axiom.Gameplay;
using U=UnityEngine;

public static class NativeReferenceCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(81);L.RuntimeLifecycle.Register<ReferenceLookup.Probe>("Scripts/ReferenceLookupProbe.cs",()=>new ReferenceLookup.Probe(),(_,_)=>{},(_,writer)=>{writer.WriteStartObject();writer.WriteEndObject();});
  var root=new U.GameObject("Oracle active");var inactive=new U.GameObject("Oracle inactive");inactive.SetActive(false);var child=new U.GameObject("Flag child");child.transform.SetParent(root.transform,false);
  var collider=root.AddComponent<U.BoxCollider>();var probe=root.AddComponent<ReferenceLookup.Probe>();
  check("reference native Object ToString includes public type",root.ToString()=="Oracle active (UnityEngine.GameObject)");
  check("reference native built-in string names are unqualified",root.GetComponent("Component")==root.transform&&root.GetComponent("BoxCollider")==collider&&root.GetComponent("UnityEngine.Component")==null&&root.GetComponent("UnityEngine.BoxCollider")==null);
  check("reference native scripts accept both string name forms",root.GetComponent("Probe")==probe&&root.GetComponent("ReferenceLookup.Probe")==probe);
  check("reference native string interface search differs from typed search",root.GetComponent("IProbe")==null&&root.GetComponent("ReferenceLookup.IProbe")==null&&ReferenceEquals(root.GetComponent<ReferenceLookup.IProbe>(),probe));
  check("reference native obsolete scene search excludes inactive",U.Object.FindSceneObjectsOfType(typeof(U.GameObject)).Contains(root)&&!U.Object.FindSceneObjectsOfType(typeof(U.GameObject)).Contains(inactive));
  check("reference native unknown find enums retain nonzero inactive inclusion",U.Object.FindObjectsByType<U.GameObject>((U.FindObjectsInactive)123,(U.FindObjectsSortMode)123).Contains(inactive));
  foreach(var flags in new[]{U.HideFlags.None,U.HideFlags.DontSaveInEditor,U.HideFlags.DontSaveInBuild,U.HideFlags.DontUnloadUnusedAsset,U.HideFlags.DontSave,U.HideFlags.HideInHierarchy}){
   root.hideFlags=flags;var visible=(flags&U.HideFlags.DontSaveInEditor)==0;
   check("reference native find flag "+flags,U.Object.FindObjectsByType<U.GameObject>(U.FindObjectsInactive.Include,U.FindObjectsSortMode.None).Contains(root)==visible&&U.Object.FindObjectsByType<U.BoxCollider>(U.FindObjectsInactive.Include,U.FindObjectsSortMode.None).Contains(collider)==visible&&collider.hideFlags==flags&&child.hideFlags==U.HideFlags.None);
  }
  var flagged=new U.GameObject("New component flags");flagged.hideFlags=U.HideFlags.DontSaveInEditor;var added=flagged.AddComponent<U.BoxCollider>();
  check("reference native attached component inherits owner flags",added.hideFlags==U.HideFlags.DontSaveInEditor);
  root.hideFlags=U.HideFlags.None;collider.hideFlags=U.HideFlags.HideInInspector;
  check("reference native component flag edit leaves owner flags independent",root.hideFlags==U.HideFlags.None);
  root.hideFlags=U.HideFlags.None;check("reference native same owner flag assignment reapplies component flags",collider.hideFlags==U.HideFlags.None);
 }
}
namespace ReferenceLookup {
 public interface IProbe {}
 public sealed class Probe:U.MonoBehaviour,IProbe {}
}
