import ast,json,sys
from pathlib import Path
root=Path(__file__).resolve().parents[2]
ledger=json.loads((root/'docs/compatibility/m20-batches.json').read_text(encoding='utf-8'))
properties=['position','localPosition','rotation','localRotation','localScale','eulerAngles','localEulerAngles','forward','right','up','lossyScale','localToWorldMatrix','worldToLocalMatrix']
methods=['Translate','Rotate','RotateAround','RotateAroundLocal','LookAt','SetPositionAndRotation','SetLocalPositionAndRotation','GetPositionAndRotation','GetLocalPositionAndRotation']
records=[r for r in ledger['records'] if r['owner']=='UnityEngine.Transform' and r['member'] in properties+methods]
source_ast=ast.parse((root/'scripts/compatibility/generate-m20-vector-corpus.py').read_text(encoding='utf-8'))
value=next(n.value for n in source_ast.body if isinstance(n,ast.Assign) and any(isinstance(t,ast.Name) and t.id=='code' for t in n.targets))
while isinstance(value,ast.Call):value=value.func.value
template=ast.literal_eval(value)
bodies=[];catalog=[];nullable=[];spaces=[];zeros=[]
for index,row in enumerate(records):
 name=row['member'];decl=row['declaration'];kind='none';is_nullable=False;is_space=False;is_zero=False
 if name in properties:
  if name in ['rotation','localRotation']:body='t.'+name+'=Quaternion.Euler(35,15,20);return Q(t.'+name+');';kind='quaternion'
  elif name in ['localToWorldMatrix','worldToLocalMatrix']:body='return M(t.'+name+');';kind='matrix'
  elif name=='lossyScale':body='return V(t.lossyScale);';kind='vector'
  else:
   values='new Vector3(8,2,-3)' if name in ['position','localPosition'] else 'new Vector3(-1,.75f,2)' if name=='localScale' else 'new Vector3(25,15,35)' if name in ['eulerAngles','localEulerAngles'] else '(profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1))'
   body='t.'+name+'='+values+';return V(t.'+name+');';kind='euler' if name in ['eulerAngles','localEulerAngles'] else 'vector';is_zero=name in ['forward','right','up']
 elif name in ['GetPositionAndRotation','GetLocalPositionAndRotation']:body='t.'+name+'(out var position,out var rotation);return V(position).Concat(Q(rotation)).ToArray();';kind='pose'
 elif name in ['SetPositionAndRotation','SetLocalPositionAndRotation']:body='t.'+name+'(new Vector3(8,2,-3),Quaternion.Euler(35,15,20));return new float[0];'
 elif name=='Translate':
  args='1.25f,-2,3.5f' if 'float x,' in decl else 'new Vector3(1.25f,-2,3.5f)'
  if 'UnityEngine.Transform relativeTo' in decl:args+=',relative';is_nullable=True
  elif 'UnityEngine.Space relativeTo' in decl:args+=',space';is_space=True
  body='t.Translate('+args+');return new float[0];'
 elif name=='Rotate':
  if 'UnityEngine.Vector3 axis' in decl:args='axis,35';is_zero=True
  elif 'float xAngle,' in decl:args='10,20,30'
  else:args='new Vector3(10,20,30)'
  if 'UnityEngine.Space relativeTo' in decl:args+=',space';is_space=True
  body='t.Rotate('+args+');return new float[0];'
 elif name in ['RotateAround','RotateAroundLocal']:
  args='new Vector3(1,0,2),axis,35' if 'UnityEngine.Vector3 point' in decl else 'axis,.35f';body='t.'+name+'('+args+');return new float[0];';is_zero=True
 elif name=='LookAt':
  is_nullable='UnityEngine.Transform target' in decl;is_zero=not is_nullable
  args='relative' if is_nullable else '(profile=="zero-input"?t.position:new Vector3(8,2,-3))'
  if 'worldUp' in decl:args+=',Vector3.up'
  body='t.LookAt('+args+');return new float[0];'
 else:raise Exception(decl)
 bodies.append('case '+str(index)+':{'+body+'}')
 profiles=['regular','mirrored','collapsed','local-mirror','double-mirror','deep-mirror','deep-collapse','collapsed-child','destroyed']
 if is_nullable:nullable.append(index);profiles.append('null')
 if is_space:spaces.append(index);profiles+=['world','world-mirrored']
 if is_zero:zeros.append(index);profiles.append('zero-input')
 catalog.append({'index':index,'id':row['id'],'declaration':decl,'profiles':profiles,'resultKind':kind,'certified':False})
code=template.replace('VectorTransformCorpus','PoseTransformCorpus').replace('index<24','index<'+str(len(records)))
code=code.replace('if(IsSpan(index))yield return Evaluate(index,"empty");','if(IsNullable(index))yield return Evaluate(index,"null");').replace('if(IsSeparate(index)){yield return Evaluate(index,"short");yield return Evaluate(index,"long");}','if(IsSpace(index)){yield return Evaluate(index,"world");yield return Evaluate(index,"world-mirrored");}if(IsZero(index))yield return Evaluate(index,"zero-input");')
code=code.replace('private static bool IsSpan(int index)=>new[]{SPAN}.Contains(index);','private static bool IsNullable(int index)=>new[]{'+','.join(map(str,nullable))+'}.Contains(index);').replace('private static bool IsSeparate(int index)=>new[]{SEPARATE}.Contains(index);','private static bool IsSpace(int index)=>new[]{'+','.join(map(str,spaces))+'}.Contains(index);private static bool IsZero(int index)=>new[]{'+','.join(map(str,zeros))+'}.Contains(index);')
code=code.replace('if(profile=="destroyed")UnityEngine.Object.DestroyImmediate(root);','var relative=profile=="null"?null:middle.transform;if(profile=="destroyed")UnityEngine.Object.DestroyImmediate(root);')
code=code.replace('Invoke(index,target,profile=="empty"?1:profile=="short"?2:profile=="long"?3:0)','Invoke(index,target,profile,relative)').replace('Invoke(int index,Transform t,int size){switch(index){BODIES','Invoke(int index,Transform t,string profile,Transform relative){var space=profile.StartsWith("world")?Space.World:Space.Self;var axis=profile=="zero-input"?Vector3.zero:new Vector3(1,2,-1);switch(index){'+ '\n'.join(bodies))
code=code.replace('profile=="mirrored"||profile=="double-mirror"||profile=="deep-mirror"','profile=="mirrored"||profile=="world-mirrored"||profile=="double-mirror"||profile=="deep-mirror"')
code=code.replace('private static float[] Flatten(Vector3[] values)', 'private static float[] M(Matrix4x4 value){var data=new float[16];for(var i=0;i<16;i++)data[i]=value[i];return data;}\n private static float[] Flatten(Vector3[] values)')
expected={'schemaVersion':1,'targetSignatures':len(records),'expectedObservations':sum(len(x['profiles']) for x in catalog),'records':catalog}
target=root/'demos/scripts/PoseTransformCorpus.cs';path=root/'docs/compatibility/evidence/m20-pose-transform-cases.json'
if '--check' in sys.argv:
 assert target.read_text(encoding='utf-8')==code
 actual=json.loads(path.read_text(encoding='utf-8'))
 assert [{k:r[k] for k in ['index','id','declaration','profiles','resultKind']} for r in actual['records']]==[{k:r[k] for k in ['index','id','declaration','profiles','resultKind']} for r in catalog]
else:
 target.write_bytes(code.encode());path.write_bytes((json.dumps(expected,indent=2)+'\n').encode())
print(json.dumps({'directSignatures':len(records),'observations':expected['expectedObservations']}))
