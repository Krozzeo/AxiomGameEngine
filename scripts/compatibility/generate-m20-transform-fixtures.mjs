import {readFile,writeFile} from 'node:fs/promises';
const root=new URL('../../',import.meta.url);
const report=JSON.parse(await readFile(new URL('docs/compatibility/evidence/m20-unity-6000.3-native-observations.json',root),'utf8'));
if(report.unityVersion!=='6000.3.27f1'||report.coreAssemblySha256!=='42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737'||report.error||report.transforms.length!==128)throw Error('Exact native reference observations required');
const fields=['parentRotation','parentScale','childRotation','childScale','leafRotation','leafScale','worldRotation','worldScale','worldMatrix'];
const values=report.transforms.flatMap(row=>fields.flatMap(field=>row[field])),bytes=Buffer.alloc(values.length*4);values.forEach((value,i)=>bytes.writeFloatLE(value,i*4));
const source=`// Generated from our native Unity 6000.3.27f1 observations, never from vendor implementation code.
// Regenerate: node scripts/compatibility/generate-m20-transform-fixtures.mjs
using UnityEngine;
public static class NativeTransformFixtures {
 public const int Count=128;
 private static readonly byte[] Data=System.Convert.FromBase64String("${bytes.toString('base64')}");
 public static bool Check(int index){
  var row=new float[44];for(var i=0;i<44;i++){var offset=(index*44+i)*4;row[i]=System.BitConverter.Int32BitsToSingle(Data[offset]|Data[offset+1]<<8|Data[offset+2]<<16|Data[offset+3]<<24);}var parent=new GameObject("Native fixture parent");var child=new GameObject("Native fixture child");var leaf=new GameObject("Native fixture leaf");
  child.transform.SetParent(parent.transform,false);leaf.transform.SetParent(child.transform,false);
  parent.transform.localPosition=new Vector3(3,0,0);child.transform.localPosition=Vector3.right;
  parent.transform.localRotation=Q(row,0);parent.transform.localScale=V(row,4);child.transform.localRotation=Q(row,7);child.transform.localScale=V(row,11);leaf.transform.localRotation=Q(row,14);leaf.transform.localScale=V(row,18);
  var expected=Q(row,21);var actual=leaf.transform.rotation;var rotation=(actual*Vector3.right-expected*Vector3.right).magnitude<2e-5f&&(actual*Vector3.up-expected*Vector3.up).magnitude<2e-5f;
  var scale=leaf.transform.lossyScale;var good=rotation&&Near(scale.x,row[25])&&Near(scale.y,row[26])&&Near(scale.z,row[27]);var matrix=leaf.transform.localToWorldMatrix;
  for(var i=0;i<16;i++)good=good&&Near(matrix[i],row[28+i]);
  var copy=UnityEngine.Object.Instantiate(leaf);good=good&&(copy.transform.position-leaf.transform.localPosition).magnitude<2e-5f&&(copy.transform.localScale-leaf.transform.localScale).magnitude<2e-5f&&(copy.transform.right-leaf.transform.localRotation*Vector3.right).magnitude<2e-5f;UnityEngine.Object.DestroyImmediate(copy);
  copy=UnityEngine.Object.Instantiate(leaf,new Vector3(8,2,1),Quaternion.identity);good=good&&(copy.transform.position-new Vector3(8,2,1)).magnitude<2e-5f&&(copy.transform.localScale-leaf.transform.localScale).magnitude<2e-5f;UnityEngine.Object.DestroyImmediate(copy);
  UnityEngine.Object.DestroyImmediate(parent);return good;
 }
 static bool Near(float a,float b)=>System.Math.Abs(a-b)<=2e-5f+1e-5f*System.Math.Max(System.Math.Abs(a),System.Math.Abs(b));
 static Quaternion Q(float[] a,int i)=>new Quaternion(a[i],a[i+1],a[i+2],a[i+3]);
 static Vector3 V(float[] a,int i)=>new Vector3(a[i],a[i+1],a[i+2]);
}
`;
await writeFile(new URL('demos/scripts/NativeTransformFixtures.cs',root),source);
console.log(JSON.stringify({fixtures:128,bytes:Buffer.byteLength(source)}));
