import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';
import {assessM20} from './m20-closure.mjs';
const root=new URL('../../',import.meta.url),read=async path=>JSON.parse(await readFile(new URL(path,root),'utf8'));
const ledger=await read('docs/compatibility/m20-batches.json'),source=await read('docs/compatibility/evidence/m20-sdk12-source.json'),native=await read('docs/compatibility/evidence/m20-unity-gameobject-state-observations.json'),owned=await read('docs/compatibility/evidence/m20-sdk12-gameobject-state-native.json');
assert.equal(source.sourceContractMatches,311);assert(owned.passed);assert(!native.error);assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');
const name='GameObjectStateCorpus.cs',hash=createHash('sha256').update(await readFile(new URL('demos/scripts/'+name,root))).digest('hex');assert.equal(native.sourceSha256,hash);
const nativeCases=Object.fromEntries(native.facts.map(row=>[row.key,row.value]));assert.equal(Object.keys(nativeCases).length,53);assert.equal(owned.checks,53);assert.equal(owned.sourceSha256,hash);assert.deepEqual(owned.mismatches,[]);for(const [sourceName,expected]of Object.entries(owned.sdkSources))assert.equal(createHash('sha256').update(await readFile(new URL('engine/scripting/templates/'+sourceName,root))).digest('hex'),expected,'Stale native SDK '+sourceName);
const wasm={};for(const mode of ['development','aot']){const report=await read('docs/compatibility/evidence/m20-sdk12-gameobject-state-'+mode+'-wasm.json');assert(report.passed&&report.executedBuild.mode===mode);assert.equal(report.corpusSourceHashes[name],hash);assert.equal(Object.keys(report.cases).length,53);assert(Object.values(report.cases).every(value=>value==='pass'));wasm[mode]=report;}
const profiles=[
 [
  "UnityEngine.GameObject",
  "public class UnityEngine.GameObject",
  [
   "type"
  ],
  "Public nonabstract sealed GameObject derives from Object, with exact compiled source shape."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.GameObject();",
  [
   "ctor.default",
   "ctor.default-components"
  ],
  "Default construction creates New Game Object with one Transform."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.GameObject(string name);",
  [
   "ctor.null-name",
   "ctor.empty-name",
   "ctor.named"
  ],
  "Named construction retains empty/unicode strings and maps a null name to New Game Object, within the documented name budget."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.GameObject(string name, System.Type[] components);",
  [
   "ctor.components",
   "ctor.null-array",
   "ctor.null-array-created"
  ],
  "The component-array constructor attaches one BoxCollider beside the mandatory Transform. Null component arrays throw NullReferenceException after creating the named object, matching native partial-construction visibility. Other component types and duplicate storage remain separate dependencies."
 ],
 [
  "UnityEngine.GameObject",
  "public int layer { public get; public set; }",
  [
   "layer.default",
   "layer.high",
   "layer.child",
   "layer.negative",
   "layer.after-negative",
   "layer.outside",
   "layer.after-outside",
   "retired.layer"
  ],
  "Layer defaults to0, accepts31 independently of child layers, leaves31 unchanged on negative/32 assignments without throwing, and rejects retired reads. Logger implementation and broader physics filtering are separate dependencies."
 ],
 [
  "UnityEngine.GameObject",
  "public bool isStatic { public get; public set; }",
  [
   "static.default",
   "static.set",
   "static.child",
   "static.reset",
   "retired.static"
  ],
  "Static state defaults false, can be set/reset independently of children and rejects retired reads; static batching/baking are outside this property profile."
 ],
 [
  "UnityEngine.GameObject",
  "public bool activeSelf { public get; }",
  [
   "active.initial",
   "active.parent-off",
   "active.child-retained",
   "active.recursive-off",
   "retired.active-self"
  ],
  "Local activation remains independent of inherited inactivity and rejects retired reads."
 ],
 [
  "UnityEngine.GameObject",
  "public bool activeInHierarchy { public get; }",
  [
   "active.initial",
   "active.parent-off",
   "active.child-retained",
   "active.recursive-on",
   "retired.active-hierarchy"
  ],
  "Hierarchy activation follows local and ancestor flags through three levels and rejects retired reads."
 ],
 [
  "UnityEngine.GameObject",
  "public bool active { public get; public set; }",
  [
   "active.alias"
  ],
  "Deprecated activation alias reads/sets the local owner state without overwriting a disabled child flag."
 ],
 [
  "UnityEngine.GameObject",
  "public void SetActive(bool value);",
  [
   "active.parent-off",
   "active.child-retained",
   "retired.set-active"
  ],
  "Local activation affects inherited visibility while retaining descendant activeSelf flags; retired receivers reject. Callback ordering remains a separate lifecycle dependency."
 ],
 [
  "UnityEngine.GameObject",
  "public void SetActiveRecursively(bool state);",
  [
   "active.recursive-on",
   "active.recursive-off",
   "active.alias"
  ],
  "Deprecated recursive activation sets all local flags across three levels, independent of previous ancestor inactivity; lifecycle callback ordering is separate."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.TransformHandle transformHandle { public get; }",
  [
   "handle.owner",
   "handle.component",
   "handle.foreign",
   "handle.default",
   "retired.handle"
  ],
  "Opaque TransformHandle equality identifies the same owner transform, differs from foreign/default handles and rejects retired owners; binary layout/unsafe external interop are outside this profile."
 ],
 [
  "UnityEngine.Component",
  "public UnityEngine.TransformHandle transformHandle { public get; }",
  [
   "handle.component",
   "handle.unbound",
   "retired.component-handle"
  ],
  "Component transform handle equals the owner handle; unbound and retired reads retain exact native exception types."
 ],
 [
  "UnityEngine.GameObject",
  "public UnityEngine.SceneManagement.Scene scene { public get; }",
  [
   "scene.initial",
   "scene.moved",
   "scene.returned",
   "retired.scene"
  ],
  "Scene ownership follows explicit root movement together with nested descendants and returns to the initial scene; retired receivers reject. Broader SceneManager APIs remain separate."
 ],
 [
  "UnityEngine.GameObject",
  "public ulong sceneCullingMask { public get; }",
  [
   "scene.mask",
   "scene.mask-moved",
   "retired.mask"
  ],
  "Default and ordinary created runtime scenes expose the observed native mask16140901064495857664; retired receivers reject. Editor preview/custom culling scenes are outside this profile."
 ],
 [
  "UnityEngine.GameObject",
  "public static UnityEngine.SceneManagement.Scene GetScene(int instanceID);",
  [
   "scene.static-int",
   "scene.component-int",
   "scene.zero-int",
   "retired.get-int"
  ],
  "Deprecated int lookup resolves live GameObject ownership, returns invalid for component/zero IDs and no longer resolves a destroyed object."
 ],
 [
  "UnityEngine.GameObject",
  "public static UnityEngine.SceneManagement.Scene GetScene(UnityEngine.EntityId entityId);",
  [
   "scene.static-entity",
   "scene.component-entity",
   "scene.zero-entity",
   "retired.get-entity"
  ],
  "EntityId lookup resolves live GameObject ownership, returns invalid for component/default IDs and no longer resolves a destroyed object."
 ]
];
const records=[];for(const [owner,declaration,caseIds,contract]of profiles){const matches=ledger.records.filter(row=>row.owner===owner&&row.declaration===declaration);assert.equal(matches.length,1);const row=matches[0];assert(source.records.find(record=>record.id===row.id)?.sourceMatched);for(const id of caseIds){assert(Object.hasOwn(nativeCases,id));assert.equal(owned.facts[id],nativeCases[id]);for(const report of Object.values(wasm))assert.equal(report.cases[id],'pass');}
 row.status='certified-behavior';row.unitySourceCertified=true;row.unityBehaviorSignatureCertified=true;row.acceptance={contract,native:{passed:true,evidence:'evidence/m20-unity-gameobject-state-observations.json',ownedEvidence:'evidence/m20-sdk12-gameobject-state-native.json',caseIds},wasm:{passed:true,evidence:'evidence/m20-sdk12-gameobject-state-development-wasm.json',aotEvidence:'evidence/m20-sdk12-gameobject-state-aot-wasm.json',caseIds}};delete row.remaining;records.push({id:row.id,owner:row.owner,declaration:row.declaration,caseIds,passed:true});}
ledger.certifiedUnitySignatures=ledger.records.filter(row=>row.unitySourceCertified&&row.unityBehaviorSignatureCertified&&row.acceptance?.native?.passed&&row.acceptance?.wasm?.passed).length;
const inventory=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);assessM20(ledger,inventory);
for(const [path,value]of [['docs/compatibility/m20-batches.json',ledger],['docs/compatibility/evidence/m20-gameobject-state-native-acceptance.json',{schemaVersion:1,passed:true,referenceCoreModuleSha256:native.coreAssemblySha256,corpusSourceSha256:hash,acceptedSignatures:records.length,records}]])await writeFile(new URL(path,root),JSON.stringify(value,null,2)+'\n');
console.log(JSON.stringify({acceptedGameObjectStateSignatures:profiles.length,totalAccepted:ledger.certifiedUnitySignatures,targets:311}));
