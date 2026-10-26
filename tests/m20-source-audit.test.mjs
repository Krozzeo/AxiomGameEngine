import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {gunzipSync} from 'node:zlib';
import {createHash} from 'node:crypto';
import {auditM20Source} from '../scripts/compatibility/m20-source-audit.mjs';
const root=new URL('../',import.meta.url),frozen=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse),compiled=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-sdk12-abi.json',root),'utf8'));
test('all frozen M20 source contracts match compiled owned facts without conferring behavior credit',async()=>{
 const result=auditM20Source(frozen,compiled),recorded=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-sdk12-source.json',root),'utf8'));
 assert.equal(result.targetRecords,311);assert.equal(result.sourceContractMatches,311);assert.equal(result.behaviorSignatureCertified,0);assert.deepEqual(result.records,recorded.records);
 for(const [name,expected]of Object.entries(recorded.sdkSourceHashes))assert.equal(createHash('sha256').update(await readFile(new URL('engine/scripting/templates/'+name,root))).digest('hex'),expected,'Stale compiled-source evidence: '+name);
});
test('source audit rejects missing declarations, changed names, constraints and parameter modes',()=>{
 for(const mutate of [rows=>rows.pop(),rows=>rows[0].declaration+=' changed',rows=>rows.find(row=>row.kind==='method'&&row.shape.genericParameters?.some(parameter=>parameter.constraints?.length)).shape.genericParameters.find(parameter=>parameter.constraints?.length).constraints=[],rows=>rows.find(row=>row.kind==='method'&&row.shape.parameterMetadata?.some(parameter=>parameter.attributes!=='Out')).shape.parameterMetadata.find(parameter=>parameter.attributes!=='Out').attributes='Out']){
  const rows=structuredClone(compiled);mutate(rows);assert(auditM20Source(frozen,rows).sourceContractMatches<311);
 }
 assert.throws(()=>auditM20Source(frozen.filter(row=>row.milestone!=='M20'),compiled),/scope/);
});

test('obsolete compiler contracts retain frozen warning/error metadata and all consumer cases',async()=>{
 const reference=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-obsolete-reference.json',root))).records;
 const consumer=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-sdk12-obsolete-compile.json',root)));
 const targets=frozen.filter(row=>row.milestone==='M20'&&row.obsolete);
 assert.equal(targets.length,53);assert.equal(reference.length,53);assert.equal(consumer.cases.length,53);assert.equal(consumer.passed,true);
 for(const target of targets){
  const facts=reference.find(row=>row.id===target.id);assert(facts);
  assert.equal(facts.attribute.blobSha256,target.shape.customAttributes.find(attribute=>attribute.name==='System.ObsoleteAttribute').blobSha256);
  assert(target.provenance.some(source=>source.assemblySha256===facts.assemblySha256));
  assert.equal(consumer.cases.find(row=>row.id===target.id)?.expectedDiagnostic,facts.attribute.obsoleteError?'CS0619':'CS0618');
 }
 assert.equal(consumer.compilerErrorCases,30);assert.equal(consumer.compilerWarningCases,23);
 const changed=structuredClone(compiled);changed.find(row=>row.obsolete).shape.customAttributes.find(attribute=>attribute.name==='System.ObsoleteAttribute').blobSha256='changed';
 assert.equal(auditM20Source(frozen,changed).sourceContractMatches,310);
});
