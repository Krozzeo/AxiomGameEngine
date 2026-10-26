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
  Scenes.SetActiveScene(other);Scenes.UnloadSceneAsync(source.name);
  check("persistent root retains descendants and identities after source unload",root&&child&&child.transform.parent==root.transform&&!sibling);
  check("persistent descendants remain queryable after source unload",U.GameObject.Find("Persistent child")==child);
  U.Object.DestroyImmediate(root);
  check("persistent root destruction retires descendants and registry entry",!root&&!child&&Scenes.Persistent.Count==0);
  try{U.Object.DontDestroyOnLoad(root);check("destroyed persistence target rejected",false);}catch(U.MissingReferenceException){check("destroyed persistence target rejected",true);}
  try{U.Object.DontDestroyOnLoad(null!);check("null persistence target rejected",false);}catch(ArgumentNullException){check("null persistence target rejected",true);}
  var before=Scenes.Persistent.Count;var unattached=new U.Object();
  try{U.Object.DontDestroyOnLoad(unattached);check("unattached persistence target has no scene effect",false);}catch(U.MissingReferenceException){check("unattached persistence target has no scene effect",Scenes.Persistent.Count==before);}
  reset(35);check("persistence registry clears on runtime reset",Scenes.Persistent.Count==0);
 }
}
