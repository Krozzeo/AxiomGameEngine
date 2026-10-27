using System.Text.Json.Nodes;
namespace Axiom.Gameplay;
public sealed class CapsuleCollider : Collider {
 internal CapsuleCollider(Entity entity):base(entity){}
 public double radius {get=>PhysicsData.Half(Entity).X;set{if(!double.IsFinite(value)||value<=0||value>10000)throw new ArgumentOutOfRangeException(nameof(value));var half=PhysicsData.Half(Entity);PhysicsData.Half(Entity,new(value,Math.Max(half.Y,value),value));}}
 public double height {get=>PhysicsData.Half(Entity).Y*2;set{if(!double.IsFinite(value)||value<0||value>20000)throw new ArgumentOutOfRangeException(nameof(value));var half=PhysicsData.Half(Entity);PhysicsData.Half(Entity,new(half.X,Math.Max(value/2,half.X),half.X));}}
}
public sealed class MeshFilter : Component {
 internal MeshFilter(Entity entity){Entity=entity;}
 public Mesh? sharedMesh=>RuntimeAssets.MeshOf(Entity);
}
public sealed class MeshCollider : Collider {
 internal MeshCollider(Entity entity):base(entity){}
 public Mesh? sharedMesh=>RuntimeAssets.MeshOf(Entity,true);
}
internal static class PrimitiveMeshes {
 private static readonly Dictionary<string,Mesh> Assets=new();
 internal static Mesh Get(string id){if(!Assets.TryGetValue(id,out var asset)||!asset){var index=Array.IndexOf(UnityEngine.PrimitiveCatalog.Ids,id);if(index<0)throw new ArgumentException("Built-in mesh identity is unavailable");Assets[id]=asset=new Mesh("Built-in/"+UnityEngine.PrimitiveCatalog.Names[index],id);}return asset;}
}
