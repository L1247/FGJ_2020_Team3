using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [System.Serializable]
    public class ScreenShakeConfig
    {
        public ScreenShakeConfig()
        {
            rotationShakeIntensity = new Vector3(0.5f, 0.5f, 0.5f);
            shakeDuration = 0.2f;
        }

        [SerializeField]
        private EEffectCondition condition;
        public EEffectCondition Condition
        {
            get { return condition; }
        }

        [SerializeField]
        private Vector3 positionShakeIntensity;
        public UnityEngine.Vector3 PositionShakeIntensity
        {
            get { return positionShakeIntensity; }
        }

        [SerializeField]
        private Vector3 rotationShakeIntensity;
        public UnityEngine.Vector3 RotationShakeIntensity
        {
            get { return rotationShakeIntensity; }
        }

        [SerializeField]
        private float shakeDuration;
        public float ShakeDuration
        {
            get { return shakeDuration; }
        }

        [Header("If condition is set to callback")]
        [SerializeField, Tooltip("This only happens when the effect condition is " +
        "set to Callback, only executing if the target is set to a specific tag")]
        private bool filterTargetTag = false;
        public bool FilterTargetTag { get { return filterTargetTag; } }

        [SerializeField]
        private string[] tags = new string[0];

        public void Apply()
        {

        }

        public bool HasMatchingTag(Transform target)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (target.CompareTag(tags[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}