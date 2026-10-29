# SphereCollider

Runtime sphere radius in world units; finite positive radius updates the actual sphere solver shape.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed class SphereCollider : Collider {internal SphereCollider(Entity entity,string? slot=null):base(entity,slot){} public double radius {get=>ColliderInstances.Half(this).X;set=>ColliderInstances.Half(this,new(value,value,value));}}

// Namespace: UnityEngine
public class SphereCollider : Collider {internal SphereCollider(Legacy.SphereCollider value):base(value){}public float radius {get=>(float)((Legacy.SphereCollider)LegacyComponent).radius;set=>((Legacy.SphereCollider)LegacyComponent).radius=value;}}
```
