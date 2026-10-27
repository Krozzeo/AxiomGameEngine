# Scene

A runtime scene identity (name, physical path, deterministic resource buildIndex), loaded state and root object lookup. Default Scene is invalid. The current editor scene initially has buildIndex -1. Creation and moving root subtrees update ownership; all entities still appear in the editor runtime Hierarchy.

SDK 8 adds the Axiom-specific `alwaysActive` property. Set it on a Scene variable or call SceneManager.SetSceneAlwaysActive. `GameObject.scene` identifies the owning loaded scene.

## Implemented declarations

```csharp
// Namespace: UnityEngine.SceneManagement
public readonly struct Scene : IEquatable<Scene> {
 internal readonly Axiom.Gameplay.SceneManagement.Scene Native;
 internal Scene(Axiom.Gameplay.SceneManagement.Scene value){Native=value;}
 public string name=>Native.name;public string path=>Native.path;public int buildIndex=>Native.buildIndex;public bool isLoaded=>Native.isLoaded;public bool IsValid()=>Native.IsValid();public int rootCount=>Native.rootCount;
 public UnityEngine.GameObject[] GetRootGameObjects()=>Native.GetRootGameObjects().Select(value=>(UnityEngine.GameObject)value).ToArray();public void GetRootGameObjects(List<UnityEngine.GameObject> rootGameObjects){ArgumentNullException.ThrowIfNull(rootGameObjects);rootGameObjects.Clear();rootGameObjects.AddRange(GetRootGameObjects());}
 public bool Equals(Scene other)=>Native.Equals(other.Native);public override bool Equals(object? other)=>other is Scene scene&&Equals(scene);public override int GetHashCode()=>Native.GetHashCode();public static bool operator ==(Scene a,Scene b)=>a.Equals(b);public static bool operator !=(Scene a,Scene b)=>!a.Equals(b);
 public static implicit operator Scene(Axiom.Gameplay.SceneManagement.Scene value)=>new(value);public static implicit operator Axiom.Gameplay.SceneManagement.Scene(Scene value)=>value.Native;
}

// Namespace: Axiom.Gameplay.SceneManagement
public readonly struct Scene {
 public string name {get;}public string path {get;}public int buildIndex {get;}internal readonly bool PersistentContext;private readonly int runtimeGeneration;internal string OwnerKey=>PersistentContext?SceneManager.PersistentOwner:name;public bool isLoaded {get{if(PersistentContext)return runtimeGeneration==Context.Generation;var title=name;return SceneManager.Loaded.Any(s=>s.name==title);}}
 internal Scene(string title,string source,int index,bool persistent=false){name=title;path=source;buildIndex=index;PersistentContext=persistent;runtimeGeneration=persistent?Context.Generation:0;}
 public bool alwaysActive {get=>SceneManager.IsSceneAlwaysActive(this);set=>SceneManager.SetSceneAlwaysActive(this,value);}
 public bool IsValid()=>!string.IsNullOrEmpty(name)&&(!PersistentContext||runtimeGeneration==Context.Generation);
 public GameObject[] GetRootGameObjects(){var title=OwnerKey;if(!IsValid())return Array.Empty<GameObject>();return Context.Entities.Keys.Where(id=>Context.Parents.GetValueOrDefault(id)==null&&SceneManager.Owners.GetValueOrDefault(id,SceneManager.GetActiveScene().name)==title).Select(id=>new GameObject(new Entity(id,Context.Generation))).ToArray();}
 public int rootCount=>GetRootGameObjects().Length;
}

// Namespace: UnityEngine
public class AsyncOperation : YieldInstruction {
 internal bool Done,EventRaised;internal readonly int CreationFrame=Legacy.Time.frameCount;
 private Action<AsyncOperation>? callbacks;
 public bool isDone=>Done;
 public virtual float progress=>Done?1:0;
 public int priority {get;set;}
 public bool allowSceneActivation {get;set;}=true;
 public event Action<AsyncOperation> completed {
  add{if(EventRaised)value(this);else callbacks+=value;}
  remove{callbacks-=value;}
 }
 internal void DispatchCompleted(){if(!Done||EventRaised||Legacy.Time.frameCount<=CreationFrame)return;EventRaised=true;var handlers=callbacks;callbacks=null;handlers?.Invoke(this);}
 internal void Abandon(){Done=true;EventRaised=true;callbacks=null;}
}

// Namespace: Axiom.Gameplay
public class AsyncOperation {public virtual bool isDone=>true;public virtual double progress=>1;private event Action<AsyncOperation>? callbacks;public event Action<AsyncOperation> completed {add{if(isDone)value(this);else callbacks+=value;}remove{callbacks-=value;}}internal void Complete(){callbacks?.Invoke(this);callbacks=null;}}
```
