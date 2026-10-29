using System.Collections.Generic;
using UnityEngine;
namespace Game {
public sealed class ObjectLifetimeLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="",Diagnostics="";
 private IEnumerator<ObjectLifetimeCorpus.Observation> iterator;
 private void Update(){if(Complete)return;if(iterator==null)iterator=ObjectLifetimeCorpus.Run().GetEnumerator();for(var i=0;i<1;i++){if(!iterator.MoveNext()){iterator.Dispose();Complete=true;return;}var row=iterator.Current;if(row==null)return;Count++;var failure=ObjectLifetimeReference.Compare(row);if(failure=="")Passed++;else{Failed++;if(Diagnostics.Length<3000)Diagnostics+=row.index+"."+row.profile+": "+failure+";";}Report+="l"+Count.ToString("D3")+":"+(failure==""?"pass":"fail")+";";}}
}
}
