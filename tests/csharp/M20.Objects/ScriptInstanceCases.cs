using System.Text.Json;
using L=Axiom.Gameplay;
using U=UnityEngine;
public static class ScriptInstanceCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(2301);var type=typeof(InstanceProbe);var names=new[]{"Value","Peer"};
  L.RuntimeLifecycle.Register<InstanceProbe>("Scripts/InstanceProbe.cs",()=>new InstanceProbe(),(script,values)=>L.ScriptValues.Apply(type,script,values),(script,writer)=>L.ScriptValues.Write(type,script,writer,names));
  var root=new U.GameObject("Repeated scripts");var first=root.AddComponent<InstanceProbe>();var second=root.AddComponent<InstanceProbe>();first.Value=10;second.Value=20;first.Peer=second;second.Peer=first;
  check("repeated Script instances have distinct identities and ordered queries",first!=second&&root.GetComponents<InstanceProbe>().SequenceEqual(new[]{first,second})&&first.GetComponentIndex()==1&&second.GetComponentIndex()==2);
  first.enabled=false;check("repeated Scripts have independent activation",!first.enabled&&second.enabled);
  using(var stream=new MemoryStream()){
   using(var writer=new Utf8JsonWriter(stream))L.ScriptValues.Write(type,first,writer,names);
   using var value=JsonDocument.Parse(stream.ToArray());check("Script references publish the selected runtime instance",value.RootElement.GetProperty("Peer").GetProperty("runtimeId").GetInt32()==second.GetInstanceID());
   L.ScriptValues.Apply(type,second,value.RootElement);check("Script reference application retains the exact peer instance",second.Peer==second);second.Value=20;second.Peer=first;
  }
  L.RuntimeLifecycle.PublishComponents(root.LegacyGameObject.Entity);
  var published=L.Context.EntityData[root.LegacyGameObject.Entity.Id].GetProperty("scriptComponents").EnumerateArray().ToArray();
  check("runtime component projection contains independent Script identities",published.Length==2&&published.Select(value=>value.GetProperty("runtimeId").GetInt32()).Distinct().Count()==2);
  L.Context.Operations.Clear();var bare=root.LegacyGameObject.Entity.Spawn(new L.Vec3(0,0,0));
  check("legacy bare Spawn does not borrow original Script instance identities",!L.Context.EntityData[bare.Id].TryGetProperty("scriptComponents",out _)&&new L.GameObject(bare).GetComponents<L.Script>().Length==0);
  check("legacy bare Spawn removes only numeric Script activation aliases",!L.Context.EntityData[bare.Id].GetProperty("componentEnabled").EnumerateObject().Any(value=>value.Name=="script:"+first.GetInstanceID()));
  L.Object.DestroyImmediate(new L.GameObject(bare));L.Context.Operations.Clear();
  var clone=U.Object.Instantiate(root);var cloned=clone.GetComponents<InstanceProbe>();
  check("cloning repeated Scripts preserves their order and fields",cloned.Length==2&&cloned[0].Value==10&&cloned[1].Value==20&&cloned[0].GetComponentIndex()==1&&cloned[1].GetComponentIndex()==2);
  check("cloning remaps cyclic Script references by identity",cloned[0].Peer==cloned[1]&&cloned[1].Peer==cloned[0]);
  check("cloning retains independent Script enabled states",!cloned[0].enabled&&cloned[1].enabled);
  cloned[0].Value=99;check("cloned Script fields are independent of the original",first.Value==10&&second.Value==20);
  L.Context.Operations.Clear();var clonedSecond=U.Object.Instantiate(second);
  check("component Instantiate returns the matching repeated slot",clonedSecond.Value==20&&clonedSecond.GetComponentIndex()==2&&clonedSecond.Peer==clonedSecond.gameObject.GetComponents<InstanceProbe>()[0]);
  U.Object.DestroyImmediate(second);check("destroying one Script retains its sibling and invalidates its references",!second&&first&&root.GetComponents<InstanceProbe>().SequenceEqual(new[]{first})&&!first.Peer);
  var replacement=root.AddComponent<InstanceProbe>();check("replacement Script receives a fresh identity and default fields",replacement.GetInstanceID()!=second.GetInstanceID()&&replacement.Value==17&&replacement.enabled&&!first.enabled);
  U.Object.Destroy(replacement);check("deferred destruction retains the selected Script until End",replacement&&first);L.RuntimeLifecycle.End();
  check("End retires only the selected repeated Script",!replacement&&first&&root.GetComponents<InstanceProbe>().Length==1);
  L.Context.Operations.Clear();var pending=U.Object.InstantiateAsync(clone);U.Object.DestroyImmediate(clone);L.Time.Step(1d/60);U.AsyncInstantiationQueue.Tick();
  var result=pending.Result[0].GetComponents<InstanceProbe>();check("async snapshot remaps repeated peers after original destruction",result.Length==2&&result[0].Peer==result[1]&&result[1].Peer==result[0]&&!result[0].enabled&&result[1].enabled);
 }
}
public sealed class InstanceProbe:U.MonoBehaviour {public int Value=17;public InstanceProbe? Peer;}
