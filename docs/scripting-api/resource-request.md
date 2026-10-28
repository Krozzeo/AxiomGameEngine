# ResourceRequest and AsyncOperation

Resources.LoadAsync<T> resolves already-resident validated resources; it is immediately done. HTTP subclasses remain pending until host completion. completed event callbacks run when the operation completes, and newly subscribed callbacks run immediately for an already completed operation. These are not scene streaming promises.


## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed class ResourceRequest : AsyncOperation {public Object? asset {get;}internal ResourceRequest(Object? value){asset=value;}}

// Namespace: UnityEngine
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

// Namespace: Axiom.Gameplay
public class AsyncOperation {public virtual bool isDone=>true;public virtual double progress=>1;private event Action<AsyncOperation>? callbacks;public event Action<AsyncOperation> completed {add{if(isDone)value(this);else callbacks+=value;}remove{callbacks-=value;}}internal void Complete(){callbacks?.Invoke(this);callbacks=null;}}
```
