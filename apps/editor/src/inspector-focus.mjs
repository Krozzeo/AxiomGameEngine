export function inspectorFocus(){let locked=false,target={entities:[],files:[]},scope=null;return {get locked(){return locked;},get target(){return structuredClone(target);},update(next,key){if(scope!==key){scope=key;locked=false;target={entities:[],files:[]};}if(!locked)target=structuredClone(next);return structuredClone(target);},toggle(next,key){this.update(next,key);locked=!locked;return locked;},copy(next,key,lock=false){scope=key;target=structuredClone(next);locked=lock;}};}

// Authoring refreshes do not contain objects created during Play. Keep those
// targets only while the same live project/workspace still owns the renderer.
export function selectionEntities(previous,next,runtime){
 const sameScope=previous?.project?.id===next?.project?.id&&(previous?.workspaceId??null)===(next?.workspaceId??null);
 return previous?.playing&&next?.playing&&sameScope&&runtime?.playing&&runtime.projectId===next.project?.id?runtime.scene.entities:next?.project?.scene.entities??[];
}

export function authoringRefreshIsCurrent(requested,current,incoming){
 return requested===current&&incoming?.project?.id===current?.project?.id&&(incoming?.workspaceId??null)===(current?.workspaceId??null)&&incoming?.playing===current?.playing&&incoming.sceneRevision>=current.sceneRevision&&(incoming.project?.revision??0)>=(current.project?.revision??0);
}

// Preserve ordered human runtime edits while the renderer finishes a prior edit.
// A stopped/restarted or different workspace must never receive a queued edit.
export async function performInspectorEdit({getState,getGeneration,isBusy,perform},operation){
 const initial=getState();if(!initial.playing)return perform(operation);const generation=getGeneration(),project=initial.project?.id,workspace=initial.workspaceId??null,deadline=performance.now()+15000;
 const current=()=>{const state=getState();if(!state.playing||state.project?.id!==project||(state.workspaceId??null)!==workspace||getGeneration()!==generation)throw Object.assign(Error('The Play session changed before this Inspector edit could be applied.'),{code:'AX_SCENE_0002'});};
 while(isBusy()){current();if(performance.now()>=deadline)throw Error('The previous Inspector edit did not finish. Retry when the editor is ready.');await new Promise(resolve=>setTimeout(resolve,10));}current();return perform(operation);
}
