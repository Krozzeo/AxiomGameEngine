# GameObject

Namespace: `Axiom.Gameplay`

## Description

Create an empty runtime GameObject, retrieve/add compiled components, activate it and query live tags/layers. One script instance per compiled physical path and one built-in storage kind per object are supported. AddComponent supports compiled scripts, box/sphere colliders in 2D/3D, Rigidbody/Rigidbody2D, Camera and AudioListener. Asset-backed AudioSource and Animator require authored configuration and can be cloned. Transform cannot be removed.

## Example

```csharp
var child = new GameObject("Sensor");
child.transform.SetParent(transform, false);
child.transform.localPosition = new Vector3(0, 1, 0);
child.tag = "Sensor";
child.AddComponent<BoxCollider>();
```

## Notes

Object.Instantiate copies the full descendant hierarchy and serialized fields of compiled scripts. Static fields and coroutine state are not duplicated. Runtime edits never persist to the authored project.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed partial class GameObject : Object {
 internal Entity Entity {get;}
 internal GameObject(Entity entity){Entity=entity;BindIdentity(entity,"gameObject");}
 public string name {get=>Context.Metadata(Entity,"name",Entity.Id);set=>Context.Property(Entity,"name",System.Text.Json.Nodes.JsonValue.Create(value));}
 public Transform transform=>new(Entity);
 public bool activeSelf=>Context.ActiveStates.GetValueOrDefault(Entity.Id,true);public bool activeInHierarchy=>Context.IsActive(Entity.Id);public void SetActive(bool active)=>Context.Activate(Entity,active);
 public static GameObject? Find(string name){foreach(var (id,data) in Context.EntityData)if(Context.IsActive(id)&&data.TryGetProperty("name",out var value)&&value.GetString()==name)return new(new Entity(id,Context.Generation));return null;}
 public T? GetComponent<T>()=>GetComponents<T>().FirstOrDefault();

}

// Namespace: Axiom.Gameplay
public sealed partial class GameObject {
 public SceneManagement.Scene scene=>SceneManagement.SceneManager.SceneOf(Entity);
 public GameObject(string name="New GameObject"){if(string.IsNullOrWhiteSpace(name)||name.Length>128)throw new ArgumentException("Invalid object name");Entity=Context.CreateObject(name);BindIdentity(Entity,"gameObject");}
 public string tag {get=>Context.Metadata(Entity,"runtimeTag","Untagged");set{if(string.IsNullOrWhiteSpace(value)||value.Length>128)throw new ArgumentException("Invalid tag");Context.Property(Entity,"runtimeTag",JsonValue.Create(value));}}
 public int layer {get{var data=Context.EntityData[Entity.Id];if(data.TryGetProperty("runtimeLayer",out var layer))return layer.GetInt32();if(data.TryGetProperty("collider",out var collider)){var mask=collider.GetProperty("layer").GetUInt32();for(var i=0;i<32;i++)if((mask&(1u<<i))!=0)return i;}return 0;}set{if(value<0||value>31)throw new ArgumentOutOfRangeException(nameof(value));Context.Property(Entity,"runtimeLayer",JsonValue.Create(value));if(Context.EntityData[Entity.Id].TryGetProperty("collider",out var c)){var node=JsonNode.Parse(c.GetRawText())!;node["layer"]=1u<<value;Context.Property(Entity,"collider",node);}}}
 public bool CompareTag(string value)=>tag==value;
 public static GameObject[] FindGameObjectsWithTag(string tag)=>Context.Entities.Keys.Where(Context.IsActive).Select(id=>new GameObject(new Entity(id,Context.Generation))).Where(e=>e.tag==tag).ToArray();
 public static GameObject? FindWithTag(string tag)=>FindGameObjectsWithTag(tag).FirstOrDefault();
 public T AddComponent<T>() where T:Component=>Context.AddComponent<T>(Entity);
 public bool TryGetComponent<T>(out T? component) {component=GetComponent<T>();return component!=null;}
 internal Component[] AllComponents() {
  _=Context.Read(Entity);var result=new List<Component>{transform};var data=Context.EntityData[Entity.Id];
  var hasCollider=data.TryGetProperty("collider",out var c);var twoD=hasCollider?c.GetProperty("dimension").GetInt32()==2:data.TryGetProperty("runtimePhysicsDimension",out var dim)&&dim.GetInt32()==2;
  if(c.ValueKind==JsonValueKind.Object){var shape=c.GetProperty("shape").GetString();result.Add(twoD?(shape=="sphere"?new CircleCollider2D(Entity):new BoxCollider2D(Entity)):shape switch {"sphere"=>new SphereCollider(Entity),"capsule"=>new CapsuleCollider(Entity),"plane" or "quad"=>new MeshCollider(Entity),_=>new BoxCollider(Entity)});}if(data.TryGetProperty("runtimeMeshFilter",out var meshFilter)&&meshFilter.GetBoolean())result.Add(new MeshFilter(Entity));
  if(Context.Bodies.ContainsKey(Entity.Id))result.Add(twoD?new Rigidbody2D(Entity):new Rigidbody(Entity));
  if(Context.Cameras.Contains(Entity.Id))result.Add(new Camera(Entity));if(Context.AudioSources.Contains(Entity.Id))result.Add(new AudioSource(Entity));if(Context.Animators.ContainsKey(Entity.Id))result.Add(new Animator(Entity));if(data.TryGetProperty("audioListener",out _))result.Add(new AudioListener(Entity));
  if(data.TryGetProperty("renderable",out var renderable))result.Add(data.TryGetProperty("sprite2D",out _)?new SpriteRenderer(Entity):new MeshRenderer(Entity));if(data.TryGetProperty("light",out _))result.Add(new Light(Entity));if(AI.NavMeshAgent.Agents.TryGetValue(Entity.Id,out var agent))result.Add(agent);
  result.AddRange(Context.LiveScripts.Where(s=>s.Entity.Id==Entity.Id));var order=Context.ComponentOrders[Entity];return result.OrderBy(c=>order.IndexOf(c.GetInstanceID())).ToArray();
 }

 public T[] GetComponents<T>()=>GetComponents(typeof(T)).Select(ScriptBridge.Convert<T>).ToArray();
 public T? GetComponentInChildren<T>(bool includeInactive)=>GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
 public T? GetComponentInParent<T>(bool includeInactive)=>GetComponentsInParent<T>(includeInactive).FirstOrDefault();
 public T[] GetComponentsInChildren<T>(bool includeInactive)=>GetComponentsInChildren(typeof(T),includeInactive).Select(ScriptBridge.Convert<T>).ToArray();
 public T[] GetComponentsInParent<T>(bool includeInactive)=>GetComponentsInParent(typeof(T),includeInactive).Select(ScriptBridge.Convert<T>).ToArray();
}

// Namespace: Axiom.Gameplay
public sealed partial class GameObject {
 public GameObject gameObject=>this;
 public Component? GetComponent(Type type)=>GetComponents(type).FirstOrDefault();
 public Component[] GetComponents(Type type){ObjectQueries.ComponentType(type);return AllComponents().Where(component=>ScriptBridge.Matches(type,component)).ToArray();}
 public bool TryGetComponent(Type type,out Component? component){component=GetComponent(type);return component!=null;}
 public void GetComponents(Type type,List<Component> results)=>ObjectQueries.Fill(results,GetComponents(type));
 public void GetComponents<T>(List<T> results)=>ObjectQueries.Fill(results,GetComponents<T>());
 public Component? GetComponentInChildren(Type type)=>GetComponentInChildren(type,false);
 public Component? GetComponentInChildren(Type type,bool includeInactive)=>GetComponentsInChildren(type,includeInactive).FirstOrDefault();
 public Component[] GetComponentsInChildren(Type type)=>GetComponentsInChildren(type,false);
 public Component[] GetComponentsInChildren(Type type,bool includeInactive){
  ObjectQueries.ComponentType(type);var result=new List<Component>();
  void Visit(Transform node,bool self){if(!self&&!includeInactive&&!node.gameObject.activeInHierarchy)return;result.AddRange(node.gameObject.GetComponents(type));for(var i=0;i<node.childCount;i++)Visit(node.GetChild(i),false);}
  Visit(transform,true);return result.ToArray();
 }
 public Component? GetComponentInParent(Type type)=>GetComponentInParent(type,false);
 public Component? GetComponentInParent(Type type,bool includeInactive)=>GetComponentsInParent(type,includeInactive).FirstOrDefault();
 public Component[] GetComponentsInParent(Type type)=>GetComponentsInParent(type,false);
 public Component[] GetComponentsInParent(Type type,bool includeInactive){
  ObjectQueries.ComponentType(type);var result=new List<Component>();var self=true;
  for(Transform? current=transform;current!=null;current=current.parent){if(self||includeInactive||current.gameObject.activeInHierarchy)result.AddRange(current.gameObject.GetComponents(type));self=false;}
  return result.ToArray();
 }
 public T? GetComponentInChildren<T>()=>GetComponentInChildren<T>(false);
 public T? GetComponentInParent<T>()=>GetComponentInParent<T>(false);
 public T[] GetComponentsInChildren<T>()=>GetComponentsInChildren<T>(false);
 public T[] GetComponentsInParent<T>()=>GetComponentsInParent<T>(false);
 public void GetComponentsInChildren<T>(List<T> results)=>GetComponentsInChildren(false,results);
 public void GetComponentsInChildren<T>(bool includeInactive,List<T> results)=>ObjectQueries.Fill(results,GetComponentsInChildren<T>(includeInactive));
 public void GetComponentsInParent<T>(bool includeInactive,List<T> results)=>ObjectQueries.Fill(results,GetComponentsInParent<T>(includeInactive));
 public Component AddComponent(Type type)=>Context.AddComponent(Entity,type);
 public int GetComponentCount()=>AllComponents().Length;
 public Component GetComponentAtIndex(int index){var values=AllComponents();if(index<0||index>=values.Length)throw new ArgumentOutOfRangeException(nameof(index));return values[index];}
 public T GetComponentAtIndex<T>(int index)=>(T)(object)GetComponentAtIndex(index);
 public int GetComponentIndex(Component? component)=>component==null?-1:Array.FindIndex(AllComponents(),value=>value==component);
}

// Namespace: UnityEngine
public sealed partial class GameObject : Object {
 internal Legacy.GameObject LegacyGameObject {get{if(!this)throw new MissingReferenceException("GameObject has been destroyed");return Wrapped as Legacy.GameObject??throw new MissingReferenceException("GameObject is not attached");}}
 internal GameObject(Legacy.GameObject native,bool facade){Wrapped=native;CacheFacade();}
 public static implicit operator GameObject(Legacy.GameObject value)=>ReferenceEquals(value,null)?null!:(GameObject)Wrap(value);
 public GameObject():this("New Game Object"){}public GameObject(string name){name??="New Game Object";if(name.Length>128)throw new ArgumentException("Object name exceeds runtime bounds",nameof(name));Wrapped=new Legacy.GameObject(Legacy.Context.CreateObject(name));CacheFacade();}public GameObject(string name,params Type[] components):this(name){foreach(var type in components)AddComponent(type);}
 public Transform transform=>LegacyGameObject.transform;public GameObject gameObject=>this;
 public bool activeSelf=>LegacyGameObject.activeSelf;public bool activeInHierarchy=>LegacyGameObject.activeInHierarchy;public void SetActive(bool value)=>LegacyGameObject.SetActive(value);
 public int layer {get=>LegacyGameObject.layer;set=>LegacyGameObject.layer=value;}public string tag {get=>LegacyGameObject.tag;set{Tags.Require(value);LegacyGameObject.tag=value;}}public bool CompareTag(string tag){Tags.Require(tag);return LegacyGameObject.CompareTag(tag);}
 public static GameObject? Find(string name)=>Legacy.GameObject.Find(name);public static GameObject? FindWithTag(string tag){Tags.Require(tag);return Legacy.GameObject.FindWithTag(tag);}public static GameObject? FindGameObjectWithTag(string tag)=>FindWithTag(tag);public static GameObject[] FindGameObjectsWithTag(string tag){Tags.Require(tag);return Legacy.GameObject.FindGameObjectsWithTag(tag).Select(go=>(GameObject)go).ToArray();}public static void FindGameObjectsWithTag(string tag,List<GameObject> results){ArgumentNullException.ThrowIfNull(results);results.Clear();results.AddRange(FindGameObjectsWithTag(tag));}
 public T AddComponent<T>() where T:Component=>(T)(Component)Wrap(LegacyGameObject.AddComponent(typeof(T)));public Component AddComponent(Type componentType)=>LegacyGameObject.AddComponent(componentType);
 public T? GetComponent<T>()=>LegacyGameObject.GetComponent<T>();public Component? GetComponent(Type type)=>LegacyGameObject.GetComponent(type);public Component? GetComponent(string type){ArgumentNullException.ThrowIfNull(type);return GetComponents<Component>().FirstOrDefault(component=>Matches(component.Native is Legacy.UnityScriptAdapter adapter?adapter.Target.GetType():component.Native.GetType()));bool Matches(Type candidate){for(var current=candidate;current!=null&&(typeof(Component).IsAssignableFrom(current)||typeof(Legacy.Component).IsAssignableFrom(current));current=current.BaseType){var script=(typeof(MonoBehaviour).IsAssignableFrom(current)&&current!=typeof(MonoBehaviour))||(typeof(Legacy.Script).IsAssignableFrom(current)&&current!=typeof(Legacy.Script)&&current!=typeof(Legacy.MonoBehaviour));if(current.Name==type||script&&current.FullName==type)return true;}return false;}}
 public T[] GetComponents<T>()=>LegacyGameObject.GetComponents<T>();public Component[] GetComponents(Type type)=>LegacyGameObject.GetComponents(type).Select(native=>(Component)Wrap(native)).ToArray();public void GetComponents<T>(List<T> results)=>LegacyGameObject.GetComponents(results);
 public T? GetComponentInChildren<T>()=>LegacyGameObject.GetComponentInChildren<T>();public T? GetComponentInChildren<T>(bool includeInactive)=>LegacyGameObject.GetComponentInChildren<T>(includeInactive);public T? GetComponentInParent<T>()=>LegacyGameObject.GetComponentInParent<T>();public T? GetComponentInParent<T>(bool includeInactive)=>LegacyGameObject.GetComponentInParent<T>(includeInactive);
 public T[] GetComponentsInChildren<T>()=>LegacyGameObject.GetComponentsInChildren<T>(false);public T[] GetComponentsInChildren<T>(bool includeInactive)=>LegacyGameObject.GetComponentsInChildren<T>(includeInactive);public T[] GetComponentsInParent<T>()=>LegacyGameObject.GetComponentsInParent<T>(false);public T[] GetComponentsInParent<T>(bool includeInactive)=>LegacyGameObject.GetComponentsInParent<T>(includeInactive);
 public bool TryGetComponent<T>(out T? component)=>LegacyGameObject.TryGetComponent(out component);public bool TryGetComponent(Type type,out Component? component){var found=LegacyGameObject.TryGetComponent(type,out var value);component=value;return found;}
 public int GetComponentCount()=>LegacyGameObject.GetComponentCount();public Component GetComponentAtIndex(int index)=>LegacyGameObject.GetComponentAtIndex(index);public T? GetComponentAtIndex<T>(int index) where T:Component=>GetComponentAtIndex(index) as T;public int GetComponentIndex(Component component)=>component?.Wrapped is Legacy.Component value?LegacyGameObject.GetComponentIndex(value):-1;
}

// Namespace: UnityEngine
public partial class GameObject {
 public TransformHandle transformHandle=>new(transform);
 public Component? GetComponentInChildren(Type type)=>GetComponentInChildren(type,false);public Component? GetComponentInChildren(Type type,bool includeInactive)=>LegacyGameObject.GetComponentInChildren(type,includeInactive);public Component? GetComponentInParent(Type type)=>GetComponentInParent(type,false);public Component? GetComponentInParent(Type type,bool includeInactive)=>LegacyGameObject.GetComponentInParent(type,includeInactive);
 public Component[] GetComponentsInChildren(Type type)=>GetComponentsInChildren(type,false);public Component[] GetComponentsInChildren(Type type,bool includeInactive)=>LegacyGameObject.GetComponentsInChildren(type,includeInactive).Select(native=>(Component)Object.Wrap(native)).ToArray();public Component[] GetComponentsInParent(Type type)=>GetComponentsInParent(type,false);public Component[] GetComponentsInParent(Type type,bool includeInactive)=>LegacyGameObject.GetComponentsInParent(type,includeInactive).Select(native=>(Component)Object.Wrap(native)).ToArray();
 public void GetComponents(Type type,List<Component> results){ArgumentNullException.ThrowIfNull(results);var values=GetComponents(type);results.Clear();results.AddRange(values);}
 public void GetComponentsInChildren<T>(List<T> results)=>GetComponentsInChildren(false,results);public void GetComponentsInChildren<T>(bool includeInactive,List<T> results)=>LegacyGameObject.GetComponentsInChildren(includeInactive,results);public void GetComponentsInParent<T>(bool includeInactive,List<T> results)=>LegacyGameObject.GetComponentsInParent(includeInactive,results);
 public bool isStatic {get=>Legacy.Context.EntityData[LegacyGameObject.Entity.Id].TryGetProperty("runtimeStatic",out var value)&&value.GetBoolean();set=>Legacy.Context.Property(LegacyGameObject.Entity,"runtimeStatic",System.Text.Json.Nodes.JsonValue.Create(value));}
 [Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]public bool active {get=>activeSelf;set=>SetActive(value);}
 [Obsolete("gameObject.SetActiveRecursively() is obsolete. Use GameObject.SetActive(), which is now inherited by children.")]public void SetActiveRecursively(bool state){var hierarchy=GetComponentsInChildren<Transform>(true);foreach(var node in hierarchy)node.gameObject.SetActive(state);}
}

// Namespace: UnityEngine
public partial class GameObject {public bool CompareTag(TagHandle tag)=>CompareTag(tag.Value??"Untagged");}

// Namespace: UnityEngine
public partial class GameObject {
 public void SendMessage(string methodName)=>SendMessage(methodName,null,SendMessageOptions.RequireReceiver);public void SendMessage(string methodName,object? value)=>SendMessage(methodName,value,SendMessageOptions.RequireReceiver);public void SendMessage(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,0,options);public void SendMessage(string methodName,object? value,SendMessageOptions options)=>Messages.Send(this,methodName,value,true,0,options);
 public void SendMessageUpwards(string methodName)=>SendMessageUpwards(methodName,null,SendMessageOptions.RequireReceiver);public void SendMessageUpwards(string methodName,object? value)=>SendMessageUpwards(methodName,value,SendMessageOptions.RequireReceiver);public void SendMessageUpwards(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,1,options);public void SendMessageUpwards(string methodName,object? value,SendMessageOptions options)=>Messages.Send(this,methodName,value,true,1,options);
 public void BroadcastMessage(string methodName)=>BroadcastMessage(methodName,null,SendMessageOptions.RequireReceiver);public void BroadcastMessage(string methodName,object? parameter)=>BroadcastMessage(methodName,parameter,SendMessageOptions.RequireReceiver);public void BroadcastMessage(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,2,options);public void BroadcastMessage(string methodName,object? parameter,SendMessageOptions options)=>Messages.Send(this,methodName,parameter,true,2,options);
}

// Namespace: UnityEngine
public partial class GameObject {
 public SceneManagement.Scene scene=>LegacyGameObject.scene;
 [Obsolete("Obsolete. Please use GameObject.GetScene(EntityId entityId) instead.")]
 public static SceneManagement.Scene GetScene(int instanceID)=>GetScene(new EntityId(instanceID));
 public static SceneManagement.Scene GetScene(EntityId entityId){try{return NativeObjects.GameObject(entityId.Value).scene;}catch(ArgumentException){return default;}}
 [Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(ReadOnlySpan<EntityId>, bool) instead.")]
 public static void SetGameObjectsActive(ReadOnlySpan<int> instanceIDs,bool active){
  var targets=instanceIDs.ToArray().Distinct().Select(NativeObjects.GameObject).ToArray();
  if(targets.Length==0)return;
  var ids=new System.Text.Json.Nodes.JsonArray(targets.Select(go=>(System.Text.Json.Nodes.JsonNode?)System.Text.Json.Nodes.JsonValue.Create(go.Entity.Id)).ToArray());
  Legacy.Context.Add(new("activeMany","",null,default,null,Data:new System.Text.Json.Nodes.JsonObject{["ids"]=ids,["active"]=active}.ToJsonString()));
  foreach(var target in targets)Legacy.Context.ActiveStates[target.Entity.Id]=active;
  Legacy.RuntimeLifecycle.Activation();
 }
 public static void SetGameObjectsActive(ReadOnlySpan<EntityId> entityIds,bool active)=>SetGameObjectsActive(entityIds.ToArray().Select(id=>id.Value).ToArray().AsSpan(),active);
 [Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(NativeArray<EntityId>, bool) instead.")]
 public static void SetGameObjectsActive(NativeArray<int> instanceIDs,bool active)=>SetGameObjectsActive(instanceIDs.ToArray().AsSpan(),active);
 public static void SetGameObjectsActive(NativeArray<EntityId> entityIds,bool active)=>SetGameObjectsActive(entityIds.ToArray().AsSpan(),active);
 [Obsolete("Obsolete. Please use GameObject.InstantiateGameObjects(EntityId, int, NativeArray<EntityId>, NativeArray<EntityId>, Scene) instead.")]
 public static void InstantiateGameObjects(int sourceInstanceID,int count,NativeArray<int> newInstanceIDs,NativeArray<int> newTransformInstanceIDs,SceneManagement.Scene destinationScene=default){
  if(!newInstanceIDs.IsCreated||!newTransformInstanceIDs.IsCreated)throw new ObjectDisposedException("Clone output arrays");
  if(count<0||newInstanceIDs.Length!=count||newTransformInstanceIDs.Length!=count)throw new ArgumentException("Clone output arrays must match count");
  if(count==0)return;
  var source=NativeObjects.GameObject(sourceInstanceID);
  var objects=Legacy.CloneBlueprint.Capture(source).InstantiateBatch(count,new InstantiateParameters{scene=destinationScene,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),false).Cast<Legacy.GameObject>().ToArray();
  for(var i=0;i<count;i++){newInstanceIDs[i]=objects[i].GetInstanceID();newTransformInstanceIDs[i]=objects[i].transform.GetInstanceID();}
  Legacy.RuntimeLifecycle.Initialize();
 }
 public static void InstantiateGameObjects(EntityId sourceEntityId,int count,NativeArray<EntityId> newEntityIds,NativeArray<EntityId> newTransformEntityIds,SceneManagement.Scene destinationScene=default){
  if(!newEntityIds.IsCreated||!newTransformEntityIds.IsCreated)throw new ObjectDisposedException("Clone output arrays");
  if(count<0||newEntityIds.Length!=count||newTransformEntityIds.Length!=count)throw new ArgumentException("Clone output arrays must match count");
  using var gameObjects=new NativeArray<int>(count,Allocator.Temp);using var transforms=new NativeArray<int>(count,Allocator.Temp);
  InstantiateGameObjects(sourceEntityId.Value,count,gameObjects,transforms,destinationScene);
  for(var i=0;i<count;i++){newEntityIds[i]=new(gameObjects[i]);newTransformEntityIds[i]=new(transforms[i]);}
 }
}

// Namespace: UnityEngine
public partial class GameObject {
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("GameObject.SampleAnimation(AnimationClip, float) has been deprecated. Use AnimationClip.SampleAnimation(GameObject, float) instead (UnityUpgradable).",true)]
 public void SampleAnimation(Object clip, float time)=>throw new NotSupportedException("GameObject.SampleAnimation(AnimationClip, float) has been deprecated. Use AnimationClip.SampleAnimation(GameObject, float) instead (UnityUpgradable).");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("GameObject.AddComponent with string argument has been deprecated. Use GameObject.AddComponent<T>() instead. (UnityUpgradable).",true)]
 public Component AddComponent(string className)=>throw new NotSupportedException("GameObject.AddComponent with string argument has been deprecated. Use GameObject.AddComponent<T>() instead. (UnityUpgradable).");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("gameObject.PlayAnimation is not supported anymore. Use animation.Play()",true)]
 public void PlayAnimation(Object animation)=>throw new NotSupportedException("gameObject.PlayAnimation is not supported anymore. Use animation.Play()");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("gameObject.StopAnimation is not supported anymore. Use animation.Stop()",true)]
 public void StopAnimation()=>throw new NotSupportedException("gameObject.StopAnimation is not supported anymore. Use animation.Stop()");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)",true)]
 public Component rigidbody=>throw new NotSupportedException("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)",true)]
 public Component rigidbody2D=>throw new NotSupportedException("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)",true)]
 public Component camera=>throw new NotSupportedException("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)",true)]
 public Component light=>throw new NotSupportedException("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)",true)]
 public Component animation=>throw new NotSupportedException("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)",true)]
 public Component constantForce=>throw new NotSupportedException("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)",true)]
 public Component renderer=>throw new NotSupportedException("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)",true)]
 public Component audio=>throw new NotSupportedException("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)",true)]
 public Component networkView=>throw new NotSupportedException("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)",true)]
 public Component collider=>throw new NotSupportedException("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)",true)]
 public Component collider2D=>throw new NotSupportedException("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)",true)]
 public Component hingeJoint=>throw new NotSupportedException("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)",true)]
 public Component particleSystem=>throw new NotSupportedException("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)");
}

// Namespace: UnityEngine
public partial class GameObject {
 public ulong sceneCullingMask {get{_=LegacyGameObject;return 16140901064495857664UL;}}
 public static GameObject CreatePrimitive(PrimitiveType type){
  if(!Enum.IsDefined(type))throw new ArgumentOutOfRangeException(nameof(type));
  var index=(int)type;var entity=Legacy.Context.CreateObject(PrimitiveCatalog.Names[index]);
  var node=JsonNode.Parse(Legacy.Context.EntityData[entity.Id].GetRawText())!.AsObject();
  node["renderable"]=new JsonObject{["kind"]="mesh",["assetId"]=PrimitiveCatalog.Ids[index]};node["runtimeMeshFilter"]=true;node["runtimeMeshAssetId"]=PrimitiveCatalog.Ids[index];node["runtimeColliderLocal"]=true;
  var shape=index switch{0=>"sphere",1 or 2=>"capsule",3=>"box",4=>"plane",_=>"quad"};var half=index switch{1 or 2=>new[]{.5,1,.5},4=>new[]{5d,0,5},5=>new[]{.5,.5,0},_=>new[]{.5,.5,.5}};
  node["collider"]=new JsonObject{["dimension"]=3,["shape"]=shape,["halfExtents"]=new JsonArray(half.Select(v=>(JsonNode?)JsonValue.Create(v)).ToArray()),["trigger"]=false,["layer"]=1,["mask"]=4294967295u};
  Legacy.Context.EntityData[entity.Id]=Legacy.Context.ParseData(node.ToJsonString());Legacy.Context.Operations[^1]=new("primitive",entity.Id,null,default,null,Data:index.ToString(System.Globalization.CultureInfo.InvariantCulture));Legacy.Context.Refresh(entity);
  Legacy.RuntimeAssets.BindPrimitive(entity,Legacy.PrimitiveMeshes.Get(PrimitiveCatalog.Ids[index]),index>=4);var native=new Legacy.GameObject(entity);Legacy.Context.ComponentOrders[entity]=new[]{native.transform.GetInstanceID(),new Legacy.MeshFilter(entity).GetInstanceID(),new Legacy.MeshRenderer(entity).GetInstanceID(),native.GetComponent<Legacy.Collider>()!.GetInstanceID()}.ToList();return native;
 }
}
```
