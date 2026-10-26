using System.Collections;
using System.Reflection;
namespace Axiom.Gameplay;

[AttributeUsage(AttributeTargets.Field)] public class SerializeReferenceAttribute : Attribute {}
internal static class CloneSerialization {
 private static bool ReferenceField(FieldInfo field)=>field.GetCustomAttribute<SerializeReferenceAttribute>()!=null;
 internal static IEnumerable<FieldInfo> Fields(Type type){for(var current=type;current!=null&&current!=typeof(Script)&&current!=typeof(UnityEngine.MonoBehaviour)&&current!=typeof(MonoBehaviour);current=current.BaseType)foreach(var field in current.GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly))if(!field.IsStatic&&!field.IsInitOnly&&field.GetCustomAttribute<NonSerializedAttribute>()==null&&(field.IsPublic||field.GetCustomAttribute<SerializeFieldAttribute>()!=null||ReferenceField(field)))yield return field;}
 private static bool Supported(Type type){if(type.IsPointer||type==typeof(IntPtr)||type==typeof(UIntPtr)||typeof(Delegate).IsAssignableFrom(type)||typeof(IDictionary).IsAssignableFrom(type))return false;if(type.IsPrimitive||type.IsEnum||type==typeof(string)||type.IsValueType||typeof(UnityEngine.Object).IsAssignableFrom(type)||typeof(Object).IsAssignableFrom(type))return true;if(type.IsArray)return type.GetArrayRank()==1&&Supported(type.GetElementType()!);if(type.IsGenericType&&type.GetGenericTypeDefinition()==typeof(List<>))return Supported(type.GetGenericArguments()[0]);return type.GetCustomAttribute<SerializableAttribute>()!=null;}
 private static object? CopyValue(object? value,Dictionary<object,object> references,int depth,ref int budget,bool preserveReferences){
  if(value==null||value is string||value is UnityEngine.Object||value is Object||value.GetType().IsPrimitive||value.GetType().IsEnum)return value;
  if(depth>64||--budget<0)throw new InvalidOperationException("Serialized clone graph exceeds safety budget");
  if(references.TryGetValue(value,out var existing)){if(preserveReferences)return existing;throw new InvalidOperationException("Inline serialization cycle requires SerializeReference");}
  var type=value.GetType();if(!Supported(type))throw new NotSupportedException("Managed field type is not serializable: "+type.FullName);
  if(value is Array array){if(array.Rank!=1)throw new NotSupportedException("Only one-dimensional arrays are serializable");var result=Array.CreateInstance(type.GetElementType()!,array.Length);references[value]=result;for(var i=0;i<array.Length;i++)result.SetValue(CopyValue(array.GetValue(i),references,depth+1,ref budget,preserveReferences),i);if(!preserveReferences)references.Remove(value);return result;}
  if(value is IList list){var result=(IList)Activator.CreateInstance(type)!;references[value]=result;foreach(var item in list)result.Add(CopyValue(item,references,depth+1,ref budget,preserveReferences));if(!preserveReferences)references.Remove(value);return result;}
  if(type.IsValueType&&!Fields(type).Any())return value;
  var clone=type.IsValueType?Activator.CreateInstance(type)!:System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);references[value]=clone;
  foreach(var field in Fields(type))if(Supported(field.FieldType)||ReferenceField(field))field.SetValue(clone,CopyValue(field.GetValue(value),references,depth+1,ref budget,preserveReferences||ReferenceField(field)));
  if(!preserveReferences)references.Remove(value);return clone;
 }
 internal static void Validate(Type type,Script source){Copy(type,source,null);}
 internal static void Copy(Type type,Script source,Script? target){var shared=new Dictionary<object,object>(ReferenceEqualityComparer.Instance);var budget=16384;var canonical=ScriptBridge.Value(source) is UnityEngine.MonoBehaviour;foreach(var field in Fields(type)){if(!Supported(field.FieldType)&&!ReferenceField(field)){if(canonical)continue;throw new NotSupportedException("Cloning this managed field requires a serializable type: "+field.Name);}var map=ReferenceField(field)?shared:new Dictionary<object,object>(ReferenceEqualityComparer.Instance);var value=CopyValue(field.GetValue(ScriptBridge.Value(source)),map,0,ref budget,ReferenceField(field));if(!ReferenceEquals(target,null))field.SetValue(ScriptBridge.Value(target!),value);}}
 internal static void Remap(Type type,Script script,Dictionary<string,Entity> entities){
  var visited=new HashSet<object>(ReferenceEqualityComparer.Instance);var budget=16384;
  object? Visit(object? value){if(value==null)return null;if(value is UnityEngine.Object canonical){var remapped=Visit(canonical.Wrapped);return remapped is Object native?UnityEngine.Object.Wrap(native):null;}if(value is Object handle){if(handle is GameObject go&&entities.TryGetValue(go.Entity.Id,out var entity))return new GameObject(entity);if(handle is Component component&&entities.TryGetValue(component.Entity.Id,out entity))return new GameObject(entity).GetComponent(ScriptBridge.ComponentType(component));return handle?handle:null;}
   if(value is string||value.GetType().IsPrimitive||value.GetType().IsEnum||!visited.Add(value))return value;if(--budget<0)throw new InvalidOperationException("Serialized reference graph exceeds safety budget");
   if(value is Array array){for(var i=0;i<array.Length;i++)array.SetValue(Visit(array.GetValue(i)),i);}
   else if(value is IList list){for(var i=0;i<list.Count;i++)list[i]=Visit(list[i]);}
   else if(Supported(value.GetType()))foreach(var field in Fields(value.GetType()))if(Supported(field.FieldType)||ReferenceField(field))field.SetValue(value,Visit(field.GetValue(value)));
   return value;
  }
  foreach(var field in Fields(type))if(Supported(field.FieldType)||ReferenceField(field))field.SetValue(ScriptBridge.Value(script),Visit(field.GetValue(ScriptBridge.Value(script))));
 }
}
