import test from 'node:test';
import assert from 'node:assert/strict';
import {applyScriptOperations,runtimeScriptEntities} from '../engine/scripting/operations.mjs';
import {worldScene,reparent} from '../engine/scene/hierarchy.mjs';
import {modelMatrix,transform} from '../engine/renderer/render-math.mjs';
import {applyRuntimeEdit} from '../engine/scene/runtime-edit.mjs';
const root='entity://11111111-1111-4111-8111-111111111111',child='entity://22222222-2222-4222-8222-222222222222';
const q=[0,0,Math.sin(Math.PI/8),Math.cos(Math.PI/8)];
const state=()=>({matrix:[Math.SQRT2,Math.SQRT1_2,0,0,-Math.SQRT2,Math.SQRT1_2,0,0,0,0,1,0,5,0,0,1],local:{position:[1,0,0],rotation:q,scale:[1,1,1]}});
const scene=()=>({entities:[{id:root,name:'Parent',runtimeParentId:null,transform:{position:[3,0,0],rotation:[0,0,0,1],scale:[2,1,1]}},{id:child,name:'Child',runtimeParentId:root,transform:{position:[5,0,0],rotation:q,scale:[Math.sqrt(2.5),Math.sqrt(2.5),1]}}]});
const packet=value=>({generation:1,operations:[{kind:'move',id:child,position:[5,0,0],data:{transformState:value}}]});

test('world-preserving parent packets carry local and affine state without a redundant pose write',()=>{
 const source=scene();source.entities[1].runtimeParentId=null;
 const next=applyScriptOperations(source,{generation:1,operations:[{kind:'parent',id:child,name:root,data:{transformState:state()}}]},1).scene;
 assert.equal(next.entities[1].runtimeParentId,root);assert.deepEqual(next.entities[1].runtimeLocalTransform,state().local);assert.deepEqual(next.entities[1].runtimeMatrix,state().matrix);
 const invalid=state();invalid.local.position=[9,0,0];
 assert.throws(()=>applyScriptOperations(source,{generation:1,operations:[{kind:'parent',id:child,name:root,data:{transformState:invalid}}]},1),/transform/);
 assert.equal(source.entities[1].runtimeParentId,null);
});

test('combined pose packets update render and physics intents atomically',()=>{
 const value=state(),source=scene(),pose={kind:'pose',id:child,position:[5,0,0],rotation:q,scale:[Math.sqrt(2.5),Math.sqrt(2.5),1],data:{transformState:value}};
 const next=applyScriptOperations(source,{generation:1,operations:[pose]},1);
 assert.deepEqual(next.positions.get(child),pose.position);assert.deepEqual(next.rotations.get(child),q);assert.deepEqual(next.scene.entities[1].transform.scale,pose.scale);
 for(const mutation of [op=>op.position[0]=NaN,op=>op.rotation=[0,0,0,0],op=>op.scale=[1,1,1],op=>delete op.data]){
  const invalid=structuredClone(pose);mutation(invalid);const before=structuredClone(source);
  assert.throws(()=>applyScriptOperations(source,{generation:1,operations:[invalid]},1),/pose|transform/);assert.deepEqual(source,before);
 }
});
test('runtime shear survives packet validation, kernel hierarchy reconstruction and renderer matrices',()=>{
 const source=scene(),before=structuredClone(source),next=applyScriptOperations(source,packet(state()),1).scene;
 const hierarchy=reparent(next.entities,[child],root),rendered=worldScene({...next,entities:hierarchy});
 const matrix=modelMatrix(rendered.entities.find(e=>e.id===child).transform),point=transform(matrix,[1,0,0]);
 assert(Math.abs(point[0]-(5+Math.SQRT2))<1e-10);assert(Math.abs(point[1]-Math.SQRT1_2)<1e-10);
 const snapshot=runtimeScriptEntities(rendered.entities,hierarchy).find(e=>e.id===child);assert.deepEqual(snapshot.runtimeLocalTransform,state().local);assert.equal(snapshot.parentId,root);assert.deepEqual(source,before);
});
test('malformed or contradictory matrix metadata rejects the entire packet without changing the source',()=>{
 for(const mutate of [value=>value.matrix[0]=NaN,value=>value.matrix[15]=2,value=>value.matrix[12]=99,value=>value.local.position[0]=4,value=>value.local.rotation=[0,0,0,0],value=>value.extra=true]){
  const value=state();mutate(value);const source=scene(),before=structuredClone(source),data=packet(value);data.operations.unshift({kind:'move',id:root,position:[3,1,0]});assert.throws(()=>applyScriptOperations(source,data,1),/transform/);assert.deepEqual(source,before);
 }
});
test('legacy world-pose intents discard stale affine metadata instead of rendering an old pose',()=>{
 const next=applyScriptOperations(scene(),packet(state()),1).scene;
 const moved=applyScriptOperations(next,{generation:1,operations:[{kind:'move',id:child,position:[8,0,0]}]},1).scene.entities.find(e=>e.id===child);
 assert.equal(moved.runtimeMatrix,undefined);assert.equal(moved.runtimeLocalTransform,undefined);assert.deepEqual(moved.transform.position,[8,0,0]);
});
test('human runtime edits accept derived metadata but cannot inject a renderer matrix',()=>{
 const next=applyScriptOperations(scene(),packet(state()),1).scene,before=structuredClone(next);
 const edited=applyRuntimeEdit(next,{type:'scene.entity.update',data:{entityId:child,transform:{position:[9,0,0]}}}).entities.find(e=>e.id===child);
 assert.equal(edited.runtimeMatrix,undefined);assert.equal(edited.runtimeLocalTransform,undefined);assert.deepEqual(edited.transform.position,[9,0,0]);assert.deepEqual(next,before);
 assert.throws(()=>applyRuntimeEdit(next,{type:'scene.entity.update',data:{entityId:child,transform:{matrix:state().matrix}}}),/derived/);
});
test('imported clone hierarchy validates affine state with an existing destination parent',()=>{
 const source=scene(),entity=source.entities.pop(),affine=state();entity.runtimeMatrix=affine.matrix;entity.runtimeLocalTransform=affine.local;entity.runtimeActive=true;entity.runtimeTag='Untagged';entity.runtimeLayer=0;
 const data={generation:1,operations:[{kind:'import',data:[entity]}]},before=structuredClone(source);
 const result=applyScriptOperations(source,data,1).scene;assert.equal(result.entities.length,2);assert.deepEqual(result.entities[1].runtimeMatrix,affine.matrix);assert.deepEqual(source,before);
 for(const mutate of [e=>e.runtimeMatrix[12]=800,e=>e.runtimeLocalTransform.rotation=[0,0,0,0],e=>e.runtimeUnknown=true,e=>e.runtimeLayer=90,e=>e.runtimeParentId=child]){
  const invalid=structuredClone(data);mutate(invalid.operations[0].data[0]);assert.throws(()=>applyScriptOperations(source,invalid,1));assert.deepEqual(source,before);
 }
});
test('bulk activation validates all target identities before publishing scene changes',()=>{
 const source=scene(),before=structuredClone(source);const data={generation:1,operations:[{kind:'activeMany',data:{ids:[root,child],active:false}}]};
 const result=applyScriptOperations(source,data,1);assert(result.changedTopology);assert(result.scene.entities.every(entity=>entity.runtimeActive===false));assert.deepEqual(source,before);
 for(const invalid of [{ids:[root,'entity://missing'],active:false},{ids:[root],active:'false'},{ids:[root],active:false,unknown:true}])assert.throws(()=>applyScriptOperations(source,{generation:1,operations:[{kind:'activeMany',data:invalid}]},1));
 assert.deepEqual(source,before);
});
