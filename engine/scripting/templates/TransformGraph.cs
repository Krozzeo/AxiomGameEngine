using System.Text.Json;
namespace Axiom.Gameplay;

internal static class TransformGraph {
 private static readonly Dictionary<Entity,Transform> Locals=new();
 private static readonly HashSet<Entity> Changed=new();
 private static readonly Dictionary<Entity,int> Capacities=new();
 internal static void Reset(){Locals.Clear();Changed.Clear();Capacities.Clear();}
 internal static void Seed(Entity entity,Transform local){Locals[entity]=local;Changed.Add(entity);}
 internal static void Forget(string id){foreach(var key in Locals.Keys.Where(e=>e.Id==id).ToArray())Locals.Remove(key);Changed.RemoveWhere(e=>e.Id==id);foreach(var key in Capacities.Keys.Where(e=>e.Id==id).ToArray())Capacities.Remove(key);}
 internal static int Count(Entity root){_=Context.Read(root);var ids=new HashSet<string>{root.Id};var changed=true;while(changed){changed=false;foreach(var entry in Context.Parents)if(entry.Value!=null&&ids.Contains(entry.Value)&&ids.Add(entry.Key))changed=true;}return ids.Count;}
 internal static int Capacity(Entity root){var count=Count(root);return Math.Max(count,Capacities.GetValueOrDefault(root,count));}
 internal static void SetCapacity(Entity root,int capacity){if(capacity<0||capacity>Limits.Entities)throw new ArgumentOutOfRangeException(nameof(capacity));capacity=Math.Max(capacity,Count(root));Locals.EnsureCapacity(Math.Max(Locals.Count,capacity));Capacities[root]=capacity;}
 internal static void Load(){
  Locals.Clear();Changed.RemoveWhere(e=>e.Generation!=Context.Generation||!Context.Entities.ContainsKey(e.Id));
  foreach(var (id,data) in Context.EntityData)if(data.TryGetProperty("runtimeLocalTransform",out var local))Locals[new(id,Context.Generation)]=Transform.Read(local);
 }
 internal static Transform Local(Entity entity){
  var world=Context.Read(entity);if(Locals.TryGetValue(entity,out var local))return local;
  var parent=Context.Parents.GetValueOrDefault(entity.Id);if(parent==null)local=new(world.Position,world.Rotation,world.Scale);
  else{var handle=new Entity(parent,Context.Generation);var p=Context.Read(handle);var inverse=World(handle).Inverse()??throw new InvalidOperationException("Cannot derive local coordinates through a singular parent");var scale=p.Scale;
   if(scale.X==0||scale.Y==0||scale.Z==0)throw new InvalidOperationException("Cannot derive local scale through a singular parent");
   local=new(inverse.Point(world.Position),Affine.Mirror(Quaternion.Inverse(p.Rotation)*world.Rotation,scale),new(world.Scale.X/scale.X,world.Scale.Y/scale.Y,world.Scale.Z/scale.Z));
  }
  Locals[entity]=local;return local;
 }
 internal static Affine World(Entity entity){var chain=new List<Entity>();var seen=new HashSet<string>();var current=entity;while(true){_=Context.Read(current);if(!seen.Add(current.Id))throw new InvalidOperationException("Hierarchy cycle");chain.Add(current);var parent=Context.Parents.GetValueOrDefault(current.Id);if(parent==null)break;current=new(parent,Context.Generation);}var world=Affine.Identity;for(var i=chain.Count-1;i>=0;i--){var local=Local(chain[i]);world=world*Affine.TRS(local.Position,local.Rotation,local.Scale);}return world;}
 internal static bool HasChanged(Entity entity){_=Context.Read(entity);return Changed.Contains(entity);}
 internal static void SetChanged(Entity entity,bool changed){_=Context.Read(entity);if(changed)Changed.Add(entity);else Changed.Remove(entity);}
 private static void Validate(Vector3 v){TransformMath.Finite(v);if(Math.Max(Math.Abs(v.X),Math.Max(Math.Abs(v.Y),Math.Abs(v.Z)))>1000000)throw new ArgumentException("AX_SCRIPT_0003: invalid transform bounds");}
 internal static Transform Relative(Entity entity,Vector3 position,Quaternion rotation,Vector3 scale,string? parent){
  if(parent==null)return new(position,rotation,scale);
  var handle=new Entity(parent,Context.Generation);var p=Context.Read(handle);var matrix=World(handle);var inverse=matrix.Inverse()??throw new InvalidOperationException("Cannot preserve a world pose through a singular parent");
  var r=TransformMath.Normalize(Affine.Mirror(Quaternion.Inverse(p.Rotation)*rotation,p.Scale));var unit=matrix*Affine.TRS(Vector3.zero,r,Vector3.one);var basis=unit.ProjectedScale(rotation);
  static double Divide(double value,double divisor){if(divisor==0)throw new InvalidOperationException("Cannot preserve world scale through a singular parent");return value/divisor;}
  return new(inverse.Point(position),r,new(Divide(scale.X,basis.X),Divide(scale.Y,basis.Y),Divide(scale.Z,basis.Z)));
 }
 internal static void SetWorld(Entity entity,Vector3? position,Quaternion? rotation,Vector3? scale,int reserve=0){
  var old=Context.Read(entity);var local=Local(entity);var parent=Context.Parents.GetValueOrDefault(entity.Id);var p=local.Position;var q=local.Rotation;
  if(position is Vector3 point)p=parent==null?point:(World(new(parent,Context.Generation)).Inverse()??throw new InvalidOperationException("Cannot set world position through a singular parent")).Point(point);
  if(rotation is Quaternion requested){q=TransformMath.Normalize(requested);if(parent!=null){var pose=Context.Read(new(parent,Context.Generation));q=Affine.Mirror(Quaternion.Inverse(pose.Rotation)*q,pose.Scale);}}
  // Position/rotation setters preserve local scale, even when world projected scale changes.
  var s=scale is Vector3 value?Relative(entity,position??old.Position,rotation??old.Rotation,value,parent).Scale:local.Scale;
  Apply(entity,new(p,q,s),parent,false,reserve,position!=null,rotation!=null,scale!=null);
 }
 internal static void SetLocal(Entity entity,Vector3? position,Quaternion? rotation,Vector3? scale){var local=Local(entity);var target=new Transform(position??local.Position,rotation is Quaternion q?TransformMath.Normalize(q):local.Rotation,scale??local.Scale);Apply(entity,target,Context.Parents.GetValueOrDefault(entity.Id),false,0,true,true,true);}
 internal static void Parent(Entity entity,Transform? parent,bool keepWorld){
  var old=Context.Read(entity);if(!ReferenceEquals(parent,null))_=Context.Read(parent.Entity);var id=parent?.Entity.Id;
  for(var current=id;current!=null;current=Context.Parents.GetValueOrDefault(current))if(current==entity.Id)throw new ArgumentException("Hierarchy cycle");
  var local=keepWorld?Relative(entity,old.Position,old.Rotation,old.Scale,id):Local(entity);
  Apply(entity,local,id,true,0,!keepWorld,!keepWorld,!keepWorld);
 }
 private static void Apply(Entity entity,Transform target,string? parent,bool reparent,int reserve,bool forcePosition,bool forceRotation,bool forceScale){
  Validate(target.Position);Validate(target.Scale);target=new(target.Position,TransformMath.Normalize(target.Rotation),target.Scale);
  var handles=new List<Entity>{entity};for(var i=0;i<handles.Count;i++)handles.AddRange(Context.Parents.Where(pair=>pair.Value==handles[i].Id).Select(pair=>new Entity(pair.Key,Context.Generation)));
  foreach(var handle in handles)_=Local(handle);
  var parentMatrix=parent==null?Affine.Identity:World(new(parent,Context.Generation));var parentRotation=parent==null?Quaternion.identity:Context.Read(new(parent,Context.Generation)).Rotation;var parentScale=parent==null?Vector3.one:Context.Read(new(parent,Context.Generation)).Scale;
  var matrices=new Dictionary<string,Affine>();var poses=new Dictionary<string,Transform>();var operations=new List<Operation>();string? parentState=null;
  foreach(var handle in handles){var first=handle==entity;var local=first?target:Locals[handle];var parentId=first?parent:Context.Parents.GetValueOrDefault(handle.Id);
   var p=first?parentMatrix:matrices[parentId!];var pr=first?parentRotation:poses[parentId!].Rotation;
   var ps=first?parentScale:poses[parentId!].Scale;var matrix=p*Affine.TRS(local.Position,local.Rotation,local.Scale);var rotation=TransformMath.Normalize(pr*Affine.Mirror(local.Rotation,ps));var pose=new Transform(matrix.Point(Vector3.zero),rotation,matrix.ProjectedScale(rotation)){Entity=handle};Validate(pose.Position);Validate(pose.Scale);if(matrix.M.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite hierarchy matrix");
   matrices[handle.Id]=matrix;poses[handle.Id]=pose;var old=Context.Read(handle);var start=operations.Count;
   if(first&&forcePosition||pose.Position!=old.Position)operations.Add(new("move",handle.Id,null,pose.Position,null));
   if(first&&forceRotation||pose.Rotation!=old.Rotation)operations.Add(new("rotation",handle.Id,null,default,null,Rotation:pose.Rotation));
   if(first&&forceScale||pose.Scale!=old.Scale)operations.Add(new("scale",handle.Id,null,pose.Scale,null));
   var state=State(matrix,local);
   if(first&&reparent)parentState=state;
   if(start==operations.Count){if(first&&reparent)continue;if(matrix.M.SequenceEqual(World(handle).M))continue;operations.Add(new("move",handle.Id,null,pose.Position,null));}
   if(operations.Count-start>1){operations.RemoveRange(start,operations.Count-start);operations.Add(new("pose",handle.Id,null,pose.Position,null,Rotation:pose.Rotation,Data:state,Scale:pose.Scale));}
   else operations[^1]=operations[^1] with {Data=state};
  }
  if(Context.Operations.Count+operations.Count+reserve+(reparent?1:0)>Limits.Operations)throw new InvalidOperationException($"AX_SCRIPT_0003: operation limit exceeded (existing={Context.Operations.Count}, requested={operations.Count+reserve+(reparent?1:0)}, subtree={handles.Count})");
  Locals[entity]=target;if(reparent){Context.Parents[entity.Id]=parent;Context.Operations.Add(new("parent",entity.Id,null,default,null,Name:parent,Data:parentState));}
  Context.Operations.AddRange(operations);foreach(var handle in handles){Context.Entities[handle.Id]=poses[handle.Id];Changed.Add(handle);}
  if(reparent){SceneManagement.SceneManager.Reparented(entity,parent);RuntimeLifecycle.Activation();}
 }
 private static string State(Affine matrix,Transform local){using var stream=new MemoryStream();using(var writer=new Utf8JsonWriter(stream)){writer.WriteStartObject();writer.WriteStartObject("transformState");writer.WriteStartArray("matrix");foreach(var value in matrix.M)writer.WriteNumberValue(value);writer.WriteEndArray();writer.WriteStartObject("local");writer.WritePropertyName("position");local.Position.Write(writer);writer.WriteStartArray("rotation");writer.WriteNumberValue(local.Rotation.X);writer.WriteNumberValue(local.Rotation.Y);writer.WriteNumberValue(local.Rotation.Z);writer.WriteNumberValue(local.Rotation.W);writer.WriteEndArray();writer.WritePropertyName("scale");local.Scale.Write(writer);writer.WriteEndObject();writer.WriteEndObject();writer.WriteEndObject();}return System.Text.Encoding.UTF8.GetString(stream.ToArray());}
}

// Canonical queries preserve ancestor reflections and invert each TRS separately.
// A collapsed scale axis has a zero reciprocal, matching the native reference.
internal static class TransformQueries {
 private static Vector3 Sign(Vector3 scale)=>new(scale.X<0?-1:1,scale.Y<0?-1:1,scale.Z<0?-1:1);
 private static Vector3 Multiply(Vector3 a,Vector3 b)=>new(a.X*b.X,a.Y*b.Y,a.Z*b.Z);
 internal static Vector3 Direction(Entity entity,Vector3 value,bool inverse){
  var local=Context.Local(entity);var parent=Context.Parents.GetValueOrDefault(entity.Id);
  if(inverse){if(parent!=null){var owner=new Entity(parent,Context.Generation);value=Multiply(Sign(Context.Local(owner).Scale),Direction(owner,value,true));}return Quaternion.Inverse(local.Rotation)*value;}
  value=local.Rotation*value;if(parent==null)return value;var ancestor=new Entity(parent,Context.Generation);return Direction(ancestor,Multiply(Sign(Context.Local(ancestor).Scale),value),false);
 }
 internal static Vector3 Inverse(Entity entity,Vector3 value,bool point){
  var local=Context.Local(entity);var parent=Context.Parents.GetValueOrDefault(entity.Id);if(parent!=null)value=Inverse(new Entity(parent,Context.Generation),value,point);if(point)value-=local.Position;value=Quaternion.Inverse(local.Rotation)*value;
  return new(local.Scale.X==0?0:value.X/local.Scale.X,local.Scale.Y==0?0:value.Y/local.Scale.Y,local.Scale.Z==0?0:value.Z/local.Scale.Z);
 }
}
