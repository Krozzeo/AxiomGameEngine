# BoxCollider2D

Runtime XY box size in world units. Rotation is about Z; size must be finite and positive.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed class BoxCollider2D : Collider2D {internal BoxCollider2D(Entity entity,string? slot=null):base(entity,slot){} public Vector2 size {get{var h=ColliderInstances.Half(this);return new(h.X*2,h.Y*2);}set=>ColliderInstances.Half(this,new(value.X/2,value.Y/2,.5));}}
```
