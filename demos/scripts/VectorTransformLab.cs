using System.Collections.Generic;
using UnityEngine;
namespace Game {
public sealed class VectorTransformLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="",Diagnostics="";
 private IEnumerator<VectorTransformCorpus.Observation> iterator;
 private void Update(){if(Complete)return;if(iterator==null)iterator=VectorTransformCorpus.Run().GetEnumerator();for(var i=0;i<2;i++){if(!iterator.MoveNext()){iterator.Dispose();Complete=true;return;}var row=iterator.Current;Count++;var failure=VectorTransformReference.Compare(row);if(failure=="")Passed++;else{Failed++;if(Diagnostics.Length<3000)Diagnostics+=row.index+"."+row.profile+": "+failure+";";}Report+="v"+Count.ToString("D3")+":"+(failure==""?"pass":"fail")+";";}}
}
}
