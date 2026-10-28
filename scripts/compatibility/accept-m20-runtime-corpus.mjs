import assert from 'node:assert/strict';
import {readFile,readdir,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';
import {assessM20} from './m20-closure.mjs';

const root=new URL('../../',import.meta.url),bytes=path=>readFile(new URL(path,root)),read=async path=>JSON.parse(await bytes(path)),hash=value=>createHash('sha256').update(value).digest('hex');
function compareClone(actual,expected){
 assert.equal(actual.meta,expected.meta);assert.equal(actual.error,expected.error);
 if(expected.error)return;
 const decode=text=>{const buffer=Buffer.from(text,'base64');assert.equal(buffer.length,68);return Array.from({length:17},(_,index)=>buffer.readFloatLE(index*4));};
 const a=decode(actual.data),b=decode(expected.data),near=(x,y)=>Number.isFinite(x)&&Number.isFinite(y)&&Math.abs(x-y)<=.00002*Math.max(1,Math.abs(y));
 for(let i=0;i<17;i++){
  if(i===3||i===10){const dot=a.slice(i,i+4).reduce((sum,value,n)=>sum+value*b[i+n],0),sign=dot<0?-1:1;for(let n=0;n<4;n++)assert(near(a[i+n]*sign,b[i+n]),'Quaternion differs');i+=3;}
  else assert(near(a[i],b[i]),'Clone pose differs at '+i);
 }
}

// This helper never changes the frozen denominator or required closure criteria.
// Each caller names a fixed direct-call corpus and records its bounded contract.
export async function acceptRuntimeCorpus({slug,area,type,prefix,signatures,observations,contract,pose=false}){
 const evidence='docs/compatibility/evidence/',raw=await bytes(evidence+'m20-unity-'+slug+'-observations.json'),native=JSON.parse(raw),owned=await read(evidence+'m20-sdk12-'+area+'-native.json'),catalog=await read(evidence+'m20-'+slug+'-cases.json'),ledger=await read('docs/compatibility/m20-batches.json'),source=await read(evidence+'m20-sdk12-source.json');
 assert.equal(source.sourceContractMatches,311);assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert(owned.passed);assert.equal(owned.checks,observations);assert.equal(native.rows.length,observations);assert.equal(owned.sourceSha256,native.sourceSha256);assert.equal(owned.evidenceSha256,hash(raw));assert.deepEqual(owned.mismatches,[]);
 const sdkNames=(await readdir(new URL('engine/scripting/templates/',root))).filter(name=>name.endsWith('.cs')&&!['Host.cs','Game.cs'].includes(name)).sort();assert.deepEqual(Object.keys(owned.sdkSources).sort(),sdkNames);
 for(const [name,expected]of Object.entries(owned.sdkSources))assert.equal(hash(await bytes('engine/scripting/templates/'+name)),expected,'Stale native SDK '+name);
 const corpusSourceHashes={};for(const suffix of ['Corpus','Reference','Lab']){const name=type+suffix+'.cs';corpusSourceHashes[name]=hash(await bytes('demos/scripts/'+name));}assert.equal(native.sourceSha256,corpusSourceHashes[type+'Corpus.cs']);
 assert.equal(new Set(native.rows.map(row=>row.index+'.'+row.profile)).size,observations);assert.equal(catalog.records.length,signatures);assert.equal(catalog.expectedObservations,observations);
 assert.deepEqual(owned.rows.map(row=>[row.index,row.profile]),native.rows.map(row=>[row.index,row.profile]));
 for(let i=0;i<observations;i++){if(pose)compareClone(owned.rows[i],native.rows[i]);else{assert.equal(owned.rows[i].error,native.rows[i].error);assert.equal(owned.rows[i].data,native.rows[i].data);}}
 const wasm={};for(const mode of ['development','aot']){const report=await read(evidence+'m20-sdk12-'+area+'-'+mode+'-wasm.json');assert(report.passed&&report.executedBuild.mode===mode);assert.equal(report.count,observations);assert.equal(report.passedChecks,observations);assert.equal(report.failedChecks,0);assert.deepEqual(report.pageErrors,[]);assert.equal(report.diagnostics,'');assert.equal(Object.keys(report.cases).length,observations);for(const [name,expected]of Object.entries(corpusSourceHashes))assert.equal(report.corpusSourceHashes[name],expected,'Stale '+mode+' corpus '+name);wasm[mode]=report;}
 const records=[];for(const entry of catalog.records){const row=ledger.records.find(row=>row.id===entry.id);assert(row&&row.owner===entry.owner&&row.declaration===entry.declaration);assert(source.records.find(record=>record.id===row.id)?.sourceMatched);const caseIds=[];
  for(const profile of entry.profiles){const index=native.rows.findIndex(row=>row.index===entry.index&&row.profile===profile);assert(index>=0);const id=prefix+String(index+1).padStart(3,'0');caseIds.push(id);for(const report of Object.values(wasm))assert.equal(report.cases[id],'pass');}
  row.status='certified-behavior';row.unitySourceCertified=true;row.unityBehaviorSignatureCertified=true;row.acceptance={contract,native:{passed:true,evidence:'evidence/m20-unity-'+slug+'-observations.json',ownedEvidence:'evidence/m20-sdk12-'+area+'-native.json',caseIndex:entry.index,profiles:entry.profiles},wasm:{passed:true,evidence:'evidence/m20-sdk12-'+area+'-development-wasm.json',aotEvidence:'evidence/m20-sdk12-'+area+'-aot-wasm.json',caseIds}};delete row.remaining;entry.certified=true;records.push({id:row.id,owner:row.owner,declaration:row.declaration,index:entry.index,profiles:entry.profiles,caseIds,passed:true});
 }
 ledger.certifiedUnitySignatures=ledger.records.filter(row=>row.unitySourceCertified&&row.unityBehaviorSignatureCertified&&row.acceptance?.native?.passed&&row.acceptance?.wasm?.passed).length;assessM20(ledger,gunzipSync(await bytes('docs/compatibility/native-api.jsonl.gz')).toString().trim().split('\n').map(JSON.parse));
 for(const [path,value]of [['docs/compatibility/m20-batches.json',ledger],[evidence+'m20-'+slug+'-cases.json',catalog],[evidence+'m20-'+slug+'-native-acceptance.json',{schemaVersion:1,passed:true,referenceCoreModuleSha256:native.coreAssemblySha256,corpusSourceHashes,acceptedSignatures:signatures,observations,records}]])await writeFile(new URL(path,root),JSON.stringify(value,null,2)+'\n');
 console.log(JSON.stringify({area,acceptedSignatures:signatures,totalAccepted:ledger.certifiedUnitySignatures,targets:311}));
}
