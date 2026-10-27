import {readFile,writeFile} from 'node:fs/promises';
import {gunzipSync} from 'node:zlib';
const root=new URL('../../',import.meta.url),read=path=>readFile(new URL(path,root),'utf8');
const frozen=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);
const observations=JSON.parse(await read('docs/compatibility/evidence/m20-unity-6000.3-play-observations.json'));
if(observations.error||observations.unityVersion!=='6000.3.27f1'||observations.coreAssemblySha256!=='42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737')throw Error('Wrong native reference');
const keys=new Set(observations.facts.map(fact=>fact.key)),nativeTest=await read('tests/csharp/M20.Objects/NativeCloneReferenceCases.cs');
const entries=[
 ['cc3ff64315c89b13813d4d34',['generic']],['c5053806fcfe25cd0e53924b',['untyped']],
 ['9c0289db94ea27500f4c4baa',['parameters-default','parameters-world']],['64316228d6bac2b71b7b49f5',['parent-null-world','parent-world','parent-local']],
 ['eb6ff574007ef48a1c2cda26',['explicit-pose']],['ce9cc94143a657f7c0d2466e',['parameters-explicit-world','parameters-explicit-local']],
 ['851f45d6adca7e4eeca55f3a',['destination-scene']],['59fcfaca6872b386dac23585',['pose-parent']],
 ['0ee966233251556d78696f58',['generic-parent']],['6a99c2f72fff0cc5fd17659f',['untyped-parent']],
 ['59fd43ecb8aa882b11f8b8b7',['untyped-parent-world']],['8e08d28359a39dc8ed318dbc',['untyped-pose']],['5a0701d8dca3a5c3b8d80cdf',['untyped-pose-parent']]
];
const records=entries.map(([suffix,cases])=>{
 const target=frozen.find(record=>record.id==='unity63abi:'+suffix&&record.milestone==='M20'&&record.owner==='UnityEngine.Object'&&record.member==='Instantiate');if(!target)throw Error('Missing frozen signature '+suffix);
 for(const key of cases){if(!nativeTest.includes('("'+key+'",'))throw Error('Missing direct owned consumer '+key);for(const field of ['position','rotation','scale'])if(!keys.has(key+'.'+field))throw Error('Missing native observation '+key);}
 return {id:target.id,declaration:target.declaration,cases,positiveNativeProfile:'Native Unity6000.3.27f1 Play mode, signed nonuniform hierarchy and explicit destination',ownedTest:'tests/csharp/M20.Objects/NativeCloneReferenceCases.cs',certified:false,remaining:'Per-signature negative/lifecycle/async and applicable asset-domain acceptance remains required. These observations are a positive scene-graph sample only.'};
});
if(records.length!==13||frozen.filter(record=>record.milestone==='M20'&&record.owner==='UnityEngine.Object'&&record.member==='Instantiate').length!==13)throw Error('Synchronous assigned scope changed');
const result={schemaVersion:1,milestone:'M20',nativeReference:'m20-unity-6000.3-play-observations.json',positiveReferenceSignatures:13,positiveVariants:records.reduce((n,r)=>n+r.cases.length,0),certifiedSignatures:0,records};
await writeFile(new URL('docs/compatibility/evidence/m20-sync-clone-native-cases.json',root),JSON.stringify(result,null,2)+'\n');
console.log(JSON.stringify({signatures:records.length,positiveVariants:result.positiveVariants,certified:0}));
