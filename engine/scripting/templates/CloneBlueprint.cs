using System.Text.Json;
using System.Text.Json.Nodes;
namespace Axiom.Gameplay;

internal sealed class CloneBlueprint {
 private sealed record NavSnapshot(double Speed,double Distance,bool Stopped,bool Rotation,bool Enabled,bool HasPath,Vector3 Destination){internal void Create(Entity entity){var agent=new AI.NavMeshAgent(entity){speed=Speed,stoppingDistance=Distance,isStopped=Stopped,updateRotation=Rotation,enabled=Enabled};AI.NavMeshAgent.Agents[entity.Id]=agent;if(HasPath)agent.SetDestination(Destination);}}
 private sealed record Node(string SourceId,JsonObject Data,Transform Local,Transform World,bool Active,Script[] Scripts,Type[] Order,NavSnapshot? Agent);
 private readonly Node[] Nodes;private readonly Type? ResultType;private readonly int Generation;
 private CloneBlueprint(Node[] nodes,Type? resultType){Nodes=nodes;ResultType=resultType;Generation=Context.Generation;}
 internal static CloneBlueprint Capture(Object original){
  ArgumentNullException.ThrowIfNull(original);if(!original)throw new ArgumentException("Cannot clone a destroyed object");var root=original is GameObject go?go:original is Component component?component.gameObject:throw new NotSupportedException("This object requires its asset cloning API");
  var ids=new List<string>{root.Entity.Id};for(var i=0;i<ids.Count;i++)ids.AddRange(Context.Parents.Where(entry=>entry.Value==ids[i]).Select(entry=>entry.Key));var nodes=new List<Node>();
  foreach(var id in ids){var entity=new Entity(id,Context.Generation);var data=JsonNode.Parse(Context.EntityData[id].GetRawText())!.AsObject();data.Remove("prefab");data.Remove("parentId");data.Remove("runtimeParentId");data.Remove("runtimeMatrix");data.Remove("runtimeLocalTransform");var scripts=new List<Script>();foreach(var script in Context.LiveScripts.Where(value=>value.Entity.Id==id)){
    var type=ScriptBridge.Type(script);ScriptValues.ValidateClone(type,script);var factory=RuntimeLifecycle.Factories[type];var snapshot=factory.Create();ScriptValues.Copy(type,script,snapshot);scripts.Add(snapshot);
   }
   var agent=AI.NavMeshAgent.Agents.GetValueOrDefault(id);var nav=agent==null?null:new NavSnapshot(agent.speed,agent.stoppingDistance,agent.isStopped,agent.updateRotation,agent.enabled,agent.hasPath,agent.destination);
   nodes.Add(new(id,data,TransformGraph.Local(entity),Context.Read(entity),Context.ActiveStates.GetValueOrDefault(id,true),scripts.ToArray(),new GameObject(entity).AllComponents().Select(ScriptBridge.ComponentType).ToArray(),nav));
  }
  var result=new CloneBlueprint(nodes.ToArray(),original is Component c?ScriptBridge.ComponentType(c):null);result.CaptureParents();return result;
 }
 internal int EntityCount=>Nodes.Length;internal int ScriptCount=>Nodes.Sum(node=>node.Scripts.Length);
 internal Object Instantiate(UnityEngine.InstantiateParameters parameters,UnityEngine.Vector3? position=null,UnityEngine.Quaternion? rotation=null,bool initialize=true){
  return InstantiateBatch(1,parameters,position is {} p?new[]{p}:Array.Empty<UnityEngine.Vector3>(),rotation is {} r?new[]{r}:Array.Empty<UnityEngine.Quaternion>(),initialize)[0];
 }
 internal Object[] InstantiateBatch(int count,UnityEngine.InstantiateParameters parameters,UnityEngine.Vector3[] positions,UnityEngine.Quaternion[] rotations,bool initialize=true){
  if(count<1||count>(Limits.Entities-Context.Entities.Count)/EntityCount)throw new ArgumentOutOfRangeException(nameof(count),"Clone count exceeds entity capacity");
  if((long)ScriptCount*count+Context.LiveScripts.Count>Limits.Attachments||(long)Nodes.Count(node=>node.Data.ContainsKey("audioSource"))*count+Context.AudioSources.Count>32)throw new InvalidOperationException("Clone exceeds component capacity");
  if((long)(1+ScriptCount*2+Nodes.Count(node=>node.Agent!=null)*8)*count+Context.Operations.Count>=Limits.Operations)throw new InvalidOperationException("Clone exceeds transport operation capacity");
  if((long)Nodes.Sum(node=>System.Text.Encoding.UTF8.GetByteCount(node.Data.ToJsonString())+4096)*count>8*1024*1024)throw new InvalidOperationException("Clone snapshot exceeds transport budget");
  foreach(var p in positions)TransformMath.Finite(p);foreach(var r in rotations)TransformMath.Normalize(r);
  var previous=Context.Entities.Keys.ToHashSet();var begin=Context.Operations.Count;var spawnCount=Context.Spawned;var dirty=SceneManagement.SceneManager.MetadataDirty;var result=new Object[count];
  try {
   for(var i=0;i<count;i++)result[i]=InstantiateCore(parameters,positions.Length==0?null:positions[i%positions.Length],rotations.Length==0?null:rotations[i%rotations.Length]);
   // One import keeps large batches within the wire operation limit.
   var imports=new JsonArray();foreach(var operation in Context.Operations.Skip(begin).Where(op=>op.Kind=="import"))foreach(var node in JsonNode.Parse(operation.Data!)!.AsArray())imports.Add(node!.DeepClone());for(var i=Context.Operations.Count-1;i>=begin;i--)if(Context.Operations[i].Kind=="import")Context.Operations.RemoveAt(i);
   var payload=imports.ToJsonString();if(System.Text.Encoding.UTF8.GetByteCount(payload)>8*1024*1024)throw new InvalidOperationException("Clone snapshot exceeds transport budget");Context.Operations.Insert(begin,new("import","",null,default,null,Data:payload));
  }catch {
   var created=Context.Entities.Keys.Where(id=>!previous.Contains(id)).ToHashSet();
   foreach(var script in Context.LiveScripts.Where(s=>created.Contains(s.Entity.Id)).ToArray()){Context.LiveScripts.Remove(script);Context.ScriptPaths.Remove(script);}
   foreach(var id in created){Context.RetireIdentities(id);Context.Entities.Remove(id);Context.EntityData.Remove(id);Context.Parents.Remove(id);Context.ActiveStates.Remove(id);Context.Bodies.Remove(id);Context.Cameras.Remove(id);Context.AudioSources.Remove(id);Context.Animators.Remove(id);Context.AnimationStates.Remove(id);Context.Grounded.Remove(id);SceneManagement.SceneManager.Owners.Remove(id);AI.NavMeshAgent.Agents.Remove(id);}
   Context.Operations.RemoveRange(begin,Context.Operations.Count-begin);Context.Spawned=spawnCount;SceneManagement.SceneManager.MetadataDirty=dirty;throw;
  }
  if(initialize)RuntimeLifecycle.Initialize();return result;
 }
 private Object InstantiateCore(UnityEngine.InstantiateParameters parameters,UnityEngine.Vector3? position,UnityEngine.Quaternion? rotation){
  if(Generation!=Context.Generation)throw new OperationCanceledException("The source runtime session ended");
  if(Context.Entities.Count+EntityCount>Limits.Entities||Context.LiveScripts.Count+ScriptCount>Limits.Attachments)throw new InvalidOperationException("Clone exceeds runtime entity/script capacity");
  if(Context.AudioSources.Count+Nodes.Count(node=>node.Data.ContainsKey("audioSource"))>32)throw new InvalidOperationException("Clone exceeds audio voice capacity");
  if(Nodes.Sum(node=>System.Text.Encoding.UTF8.GetByteCount(node.Data.ToJsonString())+1024)>8*1024*1024)throw new InvalidOperationException("Clone snapshot exceeds transport budget");
  var parent=parameters.parent?.LegacyTransform;var scene=parent!=null?parent.gameObject.scene:(SceneManagement.Scene)parameters.scene;if(!scene.IsValid())scene=SceneManagement.SceneManager.GetActiveScene();if(!scene.isLoaded)throw new ArgumentException("Destination scene must be loaded");
  if(Context.Operations.Count+1+ScriptCount*2>=Limits.Operations)throw new InvalidOperationException("AX_SCRIPT_0003: operation limit exceeded");
  var rootSource=Nodes[0];var rootLocal=parameters.worldSpace?TransformGraph.Relative(default,position??rootSource.World.Position,rotation is UnityEngine.Quaternion q?(Quaternion)q:rootSource.World.Rotation,rootSource.World.Scale,parent?.Entity.Id):new Transform(position??rootSource.Local.Position,rotation is UnityEngine.Quaternion qr?(Quaternion)qr:rootSource.Local.Rotation,rootSource.Local.Scale);
  var begin=Context.Operations.Count;var map=new Dictionary<string,Entity>();var matrices=new Dictionary<string,Affine>();var rotations=new Dictionary<string,Quaternion>();var locals=new Dictionary<string,Transform>();
  foreach(var node in Nodes){var name=node.Data["name"]!.GetValue<string>();if(node==rootSource)name=name[..Math.Min(name.Length,121)]+"(Clone)";var entity=Context.CreateObject(name);Context.Operations.RemoveAt(Context.Operations.Count-1);map[node.SourceId]=entity;}
  foreach(var node in Nodes){var entity=map[node.SourceId];var root=node==rootSource;var sourceParent=root?null:Nodes.First(candidate=>ContextParent(candidate.SourceId,node.SourceId));
   var parentId=root?parent?.Entity.Id:map[sourceParent!.SourceId].Id;var local=root?rootLocal:node.Local;var matrix=(root?(parent==null?Affine.Identity:TransformGraph.World(parent.Entity)):matrices[sourceParent!.SourceId])*Affine.TRS(local.Position,local.Rotation,local.Scale);var parentRotation=root?(parent?.rotation??Quaternion.identity):rotations[sourceParent!.SourceId];var worldRotation=TransformMath.Normalize(parentRotation*local.Rotation);
   if(matrix.M.Any(v=>!double.IsFinite(v)||Math.Abs(v)>1000000))throw new ArgumentException("Clone transform exceeds world bounds");matrices[node.SourceId]=matrix;rotations[node.SourceId]=worldRotation;locals[node.SourceId]=local;
   var data=node.Data.DeepClone().AsObject();data["id"]=entity.Id;data["name"]=Context.Metadata(entity,"name","");data["runtimeParentId"]=parentId;data["runtimeActive"]=node.Active;
   data["transform"]=Pose(matrix.Point(Vector3.zero),worldRotation,matrix.Scale);data["runtimeLocalTransform"]=Pose(local.Position,local.Rotation,local.Scale);data["runtimeMatrix"]=new JsonArray(matrix.M.Select(value=>(JsonNode?)JsonValue.Create(value)).ToArray());if(node.Scripts.Length>0)data["scriptComponents"]=new JsonArray();else data.Remove("scriptComponents");
   Context.EntityData[entity.Id]=Context.ParseData(data.ToJsonString());Context.Entities[entity.Id]=new Transform(matrix.Point(Vector3.zero),worldRotation,matrix.Scale){Entity=entity};Context.Parents[entity.Id]=parentId;Context.ActiveStates[entity.Id]=node.Active;Context.Refresh(entity);TransformGraph.Seed(entity,local);SceneManagement.SceneManager.SetOwner(entity.Id,scene.name);
  }
  foreach(var node in Nodes){var entity=map[node.SourceId];node.Agent?.Create(entity);foreach(var script in node.Scripts)RuntimeLifecycle.Clone(script,entity);}
  foreach(var script in Context.LiveScripts.Where(value=>map.Values.Any(entity=>entity.Id==value.Entity.Id)).ToArray()){ScriptValues.RemapClone(ScriptBridge.Type(script),script,map);RuntimeLifecycle.PublishComponents(script.Entity);}
  foreach(var node in Nodes){var entity=map[node.SourceId];var components=new GameObject(entity).AllComponents();Context.ComponentOrders[entity]=node.Order.Select(type=>components.First(value=>ScriptBridge.ComponentType(value)==type).GetInstanceID()).ToList();}
  var import=new JsonArray();foreach(var node in Nodes)import.Add(JsonNode.Parse(Context.EntityData[map[node.SourceId].Id].GetRawText()));var payload=import.ToJsonString();if(System.Text.Encoding.UTF8.GetByteCount(payload)>8*1024*1024)throw new InvalidOperationException("Clone snapshot exceeds transport budget");Context.Operations.Insert(begin,new("import","",null,default,null,Data:payload));
  var result=new GameObject(map[rootSource.SourceId]);return ResultType==null?result:result.GetComponent(ResultType)!;
  bool ContextParent(string parentId,string childId)=>SourceParents.GetValueOrDefault(childId)==parentId;
 }
 private readonly Dictionary<string,string?> SourceParents=new();
 internal void CaptureParents(){foreach(var node in Nodes)SourceParents[node.SourceId]=Context.Parents.GetValueOrDefault(node.SourceId);}
 private static JsonObject Pose(Vector3 p,Quaternion q,Vector3 s)=>new(){["position"]=new JsonArray(p.X,p.Y,p.Z),["rotation"]=new JsonArray(q.X,q.Y,q.Z,q.W),["scale"]=new JsonArray(s.X,s.Y,s.Z)};
}
