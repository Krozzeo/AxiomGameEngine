using System.Runtime.CompilerServices;
using System.Threading;
using Legacy=Axiom.Gameplay;
namespace UnityEngine;

public class YieldInstruction {}
public class AsyncOperation : YieldInstruction {
 internal Legacy.AsyncOperation? LegacyOperation;internal bool Done,EventRaised,Unavailable;internal void RequireHandle(){if(Unavailable)throw new NullReferenceException();}internal readonly int CreationFrame=Legacy.Time.frameCount;
 private Action<AsyncOperation>? callbacks;
 public bool isDone {get{RequireHandle();return LegacyOperation?.isDone??Done;}}
 public virtual float progress {get{RequireHandle();return LegacyOperation is {} operation?(float)operation.progress:Done?1:0;}}
 private int priorityValue;public int priority {get{RequireHandle();return priorityValue;}set{RequireHandle();priorityValue=value;}}
 private bool activate=true;public bool allowSceneActivation {get{RequireHandle();return activate;}set{RequireHandle();activate=value;}}
 public event Action<AsyncOperation> completed {
  add{RequireHandle();if(EventRaised)value(this);else callbacks+=value;}
  remove{RequireHandle();callbacks-=value;}
 }
 internal void DispatchCompleted(){if(!isDone||EventRaised||Legacy.Time.frameCount<=CreationFrame)return;EventRaised=true;var handlers=callbacks;callbacks=null;handlers?.Invoke(this);}
 internal void Abandon(){Done=true;EventRaised=true;callbacks=null;}
}

public class AsyncInstantiateOperation : AsyncOperation {
 private static float integrationTime=2;
 internal Func<Object[]>? Integrate;
 internal CancellationToken Token;internal bool CancelRequested,WasCancelled;
 internal Object[]? Objects;internal Exception? Failure;
 public Object[] Result=>Objects!;
 public override float progress {get{RequireHandle();return Done?1:Integrate!=null?.9f:0;}}
 public void Cancel(){RequireHandle();if(!Done)CancelRequested=true;}
 public bool IsWaitingForSceneActivation(){RequireHandle();return !Done&&!allowSceneActivation&&!CancelRequested&&!Token.IsCancellationRequested&&Integrate!=null;}
 public void WaitForCompletion(){RequireHandle();if(!Done){allowSceneActivation=true;Step();}if(Failure!=null)System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(Failure).Throw();}
 public static float GetIntegrationTimeMS()=>integrationTime;
 public static void SetIntegrationTimeMS(float integrationTimeMS){if(!float.IsFinite(integrationTimeMS)||integrationTimeMS<=0)throw new ArgumentOutOfRangeException(nameof(integrationTimeMS));integrationTime=integrationTimeMS;}
 internal void Step(){
  if(Done)return;
  if(CancelRequested||Token.IsCancellationRequested){Integrate=null;WasCancelled=true;Done=true;return;}
  if(!allowSceneActivation)return;
  try{Objects=Integrate!();}catch(Exception error){Failure=error;}finally{Integrate=null;Done=true;}
 }
}

public class AsyncInstantiateOperation<T> : AsyncInstantiateOperation where T:Object {
 public new T[] Result=>Objects is null?null!:typed??=Objects.Cast<T>().ToArray();private T[]? typed;
 public Awaiter GetAwaiter()=>new(this);
 public struct Awaiter : INotifyCompletion {
  private readonly AsyncInstantiateOperation<T> operation;
  public Awaiter(AsyncInstantiateOperation<T> op){operation=op;}
  public bool IsCompleted=>operation.isDone;
  public T[] GetResult(){if(operation.Failure!=null)System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(operation.Failure).Throw();if(operation.WasCancelled)throw new OperationCanceledException(operation.Token);return operation.Result;}
  public void OnCompleted(Action continuation){ArgumentNullException.ThrowIfNull(continuation);operation.completed+=_=>continuation();}
 }
}

internal static class AsyncInstantiationQueue {
 private static readonly List<AsyncInstantiateOperation> Operations=new();
 private static readonly List<AsyncOperation> AdaptedOperations=new();
 internal static AsyncOperation Adapt(Legacy.AsyncOperation value){if(AdaptedOperations.Count>=256)throw new InvalidOperationException("Asynchronous operation queue is full");var result=new AsyncOperation{LegacyOperation=value};AdaptedOperations.Add(result);return result;}
 internal static AsyncInstantiateOperation<T> Begin<T>(T original,int count,InstantiateParameters parameters,Vector3[] positions,Quaternion[] rotations,CancellationToken cancellationToken) where T:Object {
  if(ReferenceEquals(original,null)||!original)throw new ArgumentException("The Object you want to instantiate is null.");
  if(original.Native is not Legacy.GameObject and not Legacy.Component){Legacy.Debug.LogError("Cannot instantiate multiple objects that are not connected to GameObject, please use a loop over Instantiate");return new AsyncInstantiateOperation<T>{Unavailable=true,Token=cancellationToken};}
  if(count<1||count>Legacy.Limits.Entities)throw new ArgumentOutOfRangeException(nameof(count));
  foreach(var p in positions)Legacy.TransformMath.Finite(p);foreach(var q in rotations)Legacy.TransformMath.Normalize(q);
  if(Operations.Count>=256)throw new InvalidOperationException("Asynchronous instantiation queue is full");
  // Capture on the script thread: later edits/destruction must not alter this request.
  var snapshot=Legacy.CloneBlueprint.Capture(original.Native);
  var operation=new AsyncInstantiateOperation<T>{Token=cancellationToken};
  operation.Integrate=()=>snapshot.InstantiateBatch(count,parameters,positions,rotations).Select(Object.Wrap).ToArray();
  Operations.Add(operation);return operation;
 }
 internal static void Tick(){
  var clock=System.Diagnostics.Stopwatch.StartNew();
  foreach(var operation in Operations.OrderByDescending(value=>value.priority).ToArray()){
   if(Legacy.Time.frameCount<=operation.CreationFrame)continue;
   if(clock.Elapsed.TotalMilliseconds<GetBudget()||operation.CancelRequested||operation.Token.IsCancellationRequested)operation.Step();
   if(operation.Failure!=null&&!operation.EventRaised)Legacy.Debug.LogError("AsyncInstantiate failed: "+operation.Failure.Message);
   operation.DispatchCompleted();if(operation.EventRaised)Operations.Remove(operation);
  }
  foreach(var operation in AdaptedOperations.ToArray()){operation.DispatchCompleted();if(operation.EventRaised)AdaptedOperations.Remove(operation);}
  static float GetBudget()=>AsyncInstantiateOperation.GetIntegrationTimeMS();
 }
 internal static void Reset(){foreach(var operation in AdaptedOperations)operation.Abandon();AdaptedOperations.Clear();foreach(var operation in Operations){operation.Integrate=null;operation.CancelRequested=true;operation.WasCancelled=true;operation.Abandon();}Operations.Clear();}
}
