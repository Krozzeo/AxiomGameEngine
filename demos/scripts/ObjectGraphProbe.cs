using Axiom.Gameplay;
namespace Game;
public sealed class ObjectGraphProbe:MonoBehaviour {
 public GameObject? Peer;
 public string Literal="";
 [ReadOnly] public int ParentAtAwake;
 [ReadOnly] public bool GraphReady;
 public override void Awake(){ParentAtAwake=transform.parent?.GetInstanceID()??0;GraphReady=Peer!=null&&Peer.transform.parent==transform;}
}
