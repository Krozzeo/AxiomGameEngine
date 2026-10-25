using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;
namespace Game;

// M20.1 behavior lab. UnityEngine namespace migration belongs to batch 20.3.
public sealed class ObjectLifecycleLab : MonoBehaviour {
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
