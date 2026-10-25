using Axiom.Gameplay;
using Object = Axiom.Gameplay.Object;

public static class QueryCases {
 public static void Run(Action<string,bool> check,Action<int> reset){
  reset(3);
  RuntimeLifecycle.Register<QueryMarker>("Scripts/QueryMarker.cs",()=>new QueryMarker(),(_,_)=>{},(_,w)=>{w.WriteStartObject();w.WriteEndObject();});
  var root=new GameObject("Query root");var left=new GameObject("Query left");var right=new GameObject("Query right");
  left.transform.SetParent(root.transform,false);right.transform.SetParent(root.transform,false);
  var a=root.AddComponent<QueryMarker>();var b=left.AddComponent<QueryMarker>();var c=right.AddComponent<QueryMarker>();
  right.SetActive(false);b.enabled=false;
  check("query component by Type",root.GetComponent(typeof(QueryMarker))==a);
  check("query component by interface",ReferenceEquals(root.GetComponent<IQueryMarker>(),a));
  check("component forwards interface query",ReferenceEquals(root.transform.GetComponent<IQueryMarker>(),a));
  check("try component Type success",root.TryGetComponent(typeof(QueryMarker),out var typed)&&typed==a);
  check("try component missing returns null",!root.TryGetComponent(typeof(Camera),out var missing)&&missing==null);
  check("try component generic interface",root.TryGetComponent<IQueryMarker>(out var marker)&&ReferenceEquals(marker,a));
  check("self query includes Transform",root.GetComponents(typeof(Component)).Length==2);
  var list=new List<IQueryMarker>{b,c};root.GetComponents(list);check("list query replaces previous values",list.Count==1&&ReferenceEquals(list[0],a));
  check("children depth first active filter",root.GetComponentsInChildren<IQueryMarker>().SequenceEqual(new IQueryMarker[]{a,b}));
  check("children include inactive",root.GetComponentsInChildren(typeof(IQueryMarker),true).Length==3);
  root.transform.GetComponentsInChildren(true,list);check("component list hierarchy forwarding",list.SequenceEqual(new IQueryMarker[]{a,b,c}));
  check("parent query includes inactive self",right.GetComponentInParent<QueryMarker>()==c);
  check("parent query walks upwards",right.GetComponentsInParent<QueryMarker>().SequenceEqual(new[]{c,a}));
  root.SetActive(false);
  check("inactive self searched in children",root.GetComponentsInChildren<IQueryMarker>().Length==1);
  check("inactive parent excluded beyond self",right.GetComponentsInParent<IQueryMarker>().Length==1);
  check("include inactive parent hierarchy",right.GetComponentsInParent(typeof(IQueryMarker),true).Length==2);
  root.SetActive(true);
  var sorted=Object.FindObjectsByType<QueryMarker>(FindObjectsSortMode.InstanceID);
  check("find filters inactive objects not disabled behaviours",sorted.Length==2&&sorted.Contains(b));
  check("find sorts stable instance IDs",sorted.Select(x=>x.GetInstanceID()).SequenceEqual(sorted.Select(x=>x.GetInstanceID()).Order()));
  check("find includes inactive explicitly",Object.FindObjectsByType<QueryMarker>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length==3);
  check("find GameObjects",Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length==3);
  check("find first and any",Object.FindFirstObjectByType<QueryMarker>()==sorted[0]&&Object.FindAnyObjectByType(typeof(QueryMarker))!=null);
  check("obsolete find aliases preserve inactive flag",Object.FindObjectsOfType<QueryMarker>(true).Length==3&&Object.FindObjectOfType(typeof(QueryMarker))!=null);
  check("missing find returns null",Object.FindFirstObjectByType<Camera>()==null);
  var collider=root.AddComponent(typeof(BoxCollider));check("add component by Type",collider is BoxCollider&&collider==root.GetComponent<BoxCollider>());
  check("component count",root.GetComponentCount()==3);
  var count=Context.Entities.Count;var operations=Context.Operations.Count;
  try{root.GetComponent<int>();check("reject value type query",false);}catch(ArgumentException){check("reject value type query",true);}
  try{root.GetComponent((Type)null!);check("reject null Type query",false);}catch(ArgumentNullException){check("reject null Type query",true);}
  try{root.GetComponents<Component>((List<Component>)null!);check("reject null result list",false);}catch(ArgumentNullException){check("reject null result list",true);}
  try{root.AddComponent(typeof(IQueryMarker));check("reject interface attachment",false);}catch(ArgumentException){check("reject interface attachment",true);}
  try{Object.FindObjectsByType(typeof(string),FindObjectsSortMode.None);check("reject non Object find",false);}catch(ArgumentException){check("reject non Object find",true);}
  try{Object.FindObjectsByType<QueryMarker>((FindObjectsSortMode)99);check("reject invalid find mode",false);}catch(ArgumentOutOfRangeException){check("reject invalid find mode",true);}
  check("queries and rejected operations do not mutate scene",Context.Entities.Count==count&&Context.Operations.Count==operations);
 }
}
public interface IQueryMarker {}
public sealed class QueryMarker:MonoBehaviour,IQueryMarker {}
