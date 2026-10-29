import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {builtinPrimitiveAsset,builtinPrimitiveIds,builtinPrimitiveNames,builtinPrimitiveEntity} from '../engine/scene/builtin-primitives.mjs';
import {applyScriptOperations} from '../engine/scripting/operations.mjs';
import {loadKernel} from '../engine/wasm/host.mjs';
import {validateColliderShape} from '../engine/scene/collider.mjs';
const id=n=>'entity://00000000-0000-4000-8000-'+String(n).padStart(12,'0');
const bytes=await readFile(new URL('../target/wasm32-unknown-unknown/release/axiom_wasm.wasm',import.meta.url));
test('all six built-in assets have finite geometry, unit normals, correct dimensions and shared SDK identities',async()=>{
 const sdk=await readFile(new URL('../engine/scripting/templates/UnityPrimitives.cs',import.meta.url),'utf8');
 for(const [i,assetId]of builtinPrimitiveIds.entries()){
  assert(sdk.includes(assetId));const asset=builtinPrimitiveAsset(assetId);assert.equal(asset.name,builtinPrimitiveNames[i]);assert.equal(asset,builtinPrimitiveAsset(assetId));assert.equal(asset.kind,'mesh');const vertices=asset.primitives[0].vertices;assert(vertices.length>0&&vertices.length%24===0&&vertices.every(Number.isFinite));
  for(let j=0;j<vertices.length;j+=8)assert(Math.abs(Math.hypot(...vertices.slice(j+3,j+6))-1)<1e-9);
  const half=i===4?[5,0,5]:i===5?[.5,.5,0]:[.5,i===1||i===2?1:.5,.5];for(let axis=0;axis<3;axis++){const values=vertices.filter((_,j)=>j%8===axis);assert(Math.abs(Math.min(...values)+half[axis])<1e-9);assert(Math.abs(Math.max(...values)-half[axis])<1e-9);}
 }
 assert.equal(builtinPrimitiveAsset('asset://missing'),null);
});
test('runtime primitive commands and cloned built-in references validate atomically',()=>{
 const source={entities:[]},packet={generation:1,operations:builtinPrimitiveNames.map((_,i)=>({kind:'primitive',id:id(i+1),data:i}))};const created=applyScriptOperations(source,packet,1);assert.equal(created.spawned,6);assert.equal(created.scene.entities.length,6);assert.deepEqual(source,{entities:[]});
 for(const e of created.scene.entities)validateColliderShape(e.collider);
 const copied=structuredClone(created.scene.entities[1]);copied.id=id(10);const clone=applyScriptOperations(created.scene,{generation:1,operations:[{kind:'import',data:[copied]}]},1);assert.equal(clone.scene.entities.length,7);assert.equal(clone.scene.entities.at(-1).renderable.assetId,builtinPrimitiveIds[1]);
 for(const type of [-1,6,1.5,null])assert.throws(()=>applyScriptOperations(source,{generation:1,operations:[{kind:'primitive',id:id(1),data:type}]},1));assert.deepEqual(source,{entities:[]});
 for(const value of [{...copied.collider,dimension:2},{...copied.collider,halfExtents:[.5,.1,.5]},{...builtinPrimitiveEntity(id(1),4).collider,halfExtents:[5,.1,5]}])assert.throws(()=>validateColliderShape(value));
});
test('real kernel builds render packets for every built-in without external assets',async()=>{
 const kernel=await loadKernel(bytes);try{const scene={entities:builtinPrimitiveNames.map((_,i)=>{const entity=builtinPrimitiveEntity(id(i+1),i);entity.transform.position=[i*3,0,0];return entity;})};const draws=kernel.compileScene(scene,new Map());assert.equal(draws.length,6);assert(draws.every(draw=>draw.vertices instanceof Float32Array&&draw.vertices.length>0));kernel.configurePhysics(scene);assert.equal(kernel.physicsSnapshot().bodies.length,6);const hidden=structuredClone(scene);hidden.entities[0].runtimeMeshFilter=false;assert.equal(kernel.compileScene(hidden,new Map()).length,5);}finally{kernel.dispose();}
});
test('real Wasm capsule rests on a finite plane and raycasts use rounded geometry',async()=>{
 const kernel=await loadKernel(bytes);try{const ground=builtinPrimitiveEntity(id(1),4),capsule=builtinPrimitiveEntity(id(2),1);capsule.transform.position=[0,3,0];capsule.rigidBody={mass:1,velocity:[0,0,0],restitution:0,friction:.5,gravityScale:1,freezeRotation:true};const scene={entities:[ground,capsule]},original=structuredClone(scene);kernel.compileScene(scene,new Map());kernel.configurePhysics(scene);for(let i=0;i<30;i++)kernel.stepPhysics(8);const body=kernel.physicsSnapshot().bodies.find(b=>b.id===capsule.id);assert(Math.abs(body.position[1]-1)<.03);assert(Math.abs(body.velocity[1])<.01);assert.equal(kernel.raycast({origin:[0,4,0],direction:[0,-1,0]}).entityId,capsule.id);assert.equal(kernel.raycast({origin:[7,4,0],direction:[0,-1,0]}),null);assert.deepEqual(scene,original);}finally{kernel.dispose();}
});
test('collapsed primitive colliders leave physics safely and recover after expanding',async()=>{
 const kernel=await loadKernel(bytes);try{for(const type of [0,1,2,3,4,5]){const entity=builtinPrimitiveEntity(id(1),type),scene={entities:[entity]};entity.transform.scale=[0,0,0];kernel.compileScene(scene,new Map());kernel.configurePhysics(scene);assert.equal(kernel.physicsSnapshot().bodies.length,0);entity.transform.scale=[1,1,1];kernel.compileScene(scene,new Map());kernel.configurePhysics(scene);assert.equal(kernel.physicsSnapshot().bodies.length,1);}}finally{kernel.dispose();}
});
