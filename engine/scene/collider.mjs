export function validateColliderShape(value){
 const fail=()=>{throw Error('Invalid collider shape dimensions');};
 if(!value||![2,3].includes(value.dimension)||!['box','sphere','capsule','plane','quad'].includes(value.shape)||!Array.isArray(value.halfExtents)||value.halfExtents.length!==3||value.halfExtents.some(v=>!Number.isFinite(v)||v<0||v>10000))fail();
 const h=value.halfExtents;
 if(['box','sphere'].includes(value.shape)){if(h.some(v=>v<.001))fail();}
 else if(value.dimension!==3)fail();
 else if(value.shape==='capsule'){if(h[0]<.001||h[1]<h[0]||h[2]!==h[0])fail();}
 else if(value.shape==='plane'){if(h[0]<.001||h[1]!==0||h[2]<.001)fail();}
 else if(value.shape==='quad'){if(h[0]<.001||h[1]<.001||h[2]!==0)fail();}
 return value;
}
