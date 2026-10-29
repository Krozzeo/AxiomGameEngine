using Axiom.Gameplay;
using Object=Axiom.Gameplay.Object;

public static class TransformCases {
 static bool Near(Vector3 a,Vector3 b,double tolerance=1e-8)=>(a-b).magnitude<tolerance;
 static bool Pose(Transform t,Vector3 p,Vector3 forward)=>Near(t.position,p)&&Near(t.forward,forward);
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(10);check("vector logging does not recurse through normalized properties",new Vector2(1,2).ToString()=="(1, 2)"&&new Vector3(1,2,3).ToString()=="(1, 2, 3)"&&new Vector4(1,2,3,4).ToString()=="(1, 2, 3, 4)");var root=new GameObject("Transform root");var child=new GameObject("Transform child");var leaf=new GameObject("Transform leaf");
  root.transform.position=new(10,0,0);child.transform.position=new(1,0,0);child.transform.SetParent(root.transform,false);leaf.transform.position=new(0,0,2);leaf.transform.SetParent(child.transform,false);
  Context.Operations.Clear();root.transform.Translate(1,2,3,Space.World);
  check("world translation propagates through two generations",Near(child.transform.position,new(12,2,3))&&Near(leaf.transform.position,new(12,2,5)));
  check("child movement is published to host",Context.Operations.Count(op=>op.Kind=="move")==3&&Context.Operations.Any(op=>op.Id==leaf.Entity.Id));
  Context.Operations.Clear();root.transform.rotation=Quaternion.AngleAxis(90,Vector3.up);
  check("parent rotation orbits child and grandchild",Pose(child.transform,new(11,2,2),Vector3.right)&&Pose(leaf.transform,new(13,2,2),Vector3.right));
  check("child rotation is published to host",Context.Operations.Count(op=>op.Kind is "rotation" or "pose"&&op.Rotation.HasValue)==3);
  Context.Operations.Clear();root.transform.localScale=new(2,3,4);
  check("parent scaling propagates local offsets",Near(child.transform.position,new(11,2,1))&&Near(leaf.transform.position,new(19,2,1)));
  check("parent scaling publishes descendant scales",Context.Operations.Count(op=>op.Kind=="scale"||op.Kind=="pose"&&op.Scale.HasValue)==3);
  check("local transforms survive parent pose changes",Near(child.transform.localPosition,Vector3.right)&&Near(leaf.transform.localPosition,new(0,0,2)));
  Context.Operations.Clear();child.transform.SetLocalPositionAndRotation(new(0,1,0),Quaternion.identity);
  child.transform.GetLocalPositionAndRotation(out var localPosition,out var localRotation);
  check("local pose getters agree",Near(localPosition,Vector3.up)&&Near(localRotation*Vector3.forward,Vector3.forward));
  child.transform.GetPositionAndRotation(out var worldPosition,out var worldRotation);
  check("world pose getters agree",Near(worldPosition,new(11,5,3))&&Near(worldRotation*Vector3.forward,Vector3.right));
  var before=child.transform.position;var operations=Context.Operations.Count;
  try{child.transform.SetPositionAndRotation(new(20,0,0),new(0,0,0,0));check("invalid combined pose is atomic",false);}catch(ArgumentException){check("invalid combined pose is atomic",Near(child.transform.position,before)&&Context.Operations.Count==operations);}
  Context.Operations.Clear();var invalidParent=new GameObject("Too far");invalidParent.transform.position=new(1000000,0,0);var detached=new GameObject("Detached");detached.transform.position=new(10,0,0);var parentBefore=detached.transform.parent;operations=Context.Operations.Count;
  try{detached.transform.SetParent(invalidParent.transform,false);check("invalid reparent does not alter hierarchy",false);}catch(ArgumentException){check("invalid reparent does not alter hierarchy",detached.transform.parent==parentBefore&&Near(detached.transform.position,new(10,0,0))&&Context.Operations.Count==operations);}
  Context.Operations.Clear();var parentPosition=root.transform.position;var childPosition=child.transform.position;for(var i=0;i<Limits.Operations-1;i++)Context.Add(new("move",detached.Entity.Id,null,detached.transform.position,null));
  try{root.transform.Translate(Vector3.up,Space.World);check("descendant operation limit rejects entire mutation",false);}catch(InvalidOperationException){check("descendant operation limit rejects entire mutation",Near(root.transform.position,parentPosition)&&Near(child.transform.position,childPosition)&&Context.Operations.Count==Limits.Operations-1);}
  Context.Operations.Clear();var frozen=root.transform.position;
  try{root.transform.Translate(Vector3.one,(Space)500);check("invalid space rejects before mutation",false);}catch(ArgumentOutOfRangeException){check("invalid space rejects before mutation",Near(root.transform.position,frozen)&&Context.Operations.Count==0);}
  var t=new GameObject("Spatial operations").transform;t.SetPositionAndRotation(new(3,4,5),Quaternion.AngleAxis(90,Vector3.up));t.localScale=new(2,3,4);
  var point=new Vector3(1,2,3);check("point includes translation rotation and scale",Near(t.TransformPoint(point),new(15,10,3)));
  check("inverse point round trip",Near(t.InverseTransformPoint(t.TransformPoint(point)),point));
  check("vector excludes translation",Near(t.TransformVector(point),new(12,6,-2)));
  check("inverse vector round trip",Near(t.InverseTransformVector(t.TransformVector(point)),point));
  check("direction excludes scale",Near(t.TransformDirection(point),new(3,2,-1)));
  check("inverse direction round trip",Near(t.InverseTransformDirection(t.TransformDirection(point)),point));
  check("xyz overloads equal vector overloads",Near(t.TransformPoint(1,2,3),t.TransformPoint(point))&&Near(t.InverseTransformPoint(1,2,3),t.InverseTransformPoint(point))&&Near(t.TransformVector(1,2,3),t.TransformVector(point))&&Near(t.InverseTransformVector(1,2,3),t.InverseTransformVector(point))&&Near(t.TransformDirection(1,2,3),t.TransformDirection(point))&&Near(t.InverseTransformDirection(1,2,3),t.InverseTransformDirection(point)));
  t.Translate(1,0,0);check("default translation uses own rotation without scale",Near(t.position,new(3,4,4)));
  var reference=new GameObject("Translation reference").transform;reference.rotation=Quaternion.AngleAxis(90,Vector3.forward);t.Translate(1,0,0,reference);
  check("relative transform translation ignores reference scale",Near(t.position,new(3,5,4)));
  t.Translate(Vector3.up,(Transform?)null);check("null translation reference uses world",Near(t.position,new(3,6,4)));
  Context.Operations.Clear();t.rotation=Quaternion.identity;t.Rotate(90,90,0);
  check("Euler rotation follows Z X Y order",Near(t.forward,new(0,-1,0))&&Near(t.up,Vector3.right));
  var angles=new Vector3(27,63,14);t.eulerAngles=angles;var rotated=t.forward;t.eulerAngles=t.eulerAngles;
  check("Euler setter getter round trip",Near(t.forward,rotated));
  t.localEulerAngles=new(15,25,35);var local=t.localRotation;t.localEulerAngles=t.localEulerAngles;check("local Euler setter getter round trip",Near(t.localRotation*Vector3.forward,local*Vector3.forward));
  t.rotation=Quaternion.AngleAxis(90,Vector3.up);t.Rotate(Vector3.right,90,Space.Self);check("self axis rotation multiplies locally",Near(t.forward,new(0,-1,0)));
  t.rotation=Quaternion.AngleAxis(90,Vector3.up);t.Rotate(Vector3.right,90,Space.World);check("world axis rotation multiplies globally",Near(t.forward,Vector3.right));
  var zeroAxis=t.rotation;operations=Context.Operations.Count;t.Rotate(Vector3.zero,180);check("zero rotation axis is a no-op",t.rotation==zeroAxis&&Context.Operations.Count==operations);
  foreach(var angle in new[]{90d,-90d}){t.eulerAngles=new(angle,35,20);var singularForward=t.forward;var singularRight=t.right;t.eulerAngles=t.eulerAngles;check("Euler singularity round trip "+angle,Near(t.forward,singularForward)&&Near(t.right,singularRight));}
  foreach(var angle in new[]{89.9999d,-89.9999d,89.99999999d,-89.99999999d}){t.eulerAngles=new(angle,35,20);var basisForward=t.forward;var basisRight=t.right;t.eulerAngles=t.eulerAngles;check("Euler near singularity round trip "+angle,Near(t.forward,basisForward)&&Near(t.right,basisRight));}
  t.SetPositionAndRotation(Vector3.right,Quaternion.identity);t.RotateAround(Vector3.zero,Vector3.up,90);check("rotate around changes pose together",Pose(t,new(0,0,-1),Vector3.right));
  t.LookAt(t.position+Vector3.right);check("look at vector points forward",Near(t.forward,Vector3.right));
  reference.position=t.position+Vector3.up;t.LookAt(reference,Vector3.forward);check("look at transform supports custom up",Near(t.forward,Vector3.up)&&Near(t.up,Vector3.forward));
  t.forward=Vector3.back;check("forward setter handles opposite direction",Near(t.forward,Vector3.back));t.right=Vector3.up;check("right setter aligns axis",Near(t.right,Vector3.up));t.up=Vector3.down;check("up setter handles antiparallel direction",Near(t.up,Vector3.down));
  var unchanged=t.rotation;operations=Context.Operations.Count;
  try{t.LookAt(t.position);check("zero look direction rejects atomically",false);}catch(ArgumentException){check("zero look direction rejects atomically",t.rotation==unchanged&&Context.Operations.Count==operations);}
  Context.Operations.Clear();var vectors=new[]{Vector3.right,Vector3.up,Vector3.forward};var original=vectors.ToArray();t.SetPositionAndRotation(new(2,3,4),Quaternion.AngleAxis(90,Vector3.up));t.localScale=new(2,3,4);
  t.TransformPoints(vectors.AsSpan());t.InverseTransformPoints(vectors.AsSpan());check("in-place point batches round trip",vectors.Zip(original).All(pair=>Near(pair.First,pair.Second)));
  t.TransformVectors(vectors.AsSpan());t.InverseTransformVectors(vectors.AsSpan());check("in-place vector batches round trip",vectors.Zip(original).All(pair=>Near(pair.First,pair.Second)));
  t.TransformDirections(vectors.AsSpan());t.InverseTransformDirections(vectors.AsSpan());check("in-place direction batches round trip",vectors.Zip(original).All(pair=>Near(pair.First,pair.Second)));
  var overlap=new[]{Vector3.right,Vector3.up,Vector3.forward,Vector3.one};var expected=overlap.Take(3).Select(t.TransformPoint).ToArray();t.TransformPoints(overlap.AsSpan(0,3),overlap.AsSpan(1,3));check("overlapping batch spans preserve source",overlap.Skip(1).Zip(expected).All(pair=>Near(pair.First,pair.Second)));
  var destination=new[]{Vector3.one,Vector3.one};var inputs=new[]{Vector3.right,new Vector3(double.NaN,0,0)};
  try{t.TransformDirections(inputs,destination);check("batch invalid input leaves destination untouched",false);}catch(ArgumentException){check("batch invalid input leaves destination untouched",destination.All(v=>v==Vector3.one));}
  try{t.TransformPoints(original,destination);check("short batch destination leaves destination untouched",false);}catch(ArgumentException){check("short batch destination leaves destination untouched",destination.All(v=>v==Vector3.one));}
  try{t.TransformVectors(new[]{Vector3.one,new Vector3(double.MaxValue,0,0)},destination);check("batch arithmetic overflow leaves destination untouched",false);}catch(ArgumentException){check("batch arithmetic overflow leaves destination untouched",destination.All(v=>v==Vector3.one));}
  t.TransformPoints(Span<Vector3>.Empty);check("empty batch is valid",true);
  Context.Operations.Clear();var zero=new GameObject("Zero scale").transform;zero.localScale=Vector3.zero;
  try{zero.InverseTransformVector(Vector3.one);check("zero scale inverse vector rejects explicitly",false);}catch(InvalidOperationException){check("zero scale inverse vector rejects explicitly",true);}
  var collapsedChild=new GameObject("Collapsed child").transform;collapsedChild.SetParent(zero,false);zero.position=Vector3.one;check("zero scale hierarchy can translate",Near(collapsedChild.position,Vector3.one));
  collapsedChild.localPosition=new(2,0,0);zero.localScale=Vector3.one;check("collapsed hierarchy retains authored local coordinates on expansion",Near(collapsedChild.position,new(3,1,1)));
  check("transform enumeration preserves immediate sibling order",root.transform.Cast<Transform>().Select(c=>c.gameObject.name).SequenceEqual(new[]{"Transform child"}));
  Object.DestroyImmediate(reference.gameObject);try{reference.TransformPoints(Span<Vector3>.Empty);check("stale batch transform rejects even empty input",false);}catch(InvalidOperationException){check("stale batch transform rejects even empty input",true);}
 }
}
