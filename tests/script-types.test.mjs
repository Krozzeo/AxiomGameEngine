import test from 'node:test';
import assert from 'node:assert/strict';
import {scriptMetadata} from '../engine/scripting/fields.mjs';
import {isScriptType,scriptAssignable,scriptEntry} from '../engine/scripting/script-types.mjs';
test('same-file derived component registers its filename class and inherited serialized fields',()=>{
 const file={path:'Scripts/Receiver.cs',kind:'script',text:'namespace Game { public sealed class Receiver:ReceiverBase {public int Own=3;} public class ReceiverBase:UnityEngine.MonoBehaviour {[SerializeField] private int Counter=4;public bool Enabled=true;} }'};
 const meta=scriptMetadata(file);assert.equal(meta.typeName,'Game.Receiver');assert.deepEqual(meta.fields.map(field=>[field.name,field.default]),[['Own',3],['Counter',4],['Enabled',true]]);assert(scriptAssignable('Game.ReceiverBase',file,[file]));
});
test('cross-file component ancestry resolves qualified bases without registering abstract bases',()=>{
 const base={path:'Scripts/Base.cs',kind:'script',text:'namespace Shared; public abstract class Base:UnityEngine.MonoBehaviour {public int Speed=2;}'};
 const derived={path:'Scripts/Derived.cs',kind:'script',text:'namespace Game; public class Derived:Shared.Base {public string Label="ready";}'};
 assert.equal(scriptEntry(base,[base,derived]),null);assert.equal(scriptMetadata(derived,[base,derived]).typeName,'Game.Derived');assert(isScriptType('Shared.Base',[base,derived]));assert(isScriptType('Game.Derived',[base,derived]));assert(scriptAssignable('Shared.Base',derived,[base,derived]));assert(!scriptAssignable('Other',derived,[base,derived]));
});
test('cyclic and ambiguous unqualified component ancestry cannot manufacture factories',()=>{
 const cycle={path:'Scripts/A.cs',kind:'script',text:'public class A:B {} public class B:A {}'};assert.equal(scriptEntry(cycle),null);
 const derived={path:'Scripts/Derived.cs',kind:'script',text:'public class Derived:Base {}'};
 const one={path:'Scripts/One.cs',kind:'script',text:'namespace One; public class Base:UnityEngine.MonoBehaviour {}'};
 const two={path:'Scripts/Two.cs',kind:'script',text:'namespace Two; public class Base:UnityEngine.MonoBehaviour {}'};assert.equal(scriptEntry(derived,[one,two]),null);
});
