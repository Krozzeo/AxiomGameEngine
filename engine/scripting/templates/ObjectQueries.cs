namespace Axiom.Gameplay;

public enum FindObjectsInactive { Exclude, Include }
public enum FindObjectsSortMode { None, InstanceID }

internal static class ObjectQueries {
 internal static void ComponentType(Type type){ArgumentNullException.ThrowIfNull(type);if(!type.IsInterface&&!typeof(Component).IsAssignableFrom(type)&&!typeof(UnityEngine.Component).IsAssignableFrom(type))throw new ArgumentException("A component class or interface is required",nameof(type));}
 internal static void Fill<T>(List<T> results,IEnumerable<T> values){ArgumentNullException.ThrowIfNull(results);var snapshot=values.ToArray();results.Clear();results.AddRange(snapshot);}
 internal static Object[] Find(Type type,FindObjectsInactive inactive,FindObjectsSortMode sort){
  ArgumentNullException.ThrowIfNull(type);if(!typeof(Object).IsAssignableFrom(type)&&!typeof(UnityEngine.Object).IsAssignableFrom(type))throw new ArgumentException("An Object type is required",nameof(type));
  if(!Enum.IsDefined(inactive)||!Enum.IsDefined(sort))throw new ArgumentOutOfRangeException("Invalid find mode");
  var found=new List<Object>();foreach(var id in Context.Entities.Keys){if(inactive==FindObjectsInactive.Exclude&&!Context.IsActive(id))continue;var go=new GameObject(new Entity(id,Context.Generation));if(type.IsInstanceOfType(go)||typeof(UnityEngine.Object).IsAssignableFrom(type)&&type.IsInstanceOfType(UnityEngine.Object.Wrap(go)))found.Add(go);found.AddRange(go.AllComponents().Where(component=>ScriptBridge.Matches(type,component)));}
  return (sort==FindObjectsSortMode.InstanceID?found.OrderBy(o=>o.GetInstanceID()):found.AsEnumerable()).ToArray();
 }
}

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

public partial class Component {
 public int GetComponentIndex()=>gameObject.GetComponentIndex(this);
 public Component? GetComponent(Type type)=>gameObject.GetComponent(type);
 public Component[] GetComponents(Type type)=>gameObject.GetComponents(type);
 public bool TryGetComponent(Type type,out Component? component)=>gameObject.TryGetComponent(type,out component);
 public void GetComponents(Type type,List<Component> results)=>gameObject.GetComponents(type,results);
 public void GetComponents<T>(List<T> results)=>gameObject.GetComponents(results);
 public Component? GetComponentInChildren(Type type)=>gameObject.GetComponentInChildren(type);
 public Component? GetComponentInChildren(Type type,bool includeInactive)=>gameObject.GetComponentInChildren(type,includeInactive);
 public Component[] GetComponentsInChildren(Type type)=>gameObject.GetComponentsInChildren(type);
 public Component[] GetComponentsInChildren(Type type,bool includeInactive)=>gameObject.GetComponentsInChildren(type,includeInactive);
 public Component? GetComponentInParent(Type type)=>gameObject.GetComponentInParent(type);
 public Component? GetComponentInParent(Type type,bool includeInactive)=>gameObject.GetComponentInParent(type,includeInactive);
 public Component[] GetComponentsInParent(Type type)=>gameObject.GetComponentsInParent(type);
 public Component[] GetComponentsInParent(Type type,bool includeInactive)=>gameObject.GetComponentsInParent(type,includeInactive);
 public T? GetComponentInChildren<T>(bool includeInactive)=>gameObject.GetComponentInChildren<T>(includeInactive);
 public T? GetComponentInParent<T>(bool includeInactive)=>gameObject.GetComponentInParent<T>(includeInactive);
 public T[] GetComponentsInChildren<T>()=>gameObject.GetComponentsInChildren<T>();
 public T[] GetComponentsInParent<T>()=>gameObject.GetComponentsInParent<T>();
 public void GetComponentsInChildren<T>(List<T> results)=>gameObject.GetComponentsInChildren(results);
 public void GetComponentsInChildren<T>(bool includeInactive,List<T> results)=>gameObject.GetComponentsInChildren(includeInactive,results);
 public void GetComponentsInParent<T>(bool includeInactive,List<T> results)=>gameObject.GetComponentsInParent(includeInactive,results);
}

public partial class Object {
 private static Object CloneObject(Object original,Vector3? position,Quaternion? rotation,Transform? parent,bool worldPositionStays){
  ArgumentNullException.ThrowIfNull(original);if(!original)throw new ArgumentException("Cannot clone a destroyed object",nameof(original));
  if(RuntimeAssets.IsAsset(original))return RuntimeAssets.Clone(RuntimeAssets.Capture(original),1)[0];var source=original is GameObject go?go:original is Component component?component.gameObject:throw new NotSupportedException("This object requires its asset/resource cloning API");
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
