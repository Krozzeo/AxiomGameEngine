import {readFile,writeFile} from 'node:fs/promises';
const source=await readFile(new URL('../../demos/scripts/AsyncOverloadCatalog.cs',import.meta.url),'utf8');
const declaration='public sealed record Observation(string Id,string Label,AsyncInstantiateOperation<GameObject> Operation,float[] ExpectedX,bool RejectedNull);';
if(!source.includes(declaration))throw Error('Review async observation representation before adapting it to the Unity compiler');
const compatible='public sealed class Observation {public string Id,Label;public AsyncInstantiateOperation<GameObject> Operation;public float[] ExpectedX;public bool RejectedNull;public Observation(string id,string label,AsyncInstantiateOperation<GameObject> operation,float[] expected,bool rejected){Id=id;Label=label;Operation=operation;ExpectedX=expected;RejectedNull=rejected;}}';
// Adapt the result container and namespace syntax to C#9. All17 direct calls and labels remain unchanged.
const destination=new URL('./UnityOracle/AsyncOverloadCatalog.cs',import.meta.url),expected='// Generated from our portable direct-call corpus; no vendor implementation code.\n'+source.replace(declaration,compatible).replace('namespace Game;','namespace Game {')+'\n}\n';
if(process.argv.includes('--check')){if(await readFile(destination,'utf8')!==expected)throw Error('Stale native async direct-call corpus; regenerate before observing Unity');}else await writeFile(destination,expected);
