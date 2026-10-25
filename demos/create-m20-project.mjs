import {readFile} from 'node:fs/promises';
import {fileURLToPath} from 'node:url';
import {SceneWorkspace} from '../daemon/bootstrap/scene-workspace.mjs';
import {ProjectStore} from '../daemon/bootstrap/project-store.mjs';
import {HubSettings} from '../daemon/bootstrap/hub-settings.mjs';
export async function createM20Project(root,{folders=false}={}){
 const w=new SceneWorkspace(new ProjectStore(root,{folders}));await w.run('project.create',{name:'Demo · M20 Object Lifecycle Lab',dimension:3});
 const run=(type,data={})=>w.run(type,{id:w.project.id,expectedSceneRevision:w.revision,...data});
 const cube=w.project.scene.entities.find(e=>e.renderable);
 await run('scene.entity.update',{entityId:cube.id,name:'Clone template'});
 await run('scene.collider.set',{entityId:cube.id,value:{dimension:3,shape:'box',halfExtents:[.5,.5,.5],trigger:false,layer:1,mask:4294967295}});
 await run('scene.entity.create',{name:'Object Lifecycle Controller'});
 const controller=w.project.scene.entities.find(e=>e.name==='Object Lifecycle Controller');
 if(!w.project.scene.projectFiles.some(f=>f.path==='Scripts'))await run('project.files.edit',{action:'create',kind:'folder',path:'Scripts'});
 const path='Scripts/ObjectLifecycleLab.cs';await run('project.files.edit',{action:'create',kind:'script',path});
 await run('project.files.edit',{action:'write',path,text:await readFile(new URL('./scripts/ObjectLifecycleLab.cs',import.meta.url),'utf8')});
 const helper='Scripts/ObjectGraphProbe.cs';await run('project.files.edit',{action:'create',kind:'script',path:helper});await run('project.files.edit',{action:'write',path:helper,text:await readFile(new URL('./scripts/ObjectGraphProbe.cs',import.meta.url),'utf8')});
 await run('scene.script.edit',{entityId:controller.id,path,action:'attach'});
 await run('scene.script.edit',{entityId:controller.id,path,action:'values',values:{Template:{entityId:cube.id}}});
 const transformPath='Scripts/TransformContractsLab.cs';await run('project.files.edit',{action:'create',kind:'script',path:transformPath});await run('project.files.edit',{action:'write',path:transformPath,text:await readFile(new URL('./scripts/TransformContractsLab.cs',import.meta.url),'utf8')});
 await run('scene.entity.create',{name:'Transform Contracts Controller'});const transformController=w.project.scene.entities.find(e=>e.name==='Transform Contracts Controller');await run('scene.script.edit',{entityId:transformController.id,path:transformPath,action:'attach'});
 await run('scene.save');return {id:w.project.id,probeId:controller.id,transformProbeId:transformController.id,path,name:w.project.name};
}
if(process.argv[1]===fileURLToPath(import.meta.url))console.log(JSON.stringify(await createM20Project((await new HubSettings().load()).projectRoot,{folders:true})));
