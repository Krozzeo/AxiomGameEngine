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
// Namespace: Axiom.Gameplay
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

// Namespace: Axiom.Gameplay
public sealed partial class Transform {
 public Transform root {get{var current=this;while(current.parent!=null)current=current.parent;return current;}}
 public bool IsChildOf(Transform other){for(Transform? t=this;t!=null;t=t.parent)if(t.Entity==other.Entity)return true;return false;}
 public Vector3 TransformPoint(Vector3 point)=>TransformGraph.World(Entity).Point(point);
 public Vector3 InverseTransformPoint(Vector3 point)=>(TransformGraph.World(Entity).Inverse()??throw new InvalidOperationException("Cannot invert zero scale")).Point(point);
 public Vector3 TransformVector(Vector3 vector)=>TransformGraph.World(Entity).Vector(vector);
 public Vector3 InverseTransformVector(Vector3 vector)=>(TransformGraph.World(Entity).Inverse()??throw new InvalidOperationException("Cannot invert zero scale")).Vector(vector);
 public void SetPositionAndRotation(Vector3 position,Quaternion rotation)=>Context.ChangeTransform(Entity,position,rotation);
 public void SetLocalPositionAndRotation(Vector3 position,Quaternion rotation)=>Context.LocalTransform(Entity,position,rotation);
 public void Translate(Vector3 translation,Space relativeTo){TransformMath.SpaceValue(relativeTo);if(relativeTo==Space.World)position+=translation;else Translate(translation);}
 public void Rotate(Vector3 eulers,Space relativeTo=Space.Self){TransformMath.SpaceValue(relativeTo);var q=TransformMath.Euler(eulers);if(relativeTo==Space.Self)localRotation=localRotation*q;else rotation=q*rotation;}
}

// Namespace: Axiom.Gameplay
public sealed partial class Transform {
 public int GetSiblingIndex(){var siblings=Context.Parents.Where(p=>p.Value==Context.Parents.GetValueOrDefault(Entity.Id)).Select(p=>p.Key).ToList();return siblings.IndexOf(Entity.Id);}
 public void SetSiblingIndex(int index){var parent=Context.Parents.GetValueOrDefault(Entity.Id);var siblings=Context.Parents.Where(p=>p.Value==parent).Select(p=>p.Key).ToList();siblings.Remove(Entity.Id);siblings.Insert(Math.Clamp(index,0,siblings.Count),Entity.Id);var entries=Context.Parents.ToArray();Context.Parents.Clear();var queue=new Queue<string>(siblings);foreach(var entry in entries)if(entry.Value==parent){var id=queue.Dequeue();Context.Parents[id]=parent;}else Context.Parents[entry.Key]=entry.Value;Context.Sibling(Entity,index);}
 public void SetAsFirstSibling()=>SetSiblingIndex(0);
 public void SetAsLastSibling()=>SetSiblingIndex(int.MaxValue);
 public void DetachChildren(){foreach(var id in Context.Parents.Where(p=>p.Value==Entity.Id).Select(p=>p.Key).ToArray())new Transform(new Entity(id,Context.Generation)).SetParent(null);}
}

// Namespace: Axiom.Gameplay
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

// Namespace: UnityEngine
public partial class Transform : Component,System.Collections.IEnumerable {
 protected Transform(){}
 internal Transform(Legacy.Transform value){Wrapped=value;}
 public static implicit operator Transform(Legacy.Transform value)=>ReferenceEquals(value,null)?null!:(Transform)Wrap(value);
 internal Legacy.Transform LegacyTransform {get{if(!this)throw new MissingReferenceException("Transform has been destroyed or is not attached");return Wrapped as Legacy.Transform??throw new MissingReferenceException("Transform is not attached");}}
 public Matrix4x4 localToWorldMatrix=>Matrix4x4.From(Legacy.TransformGraph.World(LegacyTransform.Entity));public Matrix4x4 worldToLocalMatrix=>localToWorldMatrix.inverse;
 public bool hasChanged {get=>Legacy.TransformGraph.HasChanged(LegacyTransform.Entity);set=>Legacy.TransformGraph.SetChanged(LegacyTransform.Entity,value);}
 public Vector3 position {get=>LegacyTransform.position;set=>LegacyTransform.position=value;}public Vector3 localPosition {get=>LegacyTransform.localPosition;set=>LegacyTransform.localPosition=value;}public Quaternion rotation {get=>LegacyTransform.rotation;set=>LegacyTransform.rotation=value;}public Quaternion localRotation {get=>LegacyTransform.localRotation;set=>LegacyTransform.localRotation=value;}public Vector3 localScale {get=>LegacyTransform.localScale;set=>LegacyTransform.localScale=value;}public Vector3 lossyScale=>LegacyTransform.lossyScale;
 public Vector3 eulerAngles {get=>LegacyTransform.eulerAngles;set=>LegacyTransform.eulerAngles=value;}public Vector3 localEulerAngles {get=>LegacyTransform.localEulerAngles;set=>LegacyTransform.localEulerAngles=value;}
 public Vector3 forward {get=>LegacyTransform.forward;set=>LegacyTransform.forward=value;}public Vector3 right {get=>LegacyTransform.right;set=>LegacyTransform.right=value;}public Vector3 up {get=>LegacyTransform.up;set=>LegacyTransform.up=value;}
 public Transform? parent {get=>LegacyTransform.parent;set=>SetParent(value);}public Transform root=>LegacyTransform.root;public int childCount=>LegacyTransform.childCount;public Transform GetChild(int index){var native=LegacyTransform;if(index<0||index>=native.childCount)throw new UnityException("Transform child out of bounds");return native.GetChild(index);}public Transform? Find(string n){var native=LegacyTransform;ArgumentNullException.ThrowIfNull(n);return native.Find(n);}public bool IsChildOf(Transform parent){var native=LegacyTransform;ArgumentNullException.ThrowIfNull(parent);return native.IsChildOf(parent.LegacyTransform);}
 public void SetParent(Transform? p)=>SetParent(p,true);public void SetParent(Transform? parent,bool worldPositionStays){var native=LegacyTransform;var target=parent?parent.LegacyTransform:null;if(target!=null&&target.IsChildOf(native)){Legacy.Debug.LogError("Cannot set a Transform parent to itself or one of its descendants");return;}native.SetParent(target,worldPositionStays);}public void DetachChildren()=>LegacyTransform.DetachChildren();public int GetSiblingIndex()=>LegacyTransform.GetSiblingIndex();public void SetSiblingIndex(int index)=>LegacyTransform.SetSiblingIndex(index);public void SetAsFirstSibling()=>LegacyTransform.SetAsFirstSibling();public void SetAsLastSibling()=>LegacyTransform.SetAsLastSibling();public System.Collections.IEnumerator GetEnumerator()=>Enumerable.Range(0,childCount).Select(GetChild).ToArray().GetEnumerator();
 public void SetPositionAndRotation(Vector3 position,Quaternion rotation)=>LegacyTransform.SetPositionAndRotation(position,rotation);public void SetLocalPositionAndRotation(Vector3 localPosition,Quaternion localRotation)=>LegacyTransform.SetLocalPositionAndRotation(localPosition,localRotation);
 public void GetPositionAndRotation(out Vector3 position,out Quaternion rotation){position=this.position;rotation=this.rotation;}public void GetLocalPositionAndRotation(out Vector3 localPosition,out Quaternion localRotation){localPosition=this.localPosition;localRotation=this.localRotation;}
 public void Translate(Vector3 translation)=>Translate(translation,Space.Self);public void Translate(Vector3 translation,Space relativeTo)=>LegacyTransform.Translate(translation,(Legacy.Space)relativeTo);public void Translate(Vector3 translation,Transform? relativeTo)=>LegacyTransform.Translate(translation,relativeTo?.LegacyTransform);public void Translate(float x,float y,float z)=>Translate(new Vector3(x,y,z));public void Translate(float x,float y,float z,Space relativeTo)=>Translate(new Vector3(x,y,z),relativeTo);public void Translate(float x,float y,float z,Transform? relativeTo)=>Translate(new Vector3(x,y,z),relativeTo);
 public void Rotate(Vector3 eulers)=>Rotate(eulers,Space.Self);public void Rotate(Vector3 eulers,Space relativeTo)=>LegacyTransform.Rotate(eulers,(Legacy.Space)relativeTo);public void Rotate(float xAngle,float yAngle,float zAngle)=>Rotate(new Vector3(xAngle,yAngle,zAngle));public void Rotate(float xAngle,float yAngle,float zAngle,Space relativeTo)=>Rotate(new Vector3(xAngle,yAngle,zAngle),relativeTo);public void Rotate(Vector3 axis,float angle)=>Rotate(axis,angle,Space.Self);public void Rotate(Vector3 axis,float angle,Space relativeTo)=>LegacyTransform.Rotate(axis,angle,(Legacy.Space)relativeTo);public void RotateAround(Vector3 point,Vector3 axis,float angle)=>LegacyTransform.RotateAround(point,axis,angle);
 public void LookAt(Vector3 worldPosition)=>LookAt(worldPosition,Vector3.up);public void LookAt(Vector3 worldPosition,Vector3 worldUp)=>LegacyTransform.LookAt(worldPosition,worldUp);public void LookAt(Transform target)=>LookAt(target.position);public void LookAt(Transform target,Vector3 worldUp)=>LookAt(target.position,worldUp);
 public Vector3 TransformPoint(Vector3 position)=>LegacyTransform.TransformPoint(position);public Vector3 InverseTransformPoint(Vector3 position)=>Legacy.TransformQueries.Inverse(LegacyTransform.Entity,position,true);public Vector3 TransformVector(Vector3 vector)=>LegacyTransform.TransformVector(vector);public Vector3 InverseTransformVector(Vector3 vector)=>Legacy.TransformQueries.Inverse(LegacyTransform.Entity,vector,false);public Vector3 TransformDirection(Vector3 direction)=>Legacy.TransformQueries.Direction(LegacyTransform.Entity,direction,false);public Vector3 InverseTransformDirection(Vector3 direction)=>Legacy.TransformQueries.Direction(LegacyTransform.Entity,direction,true);
 public Vector3 TransformPoint(float x,float y,float z)=>TransformPoint(new Vector3(x,y,z));public Vector3 InverseTransformPoint(float x,float y,float z)=>InverseTransformPoint(new Vector3(x,y,z));public Vector3 TransformVector(float x,float y,float z)=>TransformVector(new Vector3(x,y,z));public Vector3 InverseTransformVector(float x,float y,float z)=>InverseTransformVector(new Vector3(x,y,z));public Vector3 TransformDirection(float x,float y,float z)=>TransformDirection(new Vector3(x,y,z));public Vector3 InverseTransformDirection(float x,float y,float z)=>InverseTransformDirection(new Vector3(x,y,z));
 private void Batch(ReadOnlySpan<Vector3> source,Span<Vector3> destination,int mode){_=Legacy.Context.Read(LegacyTransform.Entity);if(destination.Length!=source.Length)throw new InvalidOperationException("Source and destination lengths must match");var output=new Vector3[source.Length];for(var i=0;i<source.Length;i++){Legacy.TransformMath.Finite(source[i]);output[i]=mode switch{0=>TransformPoint(source[i]),1=>InverseTransformPoint(source[i]),2=>TransformVector(source[i]),3=>InverseTransformVector(source[i]),4=>TransformDirection(source[i]),_=>InverseTransformDirection(source[i])};Legacy.TransformMath.Finite(output[i]);}output.AsSpan().CopyTo(destination);}
 public void TransformPoints(Span<Vector3> positions)=>Batch(positions,positions,0);public void TransformPoints(ReadOnlySpan<Vector3> positions,Span<Vector3> transformedPositions)=>Batch(positions,transformedPositions,0);public void InverseTransformPoints(Span<Vector3> positions)=>Batch(positions,positions,1);public void InverseTransformPoints(ReadOnlySpan<Vector3> positions,Span<Vector3> transformedPositions)=>Batch(positions,transformedPositions,1);
 public void TransformVectors(Span<Vector3> vectors)=>Batch(vectors,vectors,2);public void TransformVectors(ReadOnlySpan<Vector3> vectors,Span<Vector3> transformedVectors)=>Batch(vectors,transformedVectors,2);public void InverseTransformVectors(Span<Vector3> vectors)=>Batch(vectors,vectors,3);public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors,Span<Vector3> transformedVectors)=>Batch(vectors,transformedVectors,3);
 public void TransformDirections(Span<Vector3> directions)=>Batch(directions,directions,4);public void TransformDirections(ReadOnlySpan<Vector3> directions,Span<Vector3> transformedDirections)=>Batch(directions,transformedDirections,4);public void InverseTransformDirections(Span<Vector3> directions)=>Batch(directions,directions,5);public void InverseTransformDirections(ReadOnlySpan<Vector3> directions,Span<Vector3> transformedDirections)=>Batch(directions,transformedDirections,5);
}

// Namespace: UnityEngine
public partial class Transform {
 public int hierarchyCount=>Legacy.TransformGraph.Count(root.LegacyTransform.Entity);
 public int hierarchyCapacity {get=>Legacy.TransformGraph.Capacity(root.LegacyTransform.Entity);set=>Legacy.TransformGraph.SetCapacity(root.LegacyTransform.Entity,value);}
 [Obsolete("FindChild has been deprecated. Use Find instead (UnityUpgradable) -> Find([mscorlib] System.String)",false)]public Transform? FindChild(string n)=>Find(n);
 [Obsolete("warning use Transform.childCount instead (UnityUpgradable) -> Transform.childCount",false)]public int GetChildCount()=>childCount;
 [Obsolete("warning use Transform.Rotate instead.")]public void RotateAroundLocal(Vector3 axis,float angle)=>localRotation=Quaternion.AngleAxis((float)(angle*180/System.Math.PI),axis)*localRotation;
 [Obsolete("warning use Transform.Rotate instead.")]public void RotateAround(Vector3 axis,float angle)=>Rotate(axis,(float)(angle*180/System.Math.PI),Space.World);
}

// Namespace: Axiom.Gameplay
public sealed partial class Transform : Component {
 public Vector3 Position {get;init;}
 public Quaternion Rotation {get;init;}
 public Vector3 Scale {get;init;}
 public Transform(Vector3 position, Quaternion rotation, Vector3 scale) {Position=position;Rotation=rotation;Scale=scale;}
 internal static Transform Read(JsonElement value)=>new(Vector3.Read(value.GetProperty("position")), Quaternion.Read(value.GetProperty("rotation")), Vector3.Read(value.GetProperty("scale")));
}
```
