using System.Collections;
using System.Runtime.InteropServices;
namespace Unity.Collections;
public enum Allocator {Invalid=0,None=1,Temp=2,TempJob=3,Persistent=4,AudioKernel=5}
public enum NativeArrayOptions {UninitializedMemory=0,ClearMemory=1}

// Pinned contiguous storage, shared by struct copies. Disposal invalidates aliases.
public struct NativeArray<T> : IDisposable,IEnumerable<T> where T:struct {
 private sealed class Storage {
  internal T[]? Values;private GCHandle pin;
  internal Storage(int length){Values=new T[length];pin=GCHandle.Alloc(Values,GCHandleType.Pinned);}
  internal void Release(){if(Values==null)throw new ObjectDisposedException(nameof(NativeArray<T>));Values=null;if(pin.IsAllocated)pin.Free();GC.SuppressFinalize(this);}
  ~Storage(){if(pin.IsAllocated)pin.Free();}
 }
 private readonly Storage? storage;
 public NativeArray(int length,Allocator allocator,NativeArrayOptions options=NativeArrayOptions.ClearMemory){
  if(length<0||length>1048576)throw new ArgumentOutOfRangeException(nameof(length));
  if(allocator is not (Allocator.Temp or Allocator.TempJob or Allocator.Persistent or Allocator.AudioKernel))throw new ArgumentException("A storage allocator is required",nameof(allocator));
  if(!Enum.IsDefined(options))throw new ArgumentOutOfRangeException(nameof(options));
  if(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<T>())throw new ArgumentException("NativeArray elements must not contain managed references");
  storage=new Storage(length);
 }
 public NativeArray(T[] array,Allocator allocator):this(array?.Length??throw new ArgumentNullException(nameof(array)),allocator){array.CopyTo(Values,0);}
 public NativeArray(NativeArray<T> array,Allocator allocator):this(array.ToArray(),allocator){}
 private readonly T[] Values=>storage?.Values??throw new ObjectDisposedException(nameof(NativeArray<T>));
 public readonly int Length=>Values.Length;
 public readonly bool IsCreated=>storage?.Values!=null;
 public readonly T this[int index] {get=>Values[index];set=>Values[index]=value;}
 public readonly T[] ToArray()=>(T[])Values.Clone();
 public readonly void CopyFrom(T[] array){ArgumentNullException.ThrowIfNull(array);if(array.Length!=Length)throw new ArgumentException("Array sizes must match");array.CopyTo(Values,0);}
 public readonly void CopyTo(T[] array){ArgumentNullException.ThrowIfNull(array);if(array.Length!=Length)throw new ArgumentException("Array sizes must match");Values.CopyTo(array,0);}
 public readonly void Dispose(){if(storage==null)throw new ObjectDisposedException(nameof(NativeArray<T>));storage.Release();}
 public readonly IEnumerator<T> GetEnumerator(){var values=Values;for(var i=0;i<values.Length;i++){_=Values;yield return values[i];}}
 readonly IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
}
