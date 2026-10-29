using System;
using System.Linq;
using UnityEngine;
namespace Game {
public static class ScriptInstanceCorpus {
 public static void Run(Action<string,string> record){
  Action<string,Func<object>> observe=(key,call)=>{try{var value=call();record(key,value==null?"null":Convert.ToString(value,System.Globalization.CultureInfo.InvariantCulture));}catch(Exception error){record(key,"error:"+error.GetType().FullName);}};
  var root=new GameObject("Repeated Script Source");var first=root.AddComponent<RepeatedScriptProbe>();var second=root.AddComponent<RepeatedScriptProbe>();
  observe("created.distinct",()=>first&&second&&first!=second&&first.gameObject==root&&second.gameObject==root);
  observe("created.count",()=>root.GetComponents<RepeatedScriptProbe>().Length);
  observe("created.order",()=>first.GetComponentIndex()+":"+second.GetComponentIndex());
  observe("created.defaults",()=>first.Value+":"+second.Value);
  observe("created.callbacks",()=>first.AwakeCalls+":"+first.EnableCalls+":"+second.AwakeCalls+":"+second.EnableCalls);
  first.Value=10;second.Value=20;first.Peer=second;second.Peer=first;first.enabled=false;
  observe("fields.independent",()=>first.Value+":"+second.Value);
  observe("activation.independent",()=>first.enabled+":"+second.enabled+":"+first.DisableCalls+":"+second.DisableCalls);
  RepeatedScriptProbe.Delivery.Clear();root.SendMessage("Accumulate",3);
  observe("messages.disabled",()=>first.Value+":"+second.Value+":"+first.MessageCalls+":"+second.MessageCalls);
  observe("messages.order",()=>string.Join(",",RepeatedScriptProbe.Delivery));
  var clone=UnityEngine.Object.Instantiate(root);var cloned=clone.GetComponents<RepeatedScriptProbe>();
  observe("clone.count",()=>cloned.Length);
  observe("clone.fields",()=>cloned[0].Value+":"+cloned[1].Value);
  observe("clone.peers",()=>cloned[0].Peer==cloned[1]&&cloned[1].Peer==cloned[0]);
  observe("clone.enabled",()=>cloned[0].enabled+":"+cloned[1].enabled);
  observe("clone.order",()=>cloned[0].GetComponentIndex()+":"+cloned[1].GetComponentIndex());
  observe("clone.callbacks",()=>cloned[0].AwakeCalls+":"+cloned[0].EnableCalls+":"+cloned[0].DisableCalls+":"+cloned[1].AwakeCalls+":"+cloned[1].EnableCalls+":"+cloned[1].DisableCalls);
  cloned[0].Value=99;observe("clone.fields-independent",()=>first.Value+":"+second.Value);
  var componentClone=UnityEngine.Object.Instantiate(second);
  observe("component.clone.ordinal",()=>componentClone.Value+":"+componentClone.GetComponentIndex()+":"+componentClone.gameObject.GetComponents<RepeatedScriptProbe>().Length);
  observe("component.clone.peer",()=>componentClone.Peer==componentClone.gameObject.GetComponents<RepeatedScriptProbe>()[0]);
  observe("component.clone.enabled",()=>componentClone.gameObject.GetComponents<RepeatedScriptProbe>()[0].enabled+":"+componentClone.enabled);
  UnityEngine.Object.DestroyImmediate(second);
  observe("destroy.one",()=>!second&&first&&root.GetComponents<RepeatedScriptProbe>().SequenceEqual(new[]{first}));
  observe("destroy.peer",()=>first.Peer==null);
  observe("destroy.callback",()=>second.DisableCalls+":"+second.DestroyCalls+":"+first.DestroyCalls);
  var replacement=root.AddComponent<RepeatedScriptProbe>();
  observe("replacement.id",()=>replacement.GetInstanceID()!=second.GetInstanceID());
  observe("replacement.default",()=>replacement.Value);
  observe("replacement.enabled",()=>first.enabled+":"+replacement.enabled);
  observe("replacement.order",()=>first.GetComponentIndex()+":"+replacement.GetComponentIndex());
  observe("replacement.callbacks",()=>replacement.AwakeCalls+":"+replacement.EnableCalls);
  // Keep the runtime objects available for manual Inspector/identity tests; Stop
  // restores the untouched authored scene in both browser execution modes.
 }
}
}
