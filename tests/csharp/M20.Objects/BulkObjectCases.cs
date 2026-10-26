using Unity.Collections;
using L=Axiom.Gameplay;
using U=UnityEngine;
public static class BulkObjectCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(70);var source=new U.GameObject("Bulk source");var sourceId=source.GetInstanceID();var sourceEntity=source.GetEntityId();
  using var objects=new NativeArray<int>(3,Allocator.Persistent);using var transforms=new NativeArray<int>(3,Allocator.Persistent);
  var scene=U.SceneManagement.SceneManager.CreateScene("Bulk scene");L.Context.Operations.Clear();U.GameObject.InstantiateGameObjects(sourceId,3,objects,transforms,scene);
  check("bulk clone outputs distinct native game and transform identities",objects.ToArray().Distinct().Count()==3&&objects.ToArray().All(id=>!transforms.Contains(id)));
  check("bulk clone IDs resolve to actual scene objects",objects.ToArray().All(id=>U.NativeObjects.GameObject(id).scene.name==scene.name)&&L.Context.Entities.Count==4);
  check("bulk clone uses one import operation",L.Context.Operations.Count(op=>op.Kind=="import")==1);
  U.GameObject.SetGameObjectsActive(objects,false);check("NativeArray int bulk deactivates actual objects",objects.ToArray().All(id=>!U.NativeObjects.GameObject(id).activeSelf));
  U.GameObject.SetGameObjectsActive(objects.ToArray().AsSpan(),true);check("int span bulk reactivates actual objects",objects.ToArray().All(id=>U.NativeObjects.GameObject(id).activeInHierarchy));
  using var entityIds=new NativeArray<U.EntityId>(2,Allocator.TempJob);using var transformIds=new NativeArray<U.EntityId>(2,Allocator.TempJob);
  U.GameObject.InstantiateGameObjects(sourceEntity,2,entityIds,transformIds,scene);check("EntityId bulk outputs native scene identities",entityIds.ToArray().All(id=>U.GameObject.GetScene(id)==scene)&&entityIds[0]!=transformIds[0]);
  U.GameObject.SetGameObjectsActive(entityIds,false);check("NativeArray EntityId bulk activation reaches native objects",entityIds.ToArray().All(id=>!U.NativeObjects.GameObject(id.Value).activeSelf));
  U.GameObject.SetGameObjectsActive(entityIds.ToArray().AsSpan(),true);check("EntityId span bulk activation reaches native objects",entityIds.ToArray().All(id=>U.NativeObjects.GameObject(id.Value).activeSelf));
  var count=L.Context.Entities.Count;objects[0]=12345;try{U.GameObject.InstantiateGameObjects(sourceId,2,objects,transforms);check("bulk output mismatch leaves scene and outputs untouched",false);}catch(ArgumentException){check("bulk output mismatch leaves scene and outputs untouched",L.Context.Entities.Count==count&&objects[0]==12345);}
  var emitted=L.Context.Operations.Count;try{U.GameObject.SetGameObjectsActive(new[]{sourceId,-1}.AsSpan(),false);check("bulk invalid ID has no partial native activation",false);}catch(ArgumentException){check("bulk invalid ID has no partial native activation",source.activeSelf&&L.Context.Operations.Count==emitted);}
  check("bulk GetScene invalid identity is invalid",!U.GameObject.GetScene(default(U.EntityId)).IsValid());
  var sourceArray=new[]{1,2};var owned=new NativeArray<int>(sourceArray,Allocator.Persistent);sourceArray[0]=99;var alias=owned;alias[1]=7;check("NativeArray owns independent contiguous storage with shared aliases",owned[0]==1&&owned[1]==7);
  var copy=owned.ToArray();copy[0]=88;check("NativeArray ToArray returns independent copy",owned[0]==1);owned.Dispose();check("NativeArray Dispose invalidates every alias",!owned.IsCreated&&!alias.IsCreated);try{_=alias[0];check("disposed NativeArray access is rejected",false);}catch(ObjectDisposedException){check("disposed NativeArray access is rejected",true);}
  U.Object.DestroyImmediate(source);try{using var a=new NativeArray<int>(1,Allocator.Temp);using var b=new NativeArray<int>(1,Allocator.Temp);U.GameObject.InstantiateGameObjects(sourceId,1,a,b);check("destroyed bulk source is rejected",false);}catch(ArgumentException){check("destroyed bulk source is rejected",true);}
  reset(71);try{_=U.NativeObjects.GameObject(objects[1]);check("bulk identifiers invalidate with runtime generation",false);}catch(ArgumentException){check("bulk identifiers invalidate with runtime generation",true);}
 }
}
