# Transform

Namespace: `Axiom.Gameplay`

## Description

Live spatial component for world position, quaternion rotation and scale. Lowercase properties read current runtime state and enqueue validated writes. Uppercase Position/Rotation/Scale preserve legacy snapshot access.

## Use cases

Position a spawned entity, orient a character, or derive movement axes.

## Example

The statements below belong inside a `MonoBehaviour` callback. Add the components/resources mentioned in the use case first.

```csharp
transform.position = new Vector3(2, 1, 0);
transform.rotation = Quaternion.Euler(0, 90, 0);
transform.localScale = Vector3.one * 2;
var facing = transform.forward;
Debug.Log(facing);
```

## Compatibility and limitations

Transforms use world position/rotation. localScale writes the runtime scale. Legacy Entity.Transform.Position/Rotation/Scale remain snapshots; lowercase transform properties are live. Generic component lookup covers built-in wrappers and already attached scripts. Instantiate clones one entity using the existing spawn contract; it does not yet clone a hierarchy or start new script instances. SetActive controls runtime hierarchy participation; GetComponentInChildren and GetComponentInParent traverse existing components. Dynamic AddComponent, tags and full Unity object identity semantics remain pending.


## Implemented declarations

```csharp
public sealed partial class Transform {
 internal Transform(Entity entity){Entity=entity;var v=Context.Read(entity);Position=v.Position;Rotation=v.Rotation;Scale=v.Scale;}
 public Vector3 position {get=>Entity.Transform.Position;set=>Entity.SetPosition(value);}
 public Quaternion rotation {get=>Entity.Transform.Rotation;set=>Entity.SetRotation(value);}
 public Vector3 localScale {get=>Context.Local(Entity).Scale;set=>Context.LocalTransform(Entity,scale:value);}
 public Vector3 localPosition {get=>Context.Local(Entity).Position;set=>Context.LocalTransform(Entity,position:value);}
 public Quaternion localRotation {get=>Context.Local(Entity).Rotation;set=>Context.LocalTransform(Entity,rotation:value);}
 public Vector3 lossyScale=>Entity.Transform.Scale;
 public Vector3 forward {get=>rotation*Vector3.forward;set=>rotation=TransformMath.LookRotation(value,Vector3.up);}
 public Vector3 right {get=>rotation*Vector3.right;set=>rotation=TransformMath.FromTo(Vector3.right,value);}
 public Vector3 up {get=>rotation*Vector3.up;set=>rotation=TransformMath.FromTo(Vector3.up,value);}
 public Transform? parent {get=>Context.Parents.GetValueOrDefault(Entity.Id) is string id?new(new Entity(id,Context.Generation)):null;set=>SetParent(value);}
 public void SetParent(Transform? value,bool worldPositionStays=true)=>Context.Parent(Entity,value,worldPositionStays);
 public int childCount=>Context.Parents.Count(p=>p.Value==Entity.Id);
 public Transform GetChild(int index)=>new(new Entity(Context.Parents.Where(p=>p.Value==Entity.Id).ElementAt(index).Key,Context.Generation));
 public Transform? Find(string path){var current=this;foreach(var part in path.Split('/')){Transform? found=null;for(var i=0;i<current.childCount;i++){var child=current.GetChild(i);if(child.gameObject.name==part){found=child;break;}}if(found==null)return null;current=found;}return current;}
 public void Translate(Vector3 translation)=>position+=rotation*translation;
 public void Rotate(double x,double y,double z)=>Rotate(new Vector3(x,y,z),Space.Self);
 public Vector3 TransformDirection(Vector3 direction)=>rotation*direction;
 public Vector3 InverseTransformDirection(Vector3 direction)=>Quaternion.Inverse(rotation)*direction;
}

public sealed partial class Transform {
 public Transform root {get{var current=this;while(current.parent!=null)current=current.parent;return current;}}
 public bool IsChildOf(Transform other){for(Transform? t=this;t!=null;t=t.parent)if(t.Entity==other.Entity)return true;return false;}
 public Vector3 TransformPoint(Vector3 point)=>position+rotation*new Vector3(point.X*lossyScale.X,point.Y*lossyScale.Y,point.Z*lossyScale.Z);
 public Vector3 InverseTransformPoint(Vector3 point){var p=Quaternion.Inverse(rotation)*(point-position);var s=lossyScale;if(Math.Abs(s.X*s.Y*s.Z)<1e-20)throw new InvalidOperationException("Cannot invert zero scale");return new(p.X/s.X,p.Y/s.Y,p.Z/s.Z);}
 public Vector3 TransformVector(Vector3 vector)=>rotation*new Vector3(vector.X*lossyScale.X,vector.Y*lossyScale.Y,vector.Z*lossyScale.Z);
 public Vector3 InverseTransformVector(Vector3 vector){var p=Quaternion.Inverse(rotation)*vector;var s=lossyScale;if(Math.Abs(s.X*s.Y*s.Z)<1e-20)throw new InvalidOperationException("Cannot invert zero scale");return new(p.X/s.X,p.Y/s.Y,p.Z/s.Z);}
 public void SetPositionAndRotation(Vector3 position,Quaternion rotation)=>Context.ChangeTransform(Entity,position,rotation);
 public void SetLocalPositionAndRotation(Vector3 position,Quaternion rotation)=>Context.LocalTransform(Entity,position,rotation);
 public void Translate(Vector3 translation,Space relativeTo){TransformMath.SpaceValue(relativeTo);if(relativeTo==Space.World)position+=translation;else Translate(translation);}
 public void Rotate(Vector3 eulers,Space relativeTo=Space.Self){TransformMath.SpaceValue(relativeTo);var q=TransformMath.Euler(eulers);rotation=relativeTo==Space.Self?rotation*q:q*rotation;}
}

public sealed partial class Transform {
 public int GetSiblingIndex(){var siblings=Context.Parents.Where(p=>p.Value==Context.Parents.GetValueOrDefault(Entity.Id)).Select(p=>p.Key).ToList();return siblings.IndexOf(Entity.Id);}
 public void SetSiblingIndex(int index){var parent=Context.Parents.GetValueOrDefault(Entity.Id);var siblings=Context.Parents.Where(p=>p.Value==parent).Select(p=>p.Key).ToList();siblings.Remove(Entity.Id);siblings.Insert(Math.Clamp(index,0,siblings.Count),Entity.Id);var entries=Context.Parents.ToArray();Context.Parents.Clear();var queue=new Queue<string>(siblings);foreach(var entry in entries)if(entry.Value==parent){var id=queue.Dequeue();Context.Parents[id]=parent;}else Context.Parents[entry.Key]=entry.Value;Context.Sibling(Entity,index);}
 public void SetAsFirstSibling()=>SetSiblingIndex(0);
 public void SetAsLastSibling()=>SetSiblingIndex(int.MaxValue);
 public void DetachChildren(){foreach(var id in Context.Parents.Where(p=>p.Value==Entity.Id).Select(p=>p.Key).ToArray())new Transform(new Entity(id,Context.Generation)).SetParent(null);}
}

public sealed partial class Transform : System.Collections.IEnumerable {
 public Vector3 eulerAngles {get=>TransformMath.EulerAngles(rotation);set=>rotation=TransformMath.Euler(value);}
 public Vector3 localEulerAngles {get=>TransformMath.EulerAngles(localRotation);set=>localRotation=TransformMath.Euler(value);}
 public void GetPositionAndRotation(out Vector3 position,out Quaternion rotation){position=this.position;rotation=this.rotation;}
 public void GetLocalPositionAndRotation(out Vector3 localPosition,out Quaternion localRotation){var value=Context.Local(Entity);localPosition=value.Position;localRotation=value.Rotation;}
 public void Translate(double x,double y,double z)=>Translate(new Vector3(x,y,z));
 public void Translate(double x,double y,double z,Space relativeTo)=>Translate(new Vector3(x,y,z),relativeTo);
 public void Translate(Vector3 translation,Transform? relativeTo){TransformMath.Finite(translation);position+=relativeTo==null?translation:relativeTo.TransformDirection(translation);}
 public void Translate(double x,double y,double z,Transform? relativeTo)=>Translate(new Vector3(x,y,z),relativeTo);
 public void Rotate(double x,double y,double z,Space relativeTo)=>Rotate(new Vector3(x,y,z),relativeTo);
 public void Rotate(Vector3 axis,double angle)=>Rotate(axis,angle,Space.Self);
 public void Rotate(Vector3 axis,double angle,Space relativeTo){TransformMath.SpaceValue(relativeTo);TransformMath.Finite(axis);if(!double.IsFinite(angle))throw new ArgumentException("Angle must be finite");if(axis.sqrMagnitude<1e-20)return;var delta=Quaternion.AngleAxis(angle,axis);rotation=relativeTo==Space.Self?rotation*delta:delta*rotation;}
 public void RotateAround(Vector3 point,Vector3 axis,double angle){TransformMath.Finite(point);TransformMath.Finite(axis);if(!double.IsFinite(angle))throw new ArgumentException("Angle must be finite");if(axis.sqrMagnitude<1e-20)return;var delta=Quaternion.AngleAxis(angle,axis);SetPositionAndRotation(point+delta*(position-point),delta*rotation);}
 public void LookAt(Vector3 worldPosition)=>LookAt(worldPosition,Vector3.up);
 public void LookAt(Vector3 worldPosition,Vector3 worldUp){TransformMath.Finite(worldPosition);rotation=TransformMath.LookRotation(worldPosition-position,worldUp);}
 public void LookAt(Transform target)=>LookAt(target,Vector3.up);
 public void LookAt(Transform target,Vector3 worldUp){ArgumentNullException.ThrowIfNull(target);LookAt(target.position,worldUp);}
 public Vector3 TransformPoint(double x,double y,double z)=>TransformPoint(new Vector3(x,y,z));
 public Vector3 InverseTransformPoint(double x,double y,double z)=>InverseTransformPoint(new Vector3(x,y,z));
 public Vector3 TransformVector(double x,double y,double z)=>TransformVector(new Vector3(x,y,z));
 public Vector3 InverseTransformVector(double x,double y,double z)=>InverseTransformVector(new Vector3(x,y,z));
 public Vector3 TransformDirection(double x,double y,double z)=>TransformDirection(new Vector3(x,y,z));
 public Vector3 InverseTransformDirection(double x,double y,double z)=>InverseTransformDirection(new Vector3(x,y,z));
 private void Batch(ReadOnlySpan<Vector3> source,Span<Vector3> destination,int mode){
  _=Context.Read(Entity);if(destination.Length<source.Length)throw new ArgumentException("Destination is shorter than source",nameof(destination));
  // A separate result buffer handles overlapping spans and rejects all invalid
  // inputs before modifying any caller-owned destination element.
  var result=new Vector3[source.Length];for(var i=0;i<source.Length;i++){TransformMath.Finite(source[i]);result[i]=mode switch{0=>TransformPoint(source[i]),1=>InverseTransformPoint(source[i]),2=>TransformVector(source[i]),3=>InverseTransformVector(source[i]),4=>TransformDirection(source[i]),_=>InverseTransformDirection(source[i])};TransformMath.Finite(result[i]);}
  result.AsSpan().CopyTo(destination);
 }
 public void TransformPoints(ReadOnlySpan<Vector3> positions,Span<Vector3> transformedPositions)=>Batch(positions,transformedPositions,0);
 public void TransformPoints(Span<Vector3> positions)=>Batch(positions,positions,0);
 public void InverseTransformPoints(ReadOnlySpan<Vector3> positions,Span<Vector3> transformedPositions)=>Batch(positions,transformedPositions,1);
 public void InverseTransformPoints(Span<Vector3> positions)=>Batch(positions,positions,1);
 public void TransformVectors(ReadOnlySpan<Vector3> vectors,Span<Vector3> transformedVectors)=>Batch(vectors,transformedVectors,2);
 public void TransformVectors(Span<Vector3> vectors)=>Batch(vectors,vectors,2);
 public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors,Span<Vector3> transformedVectors)=>Batch(vectors,transformedVectors,3);
 public void InverseTransformVectors(Span<Vector3> vectors)=>Batch(vectors,vectors,3);
 public void TransformDirections(ReadOnlySpan<Vector3> directions,Span<Vector3> transformedDirections)=>Batch(directions,transformedDirections,4);
 public void TransformDirections(Span<Vector3> directions)=>Batch(directions,directions,4);
 public void InverseTransformDirections(ReadOnlySpan<Vector3> directions,Span<Vector3> transformedDirections)=>Batch(directions,transformedDirections,5);
 public void InverseTransformDirections(Span<Vector3> directions)=>Batch(directions,directions,5);
 public System.Collections.IEnumerator GetEnumerator(){_=Context.Read(Entity);return Context.Parents.Where(pair=>pair.Value==Entity.Id).Select(pair=>new Transform(new Entity(pair.Key,Context.Generation))).ToArray().GetEnumerator();}
}

public sealed partial class Transform : Component {
 public Vector3 Position {get;init;}
 public Quaternion Rotation {get;init;}
 public Vector3 Scale {get;init;}
 public Transform(Vector3 position, Quaternion rotation, Vector3 scale) {Position=position;Rotation=rotation;Scale=scale;}
 internal static Transform Read(JsonElement value)=>new(Vector3.Read(value.GetProperty("position")), Quaternion.Read(value.GetProperty("rotation")), Vector3.Read(value.GetProperty("scale")));
}
```
