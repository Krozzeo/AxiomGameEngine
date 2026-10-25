# Object

Runtime creation/cloning/destruction and component queries. Instantiate recursively clones descendants and compiled script field values, then invokes the new lifecycle. Destroy(Component) removes that component; Destroy(GameObject) removes the subtree. See the Objects and components section for exact limits.

## Implemented declarations

```csharp
public partial class Object {
 public static GameObject Instantiate(GameObject original)=>(GameObject)Instantiate((Object)original);
 public static GameObject Instantiate(GameObject original,Vector3 position,Quaternion rotation)=>(GameObject)Instantiate((Object)original,position,rotation);
 public static void Destroy(GameObject? target)=>RuntimeLifecycle.DestroyAfter(target,0);
 public static void Destroy(Component? target)=>RuntimeLifecycle.DestroyAfter(target,0);
}

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
 private static bool Exists(Object? value){if(ReferenceEquals(value,null))return false;if(value.identityEntity is not Entity e)return value is not Component;if(e.Generation!=Context.Generation||!Context.Entities.ContainsKey(e.Id)||Context.ObjectEpochs.GetValueOrDefault((e,value.identitySlot!))!=value.identityEpoch)return false;if(value is Script script)return Context.LiveScripts.Any(s=>ReferenceEquals(s,script));if(value is AI.NavMeshAgent agent)return AI.NavMeshAgent.Agents.TryGetValue(e.Id,out var live)&&ReferenceEquals(agent,live);if(value is Component c&&Context.ComponentKey(c.GetType()) is string key)return Context.EntityData[e.Id].TryGetProperty(key,out _);return true;}
 public static implicit operator bool(Object? value)=>Exists(value);
}

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

public partial class Object {
 public static GameObject Instantiate(PrefabAsset original)=>Context.ImportResource(original.Path,true).First();
 public static GameObject Instantiate(PrefabAsset original,Vector3 position,Quaternion rotation){var root=Instantiate(original);root.transform.position=position;root.transform.rotation=rotation;return root;}
 public static void DontDestroyOnLoad(GameObject target){_=Context.Read(target.Entity);var root=target.transform;while(root.parent!=null)root=root.parent;SceneManagement.SceneManager.Persistent.Add(root.Entity.Id);}
}
```
