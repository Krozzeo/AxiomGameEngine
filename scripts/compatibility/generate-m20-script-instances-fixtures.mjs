import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
const root=new URL('../../',import.meta.url),bytes=path=>readFile(new URL(path,root)),hash=value=>createHash('sha256').update(value).digest('hex');
const raw=await bytes('docs/compatibility/evidence/m20-unity-script-instances-observations.json'),native=JSON.parse(raw);assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.sourceSha256,hash(await bytes('demos/scripts/ScriptInstanceCorpus.cs')));assert.equal(native.receiverSourceSha256,hash(await bytes('demos/scripts/RepeatedScriptProbe.cs')));assert.equal(native.facts.length,27);assert.equal(new Set(native.facts.map(row=>row.key)).size,27);
const entries=native.facts.map(row=>'['+JSON.stringify(row.key)+']='+JSON.stringify(row.value)).join(',\n');
const source=`using System.Collections.Generic;
namespace Game {
// Own public outcomes observed in Unity6000.3.27f1 Play; no vendor implementation.
public static class ScriptInstanceReference {
 public const string SourceSha256="${native.sourceSha256}";
 public const string EvidenceSha256="${hash(raw)}";
 public const int ExpectedCount=27;
 private static readonly Dictionary<string,string> Expected=new(){${entries}};
 public static string Compare(string key,string actual)=>!Expected.TryGetValue(key,out var expected)?"Unknown case "+key:actual==expected?"":"Expected "+expected+", actual "+actual;
}
}
`;
const target=new URL('demos/scripts/ScriptInstanceReference.cs',root);if(process.argv.includes('--check'))assert.equal(await readFile(target,'utf8'),source,'Stale GameObject state reference');else await writeFile(target,source);console.log(JSON.stringify({referenceCases:27,sourceSha256:native.sourceSha256}));
