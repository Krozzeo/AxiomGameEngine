import {colliderInstances} from '../../../engine/scripting/collider-instances.mjs';
import {componentToggle} from './component-activation.mjs';
import {componentDrag} from './reference-fields.mjs';
// Runtime-only cards. Their edits stay in the Play projection and disappear on Stop.
export function mountRuntimeColliders({document,getEntity,getScene,projectId,isPlaying,onChange,onToggle,reportError}){
 let host=null,signature='';
 function draw(){const entity=getEntity(),records=isPlaying()?colliderInstances(entity??{}).filter(c=>c.slot!=='collider'):[];const next=JSON.stringify([entity?.id,records.map(c=>[c.slot,c.type,c.enabled,c.collider])]);if(next===signature)return;if(host?.contains(document.activeElement))return;signature=next;host?.remove();host=null;if(!records.length)return;
  host=document.createElement('section');host.id='runtime-collider-components';const anchor=document.getElementById('physics-component');if(!anchor)return;anchor.after(host);
  for(const descriptor of records){const card=document.createElement('section');card.className='inspector-property';card.dataset.runtimeSlot=descriptor.slot;card.dataset.inspectorComponent=descriptor.type;const heading=document.createElement('h3');heading.className='property-heading';heading.textContent=descriptor.type+' #'+(colliderInstances(entity).filter(c=>c.type===descriptor.type).findIndex(c=>c.slot===descriptor.slot)+1);card.append(heading);host.append(card);
   componentToggle({document,card,heading,type:descriptor.type,runtimeSlot:descriptor.slot,getEntity,onChange:onToggle});componentDrag(heading,{scene:getScene,projectId,entityId:()=>entity.id,type:descriptor.type,runtimeSlot:descriptor.slot});
   const change=(property,value)=>Promise.resolve(onChange(entity.id,descriptor.slot,{...descriptor.collider,[property]:value})).catch(reportError);
   for(const [name,value]of Object.entries(descriptor.collider)){if(name==='halfExtents'){const row=document.createElement('label');row.textContent='Half extents';for(let axis=0;axis<3;axis++){const input=document.createElement('input');input.type='number';input.step='0.1';input.value=String(value[axis]);input.setAttribute('aria-label','Half extent '+['X','Y','Z'][axis]);input.addEventListener('change',()=>{const half=[...descriptor.collider.halfExtents];half[axis]=Number(input.value);void change(name,half);});row.append(input);}card.append(row);}else if(name==='trigger'){const row=document.createElement('label');row.textContent='Is trigger';const input=document.createElement('input');input.type='checkbox';input.checked=value;input.addEventListener('change',()=>void change(name,input.checked));row.append(input);card.append(row);}}
  }
 }
 return {draw};
}
