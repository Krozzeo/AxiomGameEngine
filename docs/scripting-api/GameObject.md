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
public sealed partial class GameObject : Object {
 internal Entity Entity {get;}
 internal GameObject(Entity entity){Entity=entity;BindIdentity(entity,"gameObject");}
 public string name {get=>Context.Metadata(Entity,"name",Entity.Id);set=>Context.Property(Entity,"name",System.Text.Json.Nodes.JsonValue.Create(value));}
 public Transform transform=>new(Entity);
 public bool activeSelf=>Context.ActiveStates.GetValueOrDefault(Entity.Id,true);public bool activeInHierarchy=>Context.IsActive(Entity.Id);public void SetActive(bool active)=>Context.Activate(Entity,active);
 public static GameObject? Find(string name){foreach(var (id,data) in Context.EntityData)if(Context.IsActive(id)&&data.TryGetProperty("name",out var value)&&value.GetString()==name)return new(new Entity(id,Context.Generation));return null;}
 public T? GetComponent<T>()=>GetComponents<T>().FirstOrDefault();

}

public sealed partial class GameObject {
 public SceneManagement.Scene scene=>SceneManagement.SceneManager.GetSceneByName(SceneManagement.SceneManager.Owners.GetValueOrDefault(Entity.Id,SceneManagement.SceneManager.GetActiveScene().name));
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
  if(c.ValueKind==JsonValueKind.Object){var sphere=c.GetProperty("shape").GetString()=="sphere";result.Add(twoD?(sphere?new CircleCollider2D(Entity):new BoxCollider2D(Entity)):(sphere?new SphereCollider(Entity):new BoxCollider(Entity)));}
  if(Context.Bodies.ContainsKey(Entity.Id))result.Add(twoD?new Rigidbody2D(Entity):new Rigidbody(Entity));
  if(Context.Cameras.Contains(Entity.Id))result.Add(new Camera(Entity));if(Context.AudioSources.Contains(Entity.Id))result.Add(new AudioSource(Entity));if(Context.Animators.ContainsKey(Entity.Id))result.Add(new Animator(Entity));if(data.TryGetProperty("audioListener",out _))result.Add(new AudioListener(Entity));
  if(data.TryGetProperty("renderable",out var renderable))result.Add(data.TryGetProperty("sprite2D",out _)?new SpriteRenderer(Entity):new MeshRenderer(Entity));if(data.TryGetProperty("light",out _))result.Add(new Light(Entity));if(AI.NavMeshAgent.Agents.TryGetValue(Entity.Id,out var agent))result.Add(agent);
  result.AddRange(Context.LiveScripts.Where(s=>s.Entity.Id==Entity.Id));var order=Context.ComponentOrders[Entity];return result.OrderBy(c=>order.IndexOf(c.GetInstanceID())).ToArray();
 }

 public T[] GetComponents<T>()=>GetComponents(typeof(T)).Cast<T>().ToArray();
 public T? GetComponentInChildren<T>(bool includeInactive)=>GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
 public T? GetComponentInParent<T>(bool includeInactive)=>GetComponentsInParent<T>(includeInactive).FirstOrDefault();
 public T[] GetComponentsInChildren<T>(bool includeInactive)=>GetComponentsInChildren(typeof(T),includeInactive).Cast<T>().ToArray();
 public T[] GetComponentsInParent<T>(bool includeInactive)=>GetComponentsInParent(typeof(T),includeInactive).Cast<T>().ToArray();
}

public sealed partial class GameObject {
 public GameObject gameObject=>this;
 public Component? GetComponent(Type type)=>GetComponents(type).FirstOrDefault();
 public Component[] GetComponents(Type type){ObjectQueries.ComponentType(type);return AllComponents().Where(type.IsInstanceOfType).ToArray();}
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
```
