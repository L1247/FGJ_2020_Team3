using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro
{
    // Set to not display in the search menu, to avoid clutter
    // [AddComponentMenu("")] if you want it to display anyways.
    [AddComponentMenu("")]
    public class TextPopupDisplayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Text text = null;

        [SerializeField]
        private CanvasGroup canvasGroup = null;

        [Header("Configuration")]

        [SerializeField, Tooltip("Minimum and maximum time of how long a number is displayed")]
        private Vector2 minMaxDisplayTime = new Vector2(1, 1.2f);

        [SerializeField]
        private AnimationCurve fade = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

        [SerializeField]
        private AnimationCurve verticalOffset = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [SerializeField]
        private AnimationCurve scaleMultiplier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 2));

        [SerializeField]
        private Vector3 baseScale = new Vector3(0.1f,0.1f,0.1f);

        [SerializeField]
        private Vector3 baseOffset = Vector3.zero;

        [SerializeField]
        private Vector3 randomOffset = Vector3.zero;

        [Header("Look at player configuration")]
        [SerializeField]
        private bool lookAtPlayerCamera = true;

        [SerializeField]
        private bool limitLookatXRotation = false;

        [SerializeField]
        private bool limitLookatYRotation = false;

        [SerializeField]
        private bool limitLookatZRotation = false;

        private float t;

        private float currentDisplayTime;
        private Vector3 spawnPosition;
        private Vector3 currentRandomOffset;
        private Vector3 upDirection;

        // Cache static camera for all Health Bars, this is to reduce lookup costs
        // In case you have custom methodologies, replace cachedCamera with your own reference
        private static Camera cachedCamera;
        private static int totalTextPopups;

        private void Awake()
        {
            totalTextPopups++;
        }

        private void OnDestroy()
        {
            totalTextPopups--;

            if (totalTextPopups <= 0)
            {
                cachedCamera = null;
            }
        }

        public void Configure(Vector3 spawnPosition, string content, Color color, float height)
        {
            this.transform.position = spawnPosition;
            this.enabled = true;
            this.spawnPosition = spawnPosition + baseOffset;
            this.t = 0;
            this.text.text = content;
            this.text.color = color;
            this.upDirection = transform.up;

            currentRandomOffset = transform.up * height;

            for (int i = 0; i < 3; i++)
            {
                currentRandomOffset[i] += Random.Range(-randomOffset[i], randomOffset[i]);
            }

            currentDisplayTime = Random.Range(minMaxDisplayTime.x, minMaxDisplayTime.y);
        }

        public void Update()
        {
            if (t < currentDisplayTime)
            {
                // Let the text look at the player camera
                if (lookAtPlayerCamera)
                {
                    if (cachedCamera == null)
                    {
                        cachedCamera = Camera.main;
                    }

                    Quaternion lookRot = Quaternion.LookRotation(cachedCamera.transform.forward);
                    Vector3 euler = lookRot.eulerAngles;
                    euler.x = limitLookatXRotation ? 0 : euler.x;
                    euler.y = limitLookatYRotation ? 0 : euler.y;
                    euler.z = limitLookatZRotation ? 0 : euler.z;

                    transform.rotation = Quaternion.Euler(euler);
                }

                float completionPercentage = t / currentDisplayTime;

                canvasGroup.alpha = fade.Evaluate(completionPercentage);

                Vector3 direction = upDirection * verticalOffset.Evaluate(completionPercentage);

                this.transform.position = spawnPosition + direction + currentRandomOffset;
                text.transform.localScale = baseScale * scaleMultiplier.Evaluate(completionPercentage);

                t += Time.deltaTime;
            }
            else
            {
                this.enabled = false;
                PoolManager.ReleaseObject(this.gameObject);
            }
        }

    }

}