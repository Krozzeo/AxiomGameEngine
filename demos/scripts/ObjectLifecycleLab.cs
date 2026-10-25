using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;
namespace Game;

// M20 identity/query/graph behavior lab. UnityEngine namespace migration is batch 20.3.
public interface IObjectLabMarker {}
public sealed class ObjectLifecycleLab : MonoBehaviour,IObjectLabMarker {
 public GameObject? Template;
 [ReadOnly] public int Frames;
 [ReadOnly] public int Passed;
 [ReadOnly] public int Failed;
 [ReadOnly] public string Report="";
 private GameObject? doomed;
 private GameObject? descendant;
 private Transform? retained;
 private BoxCollider? removed;
 private GameObject? live;
 private int retainedId;
 private void Check(string id,bool result){if(result)Passed++;else Failed++;Report+=id+":"+(result?"pass":"fail")+";";Debug.Log("M20_CASE "+id+" "+(result?"pass":"fail"));}
 public override void Start(){
  Check("identity.wrapper",transform==gameObject.transform);
  Check("identity.hash",transform.GetHashCode()==gameObject.transform.GetHashCode());
  Check("identity.kinds",(Object)gameObject!=(Object)transform);
  doomed=new GameObject("Deferred hierarchy");descendant=new GameObject("Deferred child");descendant.transform.SetParent(doomed.transform,false);
  retained=doomed.transform;retainedId=retained.GetInstanceID();
  Check("identity.parent",descendant.transform.parent==retained);
  live=Object.Instantiate(Template!,new Vector3(2,0,0),Quaternion.identity);
  live.name="M20 surviving clone";
  Check("clone.identity",live!=Template&&live.transform!=Template!.transform);
  Check("clone.component",live.GetComponent<BoxCollider>()!=null);
  removed=live.GetComponent<BoxCollider>();
  Check("query.interface",ReferenceEquals(gameObject.GetComponent<IObjectLabMarker>(),this));
  Check("query.type",gameObject.GetComponent(typeof(ObjectLifecycleLab))==this);
  Check("query.try",gameObject.TryGetComponent<IObjectLabMarker>(out var marker)&&ReferenceEquals(marker,this)&&!gameObject.TryGetComponent(typeof(Camera),out _));
  var results=new System.Collections.Generic.List<IObjectLabMarker>{this,this};gameObject.GetComponents(results);Check("query.list",results.Count==1);
  descendant.SetActive(false);Check("query.children-active",doomed.GetComponentsInChildren<Transform>().Length==1);
  Check("query.children-all",doomed.GetComponentsInChildren(typeof(Transform),true).Length==2);
  doomed.SetActive(false);Check("query.inactive-self",doomed.GetComponentsInChildren<Transform>().Length==1);doomed.SetActive(true);descendant.SetActive(true);
  Check("query.parent",descendant.GetComponentsInParent<Transform>().Length==2);
  Check("query.find",System.Array.Exists(Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID),go=>go==doomed));
  try{gameObject.GetComponent<int>();Check("query.invalid",false);}catch(System.ArgumentException){Check("query.invalid",true);}
  var graph=new GameObject("Graph source");graph.transform.SetParent(transform,false);var node=new GameObject("Graph child");node.transform.SetParent(graph.transform,false);
  var graphProbe=graph.AddComponent<ObjectGraphProbe>();var nodeProbe=node.AddComponent<ObjectGraphProbe>();graphProbe.Peer=node;nodeProbe.Peer=graph;graphProbe.Literal=graph.transform.Entity.Id;
  var graphClone=Object.Instantiate(graph,transform);var clonedProbe=graphClone.GetComponent<ObjectGraphProbe>()!;
  Check("graph.parent-at-awake",clonedProbe.ParentAtAwake==transform.GetInstanceID());
  Check("graph.references-at-awake",clonedProbe.GraphReady);
  Check("graph.cycle",clonedProbe.Peer!.GetComponent<ObjectGraphProbe>()!.Peer==graphClone);
  Check("graph.literal",clonedProbe.Literal==graphProbe.Literal);
  Object.DestroyImmediate(graph);Check("graph.independent",graphClone&&clonedProbe.Peer);Object.DestroyImmediate(graphClone);
 }
 public override void Update(){
  Frames++;
  if(Frames==1){Object.Destroy(doomed);Object.Destroy(doomed);Object.Destroy(removed);Check("destroy.deferred",doomed&&descendant&&retained&&removed);}
  if(Frames==2){
   Check("destroy.null",doomed==null&&descendant==null&&retained==null&&removed==null);
   Check("destroy.managed",!ReferenceEquals(doomed,null));
   Check("destroy.identity",retained!.GetInstanceID()==retainedId);
   Check("destroy.distinct",doomed!=descendant);
   Check("component.owner-survives",live&&live!.GetComponent<BoxCollider>()==null);
   var replacement=live!.AddComponent<BoxCollider>();
   Check("component.new-identity",replacement!=removed&&replacement.GetInstanceID()!=removed!.GetInstanceID());
   Check("component.old-stays-dead",!removed);
   var immediate=new GameObject("Immediate");Object.DestroyImmediate(immediate);Check("destroy.immediate",!immediate);
   Object.Destroy(live,.06);Check("destroy.delay-pending",live);
  }
  if(Frames==12)Check("destroy.delay-complete",!live);
 }
}
