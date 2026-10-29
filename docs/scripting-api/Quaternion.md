# Quaternion

Namespace: `Axiom.Gameplay`

## Description

Four-component rotation value with Euler/AngleAxis construction, inverse, composition and vector rotation. Euler and AngleAxis accept degrees. Entity rotation writes normalize the result.

## Use cases

Rotate an entity without storing ad hoc Euler-angle arrays.

## Example

The statements below belong inside a `MonoBehaviour` callback. Add the components/resources mentioned in the use case first.

```csharp
var turn = Quaternion.AngleAxis(90, Vector3.up);
transform.rotation = turn;
Debug.Log(turn * Vector3.forward);
```

## Compatibility and limitations

Existing PascalCase names remain supported. Lowercase vector fields and constants are available. Vector numbers use double rather than Unity float. Vector4Int is an Axiom extension. Legacy Vector3.Forward points toward -Z; the Unity-compatible lowercase Vector3.forward points +Z. Axiom cameras look along local -Z. Random wraps the deterministic replay generator; qualify Axiom.Gameplay.Random when System.Random is also in scope. Not every Unity math member is implemented.


## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public readonly record struct Quaternion(double X,double Y,double Z,double W) {
 public static Quaternion identity=>Identity;
 public static Quaternion Euler(double x,double y,double z)=>Euler(new Vector3(x,y,z));
 public static Quaternion operator *(Quaternion a,Quaternion b)=>new(a.W*b.X+a.X*b.W+a.Y*b.Z-a.Z*b.Y,a.W*b.Y-a.X*b.Z+a.Y*b.W+a.Z*b.X,a.W*b.Z+a.X*b.Y-a.Y*b.X+a.Z*b.W,a.W*b.W-a.X*b.X-a.Y*b.Y-a.Z*b.Z);
 public static Vector3 operator *(Quaternion q,Vector3 v){var u=new Vector3(q.X,q.Y,q.Z);return v+2*Vector3.Cross(u,Vector3.Cross(u,v)+q.W*v);}
 public static Quaternion Inverse(Quaternion q){var n=q.X*q.X+q.Y*q.Y+q.Z*q.Z+q.W*q.W;if(n<1e-20)throw new ArgumentException("Zero quaternion");return new(-q.X/n,-q.Y/n,-q.Z/n,q.W/n);}
 public static Quaternion AngleAxis(double angle,Vector3 axis){var u=axis.normalized;var a=angle*Math.PI/360;return new(u.X*Math.Sin(a),u.Y*Math.Sin(a),u.Z*Math.Sin(a),Math.Cos(a));}
 public static Quaternion Identity => new(0,0,0,1);
 public static Quaternion Euler(Vector3 degrees) {var x=degrees.X*Math.PI/360;var y=degrees.Y*Math.PI/360;var z=degrees.Z*Math.PI/360;var cx=Math.Cos(x);var sx=Math.Sin(x);var cy=Math.Cos(y);var sy=Math.Sin(y);var cz=Math.Cos(z);var sz=Math.Sin(z);return new(sx*cy*cz-cx*sy*sz,cx*sy*cz+sx*cy*sz,cx*cy*sz-sx*sy*cz,cx*cy*cz+sx*sy*sz);}
 public static implicit operator Quat(Quaternion q)=>new(q.X,q.Y,q.Z,q.W);public static implicit operator Quaternion(Quat q)=>new(q.X,q.Y,q.Z,q.W);
 internal static Quaternion Read(JsonElement e)=>new(e[0].GetDouble(),e[1].GetDouble(),e[2].GetDouble(),e[3].GetDouble());
}

// Namespace: UnityEngine
public struct Quaternion : IEquatable<Quaternion> {
 public float x,y,z,w;
 public Quaternion(float x,float y,float z,float w){this.x=x;this.y=y;this.z=z;this.w=w;}
 public static Quaternion identity=>new(0,0,0,1);
 public static Quaternion Euler(float x,float y,float z)=>Euler(new Vector3(x,y,z));public static Quaternion Euler(Vector3 euler)=>Axiom.Gameplay.TransformMath.Euler(euler);
 public Vector3 eulerAngles {get=>Axiom.Gameplay.TransformMath.EulerAngles(this);set=>this=Euler(value);}
 public static Quaternion AngleAxis(float angle,Vector3 axis)=>Axiom.Gameplay.Quaternion.AngleAxis(angle,axis);
 public static Quaternion Inverse(Quaternion q)=>Axiom.Gameplay.Quaternion.Inverse(q);public static Quaternion Normalize(Quaternion q)=>Axiom.Gameplay.TransformMath.Normalize(q);public Quaternion normalized=>Normalize(this);public void Normalize(){this=normalized;}
 public static Quaternion LookRotation(Vector3 forward)=>LookRotation(forward,Vector3.up);public static Quaternion LookRotation(Vector3 forward,Vector3 upwards)=>Axiom.Gameplay.TransformMath.LookRotation(forward,upwards);
 public static Quaternion FromToRotation(Vector3 fromDirection,Vector3 toDirection){var from=(Axiom.Gameplay.Vector3)fromDirection;var to=(Axiom.Gameplay.Vector3)toDirection;var rotation=Axiom.Gameplay.TransformMath.LookRotation(from,Axiom.Gameplay.Vector3.up);return Axiom.Gameplay.TransformMath.FromTo(from.normalized,to);}
 public static Quaternion operator *(Quaternion a,Quaternion b)=>(Axiom.Gameplay.Quaternion)a*(Axiom.Gameplay.Quaternion)b;public static Vector3 operator *(Quaternion a,Vector3 b)=>(Axiom.Gameplay.Quaternion)a*(Axiom.Gameplay.Vector3)b;
 public static bool operator ==(Quaternion a,Quaternion b)=>MathF.Abs(a.x*b.x+a.y*b.y+a.z*b.z+a.w*b.w)>0.999999f;public static bool operator !=(Quaternion a,Quaternion b)=>!(a==b);public bool Equals(Quaternion other)=>x.Equals(other.x)&&y.Equals(other.y)&&z.Equals(other.z)&&w.Equals(other.w);public override bool Equals(object? other)=>other is Quaternion q&&Equals(q);public override int GetHashCode()=>HashCode.Combine(x,y,z,w);
 public static implicit operator Axiom.Gameplay.Quaternion(Quaternion value)=>new(value.x,value.y,value.z,value.w);public static implicit operator Quaternion(Axiom.Gameplay.Quaternion value)=>new((float)value.X,(float)value.Y,(float)value.Z,(float)value.W);
}
```
