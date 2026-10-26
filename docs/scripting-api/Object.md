# Object

Runtime creation/cloning/destruction and component queries. Instantiate recursively clones descendants and compiled script field values, then invokes the new lifecycle. Destroy(Component) removes that component; Destroy(GameObject) removes the subtree. See the Objects and components section for exact limits.

## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public partial class Object {
 public Object(){UnityEngine.NativeObjects.Track(this); }
 public static GameObject Instantiate(GameObject original)=>(GameObject)Instantiate((Object)original);
 public static GameObject Instantiate(GameObject original,Vector3 position,Quaternion rotation)=>(GameObject)Instantiate((Object)original,position,rotation);
 public static void Destroy(GameObject? target)=>RuntimeLifecycle.DestroyAfter(target,0);
 public static void Destroy(Component? target)=>RuntimeLifecycle.DestroyAfter(target,0);
}

// Namespace: Axiom.Gameplay
public partial class Object {
 public static void Destroy(Object? target)=>RuntimeLifecycle.DestroyAfter(target,0);
 public static void Destroy(Object? target,double delay)=>RuntimeLifecycle.DestroyAfter(target,delay);
 public static void DestroyImmediate(Object? target)=>RuntimeLifecycle.DestroyNow(target);
 public static void DestroyImmediate(Object? target,bool allowDestroyingAssets)=>RuntimeLifecycle.DestroyNow(target);
 public static void Destroy(GameObject target,double delay)=>RuntimeLifecycle.DestroyAfter(target,delay);
 public static void Destroy(Component target,double delay)=>RuntimeLifecycle.DestroyAfter(target,delay);
 public static GameObject Instantiate(GameObject original,Transform? parent)=>Instantiate(original,parent,false);
 public static GameObject Instantiate(GameObject original,Transform? parent,bool worldPositionStays)=>(GameObject)Instantiate((Object)original,parent,worldPositionStays);
 public static T[] FindObjectsByType<T>() where T:Object=>FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
 public static T? FindFirstObjectByType<T>() where T:Object=>FindObjectsByType<T>().FirstOrDefault();
}

// Namespace: Axiom.Gameplay
public partial class Object {
 private static long nextIdentity;
 internal static int AllocateIdentity()=>checked((int)System.Threading.Interlocked.Increment(ref nextIdentity));
 private int managedIdentity;
 internal int ManagedIdentity=>managedIdentity==0?managedIdentity=AllocateIdentity():managedIdentity;
 private int nativeIdentity,identityEpoch;
 private Entity? identityEntity;
 private string? identitySlot;
 internal void BindIdentity(Entity entity,string slot){identityEntity=entity;identitySlot=slot;identityEpoch=Context.ObjectEpochs.GetValueOrDefault((entity,slot));var key=(entity,slot,identityEpoch);if(!Context.ObjectIdentities.TryGetValue(key,out nativeIdentity))Context.ObjectIdentities[key]=nativeIdentity=AllocateIdentity();if(this is Component){if(!Context.ComponentOrders.TryGetValue(entity,out var order))Context.ComponentOrders[entity]=order=new();if(!order.Contains(nativeIdentity))order.Add(nativeIdentity);}}
 public int GetInstanceID()=>nativeIdentity==0?ManagedIdentity:nativeIdentity;
 public override int GetHashCode()=>GetInstanceID();
 public override bool Equals(object? other)=>other is Object obj?this==obj:ReferenceEquals(other,null)&&!Exists(this);
 public static bool operator ==(Object? left,Object? right){if(ReferenceEquals(left,right))return true;if(ReferenceEquals(left,null))return !Exists(right);if(ReferenceEquals(right,null))return !Exists(left);return left.GetInstanceID()==right.GetInstanceID();}
 public static bool operator !=(Object? left,Object? right)=>!(left==right);
 private static bool Exists(Object? value){if(ReferenceEquals(value,null))return false;if(value.identityEntity is not Entity e)return value is not Component;if(e.Generation!=Context.Generation||!Context.Entities.ContainsKey(e.Id)||Context.ObjectEpochs.GetValueOrDefault((e,value.identitySlot!))!=value.identityEpoch)return false;if(value is Script script)return Context.LiveScripts.Any(s=>ReferenceEquals(s,script));if(value is MeshFilter filter)return Context.EntityData[e.Id].TryGetProperty("runtimeMeshFilter",out var meshFilter)&&meshFilter.GetBoolean();if(value is AI.NavMeshAgent agent)return AI.NavMeshAgent.Agents.TryGetValue(e.Id,out var live)&&ReferenceEquals(agent,live);if(value is Component c&&Context.ComponentKey(c.GetType()) is string key)return Context.EntityData[e.Id].TryGetProperty(key,out _);return true;}
 public static implicit operator bool(Object? value)=>Exists(value);
}

// Namespace: Axiom.Gameplay
public partial class Object {
 private static Object CloneObject(Object original,Vector3? position,Quaternion? rotation,Transform? parent,bool worldPositionStays){
  ArgumentNullException.ThrowIfNull(original);if(!original)throw new ArgumentException("Cannot clone a destroyed object",nameof(original));
  var source=original is GameObject go?go:original is Component component?component.gameObject:throw new NotSupportedException("This object requires its asset/resource cloning API");
  var useLocal=!position.HasValue&&parent!=null&&!worldPositionStays;var local=useLocal?Context.Local(source.Entity):null;
  var entity=Context.CloneHierarchy(source.Entity,position??source.transform.position,rotation??source.transform.rotation,false);var clone=new GameObject(entity);
  if(parent!=null)clone.transform.SetParent(parent,true);if(useLocal)Context.LocalTransform(entity,local!.Position,local.Rotation,local.Scale);
  Object result=original is Component sourceComponent?clone.GetComponent(sourceComponent.GetType())??throw new NotSupportedException("Cloned component is not available"):clone;
  RuntimeLifecycle.Initialize();return result;
 }
 public static Object Instantiate(Object original)=>CloneObject(original,null,null,null,true);
 public static Object Instantiate(Object original,Vector3 position,Quaternion rotation)=>CloneObject(original,position,rotation,null,true);
 public static Object Instantiate(Object original,Vector3 position,Quaternion rotation,Transform? parent)=>CloneObject(original,position,rotation,parent,true);
 public static Object Instantiate(Object original,Transform? parent)=>CloneObject(original,null,null,parent,false);
 public static Object Instantiate(Object original,Transform? parent,bool instantiateInWorldSpace)=>CloneObject(original,null,null,parent,instantiateInWorldSpace);
 public static T Instantiate<T>(T original) where T:Object=>(T)Instantiate((Object)original);
 public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation) where T:Object=>(T)Instantiate((Object)original,position,rotation);
 public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation,Transform? parent) where T:Object=>(T)Instantiate((Object)original,position,rotation,parent);
 public static T Instantiate<T>(T original,Transform? parent) where T:Object=>(T)Instantiate((Object)original,parent);
 public static T Instantiate<T>(T original,Transform? parent,bool worldPositionStays) where T:Object=>(T)Instantiate((Object)original,parent,worldPositionStays);
 public static GameObject Instantiate(GameObject original,Vector3 position,Quaternion rotation,Transform? parent)=>(GameObject)Instantiate((Object)original,position,rotation,parent);
 public static Object[] FindObjectsByType(Type type,FindObjectsSortMode sortMode)=>FindObjectsByType(type,FindObjectsInactive.Exclude,sortMode);
 public static Object[] FindObjectsByType(Type type,FindObjectsInactive inactive,FindObjectsSortMode sortMode)=>ObjectQueries.Find(type,inactive,sortMode);
 public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T:Object=>FindObjectsByType<T>(FindObjectsInactive.Exclude,sortMode);
 public static T[] FindObjectsByType<T>(FindObjectsInactive inactive,FindObjectsSortMode sortMode) where T:Object=>FindObjectsByType(typeof(T),inactive,sortMode).Cast<T>().ToArray();
 public static Object? FindFirstObjectByType(Type type)=>FindFirstObjectByType(type,FindObjectsInactive.Exclude);
 public static Object? FindFirstObjectByType(Type type,FindObjectsInactive inactive)=>FindObjectsByType(type,inactive,FindObjectsSortMode.InstanceID).FirstOrDefault();
 public static T? FindFirstObjectByType<T>(FindObjectsInactive inactive) where T:Object=>FindObjectsByType<T>(inactive,FindObjectsSortMode.InstanceID).FirstOrDefault();
 public static Object? FindAnyObjectByType(Type type)=>FindAnyObjectByType(type,FindObjectsInactive.Exclude);
 public static Object? FindAnyObjectByType(Type type,FindObjectsInactive inactive)=>FindObjectsByType(type,inactive,FindObjectsSortMode.None).FirstOrDefault();
 public static T? FindAnyObjectByType<T>() where T:Object=>FindAnyObjectByType<T>(FindObjectsInactive.Exclude);
 public static T? FindAnyObjectByType<T>(FindObjectsInactive inactive) where T:Object=>FindObjectsByType<T>(inactive,FindObjectsSortMode.None).FirstOrDefault();
 public static Object[] FindObjectsOfType(Type type)=>FindObjectsOfType(type,false);
 public static Object[] FindObjectsOfType(Type type,bool includeInactive)=>FindObjectsByType(type,includeInactive?FindObjectsInactive.Include:FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID);
 public static T[] FindObjectsOfType<T>() where T:Object=>FindObjectsOfType<T>(false);
 public static T[] FindObjectsOfType<T>(bool includeInactive) where T:Object=>FindObjectsOfType(typeof(T),includeInactive).Cast<T>().ToArray();
 public static Object? FindObjectOfType(Type type)=>FindObjectOfType(type,false);
 public static Object? FindObjectOfType(Type type,bool includeInactive)=>FindObjectsOfType(type,includeInactive).FirstOrDefault();
 public static T? FindObjectOfType<T>() where T:Object=>FindObjectOfType<T>(false);
 public static T? FindObjectOfType<T>(bool includeInactive) where T:Object=>FindObjectsOfType<T>(includeInactive).FirstOrDefault();
}

// Namespace: UnityEngine
public partial class Object {
 internal Legacy.Object? Wrapped;
 internal Legacy.Object Native=>Wrapped??throw new MissingReferenceException("Object has no native runtime handle");

 public int GetInstanceID()=>Wrapped?.GetInstanceID()??0;
 public override int GetHashCode()=>GetInstanceID();public override bool Equals(object? other)=>other is Object value?this==value:other==null&&!this;
 public static implicit operator bool(Object? exists)=>!ReferenceEquals(exists,null)&&(!ReferenceEquals(exists.Wrapped,null)&&(bool)exists.Wrapped);
 public static bool operator ==(Object? x,Object? y){if(ReferenceEquals(x,y))return true;if(ReferenceEquals(x,null))return !y;if(ReferenceEquals(y,null))return !x;return x.GetInstanceID()==y.GetInstanceID();}public static bool operator !=(Object? x,Object? y)=>!(x==y);
 public string name {get{var value=Native;return value is Legacy.GameObject go?go.name:value is Legacy.Component component?component.gameObject.name:value is Legacy.ProjectAsset asset?asset.name:value is Legacy.TextAsset text?text.name:GetType().Name;}set{var native=Native;if(native is Legacy.GameObject go)go.name=value;else if(native is Legacy.Component component)component.gameObject.name=value;else throw new NotSupportedException("This asset does not expose a runtime name");}}
 public override string ToString()=>this?name:"null";
 public static void Destroy(Object? obj)=>Legacy.Object.Destroy(obj?.Native);public static void Destroy(Object? obj,float t)=>Legacy.Object.Destroy(obj?.Native,t);public static void DestroyImmediate(Object? obj)=>Legacy.Object.DestroyImmediate(obj?.Native);public static void DestroyImmediate(Object? obj,bool allowDestroyingAssets)=>Legacy.Object.DestroyImmediate(obj?.Native,allowDestroyingAssets);
 public static Object Instantiate(Object original)=>Instantiate<Object>(original,new InstantiateParameters{worldSpace=true});public static Object Instantiate(Object original,Vector3 position,Quaternion rotation)=>Instantiate<Object>(original,position,rotation,new InstantiateParameters{worldSpace=true});
 public static Object Instantiate(Object original,Transform? parent)=>Instantiate<Object>(original,new InstantiateParameters{parent=parent});public static Object Instantiate(Object original,Transform? parent,bool instantiateInWorldSpace)=>Instantiate<Object>(original,new InstantiateParameters{parent=parent,worldSpace=instantiateInWorldSpace});
 public static Object Instantiate(Object original,Vector3 position,Quaternion rotation,Transform? parent)=>Instantiate<Object>(original,position,rotation,new InstantiateParameters{parent=parent,worldSpace=true});
 public static T Instantiate<T>(T original) where T:Object=>(T)Instantiate((Object)original);public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation) where T:Object=>(T)Instantiate((Object)original,position,rotation);public static T Instantiate<T>(T original,Transform? parent) where T:Object=>(T)Instantiate((Object)original,parent);public static T Instantiate<T>(T original,Transform? parent,bool worldPositionStays) where T:Object=>(T)Instantiate((Object)original,parent,worldPositionStays);public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation,Transform? parent) where T:Object=>(T)Instantiate((Object)original,position,rotation,parent);
 public static void DontDestroyOnLoad(Object target){
  ArgumentNullException.ThrowIfNull(target);if(!target)throw new MissingReferenceException("The persistence target has been destroyed");
  var value=target.Native;var owner=value is Legacy.GameObject go?go:value is Legacy.Component component?component.gameObject:throw new ArgumentException("A scene object is required",nameof(target));
  // Unity's canonical API never promotes a child request to its root. The legacy API retains its own contract.
  if(owner.transform.parent!=null){Legacy.Debug.LogWarning("DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.");return;}
  Legacy.Object.DontDestroyOnLoad(owner);
 }
}

// Namespace: UnityEngine
public partial class Object {
 private static readonly Dictionary<int,WeakReference<Object>> Facades=new();
 internal static void ResetFacades(){Facades.Clear();NativeObjects.Flags.Clear();}
 internal static void ForgetNative(int id){Facades.Remove(id);NativeObjects.Flags.Remove(id);}
 internal void CacheFacade(){if(Wrapped==null)return;var id=Wrapped.GetInstanceID();Facades[id]=new(this);if(Facades.Count>131072)foreach(var key in Facades.Where(pair=>!pair.Value.TryGetTarget(out _)).Select(pair=>pair.Key).ToArray())Facades.Remove(key);}
 internal static Object Wrap(Axiom.Gameplay.Object value){if(value is Axiom.Gameplay.UnityScriptAdapter adapter)return adapter.Target;var id=value.GetInstanceID();if(Facades.TryGetValue(id,out var weak)&&weak.TryGetTarget(out var cached))return cached;var result=value is Axiom.Gameplay.GameObject go?(Object)new GameObject(go):value is Axiom.Gameplay.Transform transform?new Transform(transform):value switch {Axiom.Gameplay.BoxCollider box=>new BoxCollider(box),Axiom.Gameplay.SphereCollider sphere=>new SphereCollider(sphere),Axiom.Gameplay.CapsuleCollider capsule=>new CapsuleCollider(capsule),Axiom.Gameplay.MeshCollider meshCollider=>new MeshCollider(meshCollider),Axiom.Gameplay.Collider collider when collider is not Axiom.Gameplay.Collider2D=>new Collider(collider),Axiom.Gameplay.MeshFilter filter=>new MeshFilter(filter),Axiom.Gameplay.MeshRenderer renderer=>new MeshRenderer(renderer),Axiom.Gameplay.Renderer renderer=>new Renderer(renderer),Axiom.Gameplay.Mesh mesh=>new Mesh(mesh),Axiom.Gameplay.Component component=>new Component(component),_=>new Object{Wrapped=value}};result.CacheFacade();return result;}
 public static implicit operator Object(Axiom.Gameplay.Object value)=>ReferenceEquals(value,null)?null!:Wrap(value);
 public static explicit operator Axiom.Gameplay.Object(Object value)=>value.Native;
}

// Namespace: UnityEngine
public partial class Object {
 public Object(){}
 public HideFlags hideFlags {get=>NativeObjects.Flags.GetValueOrDefault(GetInstanceID());set=>NativeObjects.Flags[GetInstanceID()]=value;}
 public EntityId GetEntityId()=>new(GetInstanceID());
 public static Object[] FindObjectsByType(Type type,FindObjectsSortMode sortMode)=>FindObjectsByType(type,FindObjectsInactive.Exclude,sortMode);
 public static Object[] FindObjectsByType(Type type,FindObjectsInactive findObjectsInactive,FindObjectsSortMode sortMode){ArgumentNullException.ThrowIfNull(type);if(!typeof(Object).IsAssignableFrom(type))throw new ArgumentException("An Object type is required");if(!Enum.IsDefined(findObjectsInactive)||!Enum.IsDefined(sortMode))throw new ArgumentOutOfRangeException();return Legacy.Object.FindObjectsByType(type,(Legacy.FindObjectsInactive)findObjectsInactive,(Legacy.FindObjectsSortMode)sortMode).Select(Object.Wrap).Where(obj=>(obj.hideFlags&HideFlags.DontSave)!=HideFlags.DontSave).ToArray();}
 public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T:Object=>FindObjectsByType<T>(FindObjectsInactive.Exclude,sortMode);public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive,FindObjectsSortMode sortMode) where T:Object=>FindObjectsByType(typeof(T),findObjectsInactive,sortMode).Cast<T>().ToArray();
 public static Object? FindFirstObjectByType(Type type)=>FindFirstObjectByType(type,FindObjectsInactive.Exclude);public static Object? FindFirstObjectByType(Type type,FindObjectsInactive findObjectsInactive)=>FindObjectsByType(type,findObjectsInactive,FindObjectsSortMode.InstanceID).FirstOrDefault();public static T? FindFirstObjectByType<T>() where T:Object=>FindFirstObjectByType<T>(FindObjectsInactive.Exclude);public static T? FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T:Object=>FindFirstObjectByType(typeof(T),findObjectsInactive) as T;
 public static Object? FindAnyObjectByType(Type type)=>FindAnyObjectByType(type,FindObjectsInactive.Exclude);public static Object? FindAnyObjectByType(Type type,FindObjectsInactive findObjectsInactive)=>FindObjectsByType(type,findObjectsInactive,FindObjectsSortMode.None).FirstOrDefault();public static T? FindAnyObjectByType<T>() where T:Object=>FindAnyObjectByType<T>(FindObjectsInactive.Exclude);public static T? FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T:Object=>FindAnyObjectByType(typeof(T),findObjectsInactive) as T;
 [Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID, but if you do not need this using FindObjectSortMode.None is considerably faster.",false)]public static Object[] FindObjectsOfType(Type type)=>FindObjectsOfType(type,false);[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.",false)]public static Object[] FindObjectsOfType(Type type,bool includeInactive)=>FindObjectsByType(type,includeInactive?FindObjectsInactive.Include:FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID);[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.",false)]public static T[] FindObjectsOfType<T>() where T:Object=>FindObjectsOfType<T>(false);[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.",false)]public static T[] FindObjectsOfType<T>(bool includeInactive) where T:Object=>FindObjectsOfType(typeof(T),includeInactive).Cast<T>().ToArray();
 [Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType",false)]public static Object? FindObjectOfType(Type type)=>FindObjectOfType(type,false);[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType",false)]public static Object? FindObjectOfType(Type type,bool includeInactive)=>FindObjectsOfType(type,includeInactive).FirstOrDefault();[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType",false)]public static T? FindObjectOfType<T>() where T:Object=>FindObjectOfType<T>(false);[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType",false)]public static T? FindObjectOfType<T>(bool includeInactive) where T:Object=>FindObjectOfType(typeof(T),includeInactive) as T;
 [Obsolete("Object.FindSceneObjectsOfType has been deprecated, Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindSceneObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.",false)]public static Object[] FindSceneObjectsOfType(Type type)=>FindObjectsOfType(type,true);[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]public static Object[] FindObjectsOfTypeAll(Type type)=>NativeObjects.All(type);[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]public static Object[] FindObjectsOfTypeIncludingAssets(Type type)=>NativeObjects.All(type);
 [Obsolete("use Object.Destroy instead.")]public static void DestroyObject(Object obj)=>Destroy(obj);[Obsolete("use Object.Destroy instead.")]public static void DestroyObject(Object obj,float t)=>Destroy(obj,t);
}

// Namespace: UnityEngine
public partial class Object {
 public static Object Instantiate(Object original,SceneManagement.Scene scene)=>Instantiate(original,new InstantiateParameters{scene=scene,worldSpace=true});
 public static T Instantiate<T>(T original,InstantiateParameters parameters) where T:Object {
  ArgumentNullException.ThrowIfNull(original);
  return (T)Wrap(Legacy.CloneBlueprint.Capture(original.Native).Instantiate(parameters));
 }
 public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation,InstantiateParameters parameters) where T:Object {
  ArgumentNullException.ThrowIfNull(original);
  return (T)Wrap(Legacy.CloneBlueprint.Capture(original.Native).Instantiate(parameters,position,rotation));
 }
}

// Namespace: UnityEngine
public partial class Object {
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation, CancellationToken cancellationToken) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,1,parameters,Array.Empty<Vector3>(),Array.Empty<Quaternion>(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,positions.ToArray(),rotations.ToArray(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,new[]{position},new[]{rotation},cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},positions.ToArray(),rotations.ToArray(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},positions.ToArray(),rotations.ToArray(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, CancellationToken cancellationToken) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},positions.ToArray(),rotations.ToArray(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{parent=parent,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,Array.Empty<Vector3>(),Array.Empty<Quaternion>(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,1,parameters,new[]{position},new[]{rotation},cancellationToken);
}

// Namespace: Axiom.Gameplay
public partial class Object {
 public static GameObject Instantiate(PrefabAsset original)=>Context.ImportResource(original.Path,true).First();
 public static GameObject Instantiate(PrefabAsset original,Vector3 position,Quaternion rotation){var root=Instantiate(original);root.transform.position=position;root.transform.rotation=rotation;return root;}
 public static void DontDestroyOnLoad(GameObject target){_=Context.Read(target.Entity);var root=target.transform;while(root.parent!=null)root=root.parent;SceneManagement.SceneManager.Persistent.Add(root.Entity.Id);}
}
```
