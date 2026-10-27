import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {assessM20} from './m20-closure.mjs';
import {gunzipSync} from 'node:zlib';
const root=new URL('../../',import.meta.url),read=async path=>JSON.parse(await readFile(new URL(path,root),'utf8'));
const ledger=await read('docs/compatibility/m20-batches.json'),source=await read('docs/compatibility/evidence/m20-sdk12-source.json');assert.equal(source.sourceContractMatches,311);
const evidence={native:'evidence/m20-unity-obsolete-compile.json',wasm:'evidence/m20-sdk12-obsolete-wasm-compile.json',owned:'evidence/m20-sdk12-obsolete-compile.json'};
const [native,wasm,owned]=await Promise.all(Object.values(evidence).map(path=>read('docs/compatibility/'+path)));
assert(native.passed&&wasm.passed&&owned.passed);assert.equal(native.referenceCoreModuleSha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.consumerSha256,wasm.consumerSha256);assert.equal(native.consumerSha256,owned.consumerSha256);assert.equal(wasm.assemblySha256,owned.assemblySha256);
const removed=native.cases.filter(row=>row.expectedDiagnostic==='CS0619');assert.equal(removed.length,30);
for(const entry of removed){
 const row=ledger.records.find(row=>row.id===entry.id);assert(row);assert(source.records.find(record=>record.id===row.id)?.sourceMatched);assert(wasm.cases.some(item=>item.id===entry.id&&item.expectedDiagnostic==='CS0619'));assert(owned.cases.some(item=>item.id===entry.id&&item.expectedDiagnostic==='CS0619'));
 row.status='certified-compiler-contract';row.unitySourceCertified=true;row.unityBehaviorSignatureCertified=true;
 row.acceptance={contract:'This exact member is Obsolete(error:true). Its direct public C# consumer must be rejected with CS0619 by both native Unity and the Axiom native/Wasm compilers. No normal compiled runtime invocation exists; this acceptance does not claim a usable deprecated feature.',native:{passed:true,evidence:evidence.native,caseId:entry.id,diagnostic:'CS0619',profile:'Actual frozen Unity compiler'},wasm:{passed:true,evidence:evidence.wasm,caseId:entry.id,diagnostic:'CS0619',profile:'Actual C#/Wasm compiler'},ownedNativeCompiler:evidence.owned};delete row.remaining;
}
ledger.certifiedUnitySignatures=ledger.records.filter(row=>row.unitySourceCertified===true&&row.unityBehaviorSignatureCertified===true&&row.acceptance?.native?.passed===true&&row.acceptance?.wasm?.passed===true).length;
const inventory=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);assessM20(ledger,inventory);
await writeFile(new URL('docs/compatibility/m20-batches.json',root),JSON.stringify(ledger,null,2)+'\n');console.log(JSON.stringify({acceptedCompilerErrorContracts:30,totalAccepted:ledger.certifiedUnitySignatures,targets:311}));
