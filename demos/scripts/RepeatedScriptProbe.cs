using System.Collections.Generic;
using UnityEngine;
namespace Game {
public sealed class RepeatedScriptProbe:MonoBehaviour {
 public int Value=17,AwakeCalls,EnableCalls,DisableCalls,DestroyCalls,MessageCalls;
 public RepeatedScriptProbe Peer;
 public static readonly List<int> Delivery=new List<int>();
 private void Awake(){AwakeCalls++;}
 private void OnEnable(){EnableCalls++;}
 private void OnDisable(){DisableCalls++;}
 private void OnDestroy(){DestroyCalls++;}
 private void Accumulate(int amount){Value+=amount;MessageCalls++;Delivery.Add(Value);}
}
}
