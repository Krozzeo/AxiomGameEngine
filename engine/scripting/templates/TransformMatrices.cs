namespace Axiom.Gameplay {
internal sealed class Affine {
 internal readonly double[] M;
 internal Affine(double[] values){M=values;}
 internal static Affine Identity=>new(new double[]{1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1});
 internal static Affine TRS(Vector3 p,Quaternion q,Vector3 s){var x=q.X;var y=q.Y;var z=q.Z;var w=q.W;return new(new[]{(1-2*(y*y+z*z))*s.X,2*(x*y+z*w)*s.X,2*(x*z-y*w)*s.X,0,2*(x*y-z*w)*s.Y,(1-2*(x*x+z*z))*s.Y,2*(y*z+x*w)*s.Y,0,2*(x*z+y*w)*s.Z,2*(y*z-x*w)*s.Z,(1-2*(x*x+y*y))*s.Z,0,p.X,p.Y,p.Z,1});}
 public static Affine operator *(Affine a,Affine b){var values=new double[16];for(var c=0;c<4;c++)for(var r=0;r<4;r++)for(var k=0;k<4;k++)values[c*4+r]+=a.M[k*4+r]*b.M[c*4+k];return new(values);}
 internal Vector3 Point(Vector3 p)=>new(M[0]*p.X+M[4]*p.Y+M[8]*p.Z+M[12],M[1]*p.X+M[5]*p.Y+M[9]*p.Z+M[13],M[2]*p.X+M[6]*p.Y+M[10]*p.Z+M[14]);
 internal Vector3 Vector(Vector3 p)=>new(M[0]*p.X+M[4]*p.Y+M[8]*p.Z,M[1]*p.X+M[5]*p.Y+M[9]*p.Z,M[2]*p.X+M[6]*p.Y+M[10]*p.Z);
 internal Affine? Inverse(){var a=new double[4,8];for(var r=0;r<4;r++)for(var c=0;c<8;c++)a[r,c]=c<4?M[c*4+r]:(c-4==r?1:0);for(var i=0;i<4;i++){var pivot=i;for(var r=i+1;r<4;r++)if(Math.Abs(a[r,i])>Math.Abs(a[pivot,i]))pivot=r;if(a[pivot,i]==0)return null;for(var c=0;c<8;c++)(a[i,c],a[pivot,c])=(a[pivot,c],a[i,c]);var d=a[i,i];for(var c=0;c<8;c++)a[i,c]/=d;for(var r=0;r<4;r++)if(r!=i){var v=a[r,i];for(var c=0;c<8;c++)a[r,c]-=v*a[i,c];}}var result=new double[16];for(var c=0;c<4;c++)for(var r=0;r<4;r++)result[c*4+r]=a[r,c+4];return new(result);}
 // Transform.lossyScale projects onto its signed world rotation; Matrix4x4 keeps basis lengths.
 internal Vector3 ProjectedScale(Quaternion rotation){var projected=TRS(Vector3.zero,Quaternion.Inverse(rotation),Vector3.one)*this;return new(projected.M[0],projected.M[5],projected.M[10]);}
 internal static Quaternion Mirror(Quaternion rotation,Vector3 scale){var x=scale.X<0?-1:1;var y=scale.Y<0?-1:1;var z=scale.Z<0?-1:1;return new(rotation.X*y*z,rotation.Y*x*z,rotation.Z*x*y,rotation.W);}
 internal Vector3 Scale {get{double Length(int c)=>Math.Sqrt(M[c]*M[c]+M[c+1]*M[c+1]+M[c+2]*M[c+2]);var determinant=M[0]*(M[5]*M[10]-M[9]*M[6])-M[4]*(M[1]*M[10]-M[9]*M[2])+M[8]*(M[1]*M[6]-M[5]*M[2]);return new(Length(0)*(determinant<0?-1:1),Length(4),Length(8));}}
}
}
namespace UnityEngine {
public struct Matrix4x4 : IEquatable<Matrix4x4> {
 public float m00,m10,m20,m30,m01,m11,m21,m31,m02,m12,m22,m32,m03,m13,m23,m33;
 public float this[int index] {get=>index switch{0=>m00,1=>m10,2=>m20,3=>m30,4=>m01,5=>m11,6=>m21,7=>m31,8=>m02,9=>m12,10=>m22,11=>m32,12=>m03,13=>m13,14=>m23,15=>m33,_=>throw new IndexOutOfRangeException()};set{switch(index){case 0:m00=value;break;case 1:m10=value;break;case 2:m20=value;break;case 3:m30=value;break;case 4:m01=value;break;case 5:m11=value;break;case 6:m21=value;break;case 7:m31=value;break;case 8:m02=value;break;case 9:m12=value;break;case 10:m22=value;break;case 11:m32=value;break;case 12:m03=value;break;case 13:m13=value;break;case 14:m23=value;break;case 15:m33=value;break;default:throw new IndexOutOfRangeException();}}}
 public float this[int row,int column] {get{if((uint)row>3||(uint)column>3)throw new IndexOutOfRangeException();return this[row+column*4];}set{if((uint)row>3||(uint)column>3)throw new IndexOutOfRangeException();this[row+column*4]=value;}}
 internal Axiom.Gameplay.Affine Affine {get{var values=new double[16];for(var i=0;i<16;i++)values[i]=this[i];return new(values);}}
 internal static Matrix4x4 From(Axiom.Gameplay.Affine value){var result=new Matrix4x4();for(var i=0;i<16;i++)result[i]=(float)value.M[i];return result;}
 public static Matrix4x4 identity=>From(Axiom.Gameplay.Affine.Identity);public static Matrix4x4 zero=>new();
 public static Matrix4x4 TRS(Vector3 pos,Quaternion q,Vector3 s)=>From(Axiom.Gameplay.Affine.TRS(pos,q,s));public void SetTRS(Vector3 pos,Quaternion q,Vector3 s){this=TRS(pos,q,s);}
 public Vector3 MultiplyPoint3x4(Vector3 point)=>Affine.Point(point);public Vector3 MultiplyVector(Vector3 vector)=>Affine.Vector(vector);public Vector3 MultiplyPoint(Vector3 point){var result=MultiplyPoint3x4(point);var w=m30*point.x+m31*point.y+m32*point.z+m33;return result/w;}
 public Matrix4x4 inverse=>Affine.Inverse() is Axiom.Gameplay.Affine result?From(result):zero;public static Matrix4x4 Inverse(Matrix4x4 m)=>m.inverse;
 public static Matrix4x4 operator *(Matrix4x4 a,Matrix4x4 b)=>From(a.Affine*b.Affine);
 public bool Equals(Matrix4x4 other){for(var i=0;i<16;i++)if(!this[i].Equals(other[i]))return false;return true;}public override bool Equals(object? other)=>other is Matrix4x4 m&&Equals(m);public override int GetHashCode(){var hash=new HashCode();for(var i=0;i<16;i++)hash.Add(this[i]);return hash.ToHashCode();}
 public static bool operator ==(Matrix4x4 a,Matrix4x4 b)=>a.Equals(b);public static bool operator !=(Matrix4x4 a,Matrix4x4 b)=>!a.Equals(b);
}
}
