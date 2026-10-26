import assert from 'node:assert/strict';
import {mkdtemp,readFile,writeFile,rm} from 'node:fs/promises';
import {tmpdir} from 'node:os';
import {join,resolve} from 'node:path';
import {spawn} from 'node:child_process';
import {createHash} from 'node:crypto';
import {gunzipSync} from 'node:zlib';

// Compile consumers against the owned SDK. Never load or execute a vendor assembly.
const root=new URL('../../',import.meta.url);
const frozen=gunzipSync(await readFile(new URL('docs/compatibility/native-api.jsonl.gz',root))).toString().trim().split('\n').map(JSON.parse);
const references=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-obsolete-reference.json',root))).records;
const metadata=JSON.parse(await readFile(process.argv[4]??new URL('docs/compatibility/evidence/m20-sdk12-abi.json',root)));
const dll=resolve(process.argv[2]??'tests/csharp/M20.Objects/bin/Debug/net10.0/M20.Objects.dll');
const assemblySha256=createHash('sha256').update(await readFile(dll)).digest('hex');
const targets=frozen.filter(row=>row.milestone==='M20'&&row.obsolete);
assert.equal(targets.length,53);
const lines=['public static class ObsoleteConsumer {','public static void Observe(UnityEngine.Component component, UnityEngine.GameObject gameObject, UnityEngine.Transform transform) {'];
const cases=targets.map(target=>{
 const reference=references.find(row=>row.id===target.id);assert(reference);
 const original=target.shape.customAttributes.find(attribute=>attribute.name==='System.ObsoleteAttribute');
 assert.equal(reference.attribute.blobSha256,original.blobSha256);
 const own=metadata.find(row=>row.ownerCanonical===target.owner&&row.declaration===target.declaration);assert(own);assert.equal(own.assemblySha256,assemblySha256,'Compile evidence must use the current owned assembly');
 const receiver=own.static?target.owner:({ 'UnityEngine.Component':'component','UnityEngine.GameObject':'gameObject','UnityEngine.Transform':'transform' })[target.owner];assert(receiver);
 let expression;
 if(target.kind==='property')expression='_ = '+receiver+'.'+target.member;
 else {
  const name=/([\w]+)(<[^>]+>)?\(/.exec(target.declaration);assert(name);
  expression=receiver+'.'+name[1]+(name[2]?'<UnityEngine.GameObject>':'')+'('+own.parameters.map(type=>'default('+type.replace(/`[0-9]+/g,'')+')').join(', ')+')';
 }
 lines.push(expression+';');
 return {id:target.id,declaration:target.declaration,line:lines.length,expectedDiagnostic:reference.attribute.obsoleteError?'CS0619':'CS0618',referenceAttributeSha256:original.blobSha256};
});
lines.push('}}');const source=lines.join('\n')+'\n';const folder=await mkdtemp(join(tmpdir(),'axiom-m20-obsolete-'));
try {
 const escaped=dll.replaceAll('&','&amp;').replaceAll('"','&quot;').replaceAll('<','&lt;');
 await writeFile(join(folder,'Probe.cs'),source);
 await writeFile(join(folder,'Probe.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>disable</Nullable><WarningLevel>4</WarningLevel><TreatWarningsAsErrors>false</TreatWarningsAsErrors></PropertyGroup><ItemGroup><Reference Include="M20.Objects"><HintPath>'+escaped+'</HintPath></Reference></ItemGroup></Project>');
 const dotnet=process.env.DOTNET_HOST_PATH??(process.platform==='win32'?join(process.env.ProgramFiles??'C:\\Program Files','dotnet','dotnet.exe'):'dotnet');
 const {code,output}=await new Promise((resolve,reject)=>{const child=spawn(dotnet,['build',join(folder,'Probe.csproj'),'--nologo','-v:q'],{windowsHide:true});let output='';child.stdout.on('data',value=>output+=value);child.stderr.on('data',value=>output+=value);child.on('error',reject);child.on('close',code=>resolve({code,output}));});
 assert.notEqual(code,0,'Removed APIs must fail consumer compilation');
 const diagnostics=[...output.matchAll(/Probe\.cs\((\d+),\d+\): (?:error|warning) (CS\d+)/g)].map(match=>({line:Number(match[1]),code:match[2]}));
 assert(diagnostics.length,output);
 for(const entry of cases)assert(diagnostics.some(diagnostic=>diagnostic.line===entry.line&&diagnostic.code===entry.expectedDiagnostic),JSON.stringify(entry)+'\n'+output);
 for(const diagnostic of diagnostics)assert(cases.some(entry=>entry.line===diagnostic.line&&entry.expectedDiagnostic===diagnostic.code),'Unexpected consumer diagnostic '+JSON.stringify(diagnostic)+'\n'+output);
 const report={schemaVersion:1,passed:true,scope:'Owned SDK consumer compiler diagnostics only; not runtime behavior certification.',assemblySha256,consumerSha256:createHash('sha256').update(source).digest('hex'),compilerErrorCases:cases.filter(entry=>entry.expectedDiagnostic==='CS0619').length,compilerWarningCases:cases.filter(entry=>entry.expectedDiagnostic==='CS0618').length,cases};
 if(process.argv[3])await writeFile(process.argv[3],JSON.stringify(report,null,2)+'\n');
 console.log(JSON.stringify({passed:true,errors:report.compilerErrorCases,warnings:report.compilerWarningCases,total:cases.length}));
} finally {await rm(folder,{recursive:true,force:true});}
