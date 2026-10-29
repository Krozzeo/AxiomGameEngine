using Legacy=Axiom.Gameplay;
namespace UnityEngine;
// Canonical timing reads the same simulation clock as lifecycle, physics and legacy scripts.
public static class Time {
 public static float time=>(float)Legacy.Time.time;
 public static double timeAsDouble=>Legacy.Time.timeAsDouble;
 public static float timeSinceLevelLoad=>(float)Legacy.Time.timeSinceLevelLoad;
 public static double timeSinceLevelLoadAsDouble=>Legacy.Time.timeSinceLevelLoadAsDouble;
 public static float deltaTime=>(float)Legacy.Time.deltaTime;
 public static float unscaledTime=>(float)Legacy.Time.unscaledTime;
 public static double unscaledTimeAsDouble=>Legacy.Time.unscaledTimeAsDouble;
 public static float unscaledDeltaTime=>(float)Legacy.Time.unscaledDeltaTime;
 public static float fixedTime=>(float)Legacy.Time.fixedTime;
 public static double fixedTimeAsDouble=>Legacy.Time.fixedTimeAsDouble;
 public static float fixedUnscaledTime=>(float)Legacy.Time.fixedUnscaledTime;
 public static double fixedUnscaledTimeAsDouble=>Legacy.Time.fixedUnscaledTimeAsDouble;
 public static float fixedUnscaledDeltaTime=>(float)Legacy.Time.fixedUnscaledDeltaTime;
 public static float fixedDeltaTime=>(float)Legacy.Time.fixedDeltaTime;
 public static float maximumDeltaTime=>(float)Legacy.Time.maximumDeltaTime;
 public static float smoothDeltaTime=>(float)Legacy.Time.smoothDeltaTime;
 public static float timeScale {get=>(float)Legacy.Time.timeScale;set=>Legacy.Time.timeScale=value;}
 public static int frameCount=>Legacy.Time.frameCount;
 public static bool inFixedTimeStep=>Legacy.Time.inFixedTimeStep;
 public static float realtimeSinceStartup=>(float)Legacy.Time.realtimeSinceStartup;
 public static double realtimeSinceStartupAsDouble=>Legacy.Time.realtimeSinceStartupAsDouble;
}
