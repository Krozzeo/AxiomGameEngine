using UnityEngine;
namespace Game;
public sealed class MigrationCorpusObserver : MonoBehaviour {
 public int Frames,Passed,Failed;public string Report="";private GameObject captured;
 private void Check(string id,bool passed){if(passed)Passed++;else Failed++;Report+=id+":"+(passed?"pass":"fail")+";";}
 private void Start(){
  captured=GameObject.Find("Migration child");Check("corpus.created",captured!=null);
  Check("corpus.parent-and-float-pose",captured&&captured.transform.parent&&captured.transform.parent.gameObject.name=="Unadapted Migration Corpus"&&captured.transform.position.x==2f);
  Check("corpus.rotation",captured&&(captured.transform.forward-Vector3.right).magnitude<.0001f);
 }
 private void Update(){Frames++;if(Frames==30){Check("corpus.delayed-destroy",captured==null&&GameObject.Find("Migration child")==null);Check("corpus.owner-survives",GameObject.Find("Unadapted Migration Corpus")!=null);}}
}
