using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays animations on Hit and on Death
/// </summary>

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Play Animation")]
    public class HealthEffectPlayAnimation : MonoBehaviour, IDamageable, IKillable, IReviveable, IHealable, IDamageCallback, IHealCallback
    {
        [System.Serializable]
        public class AnimationEffect
        {
            [SerializeField]
            private string playAnimation = "";

            [SerializeField, Tooltip("Keep at 0,0 if you do not want to use any crossfading." +
                "Else it will cross fade the animation randomly between the minimum time (x) and maximum time (y)")]
            private Vector2 crossFadeTime = new Vector2(0,0);
            public Vector2 CrossFadeTime
            {
                get { return crossFadeTime; }
            }

            [SerializeField]
            private EEffectCondition eEffectCondition = EEffectCondition.OnDamaged;
            public EEffectCondition EffectCondition
            {
                get { return eEffectCondition; }
            }

            private int animationHash = 0;
            public int AnimationHash
            {
                get
                {
                    if (animationHash == 0)
                    {
                        animationHash = Animator.StringToHash(playAnimation);
                    }

                    return animationHash;
                }
            }
        }

        [SerializeField]
        private AnimationEffect[] effects = null;

        [SerializeField]
        private Animator animator;

#if UNITY_EDITOR

        private void Reset()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

#endif

        public void OnDamaged(HealthInfo info)
        {
            ApplyAnimation(EEffectCondition.OnDamaged);
        }

        public void OnDeath(HealthInfo info)
        {
            ApplyAnimation(EEffectCondition.OnDeath);
        }

        public void OnRevive(HealthInfo info)
        {
            ApplyAnimation(EEffectCondition.OnRevive);
        }

        public void OnHealed(HealthInfo info)
        {
            if (info.IsHealthRestored())
            {
                ApplyAnimation(EEffectCondition.OnRestored);
            }
            else
            {
                ApplyAnimation(EEffectCondition.OnHealed);
            }
        }

        public void OnHealDone(HealthInfo info)
        {
            ApplyAnimation(EEffectCondition.OnHealCallback);
        }

        public void OnDamageDone(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                ApplyAnimation(EEffectCondition.OnDamageCallback);
            }
            else
            {
                ApplyAnimation(EEffectCondition.OnDamageCallbackKill);
            }
        }

        private void ApplyAnimation(EEffectCondition eEffectCondition)
        {
            if (animator == null)
            {
                Debug.LogWarning("Health Effect Play Animation: No animator found reference" +
                    " set on Health Effect Animation");
            }

            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].AnimationHash != 0)
                {
                    if (effects[i].EffectCondition == eEffectCondition)
                    {
                        if (effects[i].CrossFadeTime == default(Vector2))
                        {
                            animator.PlayInFixedTime(effects[i].AnimationHash);
                        }
                        else
                        {
                            float fadeTime = Random.Range(effects[i].CrossFadeTime.x, effects[i].CrossFadeTime.y);
                            animator.CrossFadeInFixedTime(effects[i].AnimationHash, fadeTime);
                        }
                    }
                }
            }
        }
    }

}