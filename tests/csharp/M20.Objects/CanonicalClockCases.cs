using L=Axiom.Gameplay;
using U=UnityEngine;
public static class CanonicalClockCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(2201);U.Time.timeScale=2;L.Time.Step(.01);
  check("canonical and legacy scripts share the scaled simulation clock",Math.Abs(U.Time.time-.02f)<1e-7&&U.Time.timeAsDouble==L.Time.time&&U.Time.frameCount==1&&U.Time.unscaledTimeAsDouble==.01);
  L.Time.inFixedTimeStep=true;L.Time.fixedTime=.04;
  check("canonical clock exposes the active fixed phase",U.Time.inFixedTimeStep&&U.Time.deltaTime==(float)L.Time.fixedDeltaTime&&U.Time.fixedTimeAsDouble==.04);
  L.Time.inFixedTimeStep=false;
  var scene=U.SceneManagement.SceneManager.CreateScene("Canonical unload");var root=new U.GameObject("Unloaded through canonical facade");U.SceneManagement.SceneManager.MoveGameObjectToScene(root,scene);
  var operation=U.SceneManagement.SceneManager.UnloadSceneAsync(scene);var delivered=0;operation.completed+=_=>delivered++;
  check("canonical unload operates on the shared scene and defers its completion callback",!root&&operation.isDone&&operation.progress==1&&delivered==0);
  U.AsyncInstantiationQueue.Tick();check("completion is not reentrant in its creation frame",delivered==0);
  L.Time.Step(1d/60);U.AsyncInstantiationQueue.Tick();U.AsyncInstantiationQueue.Tick();
  check("canonical unload completion delivers exactly once on the next frame",delivered==1);
  operation.completed+=_=>delivered++;
  check("a late completion subscription observes the already delivered operation",delivered==2);
 }
}
