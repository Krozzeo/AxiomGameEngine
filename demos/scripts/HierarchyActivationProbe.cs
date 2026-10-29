using UnityEngine;
namespace Game;

public sealed class HierarchyActivationProbe : MonoBehaviour {
 public int EnabledCalls;
 public int DisabledCalls;
 private void OnEnable(){EnabledCalls++;}
 private void OnDisable(){DisabledCalls++;}
}
