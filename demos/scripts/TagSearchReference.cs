using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-tag-search-fixtures.mjs.
public static class TagSearchReference {
 public const string SourceSha256="b7f2398b04bf7bd3b78b8d34f6cbd9a025c4f350895a01fe354467274c8a892d";
 public const string EvidenceSha256="5d6f988565566e6a576ec4bf9e53b011f7355be838ab517175fe43c62d3f89ba";
 public const int ExpectedCount=39;
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACp3QXU6EMBAH8PeeYg8gDW1P4IM+GuPuHgBhZCeWgZSpLkkPb1AXCga72bem+c/Hb3L5bIsBXAgNNK/gRC4fkbA/hUDeWpHLI3FR11BNH57eqf2kcCTk4YFqJJA/73MJHWNLQeRyDIf90DM08t7VvgHiRQCajod/E0hFyfgB02QHjC7aBM6dxRI588sd1Rqllii1RqmrUCqFUkmUWqPUCqU2UXpCHXblCW11d9i5tmWhJ5zQEUzoq1A6hdJJlI5QQs+gsfQvRpgtiJkhJoaYqyAmBTFJiIkhJoKYDcg4MXPQe8v9pe+Tt/YF3sABlTA3/45a7PlSxkWdFVTdUP57jVs7fAGU4P1m8wMAAA=="));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var row in reader.ReadToEnd().Split('\n')){var values=row.Split('|');if(values.Length!=3)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2]));}return result;}
 public static string Compare(TagSearchCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  return row.data==expected.Data?"":"Expected "+expected.Data+", actual "+row.data;
 }

}
}
