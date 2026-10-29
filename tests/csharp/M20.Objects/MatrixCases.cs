using Legacy=Axiom.Gameplay;
using U=UnityEngine;
public static class MatrixCases {
 static bool Near(U.Vector3 a,U.Vector3 b)=>(a-b).magnitude<2e-5f;
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(31);var parent=new U.GameObject("Matrix parent");var child=new U.GameObject("Matrix child");
  parent.transform.position=new(3,0,0);parent.transform.localScale=new(2,1,1);child.transform.SetParent(parent.transform,false);child.transform.localPosition=U.Vector3.right;child.transform.localRotation=U.Quaternion.Euler(0,0,45);
  var expected=new U.Vector3(5+MathF.Sqrt(2),MathF.Sqrt(.5f),0);var p=child.transform.TransformPoint(U.Vector3.right);
  check("nonuniform rotated hierarchy retains shear",Near(p,expected));
  check("local-to-world matrix agrees with point transform",Near(child.transform.localToWorldMatrix.MultiplyPoint3x4(U.Vector3.right),expected));
  check("world-to-local matrix inverts sheared hierarchy",Near(child.transform.worldToLocalMatrix.MultiplyPoint3x4(p),U.Vector3.right));
  check("inverse point transform inverts sheared hierarchy",Near(child.transform.InverseTransformPoint(p),U.Vector3.right));
  check("lossy scale projects onto world rotation",Near(child.transform.lossyScale,new(1.5f,1.5f,1)));
  parent.transform.localScale=new(-2,1,1);child.transform.rotation=U.Quaternion.identity;
  check("native world rotation setter preserves local scale under signed parent",Near(child.transform.localScale,U.Vector3.one)&&Near(child.transform.lossyScale,new(-2,1,1)));
  child.transform.localRotation=U.Quaternion.Euler(0,0,45);child.transform.SetPositionAndRotation(new(8,2,1),U.Quaternion.identity);
  check("native world combined setter preserves local scale",Near(child.transform.localScale,U.Vector3.one)&&Near(child.transform.position,new(8,2,1)));
  child.transform.localRotation=U.Quaternion.Euler(0,0,45);child.transform.position=new(6,1,0);check("world position setter preserves local rotation and scale",Near(child.transform.localScale,U.Vector3.one)&&Near(child.transform.localRotation*U.Vector3.right,new(MathF.Sqrt(.5f),MathF.Sqrt(.5f),0)));
  parent.transform.localScale=new(2,1,1);child.transform.localPosition=U.Vector3.right;child.transform.localRotation=U.Quaternion.Euler(0,0,45);
  var standalone=U.Matrix4x4.TRS(new(1,2,3),U.Quaternion.Euler(0,90,0),new(2,3,4));
  check("float TRS uses column-major convention",Near(standalone.MultiplyPoint3x4(new(1,2,3)),new(13,8,1)));
  check("matrix product and inverse agree",Near((standalone.inverse*standalone).MultiplyPoint3x4(new(1,2,3)),new(1,2,3)));
  check("singular matrix inverse is zero",U.Matrix4x4.TRS(U.Vector3.zero,U.Quaternion.identity,U.Vector3.zero).inverse==U.Matrix4x4.zero);
  child.transform.hasChanged=false;check("hasChanged can be reset",!child.transform.hasChanged);parent.transform.Translate(U.Vector3.up,U.Space.World);check("ancestor change sets descendant hasChanged",child.transform.hasChanged);
  parent.transform.localScale=U.Vector3.zero;child.transform.localPosition=new(2,3,4);child.transform.localRotation=U.Quaternion.identity;check("zero scale retains local position",Near(child.transform.localPosition,new(2,3,4)));
  parent.transform.localScale=U.Vector3.one;check("zero scale expansion restores stored local coordinates",Near(child.transform.position,new(5,4,4)));
  reset(36);var owner=new U.GameObject("Budget parent");var leaf=new U.GameObject("Budget child");var begin=Legacy.Context.Operations.Count;leaf.transform.SetParent(owner.transform,true);
  var emitted=Legacy.Context.Operations.Skip(begin).ToArray();check("world-preserving parent carries affine state in one operation",emitted.Length==1&&emitted[0].Kind=="parent"&&emitted[0].Data!.Contains("transformState")&&Near(leaf.transform.position,U.Vector3.zero));
  var packet=Legacy.Context.Output();check("real world packet includes matrix and local state",packet.Contains("transformState")&&packet.Contains("matrix")&&packet.Contains("local"));
 }
}
