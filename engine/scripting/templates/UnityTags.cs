using Legacy=Axiom.Gameplay;
namespace UnityEngine;
public class UnityException : SystemException {public UnityException(){}public UnityException(string message):base(message){}public UnityException(string message,Exception innerException):base(message,innerException){}}
public class MissingReferenceException : SystemException {public MissingReferenceException(){}public MissingReferenceException(string message):base(message){}public MissingReferenceException(string message,Exception innerException):base(message,innerException){}}
public readonly struct TagHandle {internal readonly string? Value;private TagHandle(string value){Value=value;}public static TagHandle GetExistingTag(string tagName){Tags.Require(tagName);return new(tagName);}}
internal static class Tags {
 private static readonly HashSet<string> Builtin=new(StringComparer.Ordinal){"Untagged","Respawn","Finish","EditorOnly","MainCamera","Player","GameController"};
 internal static void Require(string value){if(value==null||!Builtin.Contains(value)&&!Legacy.Context.EntityData.Values.Any(entity=>entity.TryGetProperty("runtimeTag",out var tag)&&tag.GetString()==value))throw new UnityException("Tag: "+value+" is not defined.");}
}
public partial class GameObject {public bool CompareTag(TagHandle tag)=>CompareTag(tag.Value??"Untagged");}
public partial class Component {public bool CompareTag(TagHandle tag)=>CompareTag(tag.Value??"Untagged");}
