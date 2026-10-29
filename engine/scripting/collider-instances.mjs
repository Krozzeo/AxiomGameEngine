import {validateColliderShape} from '../scene/collider.mjs';
const kinds=['BoxCollider','SphereCollider','CapsuleCollider','MeshCollider','BoxCollider2D','CircleCollider2D'];
const allowed=new Set(['slot','type','collider','enabled','hasGeometry','meshObjectId']);
export function validateRuntimeColliders(records){
 if(!Array.isArray(records)||records.length>8)throw Error('Invalid runtime collider collection');const slots=new Set();
 for(const record of records){
  if(!record||typeof record!=='object'||Array.isArray(record)||Object.keys(record).some(key=>!allowed.has(key))||!Number.isSafeInteger(record.slot)||record.slot<1||record.slot>2147483647||slots.has(record.slot)||!kinds.includes(record.type)||typeof record.enabled!=='boolean'||typeof record.hasGeometry!=='boolean'||!Number.isSafeInteger(record.meshObjectId)||record.meshObjectId<0||record.meshObjectId>2147483647)throw Error('Invalid runtime collider instance');
  slots.add(record.slot);validateColliderShape(record.collider);if(typeof record.collider.trigger!=="boolean"||!Number.isSafeInteger(record.collider.layer)||record.collider.layer<1||record.collider.layer>4294967295||!Number.isSafeInteger(record.collider.mask)||record.collider.mask<0||record.collider.mask>4294967295||Object.keys(record.collider).some(key=>!["dimension","shape","halfExtents","trigger","layer","mask"].includes(key)))throw Error("Invalid runtime collider filter");
  const c=record.collider;if((record.type.endsWith('2D')?2:3)!==c.dimension)throw Error('Collider dimension does not match its component type');
  const shape=record.type==='SphereCollider'||record.type==='CircleCollider2D'?'sphere':record.type==='CapsuleCollider'?'capsule':'box';
  if(record.type!=='MeshCollider'&&(c.shape!==shape||record.meshObjectId!==0))throw Error('Collider geometry does not match its component type');
 }
}
export function colliderInstances(entity){
 const result=[];
 if(entity.collider){const c=entity.collider,type=entity.runtimeMeshCollider?'MeshCollider':c.dimension===2?(c.shape==='sphere'?'CircleCollider2D':'BoxCollider2D'):({sphere:'SphereCollider',capsule:'CapsuleCollider',plane:'MeshCollider',quad:'MeshCollider'})[c.shape]??'BoxCollider';result.push({slot:'collider',type,collider:c,enabled:entity.runtimeColliderEnabled!==false&&entity.componentEnabled?.collider!==false,hasGeometry:entity.runtimeColliderHasGeometry!==false,local:entity.runtimeColliderLocal===true});}
 for(const record of entity.runtimeColliders??[]){const slot='collider:'+record.slot;result.push({...record,slot,enabled:record.enabled&&entity.componentEnabled?.[slot]!==false,local:true});}
 return result;
}
export function colliderAssignable(type,actual){return type===actual||type==='Collider'&&!actual.endsWith('2D')||type==='Collider2D'&&actual.endsWith('2D');}
