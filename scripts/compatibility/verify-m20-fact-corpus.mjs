import assert from 'node:assert/strict';
import {readFile,readdir} from 'node:fs/promises';
import {createHash} from 'node:crypto';
const root=new URL('../../',import.meta.url),bytes=path=>readFile(new URL(path,root)),read=async path=>JSON.parse(await bytes(path)),hash=value=>createHash('sha256').update(value).digest('hex');

export async function verifyFactCorpus({slug,type,count,extraSources=[],inspector=false}){
 const prefix='docs/compatibility/evidence/',raw=await bytes(prefix+'m20-unity-'+slug+'-observations.json'),native=JSON.parse(raw),owned=await read(prefix+'m20-sdk12-'+slug+'-native.json');
 assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');
 assert(owned.passed);assert.equal(owned.checks,count);assert.deepEqual(owned.mismatches,[]);assert.equal(owned.sourceSha256,native.sourceSha256);assert.equal(owned.evidenceSha256,hash(raw));
 const corpusSourceHashes={};for(const name of [type+'Corpus',type+'Reference',type+'Lab',...extraSources])corpusSourceHashes[name+'.cs']=hash(await bytes('demos/scripts/'+name+'.cs'));
 assert.equal(corpusSourceHashes[type+'Corpus.cs'],native.sourceSha256);if(extraSources.length){assert.equal(native.receiverSourceSha256,corpusSourceHashes[extraSources[0]+'.cs']);assert.equal(owned.receiverSourceSha256,native.receiverSourceSha256);}
 const names=(await readdir(new URL('engine/scripting/templates/',root))).filter(name=>name.endsWith('.cs')&&!['Host.cs','Game.cs'].includes(name)).sort();assert.deepEqual(Object.keys(owned.sdkSources).sort(),names);for(const name of names)assert.equal(owned.sdkSources[name],hash(await bytes('engine/scripting/templates/'+name)),'Stale native SDK '+name);
 const facts=Object.fromEntries(native.facts.map(row=>[row.key,row.value]));assert.equal(native.facts.length,count);assert.equal(Object.keys(facts).length,count);assert.deepEqual(owned.facts,facts);
 const modes={};for(const mode of ['development','aot']){
  const report=await read(prefix+'m20-sdk12-'+slug+'-'+mode+'-wasm.json');assert(report.passed&&report.executedBuild.mode===mode);assert.equal(report.count,count);assert.equal(report.passedChecks,count);assert.equal(report.failedChecks,0);assert.equal(report.diagnostics,'');assert.deepEqual(report.pageErrors,[]);assert.deepEqual(Object.keys(report.cases).sort(),Object.keys(facts).sort());assert(Object.values(report.cases).every(value=>value==='pass'));
  for(const [name,expected]of Object.entries(corpusSourceHashes))assert.equal(report.corpusSourceHashes[name],expected,'Stale '+mode+' '+name);
  if(inspector)for(const key of ['independentFieldEdit','independentActivation','exactRepeatedReference'])assert.equal(report.inspector?.[key],true,'Missing real Inspector acceptance '+key);
  modes[mode]=report;
 }
 return {native,owned,corpusSourceHashes,caseIds:Object.keys(facts),modes};
}
