import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';
import {assessM20} from './m20-closure.mjs';
const root=new URL('../../',import.meta.url),read=async path=>JSON.parse(await readFile(new URL(path,root),'utf8'));
const ledger=await read('docs/compatibility/m20-batches.json'),source=await read('docs/compatibility/evidence/m20-sdk12-source.json'),native=await read('docs/compatibility/evidence/m20-unity-6000.3-play-observations.json'),owned=await read('docs/compatibility/evidence/m20-sdk12-native.json');
assert.equal(source.sourceContractMatches,311);assert(owned.passed);assert(!native.error);assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');
for(const [name,expected]of Object.entries(owned.sdkSources)){assert.equal(createHash('sha256').update(await readFile(new URL('engine/scripting/templates/'+name,root))).digest('hex'),expected,'Stale native SDK source '+name);}
const name='ObjectMetaCorpus.cs',hash=createHash('sha256').update(await readFile(new URL('demos/scripts/'+name,root))).digest('hex');assert.equal(native.corpusSourceHashes.find(row=>row.key===name)?.value,hash);
const nativeCases=Object.fromEntries(native.facts.filter(row=>row.key.startsWith('object-meta.')).map(row=>[row.key.slice(12),row.value]));assert.equal(Object.keys(nativeCases).length,77);assert(Object.values(nativeCases).every(value=>value==='True'));
const wasm={};for(const mode of ['development','aot']){const report=await read('docs/compatibility/evidence/m20-sdk12-objects-'+mode+'-wasm.json');assert(report.passed&&report.executedBuild.mode===mode);assert.equal(report.corpusSourceHashes[name],hash);assert.equal(Object.keys(report.cases).length,77);assert(Object.values(report.cases).every(value=>value==='pass'));wasm[mode]=report;}
const profiles=[
 [
  "UnityEngine.Object",
  "public class UnityEngine.Object",
  [
   "type.object"
  ],
  "Public nonabstract nonsealed managed Object derives directly from System.Object."
 ],
 [
  "UnityEngine.Object",
  "public UnityEngine.Object();",
  [
   "ctor.object-null-like",
   "ctor.object-id",
   "ctor.object-string",
   "ctor.object-name",
   "ctor.object-flags"
  ],
  "Default construction retains a managed reference with no native handle: false/null-like equality, zero IDs/hash, null string and exact NullReferenceException for native properties."
 ],
 [
  "UnityEngine.Object",
  "public int GetInstanceID();",
  [
   "ctor.object-id",
   "id.live",
   "id.wrapper",
   "id.component-distinct",
   "id.other-distinct",
   "id.retained"
  ],
  "Live owner wrappers share identity; distinct live native objects/components differ; unbound identity is zero and retired objects retain their instance ID."
 ],
 [
  "UnityEngine.Object",
  "public UnityEngine.EntityId GetEntityId();",
  [
   "entity-id.wrapper",
   "entity-id.component",
   "entity-id.retained"
  ],
  "EntityId is stable across owner wrappers, distinct for a component, and retained after destruction."
 ],
 [
  "UnityEngine.Object",
  "public int GetHashCode();",
  [
   "ctor.object-id",
   "hash.live",
   "id.retained"
  ],
  "Hash follows stable native instance identity, including zero unbound handles and retained destroyed objects."
 ],
 [
  "UnityEngine.Object",
  "public string ToString();",
  [
   "ctor.object-string",
   "string.object",
   "string.component",
   "string.retired"
  ],
  "Live GameObject/Transform wrappers format current owner name and qualified type; unbound and retired wrappers return null text."
 ],
 [
  "UnityEngine.Object",
  "public bool Equals(object other);",
  [
   "equal.wrapper",
   "equal.foreign",
   "equal.non-object",
   "equal.live-null",
   "equal.retired-null",
   "equal.retired-live",
   "equal.unbound"
  ],
  "Equality follows native handle identity and fake-null semantics for managed wrappers, null, strings, foreign objects, unbound objects and retired handles."
 ],
 [
  "UnityEngine.Object",
  "public static bool op_Implicit(UnityEngine.Object exists);",
  [
   "bool.actual-null",
   "bool.live",
   "bool.retired",
   "ctor.object-null-like",
   "ctor.component-null-like"
  ],
  "Implicit bool distinguishes live native handles from actual null, unbound objects/components and retired owner/component wrappers."
 ],
 [
  "UnityEngine.Object",
  "public static bool op_Equality(UnityEngine.Object x, UnityEngine.Object y);",
  [
   "equal.wrapper",
   "equal.foreign",
   "equal.live-null",
   "equal.retired-null",
   "equal.unbound"
  ],
  "Operator equality follows native identity with null-like unbound and retired handles."
 ],
 [
  "UnityEngine.Object",
  "public static bool op_Inequality(UnityEngine.Object x, UnityEngine.Object y);",
  [
   "equal.foreign",
   "equal.live-null",
   "equal.retired-null",
   "equal.retired-live"
  ],
  "Inequality is the native equality complement for live foreign, actual null and destroyed wrappers."
 ],
 [
  "UnityEngine.Object",
  "public string name { public get; public set; }",
  [
   "name.component",
   "name.empty",
   "name.null",
   "name.unicode",
   "ctor.object-name",
   "name.retired",
   "name.retired-set"
  ],
  "GameObject and component names share their native owner; empty/null/unicode assignments round-trip, unbound access throws NullReferenceException and retired get/set throws MissingReferenceException."
 ],
 [
  "UnityEngine.Object",
  "public UnityEngine.HideFlags hideFlags { public get; public set; }",
  [
   "flags.owner",
   "flags.component",
   "flags.child",
   "flags.added",
   "flags.independent",
   "flags.reset-owner",
   "ctor.object-flags",
   "flags.retired",
   "flags.retired-set"
  ],
  "Owner flag updates propagate to its existing/new components, not child objects; component flags can differ until owner reset; unbound and retired access retain exact native exceptions. HideInHierarchy, NotEditable and None are covered; other flag effects are outside this profile."
 ],
 [
  "UnityEngine.Component",
  "public class UnityEngine.Component",
  [
   "type.component"
  ],
  "Public nonabstract nonsealed Component derives from UnityEngine.Object."
 ],
 [
  "UnityEngine.Component",
  "public UnityEngine.Component();",
  [
   "ctor.component-null-like",
   "ctor.component-owner",
   "ctor.component-transform"
  ],
  "Default Component construction retains an unbound null-like managed object; native owner/transform access throws exact NullReferenceException."
 ],
 [
  "UnityEngine.Component",
  "public UnityEngine.GameObject gameObject { public get; }",
  [
   "component.owner",
   "ctor.component-owner",
   "component.owner-retired"
  ],
  "Live component owner retains identity; unbound and destroyed access throw the respective native exceptions."
 ],
 [
  "UnityEngine.Component",
  "public UnityEngine.Transform transform { public get; }",
  [
   "component.transform",
   "ctor.component-transform",
   "component.transform-retired"
  ],
  "Transform component resolves itself as the owner transform and rejects unbound/retired native access."
 ],
 [
  "UnityEngine.Component",
  "public string tag { public get; public set; }",
  [
   "component.tag-default",
   "component.tag-set"
  ],
  "Component tag proxies its live owner, with default Untagged and assignment to registered Player. Undefined tag behavior is outside this profile."
 ],
 [
  "UnityEngine.Component",
  "public bool CompareTag(string tag);",
  [
   "component.tag-string"
  ],
  "Live component compares the registered Player and Untagged tags against its owner."
 ],
 [
  "UnityEngine.Component",
  "public bool CompareTag(UnityEngine.TagHandle tag);",
  [
   "component.tag-handle"
  ],
  "Live component compares a TagHandle obtained from the registered Player tag."
 ],
 [
  "UnityEngine.Component",
  "public int GetComponentIndex();",
  [
   "index.component",
   "index.added-slot",
   "index.component-retired"
  ],
  "Live Transform and added BoxCollider return their native attachment slots; retired component access throws MissingReferenceException. Repeated same-type attachments remain a separate unaccepted contract."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.GameObject gameObject { public get; }",
  [
   "game-object.self",
   "game-object.self-retired"
  ],
  "GameObject.gameObject returns the same managed wrapper, including after native destruction."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.Transform transform { public get; }",
  [
   "game-object.transform",
   "game-object.transform-retired"
  ],
  "Live owner resolves its Transform identity; destroyed owner access throws MissingReferenceException."
 ],
 [
  "UnityEngine.GameObject",
  "public string tag { public get; public set; }",
  [
   "component.tag-default",
   "component.tag-set"
  ],
  "GameObject tag round-trips default Untagged and registered Player, shared with component proxy. Undefined tags are outside this profile."
 ],
 [
  "UnityEngine.GameObject",
  "public bool CompareTag(string tag);",
  [
   "game-object.tag-string"
  ],
  "Live GameObject compares registered Player and Untagged strings."
 ],
 [
  "UnityEngine.GameObject",
  "public bool CompareTag(UnityEngine.TagHandle tag);",
  [
   "game-object.tag-handle"
  ],
  "Live GameObject compares the registered Player TagHandle."
 ],
 [
  "UnityEngine.GameObject",
  "public int GetComponentCount();",
  [
   "index.count",
   "index.added-count",
   "index.count-retired"
  ],
  "Default Transform plus one BoxCollider counts live slots and rejects retired owners; duplicate-type storage is outside this profile."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.Component GetComponentAtIndex(int index);",
  [
   "index.at",
   "index.negative",
   "index.high",
   "index.added-slot",
   "index.at-retired"
  ],
  "Index zero returns the Transform; negative/high indices throw ArgumentOutOfRangeException, destroyed owner throws MissingReferenceException; tested default and one-collider layouts."
 ],
 [
  "UnityEngine.GameObject",
  "public T GetComponentAtIndex<T>(int index);",
  [
   "index.generic",
   "index.wrong-type"
  ],
  "Generic slot lookup returns the same Transform for a compatible type and throws InvalidCastException for incompatible BoxCollider. Non-generic bounds/retirement are checked separately."
 ],
 [
  "UnityEngine.GameObject",
  "public int GetComponentIndex(UnityEngine.Component component);",
  [
   "index.owner",
   "index.foreign",
   "index.null",
   "index.added-slot",
   "index.null-retired"
  ],
  "Returns the owner Transform/BoxCollider slot, -1 for foreign/null live arguments, and MissingReferenceException on a destroyed receiver even when argument is null."
 ]
];
const records=[];for(const [owner,declaration,caseIds,contract]of profiles){const matches=ledger.records.filter(row=>row.owner===owner&&row.declaration===declaration);assert.equal(matches.length,1);const row=matches[0];assert(source.records.find(record=>record.id===row.id)?.sourceMatched);for(const id of caseIds){assert.equal(nativeCases[id],'True');assert(owned.checks.includes(id+'.native-object-meta'));for(const report of Object.values(wasm))assert.equal(report.cases[id],'pass');}
 row.status='certified-behavior';row.unitySourceCertified=true;row.unityBehaviorSignatureCertified=true;row.acceptance={contract,native:{passed:true,evidence:'evidence/m20-unity-6000.3-play-observations.json',ownedEvidence:'evidence/m20-sdk12-native.json',caseIds},wasm:{passed:true,evidence:'evidence/m20-sdk12-objects-development-wasm.json',aotEvidence:'evidence/m20-sdk12-objects-aot-wasm.json',caseIds}};delete row.remaining;records.push({id:row.id,owner:row.owner,declaration:row.declaration,caseIds,passed:true});}
ledger.certifiedUnitySignatures=ledger.records.filter(row=>row.unitySourceCertified&&row.unityBehaviorSignatureCertified&&row.acceptance?.native?.passed&&row.acceptance?.wasm?.passed).length;
const inventory=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);assessM20(ledger,inventory);
for(const [path,value]of [['docs/compatibility/m20-batches.json',ledger],['docs/compatibility/evidence/m20-object-meta-native-acceptance.json',{schemaVersion:1,passed:true,referenceCoreModuleSha256:native.coreAssemblySha256,corpusSourceSha256:hash,acceptedSignatures:records.length,records}]])await writeFile(new URL(path,root),JSON.stringify(value,null,2)+'\n');
console.log(JSON.stringify({acceptedObjectMetaSignatures:profiles.length,totalAccepted:ledger.certifiedUnitySignatures,targets:311}));
