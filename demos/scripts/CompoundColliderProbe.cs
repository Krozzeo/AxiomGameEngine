using UnityEngine;
namespace Game {
public sealed class CompoundColliderProbe:MonoBehaviour {
 public Collider Own,Other,Trigger;public int Collisions,Triggers,Exits;public bool Exact=true;
 private void OnCollisionEnter(Collision value){Collisions++;Exact&=value.collider==Other&&value.contactCount>0&&value.GetContact(0).thisCollider==Own&&value.GetContact(0).otherCollider==Other;}
 private void OnCollisionStay(Collision value){Exact&=value.collider==Other;}
 private void OnCollisionExit(Collision value){Exits++;Exact&=value.collider==Other;}
 private void OnTriggerEnter(Collider value){Triggers++;Exact&=value==Other;Trigger=value;}
}
}
