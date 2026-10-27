using System.Threading;
using Legacy=Axiom.Gameplay;
namespace UnityEngine;

public struct InstantiateParameters {
 public Transform? parent;
 public SceneManagement.Scene scene;
 public bool worldSpace;
 public bool originalImmutable;
}

public partial class Object {
 public static Object Instantiate(Object original,SceneManagement.Scene scene)=>Instantiate(original,new InstantiateParameters{scene=scene});
 public static T Instantiate<T>(T original,InstantiateParameters parameters) where T:Object {
  ArgumentNullException.ThrowIfNull(original);
  return (T)Wrap(Legacy.CloneBlueprint.Capture(original.Native).Instantiate(parameters));
 }
 public static T Instantiate<T>(T original,Vector3 position,Quaternion rotation,InstantiateParameters parameters) where T:Object {
  ArgumentNullException.ThrowIfNull(original);
  return (T)Wrap(Legacy.CloneBlueprint.Capture(original.Native).Instantiate(parameters,position,rotation));
 }
}

public partial class Object {
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation, CancellationToken cancellationToken) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,1,parameters,Array.Empty<Vector3>(),Array.Empty<Quaternion>(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,positions.ToArray(),rotations.ToArray(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,new[]{position},new[]{rotation},cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},positions.ToArray(),rotations.ToArray(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},positions.ToArray(),rotations.ToArray(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, CancellationToken cancellationToken) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},positions.ToArray(),rotations.ToArray(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent, Vector3 position, Quaternion rotation) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{parent=parent,worldSpace=true},new[]{position},new[]{rotation},default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T:Object=>AsyncInstantiationQueue.Begin(original,1,new InstantiateParameters{parent=parent,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent) where T:Object=>AsyncInstantiationQueue.Begin(original,count,new InstantiateParameters{parent=parent,worldSpace=true},Array.Empty<Vector3>(),Array.Empty<Quaternion>(),default);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,count,parameters,Array.Empty<Vector3>(),Array.Empty<Quaternion>(),cancellationToken);
 public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default) where T:Object=>AsyncInstantiationQueue.Begin(original,1,parameters,new[]{position},new[]{rotation},cancellationToken);
}
