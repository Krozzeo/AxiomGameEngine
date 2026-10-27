import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';
import {assessM20} from './m20-closure.mjs';
const root=new URL('../../',import.meta.url),read=async path=>JSON.parse(await readFile(new URL(path,root),'utf8')),hash=bytes=>createHash('sha256').update(bytes).digest('hex');
const ledger=await read('docs/compatibility/m20-batches.json'),source=await read('docs/compatibility/evidence/m20-sdk12-source.json'),catalog=await read('docs/compatibility/evidence/m20-async-overload-cases.json'),native=await read('docs/compatibility/evidence/m20-unity-6000.3-play-observations.json'),owned=await read('docs/compatibility/evidence/m20-sdk12-native.json');
assert.equal(source.sourceContractMatches,311);assert(owned.passed);assert(!native.error);assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');
const facts=Object.fromEntries(native.facts.map(row=>[row.key,row.value]));assert.equal(facts['async.lifecycle.failed'],'0');assert.equal(facts['async.lifecycle.passed'],'137');const nativeCases=Object.fromEntries(facts['async.lifecycle.report'].split(';').filter(Boolean).map(value=>value.split(':')));
const wasm={};for(const mode of ['development','aot']){const value=await read('docs/compatibility/evidence/m20-sdk12-assets-'+mode+'-wasm.json');assert(value.passed&&value.executedBuild.mode===mode);assert.equal(Object.keys(value.lifecycleCases).length,137);wasm[mode]=value;}
const corpusSourceHashes={};for(const name of ['AsyncOverloadCatalog','AsyncLifecycleLab','AsyncLifecycleProbe'])corpusSourceHashes['demos/scripts/'+name+'.cs']=hash(await readFile(new URL('demos/scripts/'+name+'.cs',root)));
const nativeHashes=Object.fromEntries(native.corpusSourceHashes.map(row=>[row.key,row.value]));for(const [path,expected]of Object.entries(corpusSourceHashes)){const name=path.split('/').at(-1);assert.equal(nativeHashes[name],expected,'Stale native corpus '+name);for(const value of Object.values(wasm))assert.equal(value.corpusSourceHashes[name],expected,'Stale Wasm corpus '+name);}
const suffixes=['held','cancelled','cancelled-completed','positive','null','initialize','completed','destroyed'];assert.equal(catalog.records.length,17);
const records=[];
for(const entry of catalog.records){
 const key=entry.label.replace('async.overload.','a'),keys=suffixes.map(suffix=>key+'.'+suffix),row=ledger.records.find(row=>row.id===entry.id);assert(row&&row.declaration===entry.declaration);assert(source.records.find(item=>item.id===entry.id)?.sourceMatched);
 assert.equal(facts['reference-async.'+entry.id+'.null'],'True');assert(facts['reference-async.'+entry.id+'.x']);
 for(const name of keys){assert.equal(nativeCases[name],'pass');assert(owned.checks.includes(name+'.shared-lifecycle'));for(const value of Object.values(wasm))assert.equal(value.lifecycleCases[name],'pass');}
 const contract='Exact direct overload: parented-source pose/count/identity, ArgumentException for null input, cancellation with one completion callback and no Awake, activation hold, one Awake/OnEnable after release, one completion callback, and actual result destruction/OnDisable/OnDestroy. The identical lifecycle corpus runs in native Unity, native Axiom and both Wasm modes. Non-scene-object async rejection follows the native negative reference; synchronous asset cloning is a separate dependency.';
 row.status='certified-behavior';row.unitySourceCertified=true;row.unityBehaviorSignatureCertified=true;row.acceptance={contract,native:{passed:true,evidence:'evidence/m20-unity-6000.3-play-observations.json',ownedEvidence:'evidence/m20-sdk12-native.json',caseIds:keys},wasm:{passed:true,evidence:'evidence/m20-sdk12-assets-development-wasm.json',aotEvidence:'evidence/m20-sdk12-assets-aot-wasm.json',caseIds:keys}};delete row.remaining;
 entry.contract=contract;entry.certified=true;records.push({id:entry.id,label:entry.label,caseIds:keys,nativeWorldX:facts['reference-async.'+entry.id+'.x'].split(',').map(Number),passed:true});
}
catalog.certified=true;ledger.certifiedUnitySignatures=ledger.records.filter(row=>row.unitySourceCertified===true&&row.unityBehaviorSignatureCertified===true&&row.acceptance?.native?.passed===true&&row.acceptance?.wasm?.passed===true).length;
const inventory=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);assessM20(ledger,inventory);
for(const [path,value]of [['docs/compatibility/m20-batches.json',ledger],['docs/compatibility/evidence/m20-async-overload-cases.json',catalog],['docs/compatibility/evidence/m20-async-native-acceptance.json',{schemaVersion:1,passed:true,referenceCoreModuleSha256:native.coreAssemblySha256,corpusSourceHashes,acceptedSignatures:17,checksPerSignature:8,records}]])await writeFile(new URL(path,root),JSON.stringify(value,null,2)+'\n');
console.log(JSON.stringify({acceptedAsyncSignatures:17,totalAccepted:ledger.certifiedUnitySignatures,targets:311}));
