using System.Collections.Generic;
namespace Game {
// Own public outcomes observed in Unity6000.3.27f1 Play; no vendor implementation.
public static class ScriptInstanceReference {
 public const string SourceSha256="7397f4d9397a7fe3b39475921b31c8f160dacbf8ef0cd264a0ea629986023019";
 public const string EvidenceSha256="19b6dc40b7805b9773c0d1fdaa4509ebc09990038a6562b9cb41eb30e3bd7db1";
 public const int ExpectedCount=27;
 private static readonly Dictionary<string,string> Expected=new(){["created.distinct"]="True",
["created.count"]="2",
["created.order"]="1:2",
["created.defaults"]="17:17",
["created.callbacks"]="1:1:1:1",
["fields.independent"]="10:20",
["activation.independent"]="False:True:1:0",
["messages.disabled"]="13:23:1:1",
["messages.order"]="13,23",
["clone.count"]="2",
["clone.fields"]="13:23",
["clone.peers"]="True",
["clone.enabled"]="False:True",
["clone.order"]="1:2",
["clone.callbacks"]="2:1:1:2:2:0",
["clone.fields-independent"]="13:23",
["component.clone.ordinal"]="23:2:2",
["component.clone.peer"]="True",
["component.clone.enabled"]="False:True",
["destroy.one"]="True",
["destroy.peer"]="True",
["destroy.callback"]="1:1:0",
["replacement.id"]="True",
["replacement.default"]="17",
["replacement.enabled"]="False:True",
["replacement.order"]="1:2",
["replacement.callbacks"]="1:1"};
 public static string Compare(string key,string actual)=>!Expected.TryGetValue(key,out var expected)?"Unknown case "+key:actual==expected?"":"Expected "+expected+", actual "+actual;
}
}
