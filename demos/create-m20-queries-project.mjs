import {readFile} from 'node:fs/promises';
import {SceneWorkspace} from '../daemon/bootstrap/scene-workspace.mjs';
import {ProjectStore} from '../daemon/bootstrap/project-store.mjs';
export async function createM20QueriesProject(root,{folders=false}={}){
 const workspace=new SceneWorkspace(new ProjectStore(root,{folders}));await workspace.run('project.create',{name:'Demo · M20 Component Queries Lab',dimension:3});
 const run=(type,data={})=>workspace.run(type,{id:workspace.project.id,expectedSceneRevision:workspace.revision,...data});
 if(!workspace.project.scene.projectFiles.some(file=>file.path==='Scripts'))await run('project.files.edit',{action:'create',kind:'folder',path:'Scripts'});
 for(const name of ['ComponentQueryCorpus','ComponentQueryReference','ComponentQueryLab']){const path='Scripts/'+name+'.cs';await run('project.files.edit',{action:'create',kind:'script',path});await run('project.files.edit',{action:'write',path,text:await readFile(new URL('./scripts/'+name+'.cs',import.meta.url),'utf8')});}
 await run('scene.entity.create',{name:'Component Queries Controller'});const controller=workspace.project.scene.entities.at(-1),path='Scripts/ComponentQueryLab.cs';await run('scene.script.edit',{entityId:controller.id,path,action:'attach'});await run('scene.save');return {id:workspace.project.id,probeId:controller.id,path,name:workspace.project.name};
}
