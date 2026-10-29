using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-object-lifetime-fixtures.mjs.
public static class ObjectLifetimeReference {
 public const string SourceSha256="370b0dbc016d005ef6b0a7008d65a8208d28b1169000995767a58bfa7678a4c0";
 public const string EvidenceSha256="88787170df05d712f99401b80e4b2f0e997867ac706e391f2e11283aae278114";
 public const int ExpectedCount=122;
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACpXWy27jIBQG4H3fxVa4zaK7WXR200U78wDUPnE9whABHjVSHr5S6jS+AOd4E0Xi5wfsz7IPdacHqNzbP2hi3WhjLpc/foTH75+HwyKijxF8dfR6gMvllzYBHme/D4e6ccPJWbDZtnsg0TUPRq9tODo/5JrugUXTNgcf2d1cx5CNDBDec/OvY8h8Oxozzc8MIgUeYu+hXXQsr/otgd6e0b650bbZ7dzGiztiuBq2Rw3D1DCqGoapYUQ1rKCGEdSwghpGUMNKahhBDUPVMKoahqhhVDUnF/rY/4eqBaPPuYuzSn11Xv+nN2eh03jrKoUcmSeEYym0co18fYE4VTnHlHOicj5XnlgFZc7nzDODSEHBOSc456hzTnXOEeec5lyQ7Ig9dgRqR1DtCMyOINoRJTuCYEeU7AiCHVGyIwh2BGpHUO0IxI6g2mnBnisdAsSQrZpninUSf1HLPQ4l9qKWVIYSYyiJDGXhRS0JCmXhRS0JCGUJoSQglChCSUUoEYSShlDhatQeNQpTo6hqFKZGEdWoghpFUKMKahRBjSqpUQQ1ClWjqGoUokZR1VA+7zYp5PNOkT7vNinkyD9q71z69s9ap9RojdMttMnkFKrKwkt5rL55702bbl0e6St4r9sce9GI7TjRndx0dpXsA7aqvuZWFyFRd39eXs8hwlD/9N04gI3PozFPHw2cYu/sRuU0kbDfxfP01/bx/GS73kL9uw+ht90LHMGDbWC92ArWVENYcfHY7TzVbS62zCdJtHeM4REAAA=="));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var row in reader.ReadToEnd().Split('\n')){var values=row.Split('|');if(values.Length!=3)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2]));}return result;}
 public static string Compare(ObjectLifetimeCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  return row.data==expected.Data?"":"Expected "+expected.Data+", actual "+row.data;
 }

}
}
