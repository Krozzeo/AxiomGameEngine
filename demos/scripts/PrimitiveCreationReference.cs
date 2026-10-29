using System.Collections.Generic;
namespace Game {
// Own public outcomes observed in Unity6000.3.27f1 Play; no vendor implementation.
public static class PrimitiveCreationReference {
 public const string SourceSha256="ffcbc5d926fe1836a9da82310f443a7c5cab19082a0031bb6186a19776a87085";
 public const string EvidenceSha256="0e89ede9351f4bb0a81d703ea1c6c118db4c299a06850ca041694d4fd532b3ad";
 public const int ExpectedCount=13;
 private static readonly Dictionary<string,string> Expected=new(){["create.Sphere"]="Sphere:Transform,MeshFilter,SphereCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Sphere"]="True",
["create.Capsule"]="Capsule:Transform,MeshFilter,CapsuleCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Capsule"]="True",
["create.Cylinder"]="Cylinder:Transform,MeshFilter,CapsuleCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Cylinder"]="True",
["create.Cube"]="Cube:Transform,MeshFilter,BoxCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Cube"]="True",
["create.Plane"]="Plane:Transform,MeshFilter,MeshCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Plane"]="True",
["create.Quad"]="Quad:Transform,MeshFilter,MeshCollider,MeshRenderer:True:0:Untagged:True",
["mesh.Quad"]="True",
["create.invalid"]="True"};
 public static string Compare(string key,string actual)=>!Expected.TryGetValue(key,out var expected)?"Unknown case "+key:actual==expected?"":"Expected "+expected+", actual "+actual;
}
}
