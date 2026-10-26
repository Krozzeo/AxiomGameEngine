using System.Reflection;
using Legacy=Axiom.Gameplay;
namespace UnityEngine;
public enum SendMessageOptions {RequireReceiver,DontRequireReceiver}
internal static class Messages {
 internal static void Send(GameObject origin,string name,object? value,bool supplied,int direction,SendMessageOptions options){
  if(string.IsNullOrEmpty(name))throw new ArgumentException("A method name is required",nameof(name));if(!Enum.IsDefined(options))throw new ArgumentOutOfRangeException(nameof(options));if(!origin.activeInHierarchy)return;
  var receivers=new List<GameObject>();if(direction==2)receivers.AddRange(origin.GetComponentsInChildren<Transform>().Select(t=>t.gameObject));else for(GameObject? current=origin;current;current=direction==1?current.transform.parent?.gameObject:null)receivers.Add(current!);
  var found=false;foreach(var go in receivers)foreach(var component in go.LegacyGameObject.AllComponents()){
   if(component is not Legacy.Script script)continue;var target=Legacy.ScriptBridge.Value(script);MethodInfo? method=null;
   for(var type=target.GetType();type!=null&&type!=typeof(MonoBehaviour)&&type!=typeof(Legacy.MonoBehaviour)&&type!=typeof(Legacy.Script);type=type.BaseType){method=type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly).FirstOrDefault(candidate=>candidate.Name==name&&!candidate.ContainsGenericParameters&&candidate.GetParameters().Length<2);if(method!=null)break;}
   if(method==null)continue;found=true;var parameters=method.GetParameters();var argument=supplied?value:null;
   if(argument is Object canonical&&parameters.Length==1&&typeof(Legacy.Object).IsAssignableFrom(parameters[0].ParameterType))argument=canonical.Native;
   else if(argument is Legacy.Object native&&parameters.Length==1&&typeof(Object).IsAssignableFrom(parameters[0].ParameterType))argument=Object.Wrap(native);
   try{method.Invoke(target,parameters.Length==0?null:new[]{argument});}catch(TargetInvocationException error){Legacy.Debug.LogError(error.InnerException??error);}catch(ArgumentException error){Legacy.Debug.LogError(error);}
  }
  if(!found&&options==SendMessageOptions.RequireReceiver)Legacy.Debug.LogError(name+" has no receiver on "+origin.name);
 }
}
public partial class GameObject {
 public void SendMessage(string methodName)=>SendMessage(methodName,null,SendMessageOptions.RequireReceiver);public void SendMessage(string methodName,object? value)=>SendMessage(methodName,value,SendMessageOptions.RequireReceiver);public void SendMessage(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,0,options);public void SendMessage(string methodName,object? value,SendMessageOptions options)=>Messages.Send(this,methodName,value,true,0,options);
 public void SendMessageUpwards(string methodName)=>SendMessageUpwards(methodName,null,SendMessageOptions.RequireReceiver);public void SendMessageUpwards(string methodName,object? value)=>SendMessageUpwards(methodName,value,SendMessageOptions.RequireReceiver);public void SendMessageUpwards(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,1,options);public void SendMessageUpwards(string methodName,object? value,SendMessageOptions options)=>Messages.Send(this,methodName,value,true,1,options);
 public void BroadcastMessage(string methodName)=>BroadcastMessage(methodName,null,SendMessageOptions.RequireReceiver);public void BroadcastMessage(string methodName,object? parameter)=>BroadcastMessage(methodName,parameter,SendMessageOptions.RequireReceiver);public void BroadcastMessage(string methodName,SendMessageOptions options)=>Messages.Send(this,methodName,null,false,2,options);public void BroadcastMessage(string methodName,object? parameter,SendMessageOptions options)=>Messages.Send(this,methodName,parameter,true,2,options);
}
public partial class Component {
 public void SendMessage(string methodName)=>gameObject.SendMessage(methodName);public void SendMessage(string methodName,object? value)=>gameObject.SendMessage(methodName,value);public void SendMessage(string methodName,SendMessageOptions options)=>gameObject.SendMessage(methodName,options);public void SendMessage(string methodName,object? value,SendMessageOptions options)=>gameObject.SendMessage(methodName,value,options);
 public void SendMessageUpwards(string methodName)=>gameObject.SendMessageUpwards(methodName);public void SendMessageUpwards(string methodName,object? value)=>gameObject.SendMessageUpwards(methodName,value);public void SendMessageUpwards(string methodName,SendMessageOptions options)=>gameObject.SendMessageUpwards(methodName,options);public void SendMessageUpwards(string methodName,object? value,SendMessageOptions options)=>gameObject.SendMessageUpwards(methodName,value,options);
 public void BroadcastMessage(string methodName)=>gameObject.BroadcastMessage(methodName);public void BroadcastMessage(string methodName,object? parameter)=>gameObject.BroadcastMessage(methodName,parameter);public void BroadcastMessage(string methodName,SendMessageOptions options)=>gameObject.BroadcastMessage(methodName,options);public void BroadcastMessage(string methodName,object? parameter,SendMessageOptions options)=>gameObject.BroadcastMessage(methodName,parameter,options);
}
