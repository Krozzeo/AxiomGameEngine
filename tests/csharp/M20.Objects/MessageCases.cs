using Legacy=Axiom.Gameplay;
using U=UnityEngine;
public static class MessageCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(33);Legacy.RuntimeLifecycle.Register<MessageProbe>("Scripts/MessageProbe.cs",()=>new MessageProbe(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  var parent=new U.GameObject("Message root");var child=new U.GameObject("Message child");child.transform.SetParent(parent.transform,false);var a=parent.AddComponent<MessageProbe>();var b=child.AddComponent<MessageProbe>();
  parent.SendMessage("Ping");check("SendMessage invokes private canonical methods",a.Count==1&&b.Count==0);
  child.SendMessageUpwards("Ping");check("SendMessageUpwards includes self and ancestors",a.Count==2&&b.Count==1);
  parent.BroadcastMessage("Ping");check("BroadcastMessage traverses active descendants",a.Count==3&&b.Count==2);
  b.enabled=false;child.SendMessage("Ping");check("SendMessage reaches disabled behaviours",b.Count==3);
  child.SetActive(false);parent.BroadcastMessage("Ping");check("BroadcastMessage skips inactive child objects",a.Count==4&&b.Count==3);
  a.SendMessage("TakeGame",parent);check("Component message forwarding retains object identity",ReferenceEquals(a.Received,parent));
  var logs=Legacy.Context.Operations.Count(op=>op.Kind=="log");parent.SendMessage("Absent",U.SendMessageOptions.DontRequireReceiver);check("DontRequireReceiver avoids missing-receiver errors",Legacy.Context.Operations.Count(op=>op.Kind=="log")==logs);
  parent.SendMessage("Absent");check("RequireReceiver reports a missing receiver",Legacy.Context.Operations.Any(op=>op.Kind=="log"&&op.Message!.Contains("has no receiver")));
  parent.tag="Player";check("TagHandle resolves existing tags",parent.CompareTag(U.TagHandle.GetExistingTag("Player"))&&a.CompareTag("Player"));
  try{parent.tag="Unknown M20 tag";check("canonical tag assignment rejects undefined tags",false);}catch(U.UnityException){check("canonical tag assignment rejects undefined tags",parent.tag=="Player");}
  parent.hideFlags=U.HideFlags.DontSave;check("canonical searches exclude DontSave objects",!U.Object.FindObjectsByType<U.GameObject>(U.FindObjectsSortMode.InstanceID).Any(go=>go==parent));parent.hideFlags=U.HideFlags.None;
  check("canonical searches find shared native objects",U.Object.FindObjectsByType<U.GameObject>(U.FindObjectsInactive.Include,U.FindObjectsSortMode.InstanceID).Any(go=>go==child));
  check("canonical GameObject facade is reused",ReferenceEquals(parent,parent.transform.gameObject)&&ReferenceEquals(parent,U.GameObject.Find("Message root")));
  check("canonical Transform enumeration yields canonical children",parent.transform.Cast<U.Transform>().Single()==child.transform);
  parent.transform.hierarchyCapacity=32;check("hierarchy capacity reserves storage and hierarchy count includes inactive",child.transform.hierarchyCount==2&&child.transform.hierarchyCapacity>=32);
 }
}
public sealed class MessageProbe:UnityEngine.MonoBehaviour {public int Count;public UnityEngine.GameObject? Received;private void Ping(){Count++;}private void TakeGame(UnityEngine.GameObject value){Received=value;}}
