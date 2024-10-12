using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingParty
{
    public class AnimationEffect : SpecialEffectController
    {
        public Animation animation;

        string queueAnimation;
        public virtual void Emit(string aname, float delay, float scale = 1)
        {
            queueAnimation = aname;
            Emit(delay, scale);
        }
        protected override IEnumerator EmitOnce(float delay)
        {
            transform.localScale = Vector3.zero;
            yield return null;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            transform.localScale = Vector3.one * realScale;
            if (animation.GetClip(queueAnimation) == null)
                Debug.LogWarning(name + " animator missing animation " + queueAnimation);
            animation.Play(queueAnimation);
            while (animation.isPlaying)
                yield return new WaitForEndOfFrame();
            Kill();
        }
        protected override void Kill()
        {
            base.Kill();
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}