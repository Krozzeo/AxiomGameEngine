using UnityEngine;
namespace Game;

// This source uses public UnityEngine names and normal private Unity messages.
public sealed class UnityMigrationLab : MonoBehaviour {
 public int Frames;
 public int Passed;
 public int Failed;
 public string Report="";
 [SerializeField] private Vector3 initialPosition=new Vector3(3,0,0);
 private GameObject root;
 private GameObject child;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private bool Near(Vector3 a,Vector3 b)=>(a-b).magnitude<0.0001f;
 private void Start(){
  var unnamed=new GameObject();Check("constructor.default-name",unnamed.name=="New Game Object"&&unnamed.GetComponents<Component>().Length==1&&unnamed.transform.parent==null);Check("constructor.active-scene",unnamed.scene==UnityEngine.SceneManagement.SceneManager.GetActiveScene());UnityEngine.Object.Destroy(unnamed);
  root=new GameObject("Unity Migration Root");child=new GameObject("Unity Migration Child");root.transform.position=initialPosition;root.transform.localScale=new Vector3(2,1,1);child.transform.SetParent(root.transform,false);child.transform.localPosition=Vector3.right;child.transform.localRotation=Quaternion.Euler(0,0,45);
  float x=child.transform.position.x;Check("source.float-field",x==5f);Component component=child.GetComponent<Transform>();UnityEngine.Object handle=component;Check("source.type-hierarchy",handle&&component.gameObject==child);
  var expected=new Vector3(6.41421356f,0.70710678f,0);Check("matrix.shear",Near(child.transform.TransformPoint(Vector3.right),expected));Check("matrix.inverse",Near(child.transform.worldToLocalMatrix.MultiplyPoint3x4(expected),Vector3.right));
  var positions=new[]{Vector3.right,Vector3.up};child.transform.TransformPoints(positions.AsSpan());child.transform.InverseTransformPoints(positions.AsSpan());Check("source.spans",Near(positions[0],Vector3.right)&&Near(positions[1],Vector3.up));
  child.transform.hasChanged=false;root.transform.position=new Vector3(4,0,0);Check("source.change-flag",child.transform.hasChanged);
 }
 private void Update(){
  Frames++;
  if(Frames==2){
   Check("matrix.next-frame",Near(child.transform.TransformPoint(Vector3.right),new Vector3(7.41421356f,0.70710678f,0)));
   Check("query.canonical-component",root.GetComponentsInChildren<Transform>(true).Length==2);
   root.transform.localScale=new Vector3(-2,1,1);child.transform.rotation=Quaternion.identity;Check("setter.rotation-local-scale",Near(child.transform.localScale,Vector3.one)&&Near(child.transform.lossyScale,new Vector3(-2,1,1)));
   child.transform.localRotation=Quaternion.Euler(0,0,45);child.transform.SetPositionAndRotation(new Vector3(8,2,1),Quaternion.identity);Check("setter.pose-local-scale",Near(child.transform.localScale,Vector3.one)&&Near(child.transform.position,new Vector3(8,2,1)));
   child.transform.localRotation=Quaternion.Euler(0,0,45);child.transform.position=new Vector3(6,1,0);Check("setter.position-local-pose",Near(child.transform.localScale,Vector3.one)&&Near(child.transform.localRotation*Vector3.right,new Vector3(.70710678f,.70710678f,0)));root.transform.localScale=Vector3.one;
   root.transform.localScale=Vector3.zero;child.transform.localPosition=new Vector3(2,3,4);root.transform.localScale=Vector3.one;
   Check("zero-scale.local-state",Near(child.transform.position,new Vector3(6,3,4)));
   var activation=child.AddComponent<HierarchyActivationProbe>();var inactive=new GameObject("Inactive destination");inactive.SetActive(false);child.transform.SetParent(inactive.transform,true);Check("activation.reparent-disable",activation.DisabledCalls==1&&!activation.isActiveAndEnabled);child.transform.SetParent(root.transform,true);Check("activation.reparent-enable",activation.EnabledCalls==2&&activation.DisabledCalls==1&&activation.isActiveAndEnabled);UnityEngine.Object.DestroyImmediate(inactive);
   UnityEngine.Object.DontDestroyOnLoad(root);Check("persistence.scene",root.scene.IsValid()&&root.scene.isLoaded&&root.scene.name=="DontDestroyOnLoad"&&root.scene.path==""&&root.scene.buildIndex==-1&&child.scene==root.scene);
  }
  if(Frames==3){Check("persistence.next-frame",root.scene.isLoaded&&child.scene==root.scene&&root.scene.rootCount==1&&root.scene.GetRootGameObjects()[0]==root);UnityEngine.Object.Destroy(root);Check("destroy.deferred",root&&child);}
  if(Frames==4)Check("destroy.null",!root&&!child&&root==null&&child==null);
 }
}
