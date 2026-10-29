using UnityEngine;
using System.Collections.Generic;
namespace Game {
public sealed class ComponentCreationLab:MonoBehaviour {
 public int Count,Passed,Failed;public bool Complete;public string Report="",Diagnostics="";
 private IEnumerator<ComponentCreationCorpus.Observation> rows;
 private void Update(){if(Complete)return;rows??=ComponentCreationCorpus.Run().GetEnumerator();if(!rows.MoveNext()){Complete=true;rows.Dispose();return;}var row=rows.Current;Count++;var failure=ComponentCreationReference.Compare(row);if(failure=="")Passed++;else{Failed++;if(Diagnostics.Length<3000)Diagnostics+=row.profile+": "+failure+";";}Report+="a"+Count.ToString("D3")+":"+(failure==""?"pass":"fail")+";";}
}
}
