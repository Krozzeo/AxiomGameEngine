# BoxCollider

Runtime box size in world units; positive finite values update the native collider. Rotation follows the owning Transform. Size is not scaled by Transform in this baseline.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public class BoxCollider : Collider {internal BoxCollider(Entity entity,string? slot=null):base(entity,slot){} public Vector3 size {get=>ColliderInstances.Half(this)*2;set=>ColliderInstances.Half(this,value/2);}}

// Namespace: UnityEngine
public class BoxCollider : Collider {internal BoxCollider(Legacy.BoxCollider value):base(value){}public Vector3 size {get=>((Legacy.BoxCollider)LegacyComponent).size;set=>((Legacy.BoxCollider)LegacyComponent).size=value;}}
```
