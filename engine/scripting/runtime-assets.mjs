import {builtinPrimitiveAsset} from '../scene/builtin-primitives.mjs';
const fail=()=>{throw Error('AX_SCRIPT_0003: invalid runtime asset registry');};
export function runtimeAssets(scene,records){
 if(!Array.isArray(records)||records.length>512)fail();const ids=new Set();
 for(const item of records){
  if(!item||typeof item!=='object'||Array.isArray(item)||Object.keys(item).sort().join(',')!=='id,name,owned,path,sourceAssetId,type'||!Number.isSafeInteger(item.id)||item.id<1||item.id>2147483647||ids.has(item.id)||item.path!=='axiom://runtime/assets/'+item.id||!['Mesh','TextAsset','Texture2D','AudioClip'].includes(item.type)||typeof item.name!=='string'||item.name.length>4096||typeof item.owned!=='boolean'||typeof item.sourceAssetId!=='string')fail();ids.add(item.id);
  if(item.sourceAssetId){const asset=scene.assets?.find(asset=>asset.id===item.sourceAssetId)??builtinPrimitiveAsset(item.sourceAssetId);if(!asset||asset.kind!==({Mesh:'mesh',Texture2D:'sprite',AudioClip:'audio'})[item.type])fail();}
 }
 return structuredClone(records);
}
export function assignRuntimeMesh(entity,data,assets){
 if(!data||Object.keys(data).sort().join(',')!=='objectId,sourceAssetId'||!Number.isSafeInteger(data.objectId)||data.objectId<0||typeof data.sourceAssetId!=='string'||entity.runtimeMeshFilter!==true)fail();
 const asset=data.objectId?assets?.find(item=>item.id===data.objectId&&item.type==='Mesh'):null;
 if(data.objectId?(!asset||asset.sourceAssetId!==data.sourceAssetId):data.sourceAssetId!=='')fail();
 entity.runtimeMeshObjectId=data.objectId;entity.runtimeMeshAssigned=data.sourceAssetId!=='';
 if(data.sourceAssetId){entity.runtimeMeshAssetId=data.sourceAssetId;entity.renderable={kind:'mesh',assetId:data.sourceAssetId};}
}
