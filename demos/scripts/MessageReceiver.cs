using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game {
public sealed class MessageReceiver:MessageReceiverBase {
 private void Ping(){Events.Add(gameObject.name+".Ping");}
 private void Take(object value){Events.Add(gameObject.name+".Take="+(value is GameObject go?go.name:value==null?"null":Convert.ToString(value,System.Globalization.CultureInfo.InvariantCulture)));}
 private void TakeInt(int value){Events.Add(gameObject.name+".Int="+value);}
 private void Throw(){Events.Add(gameObject.name+".Throw");throw new InvalidOperationException("Message receiver probe");}
}
public class MessageReceiverBase:MonoBehaviour {
 public static readonly List<string> Events=new List<string>();
 private void Inherited(){Events.Add(gameObject.name+".Inherited");}
}
}
