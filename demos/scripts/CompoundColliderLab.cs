using Axiom.Gameplay;
namespace Game {
public sealed class CompoundColliderLab:MonoBehaviour {
 public int Count,Passed,Failed,Phase;public bool Complete;public string Report="",Diagnostics="";public Collider Selected;public double ExtraWidth,SelectedWidth;public bool ExtraEnabled;
 private GameObject owner,wall;private BoxCollider primary,extra,target;private CompoundColliderProbe probe;private int frame,stageFrame;private bool ready;
 public override void Start(){
  owner=new GameObject("Compound Collider Owner");primary=owner.AddComponent<BoxCollider>();extra=owner.AddComponent<BoxCollider>();extra.size=new(2,1,1);var body=owner.AddComponent<Rigidbody>();body.useGravity=false;
  wall=new GameObject("Compound Collider Wall");wall.transform.position=new(1.2,0,0);target=wall.AddComponent<BoxCollider>();
  UnityEngine.GameObject canonical=owner;probe=canonical.AddComponent<CompoundColliderProbe>();probe.Own=(UnityEngine.Collider)extra;probe.Other=(UnityEngine.Collider)target;Selected=extra;ready=true;
  Check("two-colliders",owner.GetComponents<Collider>().Length==2&&primary!=extra);Check("one-body",owner.GetComponents<Rigidbody>().Length==1);Check("individual-size",primary.size==Vector3.one&&extra.size==new Vector3(2,1,1));
 }
 private void Check(string name,bool value){Count++;if(value)Passed++;else{Failed++;Diagnostics+=name+";";}Report+=name+":"+(value?"pass":"fail")+";";}
 public override void Update(){if(!ready)return;ExtraWidth=extra.size.X;ExtraEnabled=extra.enabled;SelectedWidth=Selected is BoxCollider selected?selected.size.X:0;if(Complete)return;frame++;if(frame-stageFrame<12)return;stageFrame=frame;
  switch(Phase++){
   case 0:Check("canonical-collision",probe.Collisions>0&&probe.Exact);Check("ray-extra-slot",Physics.Raycast(new(-3,0,0),Vector3.right,out var hit,10)&&hit.collider==extra);extra.enabled=false;break;
   case 1:Check("disabled-collider-exit",probe.Exits>0);Check("disabled-ray-primary",Physics.Raycast(new(-3,0,0),Vector3.right,out var hit2,10)&&hit2.collider==primary);extra.isTrigger=true;extra.enabled=true;break;
   case 2:Check("canonical-trigger",probe.Triggers>0&&probe.Trigger==(UnityEngine.Collider)target&&probe.Exact);Physics.IgnoreCollision(extra,target);Check("component-specific-ignore",Physics.GetIgnoreCollision(extra,target)&&!Physics.GetIgnoreCollision(primary,target));break;
   case 3:Physics.IgnoreCollision(extra,target,false);probe.Triggers=0;Check("ignore-cleared",!Physics.GetIgnoreCollision(extra,target));break;
   case 4:Check("trigger-restored",probe.Triggers>0);Axiom.Gameplay.Object.DestroyImmediate(primary);break;
   case 5:Check("primary-destroy-retains-extra",!primary&&extra&&owner.GetComponents<Collider>().Length==1&&Selected==extra);Check("extra-only-ray",Physics.Raycast(new(-3,0,0),Vector3.right,out var hit3,10)&&hit3.collider==extra);primary=owner.AddComponent<BoxCollider>();primary.enabled=false;Complete=true;break;
  }
 }
}
}
