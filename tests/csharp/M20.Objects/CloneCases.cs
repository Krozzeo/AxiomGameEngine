using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;

public static class CloneCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(4);
  RuntimeLifecycle.Register<CloneProbe>("Scripts/CloneProbe.cs",()=>new CloneProbe(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  RuntimeLifecycle.Register<UnsupportedCloneProbe>("Scripts/UnsupportedCloneProbe.cs",()=>new UnsupportedCloneProbe(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  var parent=new GameObject("Original parent");parent.transform.position=new Vector3(10,0,0);
  var root=new GameObject("Graph");root.transform.SetParent(parent.transform,false);root.transform.localPosition=new Vector3(2,0,0);
  var child=new GameObject("Graph child");child.transform.SetParent(root.transform,false);
  var a=root.AddComponent<CloneProbe>();var b=child.AddComponent<CloneProbe>();
  a.Peer=child;b.Peer=root;a.ComponentPeer=child.transform;a.Literal=root.Entity.Id;a.Nodes=new[]{child};
  var destination=new GameObject("Destination");destination.transform.position=new Vector3(20,0,0);
  var cloned=Object.Instantiate(root,destination.transform);var cloneChild=cloned.transform.GetChild(0).gameObject;var probe=cloned.GetComponent<CloneProbe>()!;
  check("parent overload defaults to local pose",cloned.transform.parent==destination.transform&&cloned.transform.localPosition.X==2&&cloned.transform.position.X==22);
  check("clone metadata uses its new identity",Context.EntityData[cloned.Entity.Id].GetProperty("id").GetString()==cloned.Entity.Id);
  check("Awake sees final clone parent",probe.ParentAtAwake==destination.transform.GetInstanceID());
  check("Awake sees remapped complete graph",probe.GraphReadyAtAwake);
  check("root child references remapped",probe.Peer==cloneChild&&cloneChild.GetComponent<CloneProbe>()!.Peer==cloned);
  check("component references remapped",probe.ComponentPeer==cloneChild.transform);
  check("literal entity ID strings preserved",probe.Literal==root.Entity.Id);
  check("reference arrays are copied and remapped",!ReferenceEquals(probe.Nodes,a.Nodes)&&probe.Nodes[0]==cloneChild&&a.Nodes[0]==child);
  Context.Operations.Clear();
  var world=Object.Instantiate(root,destination.transform,true);
  check("explicit world pose preserved",world.transform.position.X==12&&world.transform.parent==destination.transform);
  var detached=Object.Instantiate(root,(Transform?)null);
  check("null parent keeps original world pose",detached.transform.parent==null&&detached.transform.position.X==12);
  var posed=Object.Instantiate(root,new Vector3(30,0,0),Quaternion.identity,destination.transform);
  check("explicit pose plus parent uses world position",posed.transform.position.X==30&&posed.transform.parent==destination.transform);
  Context.Operations.Clear();
  var componentClone=Object.Instantiate(a,destination.transform);
  check("generic component clone returns cloned component",componentClone!=a&&componentClone.gameObject!=root&&componentClone.transform.parent==destination.transform);
  var inactive=new GameObject("Inactive graph");inactive.SetActive(false);var asleep=inactive.AddComponent<CloneProbe>();
  var inactiveClone=Object.Instantiate(inactive);var asleepClone=inactiveClone.GetComponent<CloneProbe>()!;
  check("inactive clone defers Awake",!asleepClone.AwakeCalled&&!inactiveClone.activeSelf);
  inactiveClone.SetActive(true);check("activating clone initializes it",asleepClone.AwakeCalled);
  Context.Operations.Clear();
  var unsupported=new GameObject("Unsupported managed clone");unsupported.AddComponent<UnsupportedCloneProbe>();var count=Context.Entities.Count;var operations=Context.Operations.Count;
  try{Object.Instantiate(unsupported);check("unsupported clone rejected before creating objects",false);}catch(NotSupportedException){check("unsupported clone rejected before creating objects",Context.Entities.Count==count&&Context.Operations.Count==operations);}
  try{Object.Instantiate((GameObject)null!);check("null clone rejected without scene mutation",false);}catch(ArgumentException){check("null clone rejected without scene mutation",Context.Entities.Count==count&&Context.Operations.Count==operations);}
 }
}
public sealed class CloneProbe:MonoBehaviour {
 public GameObject? Peer;public Component? ComponentPeer;public string Literal="";public GameObject[] Nodes=Array.Empty<GameObject>();
 public bool AwakeCalled;public int ParentAtAwake;public bool GraphReadyAtAwake;
 public override void Awake(){AwakeCalled=true;ParentAtAwake=transform.parent?.GetInstanceID()??0;GraphReadyAtAwake=Peer!=null&&Peer.transform.parent==transform;}
}
public sealed class UnsupportedCloneProbe:MonoBehaviour {public System.IO.Stream Values=new System.IO.MemoryStream();}
