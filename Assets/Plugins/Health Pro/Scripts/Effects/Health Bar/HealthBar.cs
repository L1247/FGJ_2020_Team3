using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro.Utilities
{
    // Set to not display in the search menu, to avoid clutter
    // [AddComponentMenu("")] if you want it to display anyways.
    [AddComponentMenu(""), DisallowMultipleComponent]
    public class HealthBar : MonoBehaviour, IDamageable, IHealable, IKillable, IHealthConfigure
    {
        [Header("References")]
        [SerializeField]
        private Slider slider = null;

        [SerializeField]
        private CanvasGroup canvasGroup = null;

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private Image healthBarImage = null;

        [Header("Canvas Group Fading")]
        [SerializeField]
        private bool fadeVisilibity = true;
        [SerializeField, Tooltip("Disable the health bar when the entitiy has died")]
        private bool disableOnDeath = true;
        [SerializeField, Tooltip("If Disable On Death is toggled, it will fade out.")]
        private bool disableOnDeathFade = false;
        [SerializeField]
        private float fadeInTime = 1;
        [SerializeField]
        private float fadeOutTime = 0.2f;

        [Header("Slider Easing")]
        [SerializeField]
        private bool easeSlider = true;
        [SerializeField]
        private float easeSliderTime = 0.15f;

        [Header("Slider Flashing")]
        [SerializeField]
        private bool flashSlider = true;
        [SerializeField]
        private float flashFadeInTime = 0.15f;
        [SerializeField]
        private float flashFadeOutTime = 0.15f;
        [SerializeField]
        private Color sliderFlashColor = Color.white;
        [SerializeField]
        private Color sliderColor = Color.red;

        [Header("Look at player configuration")]
        [SerializeField]
        private bool lookAtPlayerCamera = true;

        [SerializeField]
        private bool limitXRotation = false;

        [SerializeField]
        private bool limitYRotation = false;

        [SerializeField]
        private bool limitZRotation = false;

        private Transform owner;

        private float height;

        private Vector3 heightVector;

        private IHealthBarListener listener;

        // Cache static camera for all Health Bars, this is to reduce lookup costs
        // In case you have custom methodologies, replace cachedCamera with your own reference
        private static Camera cachedCamera;
        private static int totalHealthBars;

        private bool configured;

        private void Awake()
        {
            totalHealthBars++;
        }

        private void OnDestroy()
        {
            totalHealthBars--;

            if (totalHealthBars <= 0)
            {
                cachedCamera = null;
            }
        }

        public void Configure(Transform owner, Collider collider, IHealthBarListener listener, EObtainHeightMethod obtainHeightMethod, float heightOffset, float scale, HealthInfo info)
        {
            this.owner = owner;
            this.listener = listener;

            slider.transform.localScale = Vector3.one * scale;

            switch (obtainHeightMethod)
            {
                case EObtainHeightMethod.ColliderHeightWithHeightOffset:

                    if (collider == null)
                    {
                        collider = owner.GetComponent<Collider>();
                    }

                    if (collider != null)
                    {
                        height = (collider.bounds.extents.y) + heightOffset;
                    }
                    else
                    {
                        height = heightOffset;
#if UNITY_EDITOR
                        Debug.Log(string.Format("Display Health Bar Effect: " +
                            "Could not find collider for : {0}. Either change option" +
                            "to use Height Offset only or reference a collider.", owner.name));
#endif
                    }

                    break;
                case EObtainHeightMethod.Collider2DHeightWithHightOffset:

                    Collider2D getCollider2D = owner.GetComponent<Collider2D>();

                    if (getCollider2D != null)
                    {
                        height = (getCollider2D.bounds.size.y * 0.5f) + heightOffset;
                    }

                    break;
                case EObtainHeightMethod.HeightOffset:

                    height = heightOffset;

                    break;
                default:
                    break;
            }

            heightVector = transform.up * height;

            transform.position = owner.transform.position + heightVector;

            healthBarImage.color = sliderColor;

            OnHealthConfigure(info);

            configured = true;

            this.enabled = true;
        }

        public void OnHealthConfigure(HealthInfo info)
        {
            StopAllCoroutines();
            slider.maxValue = info.TotalHealth;
            slider.value = info.CurrentHealth;
        }

        private void Update()
        {
            if (!configured) { return; }

            // In case the healthbar has no owner, we stop this Update.
            if (this.owner == null)
            {
                this.enabled = false;
                return;
            }

            if (canvasGroup.alpha == 0)
            {
                return;
            }

            if (!owner.gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutCoroutine());
                owner = null;
                return;
            }

            // Ensure that the health bar is above the transform of the owner
            if (owner.transform.hasChanged)
            {
                transform.position = owner.transform.position + heightVector;
            }

            // Let the health bar look at the player camera
            if (lookAtPlayerCamera)
            {
                if (cachedCamera == null)
                {
                    cachedCamera = Camera.main;

                    if (cachedCamera == null)
                    {
                        Debug.LogWarning("HealthBar: Cannot find a camera with Camera.Main(). " +
                            "Have you set the tag of your main camera to MainCamera?");

                        lookAtPlayerCamera = false;

                        return;
                    }
                }

                Quaternion lookRot = Quaternion.LookRotation(cachedCamera.transform.forward);
                Vector3 euler = lookRot.eulerAngles;
                euler.x = limitXRotation ? 0 : euler.x;
                euler.y = limitYRotation ? 0 : euler.y;
                euler.z = limitZRotation ? 0 : euler.z;

                transform.rotation = Quaternion.Euler(euler);
            }
        }

        public void OnRevive(HealthInfo info)
        {
            slider.maxValue = info.TotalHealth;
            slider.value = info.CurrentHealth;
        }

        public void OnDamaged(HealthInfo healthInfo)
        {
            StopAllCoroutines();

            if (fadeVisilibity && healthInfo.CurrentHealth > 0)
            {
                if (this.gameObject.activeSelf)
                {
                    StartCoroutine(FadeBarCoroutine(1, fadeInTime));
                }
            }
            else
            {
                canvas.enabled = true;
                canvasGroup.alpha = 1;
            }

            if (easeSlider)
            {
                if (this.gameObject.activeSelf)
                {
                    StartCoroutine(EaseHealthCoroutine(healthInfo.CurrentHealth, healthInfo.TotalHealth));
                }
            }
            else
            {
                slider.maxValue = healthInfo.TotalHealth;
                slider.value = healthInfo.CurrentHealth;
            }

            if (flashSlider)
            {
                if (this.gameObject.activeSelf)
                    StartCoroutine(FlashSliderCoroutine());
            }
        }

        public void OnHealed(HealthInfo healInfo)
        {
            StopAllCoroutines();

            if (fadeVisilibity)
            {
                StartCoroutine(FadeBarCoroutine(1, fadeInTime));
            }
            else
            {
                canvas.enabled = true;
                canvasGroup.alpha = 1;
            }

            if (easeSlider)
            {
                if (this.gameObject.activeSelf)
                    StartCoroutine(EaseHealthCoroutine(healInfo.CurrentHealth, healInfo.TotalHealth));
            }
            else
            {
                slider.maxValue = healInfo.TotalHealth;
                slider.value = healInfo.CurrentHealth;
            }
        }

        public void OnDeath(HealthInfo healthInfo)
        {
            slider.value = 0;

            if (disableOnDeath)
            {
                StopAllCoroutines();

                if (fadeVisilibity || disableOnDeathFade)
                {
                    StartCoroutine(FadeOutCoroutine());
                }
                else
                {
                    this.owner = null;
                    this.gameObject.SetActive(false);
                }
            }
        }

        IEnumerator EaseHealthCoroutine(float currentHealth, float totalHealth)
        {
            canvasGroup.alpha = 1;

            if (slider.maxValue != totalHealth)
            {
                slider.maxValue = totalHealth;
            }

            float t = 0;
            float currentSliderValue = slider.value;

            while (t < easeSliderTime)
            {
                slider.value = Mathf.Lerp(currentSliderValue, currentHealth, t / easeSliderTime);
                t += Time.deltaTime;

                yield return null;
            }

            slider.value = currentHealth;

            if (fadeVisilibity)
            {
                yield return new WaitForSeconds(2f);
                StartCoroutine(FadeBarCoroutine(0, fadeOutTime));
            }
        }

        IEnumerator FadeBarCoroutine(float targetAlpha, float fadeTime)
        {
            if (targetAlpha == 1)
            {
                canvas.enabled = true;
            }

            float currentAlpha = canvasGroup.alpha;
            float t = 0;

            while (t < fadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, t / fadeTime);
                t += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;

            if (targetAlpha == 0)
            {
                canvas.enabled = false;
            }
        }

        IEnumerator FadeOutCoroutine()
        {
            float t = 0;

            while (t < fadeOutTime)
            {
                yield return null;
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, t / fadeOutTime);
                t += Time.deltaTime;
            }

            // This callback is to release any references to this health bar.
            if (listener != null)
            {
                listener.OnHealthBarClear();
            }

            this.owner = null;
            this.gameObject.SetActive(false);
        }

        IEnumerator FlashSliderCoroutine()
        {
            healthBarImage.color = sliderColor;

            float t = 0;

            while (t <= flashFadeInTime)
            {
                healthBarImage.color = Color.Lerp(sliderColor, sliderFlashColor, t / flashFadeInTime);
                t += Time.deltaTime;
                yield return null;
            }

            t = 0;

            while (t <= flashFadeOutTime)
            {
                healthBarImage.color = Color.Lerp(sliderFlashColor, sliderColor, t / flashFadeOutTime);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

}