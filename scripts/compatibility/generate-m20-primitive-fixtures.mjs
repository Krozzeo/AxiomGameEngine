import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
const root=new URL('../../',import.meta.url),bytes=path=>readFile(new URL(path,root)),hash=value=>createHash('sha256').update(value).digest('hex');
const raw=await bytes('docs/compatibility/evidence/m20-unity-primitive-creation-observations.json'),native=JSON.parse(raw);assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.sourceSha256,hash(await bytes('demos/scripts/PrimitiveCreationCorpus.cs')));assert.equal(native.facts.length,13);assert.equal(new Set(native.facts.map(row=>row.key)).size,13);
const entries=native.facts.map(row=>'['+JSON.stringify(row.key)+']='+JSON.stringify(row.value)).join(',\n');
const source=`using System.Collections.Generic;
namespace Game {
// Own public outcomes observed in Unity6000.3.27f1 Play; no vendor implementation.
public static class PrimitiveCreationReference {
 public const string SourceSha256="${native.sourceSha256}";
 public const string EvidenceSha256="${hash(raw)}";
 public const int ExpectedCount=13;
 private static readonly Dictionary<string,string> Expected=new(){${entries}};
 public static string Compare(string key,string actual)=>!Expected.TryGetValue(key,out var expected)?"Unknown case "+key:actual==expected?"":"Expected "+expected+", actual "+actual;
}
}
`;
const target=new URL('demos/scripts/PrimitiveCreationReference.cs',root);if(process.argv.includes('--check'))assert.equal(await readFile(target,'utf8'),source,'Stale GameObject state reference');else await writeFile(target,source);console.log(JSON.stringify({referenceCases:13,sourceSha256:native.sourceSha256}));
