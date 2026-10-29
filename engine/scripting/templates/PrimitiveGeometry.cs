namespace Axiom.Gameplay;
internal static class PrimitiveGeometry {
 internal static Vector3 WorldHalf(Entity entity)=>WorldHalf(new GameObject(entity).GetComponent<Collider>()!);
 internal static Vector3 WorldHalf(Collider collider){
  var entity=collider.Entity;var half=ColliderInstances.Half(collider);if(collider.RuntimeSlot==null&&(!Context.EntityData[entity.Id].TryGetProperty("runtimeColliderLocal",out var local)||!local.GetBoolean()))return half;
  var scale=Context.Read(entity).Scale;scale=new(Math.Abs(scale.X),Math.Abs(scale.Y),Math.Abs(scale.Z));var shape=collider.Data.GetProperty("shape").GetString();
  if(shape=="sphere"){var radius=half.X*Math.Max(scale.X,Math.Max(scale.Y,scale.Z));return new(radius,radius,radius);}
  if(shape=="capsule"){var radius=half.X*Math.Max(scale.X,scale.Z);return new(radius,Math.Max(radius,half.Y*scale.Y),radius);}
  return new(half.X*scale.X,half.Y*scale.Y,half.Z*scale.Z);
 }
 internal static (Vector3 Start,Vector3 End,double Radius) Segment(Collider collider){var pose=Context.Read(collider.Entity);var half=WorldHalf(collider);var offset=pose.Rotation*new Vector3(0,Math.Max(0,half.Y-half.X),0);return (pose.Position-offset,pose.Position+offset,half.X);}
 private static Vector3 OnSegment(Vector3 point,Vector3 start,Vector3 end){var axis=end-start;return start+axis*(axis.sqrMagnitude<1e-24?0:Math.Clamp(Vector3.Dot(point-start,axis)/axis.sqrMagnitude,0,1));}
 internal static Vector3 Closest(Collider collider,Vector3 point){var (start,end,radius)=Segment(collider);var center=OnSegment(point,start,end);var delta=point-center;return delta.sqrMagnitude<=radius*radius?point:center+delta.normalized*radius;}
 private static double? Sphere(Vector3 origin,Vector3 direction,Vector3 center,double radius){var offset=origin-center;var c=offset.sqrMagnitude-radius*radius;var b=Vector3.Dot(offset,direction);var determinant=b*b-c;return c<=0?0:determinant>=0&&-b-Math.Sqrt(determinant)>=0?-b-Math.Sqrt(determinant):null;}
 internal static bool Ray(Collider collider,Vector3 origin,Vector3 direction,double maximum,out RaycastHit hit){
  hit=default;var shape=collider.Data.GetProperty("shape").GetString();var pose=Context.Read(collider.Entity);var half=WorldHalf(collider);
  if(shape is "plane" or "quad"){
   var normal=pose.Rotation*(shape=="plane"?new Vector3(0,1,0):new Vector3(0,0,-1));var denominator=Vector3.Dot(direction,normal);if(denominator>=-1e-12)return false;var distance=Vector3.Dot(pose.Position-origin,normal)/denominator;if(distance<0||distance>maximum)return false;var point=origin+direction*distance;var local=Quaternion.Inverse(pose.Rotation)*(point-pose.Position);if(shape=="plane"?(Math.Abs(local.X)>half.X||Math.Abs(local.Z)>half.Z):(Math.Abs(local.X)>half.X||Math.Abs(local.Y)>half.Y))return false;hit=new(collider,distance,point,normal);return true;
  }
  var (start,end,radius)=Segment(collider);var closest=OnSegment(origin,start,end);if((origin-closest).sqrMagnitude<=radius*radius){hit=new(collider,0,origin,-direction);return true;}
  var candidates=new List<double>();foreach(var center in new[]{start,end})if(Sphere(origin,direction,center,radius) is double distance)candidates.Add(distance);
  var segment=end-start;var length=segment.magnitude;if(length>1e-12){var axis=segment/length;var offset=origin-start;var d=direction-axis*Vector3.Dot(direction,axis);var o=offset-axis*Vector3.Dot(offset,axis);var a=d.sqrMagnitude;var b=Vector3.Dot(o,d);var c=o.sqrMagnitude-radius*radius;var determinant=b*b-a*c;if(a>1e-24&&determinant>=0)foreach(var distance in new[]{(-b-Math.Sqrt(determinant))/a,(-b+Math.Sqrt(determinant))/a}){var along=Vector3.Dot(offset+direction*distance,axis);if(distance>=0&&along>=0&&along<=length)candidates.Add(distance);}}
  if(candidates.Count==0)return false;var nearest=candidates.Min();if(nearest>maximum)return false;var position=origin+direction*nearest;hit=new(collider,nearest,position,(position-OnSegment(position,start,end)).normalized);return true;
 }
 internal static bool OverlapBox(Collider collider,Vector3 center,Vector3 half,Quaternion rotation){
  var (start,end,radius)=Segment(collider);var inverse=Quaternion.Inverse(rotation);var p=inverse*(start-center);var d=inverse*(end-start);var origins=new[]{p.X,p.Y,p.Z};var directions=new[]{d.X,d.Y,d.Z};var bounds=new[]{half.X,half.Y,half.Z};var cuts=new List<double>{0,1};
  for(var i=0;i<3;i++)if(Math.Abs(directions[i])>1e-24)foreach(var boundary in new[]{-bounds[i],bounds[i]}){var t=(boundary-origins[i])/directions[i];if(t>0&&t<1)cuts.Add(t);}cuts.Sort();
  for(var interval=0;interval<cuts.Count-1;interval++){var middle=(cuts[interval]+cuts[interval+1])*.5;var numerator=0d;var denominator=0d;for(var i=0;i<3;i++){var x=origins[i]+directions[i]*middle;var boundary=Math.Clamp(x,-bounds[i],bounds[i]);if(x!=boundary){numerator+=directions[i]*(origins[i]-boundary);denominator+=directions[i]*directions[i];}}var t=denominator==0?middle:Math.Clamp(-numerator/denominator,cuts[interval],cuts[interval+1]);var squared=0d;for(var i=0;i<3;i++){var x=origins[i]+directions[i]*t;var delta=x-Math.Clamp(x,-bounds[i],bounds[i]);squared+=delta*delta;}if(squared<=radius*radius+1e-12)return true;}
  return false;
 }
}
