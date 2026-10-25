using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;
public static class OrderCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(5);RuntimeLifecycle.Register<QueryMarker>("Scripts/QueryMarker.cs",()=>new QueryMarker(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  var go=new GameObject("Order");var camera=go.AddComponent<Camera>();var collider=go.AddComponent<BoxCollider>();var script=go.AddComponent<QueryMarker>();
  check("component order follows attachment",go.GetComponents<Component>().Select(x=>x.GetType()).SequenceEqual(new[]{typeof(Transform),typeof(Camera),typeof(BoxCollider),typeof(QueryMarker)}));
  check("component index queries agree",go.GetComponentIndex(camera)==1&&camera.GetComponentIndex()==1&&go.GetComponentAtIndex<Camera>(1)==camera);
  var foreign=new GameObject("Foreign");check("foreign and null components have no index",go.GetComponentIndex(foreign.transform)==-1&&go.GetComponentIndex(null)==-1);
  try{go.GetComponentAtIndex(4);check("component index bounds",false);}catch(ArgumentOutOfRangeException){check("component index bounds",true);}
  Object.DestroyImmediate(camera);check("removal updates component indices",go.GetComponentIndex(collider)==1&&go.GetComponentIndex(script)==2);
  var added=go.AddComponent<Camera>();check("reattached component goes last",go.GetComponentIndex(added)==3&&go.GetComponentAtIndex(3)==added);
  var clone=Object.Instantiate(go);check("graph clone preserves component order",clone.GetComponents<Component>().Select(x=>x.GetType()).SequenceEqual(go.GetComponents<Component>().Select(x=>x.GetType())));
  Context.Operations.Clear();var twoD=new GameObject("2D only");twoD.AddComponent<BoxCollider2D>();var data=Context.EntityData[twoD.Entity.Id].GetRawText();var operations=Context.Operations.Count;
  try{twoD.AddComponent<Rigidbody>();check("unsupported body dimension rejects atomically",false);}catch(ArgumentException){check("unsupported body dimension rejects atomically",Context.EntityData[twoD.Entity.Id].GetRawText()==data&&Context.Operations.Count==operations);}
  var bodyOnly=new GameObject("2D body only");bodyOnly.AddComponent<Rigidbody2D>();data=Context.EntityData[bodyOnly.Entity.Id].GetRawText();operations=Context.Operations.Count;
  try{bodyOnly.AddComponent<BoxCollider>();check("unsupported collider dimension rejects atomically",false);}catch(ArgumentException){check("unsupported collider dimension rejects atomically",Context.EntityData[bodyOnly.Entity.Id].GetRawText()==data&&Context.Operations.Count==operations);}
 }
}
