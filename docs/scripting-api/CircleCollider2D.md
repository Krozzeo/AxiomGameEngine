# CircleCollider2D

Runtime circle radius in world units, finite and positive.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed class CircleCollider2D : Collider2D {internal CircleCollider2D(Entity entity,string? slot=null):base(entity,slot){} public double radius {get=>ColliderInstances.Half(this).X;set=>ColliderInstances.Half(this,new(value,value,.5));}}
```
