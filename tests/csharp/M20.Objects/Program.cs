using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;

try {
var checks = new List<string>();
void Check(string name, bool value) { if (!value) throw new Exception(name); checks.Add(name); }
void Reset(int generation) {
 RuntimeLifecycle.Reset(); Time.Reset(); Context.Generation = generation; Context.LiveScripts.Clear();
 Application.Running=true; Context.ReplaySeed=0; Context.DeterministicSpawn=false;
 var request=Context.ParseData("{\"action\":\"start\",\"entities\":[],\"keys\":[],\"mouseDelta\":[0,0],\"projectData\":{\"name\":\"M20 Objects\",\"resources\":[]}}");
 Context.Load(request); Resources.Data=Context.ParseData("[]");
 Axiom.Gameplay.SceneManagement.SceneManager.Reset(request);
 RuntimeLifecycle.Register<DestructionProbe>("Scripts/DestructionProbe.cs",()=>new DestructionProbe(),(_,_)=>{},(_,writer)=>{writer.WriteStartObject();writer.WriteEndObject();});
 DestructionProbe.Events.Clear();
}
Reset(1);
var root=new GameObject("Root");var child=new GameObject("Child");child.transform.SetParent(root.transform,false);
var first=root.transform;var second=root.transform;
Check("wrapper equality",first==second&&first.Equals(second)&&first.GetHashCode()==second.GetHashCode());
Check("parent equality",child.transform.parent==root.transform);
Check("entity/component identity separation",(Object)root!=first&&root.GetInstanceID()!=first.GetInstanceID());
var identities=new Dictionary<Object,string>{{first,"root"}};Check("dictionary lookup by native identity",identities[second]=="root");
var rootId=root.GetInstanceID();var transformId=first.GetInstanceID();
var callback=root.AddComponent<DestructionProbe>();
Object.Destroy(root);Object.Destroy(root);Object.Destroy((Object?)null);
Check("pending destroy retains objects",root&&child&&first&&callback);
RuntimeLifecycle.Update(0);RuntimeLifecycle.Late();
Check("pending destroy retains all update phases",root&&child);
RuntimeLifecycle.End();
Check("destroyed hierarchy compares null",root==null&&child==null&&first==null&&callback==null);
Check("destroyed object bool is false",!root&&!first&&!child);
Check("dictionary lookup survives destruction",identities.TryGetValue(second,out var label)&&label=="root");
Check("destroyed references retain identity",root!.GetInstanceID()==rootId&&first!.GetHashCode()==transformId);
Check("destroyed references are distinct",(Object)root!=(Object)child!);
Check("destroyed managed reference is retained",!ReferenceEquals(root,null));
Check("object.Equals null follows lifetime",root!.Equals(null));
Check("destroy callbacks once",DestructionProbe.Events.SequenceEqual(new[]{"disable","stop","destroy"}));
Object.DestroyImmediate(root);RuntimeLifecycle.End();
Check("destroy is idempotent",DestructionProbe.Events.Count==3);
var replacement=new GameObject("Replacement");var collider=replacement.AddComponent<BoxCollider>();var alias=replacement.GetComponent<BoxCollider>()!;
Check("component wrappers share identity",collider==alias);
Object.Destroy(collider);Check("component deferred",collider&&alias);
RuntimeLifecycle.End();Check("component removed without entity",collider==null&&alias==null&&replacement&&replacement.transform);
var next=replacement.AddComponent<BoxCollider>();
Check("component reattachment gets a new identity",next&&next!=collider&&next.GetInstanceID()!=collider!.GetInstanceID());
Check("removed component never revives",!alias);
Object.Destroy(replacement,.5);Time.timeScale=0;Time.Step(.25);RuntimeLifecycle.End();Check("delayed destroy pauses with scaled time",replacement);
Time.timeScale=1;Time.Step(.25);RuntimeLifecycle.End();Check("delayed destroy waits",replacement);
Object.Destroy(replacement,0);RuntimeLifecycle.End();Check("duplicate earlier deadline wins",!replacement);
var immediate=new GameObject("Immediate");Object.DestroyImmediate(immediate);Check("immediate destruction",!immediate);
var survivor=new GameObject("Survivor");var stale=survivor.transform;
Reset(2);var fresh=new GameObject("Fresh");Check("generation invalidation",!survivor&&!stale&&fresh&&fresh.GetInstanceID()!=survivor.GetInstanceID());
var reentrant=new GameObject("Reentrant");reentrant.AddComponent<DestructionProbe>();DestructionProbe.Reentrant=true;Object.DestroyImmediate(reentrant);DestructionProbe.Reentrant=false;
Check("reentrant destruction callbacks once",DestructionProbe.Events.SequenceEqual(new[]{"disable","stop","destroy"}));
DestructionProbe.Events.Clear();var callbackOwner=new GameObject("Callback owner");var callbackComponent=callbackOwner.AddComponent<DestructionProbe>();
DestructionProbe.Reentrant=true;Object.DestroyImmediate(callbackComponent);DestructionProbe.Reentrant=false;
Check("component callback may destroy its owner",!callbackOwner&&!callbackComponent&&DestructionProbe.Events.SequenceEqual(new[]{"disable","stop","destroy"}));
var identityCount=Context.ObjectIdentities.Count;var epochCount=Context.ObjectEpochs.Count;var orderCount=Context.ComponentOrders.Sum(p=>p.Value.Count);
for(var i=0;i<1000;i++){var temporary=new GameObject("Transient");_=temporary.transform;Object.DestroyImmediate(temporary);Context.Operations.Clear();}
Check("identity registries release destroyed objects",Context.ObjectIdentities.Count==identityCount&&Context.ObjectEpochs.Count==epochCount&&Context.ComponentOrders.Sum(p=>p.Value.Count)==orderCount);
var holder=new GameObject("Repeated script owner");identityCount=Context.ObjectIdentities.Count;epochCount=Context.ObjectEpochs.Count;orderCount=Context.ComponentOrders.Sum(p=>p.Value.Count);
for(var i=0;i<1000;i++){Object.DestroyImmediate(holder.AddComponent<DestructionProbe>());Context.Operations.Clear();DestructionProbe.Events.Clear();}
Check("script identities retire on repeated reattachment",Context.ObjectIdentities.Count==identityCount&&Context.ObjectEpochs.Count==epochCount&&Context.LiveScripts.Count==0&&Context.ComponentOrders.Sum(p=>p.Value.Count)==orderCount);
try{_=collider!.gameObject;Check("stale component access rejected",false);}catch(InvalidOperationException){Check("stale component access rejected",true);}
QueryCases.Run(Check,Reset);
CloneCases.Run(Check,Reset);
OrderCases.Run(Check,Reset);
TransformCases.Run(Check,Reset);
CanonicalCases.Run(Check,Reset);
PersistenceCases.Run(Check,Reset);
HierarchyActivationCases.Run(Check,Reset);
NativeReferenceCases.Run(Check,Reset);
NativeCloneReferenceCases.Run(Check,Reset);
AssetCases.Run(Check,Reset);
AsyncLifecycleCases.Run(Check,Reset);
NativeEdgeCases.Run(Check,Reset);
Reset(3201);Game.HierarchyPathLab.RunCases((key,value)=>Check(key+".native-hierarchy",value));
Reset(3301);Game.HierarchyAuxCorpus.RunCases((key,value)=>Check(key+".native-hierarchy-aux",value));
ChangeFlagInputCases.Run(Check,Reset);
Reset(3501);Game.ObjectMetaCorpus.RunCases((key,value)=>Check(key+".native-object-meta",value));
MatrixCases.Run(Check,Reset);
for(var i=0;i<NativeTransformFixtures.Count;i++){Reset(900+i);Check("exact Unity native signed hierarchy "+i,NativeTransformFixtures.Check(i));}
ManagedCloneCases.Run(Check,Reset);
MessageCases.Run(Check,Reset);
AsyncCloneCases.Run(Check,Reset);
BulkObjectCases.Run(Check,Reset);
PrimitiveCases.Run(Check,Reset);
InheritedInspectorCases.Run(Check,Reset);
AsyncOverloadCases.Run(Check,Reset);
CanonicalClockCases.Run(Check,Reset);
ScriptInstanceCases.Run(Check,Reset);
ColliderInstanceCases.Run(Check,Reset);
var sdkSources=Directory.GetFiles("engine/scripting/templates","*.cs").Where(path=>Path.GetFileName(path) is not "Host.cs" and not "Game.cs").OrderBy(path=>path,StringComparer.Ordinal).ToDictionary(Path.GetFileName,path=>Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant());
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new {passed=true,profile="native .NET / shared runtime; exact native Unity signature attribution remains separate",sdkSources,checks}));

} catch(Exception error){Console.Error.WriteLine(error);Environment.ExitCode=1;}

public sealed class DestructionProbe : MonoBehaviour {
 public static readonly List<string> Events=new();public static bool Reentrant;
 public override void OnDisable()=>Events.Add("disable");
 public override void OnStop(){Events.Add("stop");if(Reentrant)Object.DestroyImmediate(gameObject);}
 public override void OnDestroy()=>Events.Add("destroy");
}
