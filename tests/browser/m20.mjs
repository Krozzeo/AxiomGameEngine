import assert from 'node:assert/strict';
import {mkdtemp,mkdir,readFile,writeFile,rm} from 'node:fs/promises';
import {join,resolve} from 'node:path';
import {tmpdir} from 'node:os';
import {createM20Project} from '../../demos/create-m20-project.mjs';
import {startServer} from '../../daemon/bootstrap/server.mjs';
import {envelope} from '../../protocol/src/protocol.ts';
import {launchEditorBrowser} from './browser-options.mjs';
const root=await mkdtemp(join(tmpdir(),'axiom-m20-')),out=resolve('.axiom/browser-m20-evidence'),report={criteria:[],pageErrors:[],profile:'Axiom development C#/Wasm and Null editor; aliased observations, not native Unity conformance'};await mkdir(out,{recursive:true});let host,browser,page;
try{
 const demo=await createM20Project(root);host=await startServer({projectRoot:root});
 const command=async(type,data={})=>{const r=await fetch(host.origin+'/v1/commands',{method:'POST',headers:{Origin:host.origin,Authorization:'Bearer '+host.token,'Content-Type':'application/json'},body:JSON.stringify(envelope('command',{type,data}))}),v=await r.json();assert.equal(v.kind,'event',JSON.stringify(v));return v.payload.data;};
 browser=await launchEditorBrowser({headless:true,args:['--no-sandbox']});page=await browser.newPage({viewport:{width:1600,height:1050}});page.setDefaultTimeout(240000);page.on('pageerror',error=>report.pageErrors.push(error.message));
 await page.goto(host.editorUrl.replace('#','?renderer=null#'));await page.locator('[data-project-id="'+demo.id+'"]').click();await page.locator('#hub-open').click();await page.waitForFunction(()=>document.querySelector('#editor-workspace').getAttribute('aria-busy')==='false');
 await page.locator('#entities [data-entity-id="'+demo.probeId+'"] .entity').click();await page.locator('#game-tab').click();const before=await command('scene.get');
 await page.locator('#play-start').click();await page.waitForFunction(()=>Number(document.querySelector('#script-components [data-field="Frames"] input')?.value)>=12);await page.waitForFunction(()=>document.querySelector('#script-components [data-field="Report"] input')?.value.includes('destroy.delay-complete:'));
 report.cases=Object.fromEntries((await page.locator('#script-components [data-field="Report"] input').inputValue()).split(';').filter(Boolean).map(x=>x.split(':')));report.passedChecks=Number(await page.locator('#script-components [data-field="Passed"] input').inputValue());report.failedChecks=Number(await page.locator('#script-components [data-field="Failed"] input').inputValue());
 assert.equal(Object.keys(report.cases).length,32,JSON.stringify(report));assert.deepEqual(Object.keys(report.cases).filter(id=>report.cases[id]==='fail'),[],JSON.stringify(report));assert.equal(report.failedChecks,0);assert.equal(report.passedChecks,32);report.conformancePassed=false;report.knownDiscrepancies=[];report.note='M20 fixes the two original alias discrepancies; exact Unity namespace/signature certification remains pending.';
 report.criteria.push('Real C#/Wasm executes32 object identity, interface/type/list queries, graph cloning and destruction lifecycle checks');
 await page.locator('#entities [data-entity-id="'+demo.transformProbeId+'"] .entity').click();
 await page.waitForFunction(()=>Number(document.querySelector('#script-components [data-field="Frames"] input')?.value)>=8&&document.querySelector('#script-components [data-field="Report"] input')?.value.includes('cleanup.hierarchy:'));
 report.transformCases=Object.fromEntries((await page.locator('#script-components [data-field="Report"] input').inputValue()).split(';').filter(Boolean).map(x=>x.split(':')));
 assert.equal(Object.keys(report.transformCases).length,19,JSON.stringify(report));assert.deepEqual(Object.keys(report.transformCases).filter(id=>report.transformCases[id]==='fail'),[],JSON.stringify(report));
 assert.equal(Number(await page.locator('#script-components [data-field="Passed"] input').inputValue()),19);assert.equal(Number(await page.locator('#script-components [data-field="Failed"] input').inputValue()),0);
 report.criteria.push('Real C#/Wasm executes19 transform checks including next-frame host propagation, batch overlap, invalid pose atomicity and runtime cleanup');
 await page.locator('#play-stop').click();await page.waitForFunction(()=>document.querySelector('#editor-workspace').getAttribute('aria-busy')==='false');const stopped=await command('scene.get');assert.equal(stopped.playing,false);assert.deepEqual(stopped.project.scene,before.project.scene);report.criteria.push('Stop restores the complete authored scene; runtime objects, fields and resource handles do not become authoring changes');
 assert.deepEqual(report.pageErrors,[]);report.passed=true;
}catch(error){report.error=error.stack;if(page){report.compile=await page.locator('#toolbar-compile').getAttribute('data-state').catch(()=>null);report.logs=await page.locator('#logs').textContent().catch(()=>null);await page.screenshot({path:join(out,'failure.png')}).catch(()=>{});}throw error;}finally{await writeFile(join(out,'report.json'),JSON.stringify(report,null,2));console.log('M20_BROWSER='+JSON.stringify(report));await browser?.close();await host?.close();await rm(root,{recursive:true,force:true});}
