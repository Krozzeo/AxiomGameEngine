import test from 'node:test';
import assert from 'node:assert/strict';
import {applyScriptOperations} from '../engine/scripting/operations.mjs';
import {validateReference} from '../engine/scripting/references.mjs';
import {builtinPrimitiveEntity,builtinPrimitiveIds} from '../engine/scene/builtin-primitives.mjs';
import {scenePrimitives} from '../engine/wasm/host.mjs';
const id='entity://11111111-1111-4111-8111-111111111111',path='Scripts/AssetProbe.cs',ref='axiom://runtime/assets/41';
const record=()=>({id:41,path:ref,type:'Mesh',name:'Owned copy',owned:true,sourceAssetId:builtinPrimitiveIds[3]});
const scene=()=>({entities:[{...builtinPrimitiveEntity(id,3),scriptComponents:[{path,values:{Target:null}}]}],projectFiles:[{path,kind:'script',text:'using UnityEngine;public class AssetProbe:MonoBehaviour {public Mesh Target;}'}]});
const bind=()=>({generation:1,runtimeAssets:[record()],operations:[{kind:'mesh',id,data:{objectId:41,sourceAssetId:builtinPrimitiveIds[3]}}],scriptValues:[{entityId:id,path,values:{Target:{path:ref}}}]});
test('owned runtime mesh identity survives snapshots and binds actual immutable resource geometry',()=>{
 const source=scene(),before=structuredClone(source),result=applyScriptOperations(source,bind(),1).scene;
 assert.equal(result.entities[0].runtimeMeshObjectId,41);assert.equal(result.entities[0].scriptComponents[0].values.Target.path,ref);assert.equal(result.runtimeAssets[0].name,'Owned copy');
 const original=scenePrimitives(source,new Map()),copy=scenePrimitives(result,new Map());assert.deepEqual(copy.map(draw=>draw.vertices),original.map(draw=>draw.vertices));assert(copy.length>0);assert.deepEqual(source,before);
 assert.throws(()=>validateReference(result,{reference:'asset',type:'Mesh'},{path:ref}),/compatible/,'authoring must reject session-only handles');
 const next=applyScriptOperations(result,{generation:1,operations:[],scriptValues:[{entityId:id,path,values:{Target:{path:ref}}}]},1).scene;assert.equal(next.entities[0].scriptComponents[0].values.Target.path,ref);
});
test('asset retirement removes geometry without removing MeshFilter or Renderer components',()=>{
 const source=applyScriptOperations(scene(),bind(),1).scene;
 const result=applyScriptOperations(source,{generation:1,runtimeAssets:[],operations:[{kind:'mesh',id,data:{objectId:0,sourceAssetId:''}}],scriptValues:[{entityId:id,path,values:{Target:null}}]},1).scene;
 assert.equal(result.entities[0].runtimeMeshFilter,true);assert.equal(result.entities[0].renderable.kind,'mesh');assert.equal(result.entities[0].runtimeMeshAssigned,false);assert.equal(scenePrimitives(result,new Map()).length,0);assert.equal(source.entities[0].runtimeMeshObjectId,41);
 assert.throws(()=>applyScriptOperations(source,{generation:1,runtimeAssets:[],operations:[]},1),/live runtime mesh/);
});
test('registry, assignment and reference validation reject malformed asset packets atomically',()=>{
 const mutations=[p=>p.runtimeAssets[0].id=-1,p=>p.runtimeAssets.push(record()),p=>p.runtimeAssets[0].path+='x',p=>p.runtimeAssets[0].type='TextAsset',p=>p.runtimeAssets[0].sourceAssetId='asset://missing',p=>p.runtimeAssets[0].name='x'.repeat(4097),p=>p.operations[0].data.sourceAssetId=builtinPrimitiveIds[0],p=>p.operations[0].data.objectId=42,p=>p.scriptValues[0].values.Target.path+='stale',p=>p.runtimeAssets[0].extra=true];
 for(const mutate of mutations){const source=scene(),before=structuredClone(source),packet=bind();mutate(packet);assert.throws(()=>applyScriptOperations(source,packet,1));assert.deepEqual(source,before);}
});
