using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Screen Overlay On Damage")]
    public class HealthEffectScreenOverlayOnDamage : MonoBehaviour, IDamageable
    {
        // This is defined in the script defaults.
        // Which you can find by clicking on the script in the inspector.
        [SerializeField]
        private GameObject screenFlash = null;

        private static CanvasGroup screenFlashInstance;
        private static int screenFlashEffectCount;

        [SerializeField]
        public AnimationCurve intensityCurve = new AnimationCurve
            (
                new Keyframe(0, 0),
                new Keyframe(0.05f, 1, -5, -10),
                new Keyframe(0.5f, 0)
            );

        private void Awake()
        {
            screenFlashEffectCount++;

            if (screenFlashInstance == null && screenFlash != null)
            {
                screenFlashInstance = GameObject.Instantiate(screenFlash).GetComponent<CanvasGroup>();
                screenFlashInstance.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            screenFlashEffectCount--;

            if (screenFlashEffectCount <= 0)
            {
                if (screenFlashInstance != null)
                {
                    GameObject.Destroy(screenFlashInstance.gameObject);
                    screenFlashInstance = null;
                }
            }
        }

        public void OnDamaged(HealthInfo info)
        {
            StopAllCoroutines();
            StartCoroutine(ScreenFlashCoroutine());
        }

        private IEnumerator ScreenFlashCoroutine()
        {
            float curveLength = intensityCurve.length;

            float t = 0;

            screenFlashInstance.alpha = 0;

            screenFlashInstance.gameObject.SetActive(true);

            while (t < curveLength)
            {
                screenFlashInstance.alpha = intensityCurve.Evaluate(t);
                t += Time.deltaTime;
                yield return null;
            }

            screenFlashInstance.gameObject.SetActive(false);
        }
    }

}