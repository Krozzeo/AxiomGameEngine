using Axiom.Gameplay;
using Object=Axiom.Gameplay.Object;
public static class ManagedCloneCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(32);RuntimeLifecycle.Register<ManagedCloneProbe>("Scripts/ManagedCloneProbe.cs",()=>new ManagedCloneProbe(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  var root=new GameObject("Managed graph");var child=new GameObject("Managed child");child.transform.SetParent(root.transform,false);var original=root.AddComponent<ManagedCloneProbe>();
  var shared=new CloneData{Value=4,Peer=child};original.First=shared;original.Second=shared;original.Items.Add(shared);original.Reference=shared;shared.Next=shared;
  // Inline values must not carry a cycle; the reference field may preserve one.
  shared.Next=null;var cycle=new CloneData{Value=8,Peer=child};cycle.Next=cycle;original.Reference=cycle;
  var clone=Object.Instantiate(root);var copy=clone.GetComponent<ManagedCloneProbe>()!;var cloneChild=clone.transform.GetChild(0).gameObject;
  check("serializable inline classes deep clone",copy.First!=original.First&&copy.First.Value==4);
  check("inline shared references are independent serialized values",!ReferenceEquals(copy.First,copy.Second));
  check("serializable lists deep clone elements",copy.Items!=original.Items&&copy.Items[0]!=original.Items[0]&&copy.Items[0].Value==4);
  check("managed nested scene references remap",copy.First.Peer==cloneChild&&copy.Items[0].Peer==cloneChild);
  check("SerializeReference retains graph cycles",copy.Reference!=original.Reference&&ReferenceEquals(copy.Reference,copy.Reference.Next)&&copy.Reference.Peer==cloneChild);
  copy.First.Value=99;check("editing clone managed state preserves source",original.First.Value==4);
  Context.Operations.Clear();Axiom.Gameplay.AI.NavMesh.BuildGrid(Vector3.zero,8,8,1);var nav=new GameObject("Navigation clone");nav.transform.position=new(.5,0,.5);var agent=nav.AddComponent<Axiom.Gameplay.AI.NavMeshAgent>();agent.speed=2;agent.SetDestination(new(4.5,0,.5));
  var navClone=Object.Instantiate(nav);var clonedAgent=navClone.GetComponent<Axiom.Gameplay.AI.NavMeshAgent>()!;
  check("navigation agent configuration is cloned",clonedAgent!=agent&&clonedAgent.speed==2&&clonedAgent.hasPath);
  clonedAgent.ResetPath();check("navigation clone path is independent",agent.hasPath&&!clonedAgent.hasPath);
 }
}
[Serializable] public sealed class CloneData {public int Value;public GameObject? Peer;public CloneData? Next;}
public sealed class ManagedCloneProbe:MonoBehaviour {public CloneData First=new();public CloneData Second=new();public List<CloneData> Items=new();[SerializeReference] public CloneData Reference=new();}
