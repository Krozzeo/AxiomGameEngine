using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
namespace Game {
// Own source-attributed native outcomes, no vendor implementation code.
public static class ObjectFindReference {
 public const string SourceSha256="731911012532ee0ddc8022d32139e08826830fc759f8b384cfda4b4dd0f69fd7";
 public const string EvidenceSha256="fde14c25dfe7261796c6d32401920b711ab401f7aa792e1e646dd550d1407da4";
 public const int ExpectedCount=152;
 private static readonly Dictionary<string,(string Error,string Data)> Expected=Load();
 private static Dictionary<string,(string Error,string Data)> Load(){using var memory=new MemoryStream(Convert.FromBase64String("H4sIAAAAAAAACqWYz5LaMAzG7zwFDwA0cuLY6b0ce2j7AlkidjJjkp3EbGGGh+8A3Ra8/iMlJ2Bieazvk5wfyjaH9oTN5XLAwwsOX38NR1yhaV/bF4P3X6b/jaO9fV9km7bbmWOD67ard7Z9R3pkbcxDVHc0ZpFtmr6z67Hm7DOgbYfrkf9ucf1Y2/MbXn6eR4uHzfejMT9wjwN2O/x22uGbbfvucjv8e23ahrQaPpTZLu9nXm2Xe1O/jl/6ocEBm/t5wKOIG7HaLjscLTar7bLf790NnoVxnz4o9LGvu+SfIu6DYzf2g6Uk8V/Em6zrehjq8y25B8WeH9XjiHa8bm3xZJ83FMmyWghOKS2Er3wEqXwWwimZnOZsTnDWjYhZmaetzENWFrw2LSa3aeHTueC3aeFoXtDbtGC1qaSZKdlmyqiZMm2mDJkpyX1Z0pIr595BZTTXMp1rGcq1JOeqaLmqubmqaK4qnasK5aoCt6gK36IqeotqmiSaXds6qoFOa6BDGuiABjqsgY5qUIU0iJpcza2SylGIGewRMBER0LMK6FmF9ayiekI2SVDwkR93hxmSQhbQlBQaAqQsxD1ZBHwyR9/V/cPLVcB7b4OPJsmhnjc3AP/VDeC8u0EQUdiHdMmQKP0KAv6KoL0iZK+I2Cvi7ZOnyRZ85Bhd7nMup8Et5K5bOR21bidl/CVKY+h1DSt5L3ACjTiv65zkGZwJPNAEyexlH3KSQ32iyAm9LF2BJEMgyROISKtAwdVPIdFrgkCoEERUoDMqlKEbpYzcKGX8RlHMuvIRMDnUV1dqQl0pt64Uo64Ur640UyAfD5NDfQLpCQJpV6AgyDrlMBtdwWXXT4/TzRLkUZHxzBDTJ4bCOzIUE2aGwh0aCiaXielcJrxcJiZwmXC5TFCmbMwxm3/ORh20OZO2P92aEN1dFgAA"));using var gzip=new GZipStream(memory,CompressionMode.Decompress);using var reader=new StreamReader(gzip);var result=new Dictionary<string,(string,string)>();foreach(var line in reader.ReadToEnd().Split('\n')){var parts=line.Split('|');if(parts.Length!=3)throw new InvalidOperationException("Corrupt object find reference");result.Add(parts[0],(parts[1],parts[2]));}return result;}
 public static string Compare(ObjectFindCorpus.Observation row){if(!Expected.TryGetValue(row.index+"."+row.profile,out var expected))return "Unexpected query case";if(row.error!=expected.Error)return "Expected exception "+expected.Error+", actual "+row.error;if(row.data!=expected.Data)return "Expected "+expected.Data+", actual "+row.data;return "";}
}
}
