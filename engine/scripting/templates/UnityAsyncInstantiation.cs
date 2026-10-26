using System.Runtime.CompilerServices;
using System.Threading;
using Legacy=Axiom.Gameplay;
namespace UnityEngine;

public class YieldInstruction {}
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

public class AsyncInstantiateOperation : AsyncOperation {
 private static float integrationTime=2;
 internal Func<Object[]>? Integrate;
 internal CancellationToken Token;internal bool CancelRequested,WasCancelled;
 internal Object[]? Objects;internal Exception? Failure;
 public Object[] Result=>Objects!;
 public override float progress=>Done?1:Integrate!=null?.9f:0;
 public void Cancel(){if(!Done)CancelRequested=true;}
 public bool IsWaitingForSceneActivation()=>!Done&&!allowSceneActivation&&!CancelRequested&&!Token.IsCancellationRequested&&Integrate!=null;
 public void WaitForCompletion(){if(!Done){allowSceneActivation=true;Step();}if(Failure!=null)System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(Failure).Throw();}
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
 internal static AsyncInstantiateOperation<T> Begin<T>(T original,int count,InstantiateParameters parameters,Vector3[] positions,Quaternion[] rotations,CancellationToken cancellationToken) where T:Object {
  ArgumentNullException.ThrowIfNull(original);if(!original)throw new MissingReferenceException("Cannot clone a destroyed object");
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
  static float GetBudget()=>AsyncInstantiateOperation.GetIntegrationTimeMS();
 }
 internal static void Reset(){foreach(var operation in Operations){operation.Integrate=null;operation.CancelRequested=true;operation.WasCancelled=true;operation.Abandon();}Operations.Clear();}
}
