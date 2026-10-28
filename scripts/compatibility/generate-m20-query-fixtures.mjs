import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gzipSync} from 'node:zlib';
const root=new URL('../../',import.meta.url),bytes=path=>readFile(new URL(path,root)),hash=value=>createHash('sha256').update(value).digest('hex');
const raw=await bytes('docs/compatibility/evidence/m20-unity-component-query-observations.json'),native=JSON.parse(raw),catalog=JSON.parse(await bytes('docs/compatibility/evidence/m20-component-query-cases.json')),ledger=JSON.parse(await bytes('docs/compatibility/m20-batches.json'));
assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.sourceSha256,hash(await bytes('demos/scripts/ComponentQueryCorpus.cs')));assert.equal(catalog.targetSignatures,56);assert.equal(catalog.expectedObservations,476);assert.equal(native.rows.length,476);
for(const record of catalog.records){const row=ledger.records.find(row=>row.id===record.id);assert(row&&row.declaration===record.declaration&&row.owner===record.owner);}
assert.deepEqual(native.rows.map(row=>row.index+'.'+row.profile).sort(),catalog.records.flatMap(row=>row.profiles.map(profile=>row.index+'.'+profile)).sort());
const packed=gzipSync(Buffer.from(native.rows.map(row=>row.index+'.'+row.profile+'|'+row.error+'|'+row.data).join('\n')));packed[9]=10;const data=packed.toString('base64');
const source=`using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own source-attributed native outcomes, no vendor implementation code.
public static class ComponentQueryReference {
 public const string SourceSha256="${native.sourceSha256}";
 public const string EvidenceSha256="${hash(raw)}";
 public const int ExpectedCount=${native.rows.length};
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("${data}"));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var line in reader.ReadToEnd().Split('\\n')){var parts=line.Split('|');if(parts.Length!=3)throw new InvalidOperationException("Corrupt component query reference");result.Add(parts[0],(parts[1],parts[2]));}return result;}
 public static string Compare(ComponentQueryCorpus.Observation row){if(!Expected.TryGetValue(row.index+"."+row.profile,out var expected))return "Unexpected query case";if(row.error!=expected.Error)return "Expected exception "+expected.Error+", actual "+row.error;if(row.data!=expected.Data)return "Expected "+expected.Data+", actual "+row.data;return "";}
}
}
`;
const target=new URL('demos/scripts/ComponentQueryReference.cs',root);if(process.argv.includes('--check'))assert.equal(await readFile(target,'utf8'),source,'Stale component query fixture');else await writeFile(target,source);
console.log(JSON.stringify({referenceCases:476,sourceSha256:native.sourceSha256}));
