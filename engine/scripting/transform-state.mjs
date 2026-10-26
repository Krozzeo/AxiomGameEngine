import {modelMatrix,matrixMultiply,quaternionMultiply} from '../renderer/render-math.mjs';
export function transformState(value){
 const fail=()=>{throw Error('AX_SCRIPT_0003: invalid transform matrix/local state');};
 if(!value||typeof value!=='object'||Array.isArray(value)||Object.keys(value).some(k=>!['matrix','local'].includes(k)))fail();
 const array=(v,n,bound=1000000)=>Array.isArray(v)&&v.length===n&&v.every(x=>Number.isFinite(x)&&Math.abs(x)<=bound);
 const m=value.matrix,t=value.local;if(!array(m,16)||Math.abs(m[3])+Math.abs(m[7])+Math.abs(m[11])>1e-9||Math.abs(m[15]-1)>1e-9||!t||typeof t!=='object'||Array.isArray(t)||Object.keys(t).length!==3||!array(t.position,3)||!array(t.rotation,4,1)||Math.abs(Math.hypot(...t.rotation)-1)>1e-5||!array(t.scale,3))fail();
 return {matrix:[...m],local:structuredClone(t)};
}
export function validateTransformStates(entities,touched){
 const byId=new Map(entities.map(entity=>[entity.id,entity]));const close=(a,b)=>a.length===b.length&&a.every((value,i)=>Math.abs(value-b[i])<=1e-5*Math.max(1,Math.abs(value),Math.abs(b[i])));
 for(const id of touched){const e=byId.get(id);if(!e?.runtimeMatrix)continue;const t=e.runtimeLocalTransform,parent=byId.get(Object.hasOwn(e,'runtimeParentId')?e.runtimeParentId:e.parentId),local=modelMatrix(t),expected=parent?matrixMultiply(parent.runtimeMatrix??modelMatrix(parent.transform),local):local;
  const rotation=parent?quaternionMultiply(parent.transform.rotation,t.rotation):t.rotation;const q=e.transform.rotation;const sameRotation=close(rotation,q)||close(rotation.map(v=>-v),q);
  const m=e.runtimeMatrix,det=m[0]*(m[5]*m[10]-m[9]*m[6])-m[4]*(m[1]*m[10]-m[9]*m[2])+m[8]*(m[1]*m[6]-m[5]*m[2]),scale=[Math.hypot(...m.slice(0,3))*(det<0?-1:1),Math.hypot(...m.slice(4,7)),Math.hypot(...m.slice(8,11))];
  if(!close(expected,m)||!close(m.slice(12,15),e.transform.position)||!close(scale,e.transform.scale)||!sameRotation)throw Error('AX_SCRIPT_0003: transform matrix/local state does not match hierarchy pose');
 }
}
