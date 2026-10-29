using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-sync-clone-fixtures.mjs.
public static class SyncCloneReference {
 public const string SourceSha256="a622968b595634077a32acc595a39dc9f7617d353c1510f2dc33c1bf48d6ee65";
 public const string EvidenceSha256="d52a7ac3178b58d5bbef3d1d21116da5d733d0f65eb78f448db7a3d5347007b1";
 public const int ExpectedCount=91;
 private static readonly Dictionary<string,(string Error,string Data,string Meta)> Expected=Load();
 private static Dictionary<string,(string Error,string Data,string Meta)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACtWaS2/aQBSF9/krVrFn/I7UxRhsSqqUMY9S2IEZHD+wYWzziPzjK5OkJWkaLCutbhYzCwbQ0Vx9OveOjtTizC/iOS9L6RZt3am3iO2OxtrIussDhXLrLh/pWDVT4UumiHRPiN8xCCHWabm9qAjvNTP/oQjdznZG/eIwPZ0TsdrcatnkczniBbt+sTnzOGPX+jWN50fGHz5DZ0dXUmuf8ngJU1pSxPGnzZyzJC9LQtqkEkDsalkkGGqUcoxWoTLtbyKhrTv+uwo8bW/q89L1Jk1O6sBeX5YW3GPl8JjlbN0i3C/WLMntg8c2eZAmZXkltTjLA86WT98dJ0F+tBM/SFjrNsiyIPEHbMU4Szz2+g/B1gj9Zu/0X+6jth4R19HEpcOJa4yQjeKxECxNme4PhC7E/cH1RT0aHul26t4ssL1LfYMnypGOhc7q/r0qhJ7YgyjtBXsvBEqVwHklsHsSaFYCq3P7sa5tQtXciTrSOqK2Ochn32m6Wy3rCrxc1zP2wF7fZfZQU/bQK+wBqxGG7Hv4ib0bS8tcK8TOQlBGE8si/tai23lgdaSo1+sLs9ubguK9/9VrB9u5L9ICTV1nuPl2r2/dQVfAvULcxd54FoqDWGPCVOxiejj2zZHRWBps38OwfQ/XZQ83ZQ/D9z0Zsu/JcH1PBu57Mmzfk+uyJzdlT4bvewpk31Pg+p4C3PcU2L6n1GVPacqeAt/3VMjsqXDfWlTg7Kmw2VPrsqc2ZU+Fz5523nP+P20Pxbks7lfPCU8abPa05z0n0Ou7zJ7WlD0NPnv6G/PeP+yH61VI/+u8B0Aa7HlPf3Peg3J9l9nTm7Knw5/3DMg9pwF33jOA+54Bu+c06rJnNGXPgO97JuSe04Tbc5rA2TNh95xmXfbMpuyZ8NlDEmT4kASXPgQ92oIk2Pyh2uEW1Djdgj5CvOUs3xJbLnEDRxqPHT8bFmK/sBENN9NJqLpdngqpFofv2hdfTkFADriAT7g8i7iALG5dBJuHXD5AygVhyE8vCMN9e0F/BF3glRb06wuqHXVBjbMu6LWwC6w6/QQR1minYC0AAA=="));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string,string)>();foreach(var row in reader.ReadToEnd().Split('\n')){var values=row.Split('|');if(values.Length!=4)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2],values[3]));}return result;}
 public static string Compare(SyncCloneCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.meta!=expected.Meta)return "Clone metadata differs: expected "+expected.Meta+", actual "+row.meta;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  if(row.error!="")return "";var a=Decode(row.data);var b=Decode(expected.Data);if(a.Length!=b.Length)return "Result length differs";
  for(var i=0;i<a.Length;i++){
   if(i==3||i==10){var dot=0f;for(var n=0;n<4;n++)dot+=a[i+n]*b[i+n];var sign=dot<0?-1:1;for(var n=0;n<4;n++)if(!Near(a[i+n]*sign,b[i+n]))return "Quaternion at "+i;i+=3;continue;}
   if(!Near(a[i],b[i]))return "Value["+i+"]: expected "+b[i].ToString("R",System.Globalization.CultureInfo.InvariantCulture)+", actual "+a[i].ToString("R",System.Globalization.CultureInfo.InvariantCulture);
  }return "";
 }
 private static bool Near(float a,float b)=>float.IsNaN(b)?float.IsNaN(a):float.IsInfinity(b)?a==b:Math.Abs(a-b)<=.00002f*Math.Max(1,Math.Abs(b));
 private static float[] Decode(string text){var bytes=Convert.FromBase64String(text);if(!BitConverter.IsLittleEndian)for(var i=0;i<bytes.Length;i+=4)Array.Reverse(bytes,i,4);var values=new float[bytes.Length/4];Buffer.BlockCopy(bytes,0,values,0,bytes.Length);return values;}
}
}
