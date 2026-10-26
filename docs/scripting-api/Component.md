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
// Namespace: Axiom.Gameplay
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

// Namespace: Axiom.Gameplay
public partial class Component {
 public string name {get=>gameObject.name;set=>gameObject.name=value;}
 public string tag {get=>gameObject.tag;set=>gameObject.tag=value;}
 public bool CompareTag(string value)=>gameObject.CompareTag(value);
 public T[] GetComponents<T>()=>gameObject.GetComponents<T>();
 public T[] GetComponentsInChildren<T>(bool includeInactive)=>gameObject.GetComponentsInChildren<T>(includeInactive);
 public T[] GetComponentsInParent<T>(bool includeInactive)=>gameObject.GetComponentsInParent<T>(includeInactive);
}

// Namespace: Axiom.Gameplay
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

// Namespace: UnityEngine
public partial class Component : Object {
 public Component(){} internal Component(Legacy.Component value){Wrapped=value;}
 public static implicit operator Component(Legacy.Component value)=>ReferenceEquals(value,null)?null!:(Component)Wrap(value);
 internal Legacy.Component LegacyComponent {get{if(!this)throw new MissingReferenceException("Component has been destroyed or is not attached");return Wrapped as Legacy.Component??throw new MissingReferenceException("Component is not attached");}}
 public GameObject gameObject=>LegacyComponent.gameObject;public Transform transform=>LegacyComponent.transform;
 public T? GetComponent<T>()=>LegacyComponent.GetComponent<T>();public Component? GetComponent(Type type)=>(Component?)LegacyComponent.GetComponent(type);public Component? GetComponent(string type)=>gameObject.GetComponent(type);
 public T[] GetComponents<T>()=>LegacyComponent.GetComponents<T>();public Component[] GetComponents(Type type)=>LegacyComponent.GetComponents(type).Select(native=>(Component)Wrap(native)).ToArray();public void GetComponents<T>(List<T> results)=>LegacyComponent.GetComponents(results);
 public T? GetComponentInChildren<T>()=>LegacyComponent.GetComponentInChildren<T>();public T? GetComponentInChildren<T>(bool includeInactive)=>LegacyComponent.GetComponentInChildren<T>(includeInactive);public T? GetComponentInParent<T>()=>LegacyComponent.GetComponentInParent<T>();public T? GetComponentInParent<T>(bool includeInactive)=>LegacyComponent.GetComponentInParent<T>(includeInactive);
 public T[] GetComponentsInChildren<T>()=>LegacyComponent.GetComponentsInChildren<T>(false);public T[] GetComponentsInChildren<T>(bool includeInactive)=>LegacyComponent.GetComponentsInChildren<T>(includeInactive);public T[] GetComponentsInParent<T>()=>LegacyComponent.GetComponentsInParent<T>(false);public T[] GetComponentsInParent<T>(bool includeInactive)=>LegacyComponent.GetComponentsInParent<T>(includeInactive);
 public bool TryGetComponent<T>(out T? component)=>LegacyComponent.TryGetComponent(out component);public string tag {get=>LegacyComponent.tag;set{Tags.Require(value);LegacyComponent.tag=value;}}public bool CompareTag(string tag){Tags.Require(tag);return LegacyComponent.CompareTag(tag);}public int GetComponentIndex()=>LegacyComponent.GetComponentIndex();
}

// Namespace: UnityEngine
public partial class Component {
 public TransformHandle transformHandle=>new(transform);
 public Component? GetComponentInChildren(Type t)=>GetComponentInChildren(t,false);public Component? GetComponentInChildren(Type t,bool includeInactive)=>LegacyComponent.GetComponentInChildren(t,includeInactive);public Component? GetComponentInParent(Type t)=>GetComponentInParent(t,false);public Component? GetComponentInParent(Type t,bool includeInactive)=>LegacyComponent.GetComponentInParent(t,includeInactive);
 public Component[] GetComponentsInChildren(Type t)=>GetComponentsInChildren(t,false);public Component[] GetComponentsInChildren(Type t,bool includeInactive)=>LegacyComponent.GetComponentsInChildren(t,includeInactive).Select(native=>(Component)Object.Wrap(native)).ToArray();public Component[] GetComponentsInParent(Type t)=>GetComponentsInParent(t,false);public Component[] GetComponentsInParent(Type t,bool includeInactive)=>LegacyComponent.GetComponentsInParent(t,includeInactive).Select(native=>(Component)Object.Wrap(native)).ToArray();
 public void GetComponents(Type type,List<Component> results){ArgumentNullException.ThrowIfNull(results);var values=GetComponents(type);results.Clear();results.AddRange(values);}
 public void GetComponentsInChildren<T>(List<T> results)=>GetComponentsInChildren(false,results);public void GetComponentsInChildren<T>(bool includeInactive,List<T> result)=>LegacyComponent.GetComponentsInChildren(includeInactive,result);public void GetComponentsInParent<T>(bool includeInactive,List<T> results)=>LegacyComponent.GetComponentsInParent(includeInactive,results);
 public bool TryGetComponent(Type type,out Component? component){var value=GetComponent(type);component=value;return value;}
}

// Namespace: UnityEngine
public partial class Component {public bool CompareTag(TagHandle tag)=>CompareTag(tag.Value??"Untagged");}

// Namespace: UnityEngine
public partial class Component {
 public void SendMessage(string methodName)=>gameObject.SendMessage(methodName);public void SendMessage(string methodName,object? value)=>gameObject.SendMessage(methodName,value);public void SendMessage(string methodName,SendMessageOptions options)=>gameObject.SendMessage(methodName,options);public void SendMessage(string methodName,object? value,SendMessageOptions options)=>gameObject.SendMessage(methodName,value,options);
 public void SendMessageUpwards(string methodName)=>gameObject.SendMessageUpwards(methodName);public void SendMessageUpwards(string methodName,object? value)=>gameObject.SendMessageUpwards(methodName,value);public void SendMessageUpwards(string methodName,SendMessageOptions options)=>gameObject.SendMessageUpwards(methodName,options);public void SendMessageUpwards(string methodName,object? value,SendMessageOptions options)=>gameObject.SendMessageUpwards(methodName,value,options);
 public void BroadcastMessage(string methodName)=>gameObject.BroadcastMessage(methodName);public void BroadcastMessage(string methodName,object? parameter)=>gameObject.BroadcastMessage(methodName,parameter);public void BroadcastMessage(string methodName,SendMessageOptions options)=>gameObject.BroadcastMessage(methodName,options);public void BroadcastMessage(string methodName,object? parameter,SendMessageOptions options)=>gameObject.BroadcastMessage(methodName,parameter,options);
}

// Namespace: UnityEngine
public partial class Component {
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
```
