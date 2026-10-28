using L=Axiom.Gameplay;
using U=UnityEngine;
public static class InheritedInspectorCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(3901);L.RuntimeLifecycle.Register<InheritedInspectorProbe>("Scripts/InheritedInspectorProbe.cs",()=>new InheritedInspectorProbe(),(s,v)=>L.ScriptValues.Apply(typeof(InheritedInspectorProbe),s,v),(s,w)=>L.ScriptValues.Write(typeof(InheritedInspectorProbe),s,w,new[]{"Hidden"}));
  var root=new U.GameObject("Inherited Inspector owner");var probe=root.AddComponent<InheritedInspectorProbe>();
  var component=L.Context.EntityData[root.LegacyGameObject.Entity.Id].GetProperty("scriptComponents")[0];check("inherited private serialized field publishes its actual initializer",component.GetProperty("values").GetProperty("Hidden").GetInt32()==17);
  L.ScriptValues.Apply(typeof(InheritedInspectorProbe),probe,L.Context.ParseData("{\"Hidden\":29}"));check("inherited private serialized field accepts an explicit edit",probe.ReadHidden()==29);
  var clone=U.Object.Instantiate(probe);check("cloning retains inherited private serialized state",clone.ReadHidden()==29&&clone!=probe);U.Object.DestroyImmediate(root);U.Object.DestroyImmediate(clone.gameObject);
 }
}
public sealed class InheritedInspectorProbe:InheritedInspectorBase {}
public class InheritedInspectorBase:U.MonoBehaviour {[U.SerializeField] private int Hidden=17;public int ReadHidden()=>Hidden;}
