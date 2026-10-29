const validId=value=>Number.isSafeInteger(value)&&value>0&&value<=2147483647;

// IDs belong only to the live projection. Saved authoring components keep their
// existing schema, and Stop restores that original document exactly.
export function runtimeScriptComponent(components,path,runtimeId,{bind=false}={}){
 const matches=(components??[]).filter(component=>component.path===path);
 if(runtimeId!==undefined){
  if(!validId(runtimeId))throw Error('Invalid runtime Script identity');
  const existing=matches.find(component=>component.runtimeId===runtimeId);
  if(existing)return existing;
  if(bind&&matches.length===1&&matches[0].runtimeId===undefined){matches[0].runtimeId=runtimeId;return matches[0];}
  return null;
 }
 if(matches.length>1)throw Error('Choose a specific runtime Script instance');
 return matches[0]??null;
}

export function validateRuntimeScriptInstances(components){
 if(!Array.isArray(components)||components.length>8)throw Error('Runtime Script attachment limit exceeded');
 const identities=new Set(),paths=new Map();
 for(const component of components){
  if(component.runtimeId!==undefined){if(!validId(component.runtimeId)||identities.has(component.runtimeId))throw Error('Duplicate or invalid runtime Script identity');identities.add(component.runtimeId);}
  const group=paths.get(component.path)??[];group.push(component);paths.set(component.path,group);
 }
 for(const group of paths.values())if(group.length>1&&group.some(component=>component.runtimeId===undefined))throw Error('Repeated runtime Scripts require independent identities');
}
