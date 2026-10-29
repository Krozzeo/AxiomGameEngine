import {readFile,writeFile} from 'node:fs/promises';
import {gunzipSync} from 'node:zlib';
import {fileURLToPath} from 'node:url';
import {resolve} from 'node:path';

export function assessM20(ledger,inventory){
 const targets=inventory.filter(record=>record.milestone==='M20');
 if(targets.length!==311||ledger.targetRecords!==311)throw Error('M20 frozen denominator must remain 311');
 const expected=new Map(targets.map(record=>[record.id,record]));
 if(ledger.records.length!==expected.size||new Set(ledger.records.map(record=>record.id)).size!==expected.size)throw Error('M20 ledger contains missing or duplicate identities');
 const areas={};const unaccepted=[];
 for(const record of ledger.records){
  const target=expected.get(record.id);
  if(!target||record.owner!==target.owner||record.declaration!==target.declaration)throw Error('M20 ledger changed a frozen target: '+record.id);
  const area=areas[record.owner]??={targets:0,aliasBehaviorTargets:0,certifiedTargets:0};area.targets++;
  if(record.status==='alias-behavior-tested')area.aliasBehaviorTargets++;
  // A source name match, a deferred dependency or a family-level demo cannot
  // certify an overload. Require attributed evidence in both execution profiles.
  const accepted=record.unitySourceCertified===true&&record.unityBehaviorSignatureCertified===true&&record.acceptance?.native?.passed===true&&record.acceptance?.wasm?.passed===true&&typeof record.acceptance?.contract==='string'&&record.acceptance.contract.length>0;
  if(accepted)area.certifiedTargets++;else unaccepted.push({id:record.id,owner:record.owner,declaration:record.declaration,status:record.status,reason:record.remaining??'No exact source/overload and native/Wasm behavioral acceptance'});
 }
 const certified=311-unaccepted.length;
 if(certified!==ledger.certifiedUnitySignatures)throw Error('M20 certified count does not match attributed acceptance');
 const criteria=ledger.closureCriteria??{};
 const required=['unadaptedUnityCorpus','hierarchyReferenceLifecycle','regressionAndDependencies'];
 const failedCriteria=required.filter(key=>criteria[key]?.status!=='passed');
 const complete=unaccepted.length===0&&failedCriteria.length===0;
 if(ledger.status==='complete'&&!complete)throw Error('M20 cannot be closed with unaccepted signatures or required criteria');
 return {milestone:'M20',complete,targetRecords:311,certifiedUnitySignatures:certified,areas,failedCriteria,unaccepted};
}

if(process.argv[1]&&resolve(process.argv[1])===fileURLToPath(import.meta.url)){
 const root=new URL('../../',import.meta.url),ledger=JSON.parse(await readFile(new URL('docs/compatibility/m20-batches.json',root),'utf8'));
 const inventory=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);
 const report=assessM20(ledger,inventory);const flag=process.argv.indexOf('--output');
 if(flag>=0){if(!process.argv[flag+1])throw Error('--output needs a path');await writeFile(process.argv[flag+1],JSON.stringify(report,null,2)+'\n');}
 console.log(JSON.stringify({milestone:report.milestone,complete:report.complete,targetRecords:report.targetRecords,certifiedUnitySignatures:report.certifiedUnitySignatures,areas:report.areas,failedCriteria:report.failedCriteria}));
 if(process.argv.includes('--require-closed')&&!report.complete)process.exitCode=2;
}
