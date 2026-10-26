namespace UnityEngine;

public struct Vector3 : IEquatable<Vector3> {
 public float x,y,z;
 public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}
 public Vector3(float x,float y):this(x,y,0){}
 public static Vector3 zero=>new(0,0,0);public static Vector3 one=>new(1,1,1);
 public static Vector3 right=>new(1,0,0);public static Vector3 left=>new(-1,0,0);public static Vector3 up=>new(0,1,0);public static Vector3 down=>new(0,-1,0);public static Vector3 forward=>new(0,0,1);public static Vector3 back=>new(0,0,-1);
 public float sqrMagnitude=>x*x+y*y+z*z;public float magnitude=>MathF.Sqrt(sqrMagnitude);public Vector3 normalized=>magnitude>1e-5f?this/magnitude:zero;
 public void Normalize(){this=normalized;}public static Vector3 Normalize(Vector3 value)=>value.normalized;
 public static Vector3 operator +(Vector3 a,Vector3 b)=>new(a.x+b.x,a.y+b.y,a.z+b.z);public static Vector3 operator -(Vector3 a,Vector3 b)=>new(a.x-b.x,a.y-b.y,a.z-b.z);public static Vector3 operator -(Vector3 a)=>new(-a.x,-a.y,-a.z);
 public static Vector3 operator *(Vector3 a,float b)=>new(a.x*b,a.y*b,a.z*b);public static Vector3 operator *(float a,Vector3 b)=>b*a;public static Vector3 operator /(Vector3 a,float b)=>new(a.x/b,a.y/b,a.z/b);
 public static float Dot(Vector3 a,Vector3 b)=>a.x*b.x+a.y*b.y+a.z*b.z;public static Vector3 Cross(Vector3 a,Vector3 b)=>new(a.y*b.z-a.z*b.y,a.z*b.x-a.x*b.z,a.x*b.y-a.y*b.x);
 public static Vector3 Scale(Vector3 a,Vector3 b)=>new(a.x*b.x,a.y*b.y,a.z*b.z);public static float Distance(Vector3 a,Vector3 b)=>(a-b).magnitude;public static Vector3 Lerp(Vector3 a,Vector3 b,float t)=>a+(b-a)*Math.Clamp(t,0,1);
 public static bool operator ==(Vector3 a,Vector3 b)=>(a-b).sqrMagnitude<9.99999944e-11f;public static bool operator !=(Vector3 a,Vector3 b)=>!(a==b);public bool Equals(Vector3 other)=>x.Equals(other.x)&&y.Equals(other.y)&&z.Equals(other.z);public override bool Equals(object? other)=>other is Vector3 value&&Equals(value);public override int GetHashCode()=>HashCode.Combine(x,y,z);
 public override string ToString()=>$"({x:F2}, {y:F2}, {z:F2})";
 public static implicit operator Axiom.Gameplay.Vector3(Vector3 value)=>new(value.x,value.y,value.z);public static implicit operator Vector3(Axiom.Gameplay.Vector3 value)=>new((float)value.X,(float)value.Y,(float)value.Z);
}
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
public enum Space {World,Self}
[AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Axiom.Gameplay.SerializeFieldAttribute {}
[AttributeUsage(AttributeTargets.Field)] public sealed class HideInInspector : Axiom.Gameplay.HideInInspectorAttribute {}
[AttributeUsage(AttributeTargets.Field)] public sealed class SerializeReference : Axiom.Gameplay.SerializeReferenceAttribute {}
