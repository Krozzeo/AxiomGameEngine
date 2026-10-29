using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own captured public outcomes, not vendor implementation code. Regenerate with generate-m20-component-creation-fixtures.mjs.
public static class ComponentCreationReference {
 public const string SourceSha256="c0066ee434b837cf10e3708d5f84b34d60ceeb9cceb54069a862cb004e7dda8b";
 public const string EvidenceSha256="8b0792bb8efdc43b70eb1cc5768a2c8eaa4cf740e2553dd6944f19ab2be86ed4";
 public const int ExpectedCount=140;
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACs2YwW7iQAyG732W3Qhvy4Vjq/bGagXtA2TBwEhhkk6Siko8/AoIycQzoeOZJN1LVQb/dn7bTdJvEr2qWOabVO2jjcJ8dzzKMklm5x8vcZLjDGZ1yN1EC1+XWSJWcYHuEoWFULg+vklRfD7LrZAYzUWeC7ld4AYVyhU+H1aYFSKVx5b0lP2nwhWKD1TH5Wde4D76XSaJXfiU7rNUoizcXDXhzq4aCdtVI2W6esRd/CHSUrm5asKdXTUStqtGynQ1T2XKdNaWOLtry9gO23L2RiaJWKOjwTqasY+VwmMdKyV3G9MDNaUd6Vf6q7nSH3rIqyqRJNL86pFUNbu3pzTSaz8u10I+k/L8rdfEAf3biwOuv7D88LVlH/vLbIcK6STbp13DJFGTaGom1CZKwslHY67t752KsedH9MwREnU1RQebXbN0sfwUZ3mZGBMjx10jo2GTaDoDS1ZtbFRCPxuDIwHONfm3LpKAe08m8mp+Tn67JujqfY75jo5QP+uaXyvG/GtupdVm2JKZOe47KrDK8R+oupr71tCuLNd4Kt3Zwikx+CKSgtw7T8eLSyLiW7t16sYvOfThXU5uja6KqJ58WhYyq4BMXmOotB5DqKteR0DfUx54rb+bRNdf3d6V6mjnd6VawW5VrfRoFHWln90adB2jjdrmuZd8XsvTS1f6XaAM4wLXy5USWfFHpX/x2nXLV13NsoR25NamYMtvOzNuu5YgxhV4bLKZhDm+NyneS7T02Piiq8NGoDWr1t2eM7O7ZqZg9myB7+Wp5GN6IGvZPtd9df2vQzW29K3NJCWMg5svNjTaqbzHWpIMzP7mhRJy6/bgqGKdHxtVPNtTpWM6OYW7+ThHOrs4R7M9nFUsB8BDi8BHi+CPFsEXLQIPLQIfLYI/WgRftAg8tAh8tAj+aBF80SLw0SKVOLsLQotUzt5Id7QIbLQI3mgRPNEi9IUW4XvRIoSgRfBHi/B/oEXoGy0aCYdEi0Yx9vxC0KKhHgMtwiBo0cw6PFo0a/JvXSFo0ZSPhxZhGLQI46JFCEKLEIAWjcrjokXoBS1Cb2gRAtAieKNFGAYtAgstAhstgjdaBE+0CD2iRegZLUIQWoQAtGhU7nOBhkKL9txjokX7FXhsciBahEHQoi1rP2jRlpndtUC0CMOiRUv6MdGipbzHWgagxX9JLUUtKScAAA=="));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var row in reader.ReadToEnd().Split('\n')){var values=row.Split('|');if(values.Length!=3)throw new InvalidOperationException("Corrupt vector reference");result.Add(values[0],(values[1],values[2]));}return result;}
 public static string Compare(ComponentCreationCorpus.Observation row){
  var key=row.index+"."+row.profile;if(!Expected.TryGetValue(key,out var expected))return "Unexpected case "+key;
  if(row.error!=expected.Error)return "Exception: expected "+expected.Error+", actual "+row.error;
  return row.data==expected.Data?"":"Expected "+expected.Data+", actual "+row.data;
 }

}
}
