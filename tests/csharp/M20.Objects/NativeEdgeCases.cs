using U=UnityEngine;
public static class NativeEdgeCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(3101);var unnamed=new U.GameObject(null!);check("native null constructor name uses default name",unnamed.name=="New Game Object");
  try{_=new U.GameObject("Null components",(Type[])null!);check("native null constructor component array rejects",false);}catch(NullReferenceException){check("native null constructor component array rejects",true);}
  check("native sync null source uses ArgumentException",ThrowsExact(()=>U.Object.Instantiate((U.Object)null!),typeof(ArgumentException)));
  check("native async null source uses ArgumentException",ThrowsExact(()=>U.Object.InstantiateAsync((U.GameObject)null!),typeof(ArgumentException)));
  var parent=new U.GameObject("Mirrored parent");parent.transform.position=new(3,0,0);parent.transform.rotation=U.Quaternion.Euler(0,30,15);parent.transform.localScale=new(-2,1,1);var child=new U.GameObject("Rotation child");child.transform.SetParent(parent.transform,false);child.transform.localPosition=U.Vector3.right;child.transform.localScale=new(.5f,2,3);
  child.transform.rotation=U.Quaternion.identity;child.transform.Rotate(U.Vector3.up,90,U.Space.Self);check("native axis self rotation uses rotation-space axis under mirrored parent",Near(child.transform.forward,U.Vector3.right));
  child.transform.rotation=U.Quaternion.identity;child.transform.Rotate(new U.Vector3(0,90,0),U.Space.Self);check("native Euler self rotation composes local quaternion under mirrored parent",Near(child.transform.forward,-U.Vector3.right));
  child.transform.rotation=U.Quaternion.identity;child.transform.RotateAroundLocal(U.Vector3.up,MathF.PI/2);check("native obsolete local rotation uses radians and local pre-multiplication",Near(child.transform.forward,new(-.9949323f,-.09914385f,.0167467482f)));
  child.transform.rotation=U.Quaternion.identity;child.transform.RotateAround(U.Vector3.up,MathF.PI/2);check("native obsolete world rotation uses radians",Near(child.transform.forward,U.Vector3.right));
  static bool ThrowsExact(Action action,Type type){try{action();return false;}catch(Exception error){return error.GetType()==type;}}
  static bool Near(U.Vector3 a,U.Vector3 b)=>(a-b).magnitude<2e-5f;
 }
}
