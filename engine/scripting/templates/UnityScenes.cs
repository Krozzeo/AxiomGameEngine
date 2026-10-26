namespace UnityEngine.SceneManagement;
public readonly struct Scene : IEquatable<Scene> {
 internal readonly Axiom.Gameplay.SceneManagement.Scene Native;
 internal Scene(Axiom.Gameplay.SceneManagement.Scene value){Native=value;}
 public string name=>Native.name;public string path=>Native.path;public int buildIndex=>Native.buildIndex;public bool isLoaded=>Native.isLoaded;public bool IsValid()=>Native.IsValid();public int rootCount=>Native.rootCount;
 public UnityEngine.GameObject[] GetRootGameObjects()=>Native.GetRootGameObjects().Select(value=>(UnityEngine.GameObject)value).ToArray();public void GetRootGameObjects(List<UnityEngine.GameObject> rootGameObjects){ArgumentNullException.ThrowIfNull(rootGameObjects);rootGameObjects.Clear();rootGameObjects.AddRange(GetRootGameObjects());}
 public bool Equals(Scene other)=>Native.Equals(other.Native);public override bool Equals(object? other)=>other is Scene scene&&Equals(scene);public override int GetHashCode()=>Native.GetHashCode();public static bool operator ==(Scene a,Scene b)=>a.Equals(b);public static bool operator !=(Scene a,Scene b)=>!a.Equals(b);
 public static implicit operator Scene(Axiom.Gameplay.SceneManagement.Scene value)=>new(value);public static implicit operator Axiom.Gameplay.SceneManagement.Scene(Scene value)=>value.Native;
}
public static class SceneManager {
 public static Scene GetActiveScene()=>Axiom.Gameplay.SceneManagement.SceneManager.GetActiveScene();public static Scene GetSceneByName(string name)=>Axiom.Gameplay.SceneManagement.SceneManager.GetSceneByName(name);public static Scene GetSceneByPath(string scenePath)=>Axiom.Gameplay.SceneManagement.SceneManager.GetSceneByPath(scenePath);public static Scene CreateScene(string sceneName)=>Axiom.Gameplay.SceneManagement.SceneManager.CreateScene(sceneName);public static bool SetActiveScene(Scene scene)=>Axiom.Gameplay.SceneManagement.SceneManager.SetActiveScene(scene);public static void MoveGameObjectToScene(UnityEngine.GameObject go,Scene scene)=>Axiom.Gameplay.SceneManagement.SceneManager.MoveGameObjectToScene(go.LegacyGameObject,scene);
}
