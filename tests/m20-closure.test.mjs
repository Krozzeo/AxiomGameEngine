import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {gunzipSync} from 'node:zlib';
import {assessM20} from '../scripts/compatibility/m20-closure.mjs';
const ledger=JSON.parse(await readFile(new URL('../docs/compatibility/m20-batches.json',import.meta.url),'utf8'));
const inventory=gunzipSync(await readFile(new URL('../docs/compatibility/native-api.jsonl.gz',import.meta.url))).toString().trim().split('\n').map(JSON.parse);
test('M20 closure cannot waive signatures, relabel target declarations or count an alias demo as conformance',()=>{
 const result=assessM20(ledger,inventory);assert.equal(result.complete,false);assert.equal(result.unaccepted.length,311);
 for(const mutate of [copy=>copy.records.pop(),copy=>copy.records[0].declaration='public void Renamed();',copy=>copy.records[1].id=copy.records[0].id,copy=>copy.status='complete']){
  const copy=structuredClone(ledger);mutate(copy);assert.throws(()=>assessM20(copy,inventory));
 }
 const fabricated=structuredClone(ledger);fabricated.records[0].unitySourceCertified=true;fabricated.records[0].unityBehaviorSignatureCertified=true;fabricated.certifiedUnitySignatures=1;
 assert.throws(()=>assessM20(fabricated,inventory),/attributed acceptance/);
});
