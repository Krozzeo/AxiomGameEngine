import {readFile,writeFile,readdir} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';
import {resolve} from 'node:path';
import {fileURLToPath} from 'node:url';
const hash=data=>createHash('sha256').update(data).digest('hex');
const flags=value=>(value??'').split(', ').filter(v=>['Public','Family','FamORAssem','Static','Virtual','Abstract','Final','NewSlot','Sealed','SequentialLayout'].includes(v)).sort();
const parameterModes=shape=>(shape.parameterMetadata??[]).map(parameter=>({sequence:parameter.sequence,attributes:parameter.attributes,sourceAttributes:parameter.customAttributes.filter(attribute=>['System.ParamArrayAttribute','System.Runtime.CompilerServices.IsReadOnlyAttribute'].includes(attribute.name)).map(attribute=>attribute.name).sort()}));
const obsoleteContract=record=>record?.shape.customAttributes?.find(attribute=>attribute.name==='System.ObsoleteAttribute')?.blobSha256??null;
export function auditM20Source(reference,compiled){
 const targets=reference.filter(record=>record.milestone==='M20');if(targets.length!==311)throw Error('Frozen M20 scope changed');
 const records=targets.map(target=>{
  const own=compiled.find(candidate=>candidate.publicContract&&candidate.ownerCanonical===target.owner&&candidate.kind===target.kind&&candidate.declaration===target.declaration);
  const checks={declaration:!!own,baseType:own?.baseType===target.baseType,sourceFlags:!!own&&JSON.stringify(flags(own.shape.attributes))===JSON.stringify(flags(target.shape.attributes)),genericConstraints:!!own&&JSON.stringify(own.shape.genericParameters??[])===JSON.stringify(target.shape.genericParameters??[]),parameterModes:!!own&&JSON.stringify(parameterModes(own.shape))===JSON.stringify(parameterModes(target.shape)),obsolete:own?.obsolete===target.obsolete};
  checks.obsoleteCompilerContract=!!own&&obsoleteContract(own)===obsoleteContract(target);
  return {id:target.id,owner:target.owner,declaration:target.declaration,checks,sourceMatched:Object.values(checks).every(Boolean),assemblySha256:own?.assemblySha256??null,behaviorCertified:false};
 });
 return {schemaVersion:1,milestone:'M20',targetRecords:311,declarationMatches:records.filter(record=>record.checks.declaration).length,sourceContractMatches:records.filter(record=>record.sourceMatched).length,behaviorSignatureCertified:0,scope:'Compiled public source contracts against the frozen ABI. Implementation/native-binding attributes and private vendor layouts are not an Axiom interop ABI. A match never certifies behavior.',records};
}
if(process.argv[1]&&resolve(process.argv[1])===fileURLToPath(import.meta.url)){
 const input=process.argv[2];if(!input)throw Error('Supply the owned SDK PE metadata JSON from Metadata --sdk');
 const root=new URL('../../',import.meta.url),inventory=await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root)),compiledBytes=await readFile(input),report=auditM20Source(gunzipSync(inventory).toString().trim().split('\n').map(JSON.parse),JSON.parse(compiledBytes));
 report.frozenInventorySha256=hash(gunzipSync(inventory));report.compiledMetadataSha256=hash(compiledBytes);report.sdkSourceHashes={};
 for(const name of (await readdir(new URL('engine/scripting/templates/',root))).filter(name=>name.endsWith('.cs')).sort())report.sdkSourceHashes[name]=hash(await readFile(new URL('engine/scripting/templates/'+name,root)));
 if(process.argv[3])await writeFile(process.argv[3],JSON.stringify(report,null,2)+'\n');
 console.log(JSON.stringify({targets:311,declarations:report.declarationMatches,sourceContracts:report.sourceContractMatches,behaviorCertified:0,mismatches:report.records.filter(record=>!record.sourceMatched).map(({id,declaration,checks})=>({id,declaration,checks}))}));
 if(report.sourceContractMatches!==311)process.exitCode=2;
}
