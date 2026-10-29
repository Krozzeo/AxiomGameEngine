# Coroutine

Namespace: `Axiom.Gameplay`

Coroutine handles and supported yield waits. See MonoBehaviour for scheduling, cancellation and limits.


## Implemented declarations

```csharp
// Namespace: Axiom.Gameplay
public sealed class Coroutine {
 internal readonly Script Owner;internal readonly IEnumerator Routine;internal readonly Stack<IEnumerator> Stack=new();internal object? Waiting;internal double Until;internal int Frame;internal bool Done;
 internal Coroutine(Script owner,IEnumerator routine){Owner=owner;Routine=routine;Stack.Push(routine);}
}

// Namespace: Axiom.Gameplay
public sealed class WaitForSeconds {internal double Duration;public WaitForSeconds(double seconds){if(!double.IsFinite(seconds)||seconds<0)throw new ArgumentException("Invalid wait");Duration=seconds;}}

// Namespace: Axiom.Gameplay
public sealed class WaitForSecondsRealtime {public double waitTime;public WaitForSecondsRealtime(double seconds){if(!double.IsFinite(seconds)||seconds<0)throw new ArgumentException("Invalid wait");waitTime=seconds;}}

// Namespace: Axiom.Gameplay
public sealed class WaitUntil {internal Func<bool> Test;public WaitUntil(Func<bool> predicate){Test=predicate??throw new ArgumentNullException(nameof(predicate));}}

// Namespace: Axiom.Gameplay
public sealed class WaitWhile {internal Func<bool> Test;public WaitWhile(Func<bool> predicate){Test=predicate??throw new ArgumentNullException(nameof(predicate));}}

// Namespace: Axiom.Gameplay
public sealed class WaitForFixedUpdate {}

// Namespace: Axiom.Gameplay
public sealed class WaitForEndOfFrame {}
```
