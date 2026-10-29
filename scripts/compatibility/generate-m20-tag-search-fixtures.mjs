import assert from 'node:assert/strict';
import {readFile,writeFile} from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {gzipSync} from 'node:zlib';
const root=new URL('../../',import.meta.url),path='docs/compatibility/evidence/m20-unity-tag-search-observations.json',raw=await readFile(new URL(path,root)),native=JSON.parse(raw),catalog=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-tag-search-cases.json',root),'utf8')),hash=bytes=>createHash('sha256').update(bytes).digest('hex');
assert(!native.error);assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.sourceSha256,hash(await readFile(new URL('demos/scripts/TagSearchCorpus.cs',root))));
const keys=catalog.records.flatMap(row=>row.profiles.map(profile=>row.index+'.'+profile));assert.equal(native.rows.length,catalog.expectedObservations);assert.deepEqual(native.rows.map(row=>row.index+'.'+row.profile).sort(),keys.sort());
// Freeze the transport-only gzip OS byte to the existing fixture value. Native outcomes and compressed payload stay identical across Windows/Linux/macOS.
const packed=gzipSync(Buffer.from(native.rows.map(row=>row.index+'.'+row.profile+'|'+row.error+'|'+row.data).join('\n')));packed[9]=10;const data=packed.toString('base64');
const source=`using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-tag-search-fixtures.mjs.
public static class TagSearchReference {
 public const string SourceSha256="${native.sourceSha256}";
 public const string EvidenceSha256="${hash(raw)}";
 public const int ExpectedCount=${native.rows.length};
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("${data}"));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var row in reader.ReadToEnd().Split('\\n')){var values=row.Split('|');if(values.Length!=3)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2]));}return result;}
 public static string Compare(TagSearchCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  return row.data==expected.Data?"":"Expected "+expected.Data+", actual "+row.data;
 }

}
}
`;
const target=new URL('demos/scripts/TagSearchReference.cs',root);if(process.argv.includes('--check'))assert.equal(await readFile(target,'utf8'),source,'Stale vector reference fixture');else await writeFile(target,source);
console.log(JSON.stringify({referenceCases:native.rows.length,sourceSha256:native.sourceSha256}));
