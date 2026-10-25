# Component

Namespace: `Axiom.Gameplay`

## Description

Base for script and built-in component wrappers. Entity identifies its owner; gameObject and transform expose the owner facade. Component searches walk the current runtime hierarchy.

## Use cases

Retrieve the owning Rigidbody without copying scene JSON or inventing entity IDs.

## Example

The statements below belong inside a `MonoBehaviour` callback. Add the components/resources mentioned in the use case first.

```csharp
var body = GetComponent<Rigidbody>();
if (body != null) body.linearVelocity = Vector3.up * 3;
var camera = GetComponentInChildren<Camera>();
if (camera != null) Debug.Log(camera.gameObject.name);
```

## Compatibility and limitations

Transforms use world position/rotation. localScale writes the runtime scale. Legacy Entity.Transform.Position/Rotation/Scale remain snapshots; lowercase transform properties are live. Generic component lookup covers built-in wrappers and already attached scripts. Instantiate clones one entity using the existing spawn contract; it does not yet clone a hierarchy or start new script instances. SetActive controls runtime hierarchy participation; GetComponentInChildren and GetComponentInParent traverse existing components. Dynamic AddComponent, tags and full Unity object identity semantics remain pending.


## Implemented declarations

```csharp
public partial class Component : Object {
 private Entity entity;
 public Entity Entity {get=>entity;internal set{entity=value;BindIdentity(value,Context.ComponentKey(GetType())??(this is Script?"script:"+ManagedIdentity:GetType().FullName!));}}
 public GameObject gameObject {get{if(!this)throw new InvalidOperationException("Component reference has been destroyed or is not attached");return new(Entity);}}
 public Transform transform=>gameObject.transform;
 public T? GetComponent<T>()=>gameObject.GetComponent<T>();
 public T? GetComponentInChildren<T>()=>gameObject.GetComponentInChildren<T>();
 public T? GetComponentInParent<T>()=>gameObject.GetComponentInParent<T>();
 public bool TryGetComponent<T>(out T? component) {component=GetComponent<T>();return component!=null;}
}

public partial class Component {
 public string name {get=>gameObject.name;set=>gameObject.name=value;}
 public string tag {get=>gameObject.tag;set=>gameObject.tag=value;}
 public bool CompareTag(string value)=>gameObject.CompareTag(value);
 public T[] GetComponents<T>()=>gameObject.GetComponents<T>();
 public T[] GetComponentsInChildren<T>(bool includeInactive)=>gameObject.GetComponentsInChildren<T>(includeInactive);
 public T[] GetComponentsInParent<T>(bool includeInactive)=>gameObject.GetComponentsInParent<T>(includeInactive);
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
```
