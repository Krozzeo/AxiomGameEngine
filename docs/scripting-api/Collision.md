# Collision

One callback per colliding pair per phase, preserving all native manifold points (up to four per pair, global snapshot cap 128). contacts, contactCount, GetContact and GetContacts expose world points, receiver-facing normals and negative penetration separation. relativeVelocity is self-minus-other linear velocity at post-solver dispatch, not the pre-impact/impulse value. Exit has zero contacts. Solver impulse totals and complete pre-impact contact velocities are pending.

## Implemented declarations

```csharp
// Namespace: UnityEngine
public sealed class Collision {
 private readonly Legacy.Collision value;
 internal Collision(Legacy.Collision value){this.value=value;}
 public Collider collider=>(Collider)Object.Wrap(value.collider);public GameObject gameObject=>collider.gameObject;public Transform transform=>collider.transform;
 public Vector3 relativeVelocity=>value.relativeVelocity;public int contactCount=>value.contactCount;
 public ContactPoint[] contacts=>value.contacts.Select(point=>new ContactPoint(point)).ToArray();
 public ContactPoint GetContact(int index)=>new(value.GetContact(index));
 public int GetContacts(ContactPoint[] buffer){ArgumentNullException.ThrowIfNull(buffer);var count=Math.Min(buffer.Length,contactCount);for(var i=0;i<count;i++)buffer[i]=GetContact(i);return count;}
}

// Namespace: Axiom.Gameplay
public sealed class Collision {
 public Collider collider {get;}public GameObject gameObject=>collider.gameObject;public Transform transform=>collider.transform;public Rigidbody? rigidbody=>collider.attachedRigidbody;
 public Vector3 normal {get;}public Vector3 point {get;}public Vector3 relativeVelocity {get;}
 public ContactPoint[] contacts {get;}public int contactCount=>contacts.Length;
 public ContactPoint GetContact(int index)=>contacts[index];public int GetContacts(ContactPoint[] buffer){var count=Math.Min(buffer.Length,contacts.Length);Array.Copy(contacts,buffer,count);return count;}
 internal Collision(Collider collider,Vector3 normal,Vector3 point,ContactPoint[] contacts,Vector3 relativeVelocity){this.collider=collider;this.normal=normal;this.point=point;this.contacts=contacts;this.relativeVelocity=relativeVelocity;}
}

// Namespace: UnityEngine
public readonly struct ContactPoint {
 private readonly Legacy.ContactPoint value;
 internal ContactPoint(Legacy.ContactPoint value){this.value=value;}
 public Vector3 point=>value.point;public Vector3 normal=>value.normal;public float separation=>(float)value.separation;
 public Collider thisCollider=>(Collider)Object.Wrap(value.thisCollider);public Collider otherCollider=>(Collider)Object.Wrap(value.otherCollider);
}

public readonly record struct ContactPoint(Vector3 point,Vector3 normal,double separation,Collider thisCollider,Collider otherCollider);
```
