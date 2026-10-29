using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-bulk-object-fixtures.mjs.
public static class BulkObjectReference {
 public const string SourceSha256="9be7161edd14fc80126a062c66944c32c16ad0f9d9a39848075b703395bc05c6";
 public const string EvidenceSha256="3b551c8bb5b5e3852f5d3a02021c7bdf7c3f368b80d180ad4a93e4cffd6a21fc";
 public const int ExpectedCount=50;
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACqWSzW6EIBSF9/MsjhGQzey6aF+gfQGi158EwcC1rY0P31gc4zRGGNwROAf4zj1ZKttPmCarB1PA7U1IC4nGBoxbX7K0HHrZFgL3VR9mmEWF7nqtQOGR6AeMPjo3gK2BcpXM2/8+A12P457g7wrioyEhNCSEhnhoiJ+GeGioj4aG0NAQGuqhoX4a6qUpoRKDxKswRozT+2gRuvTF1EMHCl+/C+ix1erA39peWygjL2C+OFlInCwkTuaJk/njZJ442ck42dk48yVOCarG5kaSqpUSSqdDI5SttOnsXYxfetXSB+2eIVlcLsbFlt1tW/UlTxXUAue/PI/gDq4Pz6wkbmybt9xkVtemCM9a1/GHGxttMGZIWtURtnPlys+Wiz9TLh5VLh5SLh5fLh5VLh5fLh5bLh5ZLh5XLn6yXPxkuX4BgX6S+vMIAAA="));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var row in reader.ReadToEnd().Split('\n')){var values=row.Split('|');if(values.Length!=3)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2]));}return result;}
 public static string Compare(BulkObjectCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  return row.data==expected.Data?"":"Expected "+expected.Data+", actual "+row.data;
 }

}
}
