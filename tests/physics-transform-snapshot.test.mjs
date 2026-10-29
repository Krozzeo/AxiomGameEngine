import test from 'node:test';
import assert from 'node:assert/strict';
import {readFile} from 'node:fs/promises';
import {loadKernel} from '../engine/wasm/host.mjs';
import {modelMatrix} from '../engine/renderer/render-math.mjs';
const bytes=await readFile(new URL('../target/wasm32-unknown-unknown/release/axiom_wasm.wasm',import.meta.url));
test('real physics packets publish current world, affine and local poses for the next script phase',async()=>{
 const id='entity://11111111-1111-4111-8111-111111111111',pose={position:[0,1,0],rotation:[0,0,0,1],scale:[1,1,1]};
 const body={id,name:'Dynamic script owner',transform:pose,runtimeLocalTransform:structuredClone(pose),runtimeMatrix:modelMatrix(pose),collider:{dimension:3,shape:'box',halfExtents:[.25,.5,.25],trigger:false,layer:1,mask:0xffffffff},rigidBody:{mass:1,velocity:[0,0,0],restitution:0,friction:.1,gravityScale:1}};
 const scene={entities:[body]},before=structuredClone(scene),kernel=await loadKernel(bytes);
 try{kernel.compileScene(scene,new Map());kernel.configurePhysics(scene);const packet=kernel.stepScene(1/60,1n,1,{draw:false}),actual=packet.transforms.find(e=>e.id===id);assert(actual.transform.position[1]<1);assert.deepEqual(actual.runtimeLocalTransform.position,actual.transform.position);assert.deepEqual(actual.runtimeMatrix.slice(12,15),actual.transform.position);assert.deepEqual(scene,before);}finally{kernel.dispose();}
});
