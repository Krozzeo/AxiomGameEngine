using UnityEngine;
public sealed class AssetLifetimeLab:MonoBehaviour {
 public int Frames,Passed,Failed;
 public string Report="";
 public Mesh CopiedMesh;
 public TextAsset TextCopy;
 private GameObject owner;
 private Mesh source,copy;
 private TextAsset text;
 private AsyncInstantiateOperation<GameObject> pending;
 private AsyncInstantiateOperation<TextAsset> textPending;
 private void Check(string key,bool pass){if(pass)Passed++;else Failed++;Report+=key+":"+(pass?"pass":"fail")+";";}
 void Start(){
  owner=GameObject.CreatePrimitive(PrimitiveType.Cube);owner.transform.position=new Vector3(2,1,0);source=owner.GetComponent<MeshFilter>().sharedMesh;copy=UnityEngine.Object.Instantiate(source);CopiedMesh=copy;
  Check("asset.identity",copy&&copy!=source&&copy.GetInstanceID()!=source.GetInstanceID());Check("asset.name",copy.name=="Cube(Clone)");copy.name="Independent copy";Check("asset.rename",source.name=="Cube");
  owner.GetComponent<MeshFilter>().sharedMesh=copy;Check("asset.binding",owner.GetComponent<MeshFilter>().sharedMesh==copy);Check("asset.find",System.Array.IndexOf(UnityEngine.Object.FindObjectsByType<Mesh>(FindObjectsSortMode.None),copy)>=0);Check("asset.borrowed-excluded",System.Array.IndexOf(UnityEngine.Object.FindObjectsByType<Mesh>(FindObjectsSortMode.None),source)<0);
  text=new TextAsset("Immutable asset content");text.name="Source text";TextCopy=UnityEngine.Object.Instantiate(text);Check("asset.text-content",TextCopy.text==text.text);Check("asset.text-name",TextCopy.name=="Source text(Clone)");
 }
 void Update(){
  Frames++;
  if(Frames==2){Check("asset.next-frame-reference",CopiedMesh==copy&&TextCopy&&TextCopy.text=="Immutable asset content");UnityEngine.Object.Destroy(copy);Check("asset.destroy-deferred",copy&&owner.GetComponent<MeshFilter>().sharedMesh==copy);}
  if(Frames==3){Check("asset.destroy-complete",!copy&&CopiedMesh==null);Check("asset.component-survives",owner.GetComponent<MeshFilter>()!=null&&owner.GetComponent<MeshRenderer>()!=null&&owner.GetComponent<MeshFilter>().sharedMesh==null);owner.GetComponent<MeshFilter>().sharedMesh=source;CopiedMesh=source;Check("asset.restore-source",owner.GetComponent<MeshFilter>().sharedMesh==source);pending=UnityEngine.Object.InstantiateAsync(owner,2);source.name="Changed after snapshot";textPending=UnityEngine.Object.InstantiateAsync(text);}
  if(Frames==6){Check("asset.async-snapshot",pending.isDone&&pending.Result.Length==2&&pending.Result[0]!=pending.Result[1]&&pending.Result[0].GetComponent<MeshFilter>().sharedMesh==source);var rejected=false;try{_=textPending.isDone;}catch(System.NullReferenceException){rejected=true;}Check("asset.async-rejects-text",rejected);Check("asset.borrowed-next-frame",CopiedMesh==source&&CopiedMesh.name=="Changed after snapshot");foreach(var clone in pending.Result)UnityEngine.Object.DestroyImmediate(clone);UnityEngine.Object.DestroyImmediate(text);UnityEngine.Object.DestroyImmediate(TextCopy);TextCopy=null;CopiedMesh=null;UnityEngine.Object.DestroyImmediate(owner);Check("asset.cleanup",!owner&&!TextCopy&&!CopiedMesh);}
 }
}
