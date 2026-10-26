// Owned immutable meshes use the same normalized asset representation as imports.
export const builtinPrimitiveNames=['Sphere','Capsule','Cylinder','Cube','Plane','Quad'];
export const builtinPrimitiveIds=[
 'asset://a4e7bf07e6b0af06a37bff887e47cd30ccaa82c1dc61ee4600b2f77568a6dbb8',
 'asset://454a343948c0661a09f1b6dbbf2f42b5f33897420ac1f80dc7036ccc20cf168a',
 'asset://3b0a942875d3bf88ad8589b2c4f310aa4d59792d119f093d3f54fe3eb73735b6',
 'asset://797106b5b49558fb07ad54a058005627cdcf201d0afc4bc88f294d0296ba6378',
 'asset://af4460fe5fa74a628838c5635391163d7f080ef3fc9e8063f9af9f84936b3de4',
 'asset://8c831db589568c0dd95db5ae845e53bf329de0d40d83b19aba21ecba58fd5579'];
const cache=new Map();
export function builtinPrimitiveAsset(id){
 const type=builtinPrimitiveIds.indexOf(id);if(type<0)return null;if(cache.has(id))return cache.get(id);
 const vertices=[];const triangle=(a,b,c)=>{const u=b.map((v,i)=>v-a[i]),v=c.map((v,i)=>v-a[i]),n=[u[1]*v[2]-u[2]*v[1],u[2]*v[0]-u[0]*v[2],u[0]*v[1]-u[1]*v[0]],length=Math.hypot(...n);if(length<1e-12)return;for(const point of [a,b,c])vertices.push(...point,...n.map(x=>x/length),point[0]+.5,point[2]+.5);};
 const quad=(a,b,c,d)=>{triangle(a,b,c);triangle(a,c,d);};
 if(type===3){const points=[[-.5,-.5,-.5],[.5,-.5,-.5],[.5,-.5,.5],[-.5,-.5,.5],[-.5,.5,-.5],[.5,.5,-.5],[.5,.5,.5],[-.5,.5,.5]];for(const face of [[0,1,2,3],[4,7,6,5],[0,4,5,1],[1,5,6,2],[2,6,7,3],[3,7,4,0]])quad(...face.map(i=>points[i]));}
 else if(type===4){for(let z=0;z<10;z++)for(let x=0;x<10;x++)quad([x-5,0,z-5],[x-5,0,z-4],[x-4,0,z-4],[x-4,0,z-5]);}
 else if(type===5)quad([-.5,-.5,0],[-.5,.5,0],[.5,.5,0],[.5,-.5,0]);
 else if(type===2){const point=(i,y)=>[.5*Math.cos(i*Math.PI/16),y,.5*Math.sin(i*Math.PI/16)];for(let i=0;i<32;i++){quad(point(i,-1),point(i,1),point(i+1,1),point(i+1,-1));triangle([0,1,0],point(i+1,1),point(i,1));triangle([0,-1,0],point(i,-1),point(i+1,-1));}}
 else {const rings=[];if(type===0){for(let y=0;y<=24;y++)rings.push([.5*Math.sin(y*Math.PI/24),.5*Math.cos(y*Math.PI/24)]);}else {for(let y=0;y<=12;y++)rings.push([.5*Math.sin(y*Math.PI/24),.5+.5*Math.cos(y*Math.PI/24)]);for(let y=12;y<=24;y++)rings.push([.5*Math.sin(y*Math.PI/24),-.5+.5*Math.cos(y*Math.PI/24)]);}const point=(i,y)=>[rings[y][0]*Math.cos(i*Math.PI/16),rings[y][1],rings[y][0]*Math.sin(i*Math.PI/16)];for(let y=0;y<rings.length-1;y++)for(let i=0;i<32;i++)quad(point(i,y),point(i+1,y),point(i+1,y+1),point(i,y+1));}
 const asset=Object.freeze({id,kind:'mesh',name:builtinPrimitiveNames[type],primitives:Object.freeze([Object.freeze({vertices:Object.freeze(vertices),color:Object.freeze([.72,.76,.82,1]),texture:null,unlit:false,material:Object.freeze({metallic:0,roughness:.5,emissive:Object.freeze([0,0,0]),alphaMode:'opaque',alphaCutoff:.5})})])});cache.set(id,asset);return asset;
}
export function builtinPrimitiveEntity(id,type){
 if(!Number.isInteger(type)||type<0||type>=builtinPrimitiveNames.length)throw Error('AX_SCRIPT_0003: invalid primitive type');
 const shape=['sphere','capsule','capsule','box','plane','quad'][type],half=[[.5,.5,.5],[.5,1,.5],[.5,1,.5],[.5,.5,.5],[5,0,5],[.5,.5,0]][type];
 return {id,name:builtinPrimitiveNames[type],transform:{position:[0,0,0],rotation:[0,0,0,1],scale:[1,1,1]},renderable:{kind:'mesh',assetId:builtinPrimitiveIds[type]},collider:{dimension:3,shape,halfExtents:[...half],trigger:false,layer:1,mask:4294967295},runtimeParentId:null,runtimeMeshFilter:true,runtimeMeshAssetId:builtinPrimitiveIds[type],runtimeColliderLocal:true};
}
