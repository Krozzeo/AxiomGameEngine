namespace UnityEngine;

// Interface attributes from the pinned 6000.3.27f1 ABI. Removed APIs remain compile errors.
public partial class Component {
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)",true)]
 public Component rigidbody=>throw new NotSupportedException("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)",true)]
 public Component rigidbody2D=>throw new NotSupportedException("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)",true)]
 public Component camera=>throw new NotSupportedException("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)",true)]
 public Component light=>throw new NotSupportedException("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)",true)]
 public Component animation=>throw new NotSupportedException("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)",true)]
 public Component constantForce=>throw new NotSupportedException("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)",true)]
 public Component renderer=>throw new NotSupportedException("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)",true)]
 public Component audio=>throw new NotSupportedException("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)",true)]
 public Component networkView=>throw new NotSupportedException("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)",true)]
 public Component collider=>throw new NotSupportedException("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)",true)]
 public Component collider2D=>throw new NotSupportedException("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)",true)]
 public Component hingeJoint=>throw new NotSupportedException("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)",true)]
 public Component particleSystem=>throw new NotSupportedException("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)");
}
public partial class GameObject {
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("GameObject.SampleAnimation(AnimationClip, float) has been deprecated. Use AnimationClip.SampleAnimation(GameObject, float) instead (UnityUpgradable).",true)]
 public void SampleAnimation(Object clip, float time)=>throw new NotSupportedException("GameObject.SampleAnimation(AnimationClip, float) has been deprecated. Use AnimationClip.SampleAnimation(GameObject, float) instead (UnityUpgradable).");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("GameObject.AddComponent with string argument has been deprecated. Use GameObject.AddComponent<T>() instead. (UnityUpgradable).",true)]
 public Component AddComponent(string className)=>throw new NotSupportedException("GameObject.AddComponent with string argument has been deprecated. Use GameObject.AddComponent<T>() instead. (UnityUpgradable).");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("gameObject.PlayAnimation is not supported anymore. Use animation.Play()",true)]
 public void PlayAnimation(Object animation)=>throw new NotSupportedException("gameObject.PlayAnimation is not supported anymore. Use animation.Play()");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("gameObject.StopAnimation is not supported anymore. Use animation.Stop()",true)]
 public void StopAnimation()=>throw new NotSupportedException("gameObject.StopAnimation is not supported anymore. Use animation.Stop()");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)",true)]
 public Component rigidbody=>throw new NotSupportedException("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)",true)]
 public Component rigidbody2D=>throw new NotSupportedException("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)",true)]
 public Component camera=>throw new NotSupportedException("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)",true)]
 public Component light=>throw new NotSupportedException("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)",true)]
 public Component animation=>throw new NotSupportedException("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)",true)]
 public Component constantForce=>throw new NotSupportedException("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)",true)]
 public Component renderer=>throw new NotSupportedException("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)",true)]
 public Component audio=>throw new NotSupportedException("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)",true)]
 public Component networkView=>throw new NotSupportedException("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)",true)]
 public Component collider=>throw new NotSupportedException("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)",true)]
 public Component collider2D=>throw new NotSupportedException("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)",true)]
 public Component hingeJoint=>throw new NotSupportedException("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)");
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
 [Obsolete("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)",true)]
 public Component particleSystem=>throw new NotSupportedException("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)");
}
