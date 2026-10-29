using System.Text.Json.Nodes;
namespace Axiom.Gameplay;
public sealed class CapsuleCollider : Collider {
 internal CapsuleCollider(Entity entity,string? slot=null):base(entity,slot){}
 public double radius {get=>ColliderInstances.Half(this).X;set{if(!double.IsFinite(value)||value<=0||value>10000)throw new ArgumentOutOfRangeException(nameof(value));var half=ColliderInstances.Half(this);ColliderInstances.Half(this,new(value,Math.Max(half.Y,value),value));}}
 public double height {get=>ColliderInstances.Half(this).Y*2;set{if(!double.IsFinite(value)||value<0||value>20000)throw new ArgumentOutOfRangeException(nameof(value));var half=ColliderInstances.Half(this);ColliderInstances.Half(this,new(half.X,Math.Max(value/2,half.X),half.X));}}
}
public sealed class MeshFilter : Component {
 internal MeshFilter(Entity entity){Entity=entity;}
 public Mesh? sharedMesh=>RuntimeAssets.MeshOf(Entity);
}
public sealed class MeshCollider : Collider {
 internal MeshCollider(Entity entity,string? slot=null):base(entity,slot){}
 public Mesh? sharedMesh=>ColliderInstances.MeshOf(this);
}
internal static class PrimitiveMeshes {
 private static readonly Dictionary<string,Mesh> Assets=new();
 internal static Mesh Get(string id){if(!Assets.TryGetValue(id,out var asset)||!asset){var index=Array.IndexOf(UnityEngine.PrimitiveCatalog.Ids,id);if(index<0)throw new ArgumentException("Built-in mesh identity is unavailable");Assets[id]=asset=new Mesh("Built-in/"+UnityEngine.PrimitiveCatalog.Names[index],id);}return asset;}
}
