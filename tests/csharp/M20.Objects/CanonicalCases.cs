using Legacy=Axiom.Gameplay;
using U=UnityEngine;
public static class CanonicalCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(30);var canonical=new U.GameObject("Canonical");
  check("canonical public inheritance matches source contracts",typeof(U.Component).BaseType==typeof(U.Object)&&typeof(U.GameObject).BaseType==typeof(U.Object)&&typeof(U.Transform).BaseType==typeof(U.Component)&&typeof(U.Behaviour).BaseType==typeof(U.Component)&&typeof(U.MonoBehaviour).BaseType==typeof(U.Behaviour));
  check("canonical new object uses shared native scene",Legacy.GameObject.Find("Canonical")?.GetInstanceID()==canonical.GetInstanceID());
  var legacy=Legacy.GameObject.Find("Canonical")!;
  check("canonical and legacy wrappers have shared identity",canonical==legacy&&canonical.GetHashCode()==legacy.GetHashCode());
  U.Component component=canonical.transform;U.Object handle=component;
  check("canonical inheritance and native identity",handle&&component is U.Component&&component.GetInstanceID()==legacy.transform.GetInstanceID());
  canonical.transform.localPosition=new(1,2,3);float x=canonical.transform.position.x;
  check("canonical float vector writes legacy pose",x==1f&&legacy.transform.position==new Legacy.Vector3(1,2,3));
  check("canonical component query returns shared facade",canonical.GetComponent<U.Transform>()==legacy.transform&&canonical.GetComponents<U.Component>().Length==1);
  canonical.transform.Rotate(90f,90f,0f);check("canonical Euler follows ZXY",(canonical.transform.forward-U.Vector3.down).magnitude<1e-5f);
  Legacy.RuntimeLifecycle.Register<CanonicalProbe>("Scripts/CanonicalProbe.cs",()=>new CanonicalProbe(),(s,v)=>Legacy.ScriptValues.Apply(typeof(CanonicalProbe),s,v),(s,w)=>Legacy.ScriptValues.Write(typeof(CanonicalProbe),s,w,new[]{"Peer","Offset"}));
  var script=canonical.AddComponent<CanonicalProbe>();Legacy.RuntimeLifecycle.StartAll();Legacy.RuntimeLifecycle.Update(.1);
  check("canonical private lifecycle messages execute once",script.AwakeCalls==1&&script.StartCalls==1&&script.UpdateCalls==1);
  check("canonical and legacy script queries interoperate",ReferenceEquals(legacy.GetComponent<CanonicalProbe>(),script)&&canonical.GetComponent<Legacy.Script>()?.GetInstanceID()==script.GetInstanceID());
  var nativeScript=(Legacy.Script)script.Native;var factory=Legacy.RuntimeLifecycle.Factories[typeof(CanonicalProbe)];factory.Apply(nativeScript,Legacy.Context.ParseData("{\"Peer\":{\"entityId\":\""+legacy.Entity.Id+"\"},\"Offset\":[2,3,4]}"));
  check("canonical serialized references and floats hydrate through shared host",script.Peer==canonical&&script.Offset==new U.Vector3(2,3,4));
  using(var stream=new MemoryStream()){using(var writer=new System.Text.Json.Utf8JsonWriter(stream))factory.Write(nativeScript,writer);var snapshot=System.Text.Json.JsonDocument.Parse(stream.ToArray()).RootElement;check("canonical field snapshots retain native scene references",snapshot.GetProperty("Peer").GetProperty("entityId").GetString()==legacy.Entity.Id&&snapshot.GetProperty("Offset")[1].GetSingle()==3f);}
  Legacy.ScriptValues.Apply(typeof(CanonicalProbe),script,Legacy.Context.ParseData("{\"Any\":{\"entityId\":\""+legacy.Entity.Id+"\"}}"));
  check("canonical base Object reference retains GameObject kind",script.Any is U.GameObject&&script.Any==canonical);
  script.Any=null;using(var stream=new MemoryStream()){using(var writer=new System.Text.Json.Utf8JsonWriter(stream))Legacy.ScriptValues.Write(typeof(CanonicalProbe),script,writer,new[]{"Any"});check("canonical null reference serializes as JSON null",System.Text.Json.JsonDocument.Parse(stream.ToArray()).RootElement.GetProperty("Any").ValueKind==System.Text.Json.JsonValueKind.Null);}
  var oldProbe=legacy.AddComponent<DestructionProbe>();var fields=new LegacyReferenceProbe();Legacy.ScriptValues.Apply(typeof(LegacyReferenceProbe),fields,Legacy.Context.ParseData("{\"Target\":{\"entityId\":\""+legacy.Entity.Id+"\"},\"Other\":{\"entityId\":\""+legacy.Entity.Id+"\",\"path\":\"Scripts/DestructionProbe.cs\"}}"));
  check("legacy component and script references hydrate after facade separation",fields.Target==legacy.transform&&ReferenceEquals(fields.Other,oldProbe));Legacy.Object.DestroyImmediate(oldProbe);DestructionProbe.Events.Clear();
  var clone=U.Object.Instantiate(canonical);check("canonical cloning preserves shared identity and script type",clone&&clone.GetComponent<CanonicalProbe>()!=null&&clone!=canonical);
  U.Object.Destroy(canonical);check("canonical deferred destroy retains handle",canonical);Legacy.RuntimeLifecycle.End();
  check("canonical destroyed wrapper compares null",!canonical&&canonical==null&&legacy==null&&script==null);
  check("canonical destruction callbacks run once",script!.DisableCalls==1&&script.DestroyCalls==1);
  U.Object.DestroyImmediate(clone);
  var cachedOwner=new U.GameObject("Cache owner");var cachedScript=cachedOwner.AddComponent<CanonicalProbe>();var cachedId=cachedScript.GetInstanceID();cachedScript.hideFlags=U.HideFlags.HideInInspector;U.Object.DestroyImmediate(cachedScript);check("removed canonical script releases its flag cache",!U.NativeObjects.Flags.ContainsKey(cachedId));
  cachedOwner.hideFlags=U.HideFlags.DontSave;reset(33);check("runtime reset releases canonical facade and flag state",U.NativeObjects.Flags.Count==0&&!cachedOwner);
 }
}
public sealed class CanonicalProbe : UnityEngine.MonoBehaviour {
 public UnityEngine.Object? Any;public UnityEngine.GameObject? Peer;public UnityEngine.Vector3 Offset;
 public int AwakeCalls,StartCalls,UpdateCalls,DisableCalls,DestroyCalls;
 private void Awake(){AwakeCalls++;}private void Start(){StartCalls++;}private void Update(){UpdateCalls++;}private void OnDisable(){DisableCalls++;}private void OnDestroy(){DestroyCalls++;}
}

public sealed class LegacyReferenceProbe {public Axiom.Gameplay.Transform? Target;public DestructionProbe? Other;}
