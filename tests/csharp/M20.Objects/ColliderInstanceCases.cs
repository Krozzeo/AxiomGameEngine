using System.Text.Json;
using L=Axiom.Gameplay;
using U=UnityEngine;
public static class ColliderInstanceCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(2401);var type=typeof(ColliderEventProbe);var names=new[]{"Peer"};
  L.RuntimeLifecycle.Register<ColliderEventProbe>("Scripts/ColliderEventProbe.cs",()=>new ColliderEventProbe(),(script,values)=>L.ScriptValues.Apply(type,script,values),(script,writer)=>L.ScriptValues.Write(type,script,writer,names));
  var owner=new U.GameObject("Owner");var a=owner.AddComponent<U.BoxCollider>();var b=owner.AddComponent<U.BoxCollider>();a.size=new(2,4,6);b.size=new(8,10,12);a.enabled=false;
  check("collider instances retain distinct identities, geometry and activation",a!=b&&a.size==new U.Vector3(2,4,6)&&b.size==new U.Vector3(8,10,12)&&!a.enabled&&b.enabled);
  var probe=owner.AddComponent<ColliderEventProbe>();probe.Peer=b;
  using(var stream=new MemoryStream()){using(var writer=new Utf8JsonWriter(stream))L.ScriptValues.Write(type,probe,writer,names);using var data=JsonDocument.Parse(stream.ToArray());var reference=data.RootElement.GetProperty("Peer");check("collider references publish exact logical slots",reference.GetProperty("runtimeSlot").GetString()==((L.Component)b.Native).RuntimeSlot);probe.Peer=a;L.ScriptValues.Apply(type,probe,data.RootElement);check("collider reference reads restore the selected sibling",probe.Peer==b);}
  var clone=U.Object.Instantiate(owner);var cloned=clone.GetComponents<U.BoxCollider>();check("clone retains both collider instances and independent shape state",cloned.Length==2&&cloned[0].size==a.size&&cloned[1].size==b.size&&!cloned[0].enabled&&cloned[1].enabled);check("clone remaps a reference to the second collider",clone.GetComponent<ColliderEventProbe>().Peer==cloned[1]);
  var other=new U.GameObject("Other");var c=other.AddComponent<U.BoxCollider>();var d=other.AddComponent<U.SphereCollider>();var nativeB=(L.Collider)b.Native;var nativeC=(L.Collider)c.Native;var nativeD=(L.Collider)d.Native;
  L.Physics.IgnoreCollision(nativeB,nativeD);check("collision exclusion distinguishes sibling colliders",L.Physics.GetIgnoreCollision(nativeB,nativeD)&&!L.Physics.GetIgnoreCollision(nativeB,nativeC));
  string Contact(U.Collider own,U.Collider target,bool trigger)=>JsonSerializer.Serialize(new {a=owner.LegacyGameObject.Entity.Id,b=other.LegacyGameObject.Entity.Id,aSlot=L.ColliderInstances.Key((L.Collider)own.Native),bSlot=L.ColliderInstances.Key((L.Collider)target.Native),trigger,normal=new[]{0,1,0},point=new[]{0,0,0},depth=.1,relativeVelocity=new[]{1,0,0}});
  void Send(params string[] contacts)=>L.Context.DispatchContacts(L.Context.ParseData("{\"contacts\":["+string.Join(',',contacts)+"]}"));
  Send(Contact(b,c,false),Contact(b,d,true));check("canonical physics callbacks preserve collider and contact point identity",probe.Events.SequenceEqual(new[]{"collision-enter","trigger-enter"})&&probe.LastCollision!.collider==c&&probe.LastCollision.GetContact(0).thisCollider==b&&probe.LastCollision.GetContact(0).otherCollider==c&&probe.LastTrigger==d);
  Send(Contact(b,c,false),Contact(b,d,true));check("separate collider manifolds produce independent stay events",probe.Events.TakeLast(2).SequenceEqual(new[]{"collision-stay","trigger-stay"}));
  Send(Contact(b,d,false));check("trigger changes produce separate exit and enter events",probe.Events.TakeLast(3).SequenceEqual(new[]{"collision-enter","collision-exit","trigger-exit"}));
  U.Object.DestroyImmediate(a);check("destroying the primary collider retains additional collider identity",!a&&b&&owner.GetComponents<U.BoxCollider>().SequenceEqual(new[]{b}));
  Send();Send(Contact(b,d,true));check("extra-only collider owners still receive physics events",probe.LastTrigger==d&&probe.Events.Last()=="trigger-enter");
  owner.layer=7;check("owner layer changes update every remaining collider",nativeB.Data.GetProperty("layer").GetUInt32()==128);
  var retiredId=b.GetInstanceID();U.Object.DestroyImmediate(b);var replacement=owner.AddComponent<U.BoxCollider>();check("replaced collider never revives a retired wrapper",!b&&replacement.GetInstanceID()!=retiredId);
  var cube=U.GameObject.CreatePrimitive(U.PrimitiveType.Cube);var mesh=cube.GetComponent<U.MeshFilter>().sharedMesh!;var mesh1=cube.AddComponent<U.MeshCollider>();var mesh2=cube.AddComponent<U.MeshCollider>();U.Object.DestroyImmediate(mesh,true);check("retiring a shared mesh removes geometry from every collider instance",!mesh1.sharedMesh&&!mesh2.sharedMesh&&!((L.Collider)mesh1.Native).HasGeometry&&!((L.Collider)mesh2.Native).HasGeometry);
 }
}
public sealed class ColliderEventProbe:U.MonoBehaviour {
 public U.Collider? Peer;public List<string> Events=new();public U.Collision? LastCollision;public U.Collider? LastTrigger;
 void OnCollisionEnter(U.Collision value){Events.Add("collision-enter");LastCollision=value;}void OnCollisionStay(U.Collision value){Events.Add("collision-stay");LastCollision=value;}void OnCollisionExit(U.Collision value){Events.Add("collision-exit");LastCollision=value;}
 void OnTriggerEnter(U.Collider value){Events.Add("trigger-enter");LastTrigger=value;}void OnTriggerStay(U.Collider value){Events.Add("trigger-stay");LastTrigger=value;}void OnTriggerExit(U.Collider value){Events.Add("trigger-exit");LastTrigger=value;}
}
