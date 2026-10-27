import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {composeWorldTransform} from '../engine/scene/hierarchy.mjs';
import {applyScriptOperations} from '../engine/scripting/operations.mjs';
const native=JSON.parse(await readFile(new URL('../docs/compatibility/evidence/m20-unity-6000.3-native-observations.json',import.meta.url)));
const near=(a,b)=>a.length===b.length&&a.every((v,i)=>Math.abs(v-b[i])<=2e-5+1e-5*Math.max(Math.abs(v),Math.abs(b[i])));
test('128 exact native signed hierarchies agree with host composition and packet acceptance',()=>{
 assert.equal(native.unityVersion,'6000.3.27f1');assert.equal(native.coreAssemblySha256,'42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737');assert.equal(native.error,"");assert.equal(native.transforms.length,128);
 for(const [i,row] of native.transforms.entries()){
  const pose=(prefix,position)=>({position,rotation:row[prefix+'Rotation'],scale:row[prefix+'Scale']});
  const local=[pose('parent',[3,0,0]),pose('child',[1,0,0]),pose('leaf',[0,0,0])],world=[];
  for(const t of local)world.push(composeWorldTransform(world.at(-1),t,true));
  assert(near(world[2].matrix,row.worldMatrix),'matrix '+i);assert(near(world[2].scale,row.worldScale),'scale '+i);
  assert(near(world[2].rotation,row.worldRotation)||near(world[2].rotation.map(v=>-v),row.worldRotation),'rotation '+i);
  const ids=['entity://11111111-1111-4111-8111-111111111111','entity://22222222-2222-4222-8222-222222222222','entity://33333333-3333-4333-8333-333333333333'];
  const entities=world.map((t,index)=>({id:ids[index],name:'Native '+index,runtimeParentId:index?ids[index-1]:null,transform:{position:t.position,rotation:t.rotation,scale:t.scale},runtimeMatrix:t.matrix,runtimeLocalTransform:local[index]}));
  const packet={generation:1,operations:[{kind:'import',data:entities}]};assert.doesNotThrow(()=>applyScriptOperations({entities:[]},packet,1),'native packet '+i);
  const invalid=structuredClone(packet);invalid.operations[0].data[2].transform.scale[0]+=1;assert.throws(()=>applyScriptOperations({entities:[]},invalid,1),/transform/);
 }
});
