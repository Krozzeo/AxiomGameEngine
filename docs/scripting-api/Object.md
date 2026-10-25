# Object

Runtime creation/cloning/destruction and component queries. Instantiate recursively clones descendants and compiled script field values, then invokes the new lifecycle. Destroy(Component) removes that component; Destroy(GameObject) removes the subtree. See the Objects and components section for exact limits.

## Implemented declarations

```csharp
public partial class Object {
 public static GameObject Instantiate(GameObject original)=>Instantiate(original,original.transform.position,original.transform.rotation);
 public static GameObject Instantiate(GameObject original,Vector3 position,Quaternion rotation){var entity=Context.CloneHierarchy(original.Entity,position,rotation);entity.SetRotation(rotation);return new GameObject(entity);}
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
 public static GameObject Instantiate(GameObject original,Transform? parent)=>Instantiate(original,parent,true);
 public static GameObject Instantiate(GameObject original,Transform? parent,bool worldPositionStays){var clone=Instantiate(original);var local=Context.Local(original.Entity);clone.transform.SetParent(parent,true);if(!worldPositionStays)Context.LocalTransform(clone.Entity,local.Position,local.Rotation,local.Scale);return clone;}
 public static T[] FindObjectsByType<T>() where T:Component=>Context.Entities.Keys.Where(Context.IsActive).SelectMany(id=>new GameObject(new Entity(id,Context.Generation)).GetComponents<T>()).ToArray();
 public static T? FindFirstObjectByType<T>() where T:Component=>FindObjectsByType<T>().FirstOrDefault();
}

public partial class Object {
 private static long nextIdentity;
 internal static int AllocateIdentity()=>checked((int)System.Threading.Interlocked.Increment(ref nextIdentity));
 private int managedIdentity;
 internal int ManagedIdentity=>managedIdentity==0?managedIdentity=AllocateIdentity():managedIdentity;
 private int nativeIdentity,identityEpoch;
 private Entity? identityEntity;
 private string? identitySlot;
 internal void BindIdentity(Entity entity,string slot){identityEntity=entity;identitySlot=slot;identityEpoch=Context.ObjectEpochs.GetValueOrDefault((entity,slot));var key=(entity,slot,identityEpoch);if(!Context.ObjectIdentities.TryGetValue(key,out nativeIdentity))Context.ObjectIdentities[key]=nativeIdentity=AllocateIdentity();}
 public int GetInstanceID()=>nativeIdentity==0?ManagedIdentity:nativeIdentity;
 public override int GetHashCode()=>GetInstanceID();
 public override bool Equals(object? other)=>other is Object obj?this==obj:ReferenceEquals(other,null)&&!Exists(this);
 public static bool operator ==(Object? left,Object? right){if(ReferenceEquals(left,right))return true;if(ReferenceEquals(left,null))return !Exists(right);if(ReferenceEquals(right,null))return !Exists(left);return left.GetInstanceID()==right.GetInstanceID();}
 public static bool operator !=(Object? left,Object? right)=>!(left==right);
 private static bool Exists(Object? value){if(ReferenceEquals(value,null))return false;if(value.identityEntity is not Entity e)return value is not Component;if(e.Generation!=Context.Generation||!Context.Entities.ContainsKey(e.Id)||Context.ObjectEpochs.GetValueOrDefault((e,value.identitySlot!))!=value.identityEpoch)return false;if(value is Script script)return Context.LiveScripts.Any(s=>ReferenceEquals(s,script));if(value is AI.NavMeshAgent agent)return AI.NavMeshAgent.Agents.TryGetValue(e.Id,out var live)&&ReferenceEquals(agent,live);if(value is Component c&&Context.ComponentKey(c.GetType()) is string key)return Context.EntityData[e.Id].TryGetProperty(key,out _);return true;}
 public static implicit operator bool(Object? value)=>Exists(value);
}

public partial class Object {
 public static GameObject Instantiate(PrefabAsset original)=>Context.ImportResource(original.Path,true).First();
 public static GameObject Instantiate(PrefabAsset original,Vector3 position,Quaternion rotation){var root=Instantiate(original);root.transform.position=position;root.transform.rotation=rotation;return root;}
 public static void DontDestroyOnLoad(GameObject target){_=Context.Read(target.Entity);var root=target.transform;while(root.parent!=null)root=root.parent;SceneManagement.SceneManager.Persistent.Add(root.Entity.Id);}
}
```
