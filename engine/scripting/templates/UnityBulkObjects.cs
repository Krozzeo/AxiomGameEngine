using Unity.Collections;
using Legacy=Axiom.Gameplay;
namespace UnityEngine;
public partial class GameObject {
 private static Legacy.GameObject? BulkTarget(int id){try{return NativeObjects.GameObject(id);}catch(ArgumentException){return null;}}
 public SceneManagement.Scene scene=>LegacyGameObject.scene;
 [Obsolete("Obsolete. Please use GameObject.GetScene(EntityId entityId) instead.")]
 public static SceneManagement.Scene GetScene(int instanceID)=>GetScene(new EntityId(instanceID));
 public static SceneManagement.Scene GetScene(EntityId entityId){try{return NativeObjects.GameObject(entityId.Value).scene;}catch(ArgumentException){return default;}}
 [Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(ReadOnlySpan<EntityId>, bool) instead.")]
 public static void SetGameObjectsActive(ReadOnlySpan<int> instanceIDs,bool active){
  var targets=instanceIDs.ToArray().Distinct().Select(BulkTarget).OfType<Legacy.GameObject>().ToArray();
  if(targets.Length==0)return;
  var ids=new System.Text.Json.Nodes.JsonArray(targets.Select(go=>(System.Text.Json.Nodes.JsonNode?)System.Text.Json.Nodes.JsonValue.Create(go.Entity.Id)).ToArray());
  Legacy.Context.Add(new("activeMany","",null,default,null,Data:new System.Text.Json.Nodes.JsonObject{["ids"]=ids,["active"]=active}.ToJsonString()));
  foreach(var target in targets)Legacy.Context.ActiveStates[target.Entity.Id]=active;
  Legacy.RuntimeLifecycle.Activation();
 }
 public static void SetGameObjectsActive(ReadOnlySpan<EntityId> entityIds,bool active)=>SetGameObjectsActive(entityIds.ToArray().Select(id=>id.Value).ToArray().AsSpan(),active);
 [Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(NativeArray<EntityId>, bool) instead.")]
 public static void SetGameObjectsActive(NativeArray<int> instanceIDs,bool active){if(!instanceIDs.IsCreated)throw new ArgumentException("The input NativeArray must be created.");SetGameObjectsActive(instanceIDs.ToArray().AsSpan(),active);}
 public static void SetGameObjectsActive(NativeArray<EntityId> entityIds,bool active){if(!entityIds.IsCreated)throw new ArgumentException("The input NativeArray must be created.");SetGameObjectsActive(entityIds.ToArray().AsSpan(),active);}
 [Obsolete("Obsolete. Please use GameObject.InstantiateGameObjects(EntityId, int, NativeArray<EntityId>, NativeArray<EntityId>, Scene) instead.")]
 public static void InstantiateGameObjects(int sourceInstanceID,int count,NativeArray<int> newInstanceIDs,NativeArray<int> newTransformInstanceIDs,SceneManagement.Scene destinationScene=default){
  if(!newInstanceIDs.IsCreated||!newTransformInstanceIDs.IsCreated)throw new ArgumentException("Clone output arrays must be created");
  if(count<0||newInstanceIDs.Length!=count||newTransformInstanceIDs.Length!=count)throw new ArgumentException("Clone output arrays must match count");
  if(count==0)return;
  var source=BulkTarget(sourceInstanceID);if(source==null)return;
  var objects=Legacy.CloneBlueprint.Capture(source).InstantiateBatch(count,new InstantiateParameters{scene=destinationScene,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),false).Cast<Legacy.GameObject>().ToArray();
  for(var i=0;i<count;i++){newInstanceIDs[i]=objects[i].GetInstanceID();newTransformInstanceIDs[i]=objects[i].transform.GetInstanceID();}
  Legacy.RuntimeLifecycle.Initialize();
 }
 public static void InstantiateGameObjects(EntityId sourceEntityId,int count,NativeArray<EntityId> newEntityIds,NativeArray<EntityId> newTransformEntityIds,SceneManagement.Scene destinationScene=default){
  if(!newEntityIds.IsCreated||!newTransformEntityIds.IsCreated)throw new ArgumentException("Clone output arrays must be created");
  if(count<0||newEntityIds.Length!=count||newTransformEntityIds.Length!=count)throw new ArgumentException("Clone output arrays must match count");
  using var gameObjects=new NativeArray<int>(count,Allocator.Temp);using var transforms=new NativeArray<int>(count,Allocator.Temp);
  InstantiateGameObjects(sourceEntityId.Value,count,gameObjects,transforms,destinationScene);
  for(var i=0;i<count;i++){newEntityIds[i]=new(gameObjects[i]);newTransformEntityIds[i]=new(transforms[i]);}
 }
}
