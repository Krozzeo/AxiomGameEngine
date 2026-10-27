using UnityEngine;
namespace Game {
public sealed class AsyncLifecycleProbe:MonoBehaviour {
 private int awakeCalls,enableCalls;
 public int AwakeCalls=>awakeCalls;
 public int EnableCalls=>enableCalls;
 public static int GlobalAwakes,GlobalDisables,GlobalDestroys;
 public static void ResetCounters(){GlobalAwakes=GlobalDisables=GlobalDestroys=0;}
 private void Awake(){awakeCalls++;GlobalAwakes++;}
 private void OnEnable(){enableCalls++;}
 private void OnDisable(){GlobalDisables++;}
 private void OnDestroy(){GlobalDestroys++;}
}
}
