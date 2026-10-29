# Behaviour

Namespace: `Axiom.Gameplay`

Enable/disable script execution without deleting its component.


SDK 8: `enabled` is shared by C# and the live Inspector checkbox. Disabling suppresses regular Update/FixedUpdate/LateUpdate, invokes OnDisable; enabling invokes OnEnable and resumes updates. Runtime activation is discarded on Stop. This toggles the component, not the GameObject subtree. Rigidbody and Transform have no enabled checkbox.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public class Behaviour : Component {
 private bool active=true;
 public bool enabled {get=>this is Script script?RuntimeLifecycle.ScriptEnabled(script):active;set{if(this is Script script){ComponentActivation.Write(Entity,RuntimeLifecycle.ActivationKey(script),value);RuntimeLifecycle.Activation();}else active=value;}}
 public bool isActiveAndEnabled=>enabled&&Context.Entities.ContainsKey(Entity.Id)&&Context.IsActive(Entity.Id)&&(this is not Script script||Context.LiveScripts.Contains(script));
}

// Namespace: UnityEngine
public class Behaviour : Component {
 public bool enabled {get=>Wrapped is Legacy.Behaviour value?value.enabled:throw new InvalidOperationException("Behaviour is not attached");set{if(Wrapped is Legacy.Behaviour behaviour)behaviour.enabled=value;else throw new InvalidOperationException("Behaviour is not attached");}}
 public bool isActiveAndEnabled=>Wrapped is Legacy.Behaviour value&&value.isActiveAndEnabled;
}
```
