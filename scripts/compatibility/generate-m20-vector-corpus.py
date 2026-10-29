"""Generate our direct-call corpus from the frozen M20 interface inventory."""
import json
import sys
from pathlib import Path

root = Path(__file__).resolve().parents[2]
ledger = json.loads((root / 'docs/compatibility/m20-batches.json').read_text(encoding='utf-8'))
scalar = ['TransformPoint', 'InverseTransformPoint', 'TransformVector', 'InverseTransformVector', 'TransformDirection', 'InverseTransformDirection']
plural = ['TransformPoints', 'InverseTransformPoints', 'TransformVectors', 'InverseTransformVectors', 'TransformDirections', 'InverseTransformDirections']
records = [row for row in ledger['records'] if row['owner'] == 'UnityEngine.Transform' and row['member'] in scalar + plural]
assert len(records) == 24
bodies, catalog = [], []
for index, row in enumerate(records):
    method, declaration = row['member'], row['declaration']
    span, separate = method in plural, 'ReadOnlySpan' in declaration
    if not span:
        args = '1.25f,-2,3.5f' if 'float x,' in declaration else 'new Vector3(1.25f,-2,3.5f)'
        body = 'return V(t.' + method + '(' + args + '));'
    else:
        body = 'var input=size==1?new Vector3[0]:new[]{new Vector3(1.25f,-2,3.5f),new Vector3(-3,1,.5f)};'
        if separate:
            body += 'var output=new Vector3[size==1?0:size==2?1:size==3?3:2];for(var n=0;n<output.Length;n++)output[n]=new Vector3(7,8,9);t.' + method + '(new ReadOnlySpan<Vector3>(input),new Span<Vector3>(output));return Flatten(output).Concat(Flatten(input)).ToArray();'
        else:
            body += 't.' + method + '(new Span<Vector3>(input));return Flatten(input);'
    bodies.append('case ' + str(index) + ':{' + body + '}')
    profiles = ['regular', 'mirrored', 'collapsed', 'local-mirror', 'double-mirror', 'deep-mirror', 'deep-collapse', 'collapsed-child', 'destroyed'] + (['empty'] if span else []) + (['short', 'long'] if separate else [])
    catalog.append({'index': index, 'id': row['id'], 'declaration': declaration, 'profiles': profiles, 'certified': False})

code = '''using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Game {
// Owned direct public calls. Identical source executes in native Unity, Axiom .NET and Wasm.
public static class VectorTransformCorpus {
 [Serializable] public sealed class Observation {public int index;public string profile="",data="",error="";}
 public static IEnumerable<Observation> Run(){
  for(var index=0;index<24;index++){
   foreach(var profile in new[]{"regular","mirrored","collapsed","local-mirror","double-mirror","deep-mirror","deep-collapse","collapsed-child","destroyed"})yield return Evaluate(index,profile);
   if(IsSpan(index))yield return Evaluate(index,"empty");
   if(IsSeparate(index)){yield return Evaluate(index,"short");yield return Evaluate(index,"long");}
  }
 }
 private static bool IsSpan(int index)=>new[]{SPAN}.Contains(index);
 private static bool IsSeparate(int index)=>new[]{SEPARATE}.Contains(index);
 private static Observation Evaluate(int index,string profile){
  var row=new Observation{index=index,profile=profile};var root=new GameObject("Vector reference parent");var middle=new GameObject("Vector reference middle");middle.transform.SetParent(root.transform,false);var child=new GameObject("Vector reference child");child.transform.SetParent(middle.transform,false);
  root.transform.position=new Vector3(3,-2,5);root.transform.rotation=Quaternion.Euler(0,30,15);root.transform.localScale=(profile=="mirrored"||profile=="double-mirror"||profile=="deep-mirror")?new Vector3(-2,1,3):profile=="collapsed"?new Vector3(0,2,1):new Vector3(2,1,3);
  if(profile=="deep-mirror"||profile=="deep-collapse"){middle.transform.localPosition=new Vector3(0,1,0);middle.transform.localRotation=Quaternion.Euler(10,15,20);middle.transform.localScale=profile=="deep-mirror"?new Vector3(1,-2,3):new Vector3(0,2,1);}
  var target=child.transform;target.localPosition=new Vector3(1,2,3);target.localRotation=Quaternion.Euler(20,40,45);target.localScale=profile=="local-mirror"||profile=="double-mirror"?new Vector3(-.5f,2,3):profile=="collapsed-child"?new Vector3(0,2,3):new Vector3(.5f,2,3);
  if(profile=="destroyed")UnityEngine.Object.DestroyImmediate(root);
  try{var result=Invoke(index,target,profile=="empty"?1:profile=="short"?2:profile=="long"?3:0);row.data=Encode(profile=="destroyed"?result:State(target).Concat(result).ToArray());}catch(Exception error){row.error=error.GetType().FullName;}finally{if(root)UnityEngine.Object.DestroyImmediate(root);}
  return row;
 }
 private static float[] Invoke(int index,Transform t,int size){switch(index){BODIES default:throw new ArgumentOutOfRangeException(nameof(index));}}
 private static float[] V(Vector3 v)=>new[]{v.x,v.y,v.z};private static float[] Q(Quaternion q)=>new[]{q.x,q.y,q.z,q.w};
 private static float[] Flatten(Vector3[] values)=>values.SelectMany(V).ToArray();
 private static float[] State(Transform t)=>V(t.position).Concat(Q(t.rotation)).Concat(V(t.localPosition)).Concat(Q(t.localRotation)).Concat(V(t.localScale)).ToArray();
 private static string Encode(float[] values){var bytes=new byte[values.Length*4];Buffer.BlockCopy(values,0,bytes,0,bytes.Length);if(!BitConverter.IsLittleEndian)for(var i=0;i<bytes.Length;i+=4)Array.Reverse(bytes,i,4);return Convert.ToBase64String(bytes);}
}
}
'''.replace('SPAN', ','.join(str(x['index']) for x in catalog if len(x['profiles']) > 9)).replace('SEPARATE', ','.join(str(x['index']) for x in catalog if len(x['profiles']) > 10)).replace('BODIES', '\n'.join(bodies))
target = root / 'demos/scripts/VectorTransformCorpus.cs'
catalog_path = root / 'docs/compatibility/evidence/m20-vector-transform-cases.json'
expected = {'schemaVersion': 1, 'targetSignatures': 24, 'expectedObservations': sum(len(x['profiles']) for x in catalog), 'records': catalog}
if '--check' in sys.argv:
    assert target.read_text(encoding='utf-8') == code, 'Stale direct-call corpus'
    actual = json.loads(catalog_path.read_text(encoding='utf-8'))
    assert actual['targetSignatures'] == 24 and actual['expectedObservations'] == expected['expectedObservations']
    assert [{k: row[k] for k in ['index', 'id', 'declaration', 'profiles']} for row in actual['records']] == [{k: row[k] for k in ['index', 'id', 'declaration', 'profiles']} for row in catalog], 'Stale frozen call attribution'
else:
    target.write_bytes(code.encode())
    catalog_path.write_bytes((json.dumps(expected, indent=2) + '\n').encode())
print(json.dumps({'directSignatures': 24, 'observations': expected['expectedObservations']}))
