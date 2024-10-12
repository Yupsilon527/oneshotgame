using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingParty
{
    public class SpecialEffectController : MonoBehaviour
    {
        protected ParticleSystem[] particles;
        protected Animator animator;
        protected float realScale = 1;
        protected virtual void Awake()
        {
            List<ParticleSystem> pSys = new List<ParticleSystem>();
            if (TryGetComponent(out ParticleSystem particle))
                pSys.Add(particle);
            pSys.AddRange(GetComponentsInChildren<ParticleSystem>());
            particles = pSys.ToArray();
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        public virtual void Emit(float delay, float scale = 1)
        {
            realScale = scale;
            StopAllCoroutines();
            StartCoroutine(EmitOnce(delay));
        }
        protected virtual IEnumerator EmitOnce(float delay)
        {
            reps = 0;
            transform.localScale = Vector3.zero;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            transform.localScale = Vector3.one * realScale;
            Play();
        }

        public virtual void Emit(float delay, int repeats, float scale = 1)
        {
            realScale = scale;
            float duration = 0;
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                //pmain.startColor = color;
                //pmain.startSpeedMultiplier = scale;
                duration = Mathf.Max(pmain.duration + pmain.startLifetime.constantMax);
            }
            StartCoroutine(EmitMultiple(delay, duration, repeats, scale));
        }
        public virtual void ChangeColor(Color color)
        {
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                pmain.startColor = color;
            }
        }
        int reps = 0;
        protected virtual IEnumerator EmitMultiple(float delay, float duration, int repeats, float scale)
        {
            realScale = scale;
            reps = repeats;
            transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);
            transform.localScale = Vector3.one * realScale;
            while (reps > 0)
            {
                Play();
                reps--;
                yield return new WaitForSeconds(duration);
            }
        }
        protected void Play()
        {
            foreach (ParticleSystem p in particles)
            {
                p.Play();
            }
            if (animator != null)
                animator.SetTrigger("Play");
        }
        public ObjectPool assignedObjectPool;
        public void Stop()
        {
            foreach (ParticleSystem p in particles)
            {
                p.Stop();
            }
            assignedObjectPool?.DeactivateObject(gameObject);
            /*if (attachedModifier!=null)
            {
                attachedModifier.DetachParticle(this);
            }*/
        }
        private void OnParticleSystemStopped()
        {
            Kill();
        }
        protected virtual void Kill()
        {
            if (reps == 0)
            {
                Stop();
            }
        }
    }

}