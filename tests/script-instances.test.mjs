import test from 'node:test';
import assert from 'node:assert/strict';
import {applyScriptOperations} from '../engine/scripting/operations.mjs';
import {applyRuntimeEdit,validateRuntimeEntity} from '../engine/scene/runtime-edit.mjs';
import {validateReference} from '../engine/scripting/references.mjs';
import {validateScriptComponents} from '../engine/scripting/fields.mjs';
import {referenceLabel} from '../apps/editor/src/reference-fields.mjs';

const id='entity://11111111-1111-4111-8111-111111111111',path='Scripts/Probe.cs';
const component=(runtimeId,value)=>({path,runtimeId,values:{Value:value,Peer:null}});
function scene(components=[component(101,10),component(102,20)]){return {entities:[{id,name:'Repeated',transform:{position:[0,0,0],rotation:[0,0,0,1],scale:[1,1,1]},scriptComponents:components}],projectFiles:[{path,kind:'script',text:'using UnityEngine; public class Probe:MonoBehaviour {public int Value=17;public Probe Peer;}'}]};}
const field={name:'Peer',type:'Probe',reference:'component'};
const packet=scriptValues=>({generation:1,operations:[],scriptValues});

test('runtime snapshots and forward references identify each repeated Script independently',()=>{
 const source=scene(),before=structuredClone(source),ref=runtimeId=>({entityId:id,path,component:'Probe',runtimeId});
 const result=applyScriptOperations(source,packet([{entityId:id,path,runtimeId:101,values:{Value:11,Peer:ref(102)}},{entityId:id,path,runtimeId:102,values:{Value:22,Peer:ref(101)}}]),1).scene;
 assert.deepEqual(result.entities[0].scriptComponents.map(c=>[c.runtimeId,c.values.Value,c.values.Peer.runtimeId]),[[101,11,102],[102,22,101]]);assert.deepEqual(source,before);
 assert.equal(referenceLabel(result,ref(102),'Probe'),'Repeated · Probe #2');
});

test('initial authored Script binds once; stale or conflicting identities cannot retarget it',()=>{
 const source=scene([{path,values:{Value:17,Peer:null}}]);
 const result=applyScriptOperations(source,packet([{entityId:id,path,runtimeId:101,values:{Value:23}}]),1).scene;
 assert.equal(result.entities[0].scriptComponents[0].runtimeId,101);
 assert.throws(()=>applyScriptOperations(result,packet([{entityId:id,path,runtimeId:102,values:{Value:99}}]),1));assert.equal(result.entities[0].scriptComponents[0].values.Value,23);
});

test('live Inspector edits target one repeated instance and reject ambiguous or retired targets atomically',()=>{
 const source=scene(),before=structuredClone(source),data={entityId:id,path,runtimeId:102,action:'values',values:{Value:45}};
 const result=applyRuntimeEdit(source,{type:'scene.script.edit',data});assert.deepEqual(result.entities[0].scriptComponents.map(c=>c.values.Value),[10,45]);assert.deepEqual(source,before);
 for(const runtimeId of [undefined,999,0,-1,NaN])assert.throws(()=>applyRuntimeEdit(source,{type:'scene.script.edit',data:{...data,runtimeId}}));
 assert.deepEqual(validateReference(source,field,{entityId:id,path,runtimeId:102},{runtime:true}),{entityId:id,path,component:'Probe',runtimeId:102});
 assert.throws(()=>validateReference(source,field,{entityId:id,path,runtimeId:102}));assert.throws(()=>validateReference(source,field,{entityId:id,path,runtimeId:999},{runtime:true}));
});

test('malformed repeated components, duplicate snapshots and forged identity reuse reject the packet',()=>{
 const source=scene(),before=structuredClone(source);
 for(const components of [[component(101,1),component(101,2)],[{path,values:{}},{path,values:{}}],[component(-1,1)]]){
  assert.throws(()=>validateRuntimeEntity(scene(components).entities[0]));assert.throws(()=>applyScriptOperations(source,{generation:1,operations:[{kind:'property',id,name:'scriptComponents',data:components}]},1));
 }
 const snapshot={entityId:id,path,runtimeId:101,values:{Value:99}};assert.throws(()=>applyScriptOperations(source,packet([snapshot,snapshot]),1));assert.deepEqual(source,before);
});

test('runtime instance support keeps duplicate authored attachments prohibited',()=>{
 const source=scene();assert.throws(()=>validateScriptComponents(source),/no duplicates/);
 validateRuntimeEntity(source.entities[0]);
 const single=scene([{path,values:{Value:17,Peer:null}}]);validateScriptComponents(single);
});

test('legacy bare spawn cannot copy a live Script identity or its per-instance activation alias',()=>{
 const source=scene();source.entities[0].componentEnabled={'script:101':false,'script:102':true,collider:false};const spawnedId='entity://00000000-0000-4000-8000-000000000099';const result=applyScriptOperations(source,{generation:1,operations:[{kind:'spawn',id:spawnedId,template:id,position:[1,0,0]}]},1).scene;const clone=result.entities.find(entity=>entity.id===spawnedId);assert.equal(clone.scriptComponents,undefined);assert.deepEqual(clone.componentEnabled,{collider:false});assert.deepEqual(source.entities[0].scriptComponents.map(component=>component.runtimeId),[101,102]);
});
