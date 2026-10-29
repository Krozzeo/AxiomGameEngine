using UnityEngine;

// Source must compile unchanged. Do not add aliases or rewrite this corpus
// to the legacy namespace merely to turn the migration gate green.
public sealed class HierarchyMigration : MonoBehaviour {
 [SerializeField] private Vector3 initialPosition;
 private GameObject child;
 void Start(){
  child=new GameObject("Migration child");
  child.transform.SetParent(transform,false);
  child.transform.localPosition=initialPosition;
  float x=child.transform.position.x;
  child.transform.Rotate(0f,90f,0f,Space.World);
  Transform owner=child.GetComponent<Transform>();
  Component component=owner;
  UnityEngine.Object handle=component;
  bool alive=handle;
  if(alive&&x>=0f)Destroy(child,0.1f);
 }
}
