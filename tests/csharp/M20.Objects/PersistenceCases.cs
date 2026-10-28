using L=Axiom.Gameplay;
using U=UnityEngine;
using Scenes=Axiom.Gameplay.SceneManagement.SceneManager;

public static class PersistenceCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(34);
  var source=Scenes.GetActiveScene();var other=Scenes.CreateScene("Destination");
  var root=new U.GameObject("Persistent root");var child=new U.GameObject("Persistent child");child.transform.SetParent(root.transform,false);
  var sibling=new U.GameObject("Ordinary sibling");
  U.Object.DontDestroyOnLoad(child);
  check("canonical child persistence does not promote its ancestor",!Scenes.Persistent.Contains(root.LegacyGameObject.Entity.Id)&&!Scenes.Persistent.Contains(child.LegacyGameObject.Entity.Id));
  U.Object.DontDestroyOnLoad(child.transform);
  check("canonical child component persistence does not promote its ancestor",Scenes.Persistent.Count==0);
  U.Object.DontDestroyOnLoad(root.transform);U.Object.DontDestroyOnLoad(root);
  check("canonical root and root component persistence is idempotent",Scenes.Persistent.SetEquals(new[]{root.LegacyGameObject.Entity.Id}));
  var persistent=root.scene;
  check("persistent hierarchy belongs to a valid loaded private scene",persistent.IsValid()&&persistent.isLoaded&&persistent.name=="DontDestroyOnLoad"&&persistent.path==""&&persistent.buildIndex==-1&&child.scene==persistent);
  check("persistent scene is separate from normal loaded scene enumeration",Scenes.sceneCount==2&&source.rootCount==1&&persistent.rootCount==1&&persistent.GetRootGameObjects()[0]==root);
  check("private persistent scene cannot become the active creation scene",!Scenes.SetActiveScene(persistent)&&Scenes.GetActiveScene().name==source.name);
  Scenes.SetActiveScene(other);Scenes.UnloadSceneAsync(source.name);
  check("persistent root retains descendants and identities after source unload",root&&child&&child.transform.parent==root.transform&&!sibling);
  check("persistent descendants remain queryable after source unload",U.GameObject.Find("Persistent child")==child);
  check("persistent scene remains valid after source unload",root.scene==persistent&&child.scene==persistent&&persistent.isLoaded);
  using(var output=System.Text.Json.JsonDocument.Parse(L.Context.Output())){var documents=output.RootElement.GetProperty("sceneDocuments");check("host packet preserves persistent scene ownership",documents.GetProperty("open").EnumerateArray().Any(value=>value.GetString()==Scenes.PersistentOwner)&&documents.GetProperty("owners").GetProperty(root.LegacyGameObject.Entity.Id).GetString()==Scenes.PersistentOwner);}
  U.Object.DestroyImmediate(root);
  check("persistent root destruction retires descendants and registry entry",!root&&!child&&Scenes.Persistent.Count==0);
  try{U.Object.DontDestroyOnLoad(root);check("destroyed persistence target rejected",false);}catch(U.MissingReferenceException){check("destroyed persistence target rejected",true);}
  try{U.Object.DontDestroyOnLoad(null!);check("null persistence target rejected",false);}catch(ArgumentNullException){check("null persistence target rejected",true);}
  var before=Scenes.Persistent.Count;var unattached=new U.Object();
  try{U.Object.DontDestroyOnLoad(unattached);check("unattached persistence target has no scene effect",false);}catch(ArgumentNullException){check("unattached persistence target has no scene effect",Scenes.Persistent.Count==before);}
  reset(35);check("persistence registry clears on runtime reset",Scenes.Persistent.Count==0&&!persistent.IsValid()&&!persistent.isLoaded);
  var permanent=new U.GameObject("Permanent");var detached=new U.GameObject("Detached");detached.transform.SetParent(permanent.transform,false);U.Object.DontDestroyOnLoad(permanent);var permanentScene=permanent.scene;
  detached.transform.SetParent(null,true);check("detached persistent child retains its scene",detached.scene==permanentScene&&permanentScene.rootCount==2);
  var attached=new U.GameObject("New child");attached.transform.SetParent(permanent.transform,false);check("parenting into persistent hierarchy transfers scene ownership",attached.scene==permanentScene);
  var clone=U.Object.Instantiate(new U.GameObject("Clone source"),permanent.transform,true);check("cloning under a persistent parent uses the private destination scene",clone.scene==permanentScene&&clone.transform.parent==permanent.transform);
  var destination=Scenes.CreateScene("Return");var ordinary=new U.GameObject("Ordinary");Scenes.MoveGameObjectToScene(ordinary.LegacyGameObject,destination);detached.transform.SetParent(ordinary.transform,false);
  check("parenting out of persistence transfers subtree to parent scene",detached.scene==(U.SceneManagement.Scene)destination&&permanentScene.rootCount==1);
  Scenes.MoveGameObjectToScene(permanent.LegacyGameObject,destination);
  check("moving a persistent root back to a normal scene clears persistence",permanent.scene==(U.SceneManagement.Scene)destination&&attached.scene==permanent.scene&&clone.scene==permanent.scene&&!Scenes.HasPersistentScene&&Scenes.Persistent.Count==0);
  Scenes.UnloadSceneAsync(destination.name);check("returned hierarchy obeys normal scene unload",!permanent&&!attached&&!clone&&!detached&&!ordinary);
  reset(39);var legacyParent=new L.GameObject("Legacy persistent root");var legacyChild=new L.GameObject("Legacy persistent child");legacyChild.transform.SetParent(legacyParent.transform);L.Object.DontDestroyOnLoad(legacyChild);
  check("legacy root promotion shares actual persistent scene ownership",legacyParent.scene.isLoaded&&legacyParent.scene.name=="DontDestroyOnLoad"&&legacyChild.scene.Equals(legacyParent.scene));
  var legacySource=Scenes.GetActiveScene();var legacyDestination=Scenes.CreateScene("Legacy destination");Scenes.SetActiveScene(legacyDestination);Scenes.UnloadSceneAsync(legacySource.name);check("legacy persistent wrappers keep valid scenes after source unload",legacyParent&&legacyChild&&legacyParent.scene.isLoaded&&legacyChild.scene.IsValid());

 }
}
