namespace Axiom.Gameplay;

// The legacy SDK uses double precision. These methods implement its transform
// behavior; they are not a claim of canonical UnityEngine float signatures.
internal static class TransformMath {
 internal static void Finite(Vector3 value) {
  if(!double.IsFinite(value.X)||!double.IsFinite(value.Y)||!double.IsFinite(value.Z))throw new ArgumentException("Transform vector must be finite");
 }
 internal static Quaternion Normalize(Quaternion value) {
  var length=Math.Sqrt(value.X*value.X+value.Y*value.Y+value.Z*value.Z+value.W*value.W);
  if(!double.IsFinite(length)||length<1e-9)throw new ArgumentException("AX_SCRIPT_0003: invalid rotation");
  return new(value.X/length,value.Y/length,value.Z/length,value.W/length);
 }
 // Intrinsic Z, then X, then Y. Keep the older Quaternion.Euler API unchanged.
 internal static Quaternion Euler(Vector3 degrees) {
  Finite(degrees);
  return Quaternion.AngleAxis(degrees.Y,Vector3.up)*Quaternion.AngleAxis(degrees.X,Vector3.right)*Quaternion.AngleAxis(degrees.Z,Vector3.forward);
 }
 internal static Vector3 EulerAngles(Quaternion value) {
  var q=Normalize(value);var right=q*Vector3.right;var up=q*Vector3.up;var forward=q*Vector3.forward;
  var x=Math.Asin(Math.Clamp(-forward.Y,-1,1));double y,z;
  if(Math.Abs(Math.Cos(x))>1e-7){y=Math.Atan2(forward.X,forward.Z);z=Math.Atan2(right.Y,up.Y);}
  else{y=Math.Atan2(-right.Z,right.X);z=0;}
  static double Degrees(double radians){var result=radians*180/Math.PI%360;return result<0?result+360:result;}
  return new(Degrees(x),Degrees(y),Degrees(z));
 }
 internal static Quaternion FromTo(Vector3 from,Vector3 to) {
  Finite(to);if(to.sqrMagnitude<1e-20)throw new ArgumentException("Direction must be nonzero");
  var target=to.normalized;var dot=Math.Clamp(Vector3.Dot(from,target),-1,1);
  if(dot>1-1e-12)return Quaternion.identity;
  if(dot< -1+1e-12){var axis=Vector3.Cross(from,Vector3.up);if(axis.sqrMagnitude<1e-12)axis=Vector3.Cross(from,Vector3.right);return Quaternion.AngleAxis(180,axis);}
  var cross=Vector3.Cross(from,target);return Normalize(new(cross.X,cross.Y,cross.Z,1+dot));
 }
 internal static Quaternion LookRotation(Vector3 forward,Vector3 up) {
  Finite(forward);Finite(up);if(forward.sqrMagnitude<1e-20)throw new ArgumentException("Look direction must be nonzero");
  var first=FromTo(Vector3.forward,forward);var axis=forward.normalized;
  var targetUp=up-axis*Vector3.Dot(up,axis);
  if(targetUp.sqrMagnitude<1e-20)return first;
  targetUp=targetUp.normalized;var current=first*Vector3.up;
  var angle=Math.Atan2(Vector3.Dot(axis,Vector3.Cross(current,targetUp)),Math.Clamp(Vector3.Dot(current,targetUp),-1,1))*180/Math.PI;
  return Normalize(Quaternion.AngleAxis(angle,axis)*first);
 }
 internal static void SpaceValue(Space value){if(value!=Space.Self&&value!=Space.World)throw new ArgumentOutOfRangeException(nameof(value));}
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

internal static partial class Context {
 internal static void ChangeTransform(Entity entity,Vector3? position=null,Quaternion? rotation=null,Vector3? scale=null,int reservedOperations=0)=>TransformGraph.SetWorld(entity,position,rotation,scale,reservedOperations);
 internal static void Reparent(Entity entity,Transform? parent,bool worldPositionStays)=>TransformGraph.Parent(entity,parent,worldPositionStays);
}
