using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Punch Scale On Damage")]
    public class HealthEffectPunchScaleOnDamage : MonoBehaviour, IDamageable, IKillable
    {
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private float effectTime = 0.5f;

        [SerializeField]
        AnimationCurve curve = new AnimationCurve()
        {
            keys = new Keyframe[2] { new Keyframe(0,1.5f), new Keyframe(1f,1f) }
        };

        private Vector3 baseScale;

        private void Start()
        {
            baseScale = target.transform.localScale;
        }

        public void OnDamaged(HealthInfo info)
        {
            StopAllCoroutines();
            StartCoroutine(PunchScaleCoroutine());   
        }

        public void OnDeath(HealthInfo info)
        {
            target.transform.localScale = baseScale;
        }

        private IEnumerator PunchScaleCoroutine()
        {
            float t = 0;

            while (t < effectTime)
            {
                target.transform.localScale = baseScale * curve.Evaluate(t / effectTime);
                yield return null;
                t += Time.deltaTime;
            }
        }
    }
}