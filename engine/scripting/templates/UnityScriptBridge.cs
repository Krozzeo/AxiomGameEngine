using System.Reflection;
namespace Axiom.Gameplay {
internal sealed class UnityScriptAdapter : Script {
 internal readonly UnityEngine.MonoBehaviour Target;
 internal UnityScriptAdapter(UnityEngine.MonoBehaviour target){Target=target;target.Wrapped=this;}
 public override void OnUpdate(double deltaSeconds)=>Target.Dispatch("Update");
}
internal static class ScriptBridge {
 internal static Type NativeType(Type type)=>type==typeof(UnityEngine.Transform)?typeof(Transform):type==typeof(UnityEngine.BoxCollider)?typeof(BoxCollider):type==typeof(UnityEngine.SphereCollider)?typeof(SphereCollider):type==typeof(UnityEngine.CapsuleCollider)?typeof(CapsuleCollider):type==typeof(UnityEngine.MeshCollider)?typeof(MeshCollider):type==typeof(UnityEngine.MeshFilter)?typeof(MeshFilter):type==typeof(UnityEngine.MeshRenderer)?typeof(MeshRenderer):type;
 internal static object Value(Script script)=>script is UnityScriptAdapter adapter?adapter.Target:script;
 internal static Type Type(Script script)=>Value(script).GetType();
 internal static Type ComponentType(Component component)=>component is Script script?Type(script):component.GetType();
 internal static Script Attach(object value)=>value is Script script?script:value is UnityEngine.MonoBehaviour behaviour?new UnityScriptAdapter(behaviour):throw new ArgumentException("A Script or MonoBehaviour type is required");
 internal static void Message(Script script,string name){if(script is UnityScriptAdapter adapter){adapter.Target.Dispatch(name);return;}if(script is MonoBehaviour legacy){switch(name){case "Awake":legacy.Awake();break;case "OnEnable":legacy.OnEnable();break;case "Start":legacy.Start();break;case "FixedUpdate":legacy.FixedUpdate();break;case "LateUpdate":legacy.LateUpdate();break;case "OnDisable":legacy.OnDisable();break;case "OnDestroy":legacy.OnDestroy();break;}}}
 internal static bool Matches(Type type,Component component)=>type.IsInstanceOfType(component)||component is UnityScriptAdapter adapter&&type.IsInstanceOfType(adapter.Target)||typeof(UnityEngine.Component).IsAssignableFrom(type)&&type.IsInstanceOfType(UnityEngine.Object.Wrap(component));
 internal static T Convert<T>(Object value){if(value is UnityScriptAdapter adapter&&adapter.Target is T script)return script;if(value is T direct)return direct;return (T)(object)UnityEngine.Object.Wrap(value);}
}
}
namespace UnityEngine {
public partial class Object {
 private static readonly Dictionary<int,WeakReference<Object>> Facades=new();
 internal static void ResetFacades(){Facades.Clear();NativeObjects.Flags.Clear();}
 internal static void ForgetNative(int id){Facades.Remove(id);NativeObjects.Flags.Remove(id);}
 internal void CacheFacade(){if(Wrapped==null)return;var id=Wrapped.GetInstanceID();Facades[id]=new(this);if(Facades.Count>131072)foreach(var key in Facades.Where(pair=>!pair.Value.TryGetTarget(out _)).Select(pair=>pair.Key).ToArray())Facades.Remove(key);}
 internal static Object Wrap(Axiom.Gameplay.Object value){if(value is Axiom.Gameplay.UnityScriptAdapter adapter)return adapter.Target;var id=value.GetInstanceID();if(Facades.TryGetValue(id,out var weak)&&weak.TryGetTarget(out var cached))return cached;var result=value is Axiom.Gameplay.GameObject go?(Object)new GameObject(go,true):value is Axiom.Gameplay.Transform transform?new Transform(transform):value switch {Axiom.Gameplay.BoxCollider box=>new BoxCollider(box),Axiom.Gameplay.SphereCollider sphere=>new SphereCollider(sphere),Axiom.Gameplay.CapsuleCollider capsule=>new CapsuleCollider(capsule),Axiom.Gameplay.MeshCollider meshCollider=>new MeshCollider(meshCollider),Axiom.Gameplay.Collider collider when collider is not Axiom.Gameplay.Collider2D=>new Collider(collider),Axiom.Gameplay.MeshFilter filter=>new MeshFilter(filter),Axiom.Gameplay.MeshRenderer renderer=>new MeshRenderer(renderer),Axiom.Gameplay.Renderer renderer=>new Renderer(renderer),Axiom.Gameplay.Mesh mesh=>new Mesh(mesh),Axiom.Gameplay.TextAsset text=>new TextAsset(text),Axiom.Gameplay.Texture2D texture=>new Texture2D(texture),Axiom.Gameplay.AudioClip clip=>new AudioClip(clip),Axiom.Gameplay.Component component=>new Component(component),_=>new Object{Wrapped=value}};result.CacheFacade();return result;}
 public static implicit operator Object(Axiom.Gameplay.Object value)=>ReferenceEquals(value,null)?null!:Wrap(value);
 public static explicit operator Axiom.Gameplay.Object(Object value)=>value.Native;
}
}
