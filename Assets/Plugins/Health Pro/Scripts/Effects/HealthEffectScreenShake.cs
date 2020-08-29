using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Screen Shake")]
    public class HealthEffectScreenShake : MonoBehaviour, IDamageable, IKillable, IHealable, IHealCallback , IDamageCallback
    {
        [SerializeField]
        private ScreenShakeConfig[] shakeConfigs = null;

        [SerializeField]
        protected new Transform camera;

        protected ScreenShaker cameraShaker;

        protected static ScreenShaker cachedCameraShaker;
        protected static int totalCameraShakerEffects;

        private void Awake()
        {
            totalCameraShakerEffects++;

            cameraShaker = AssignShaker();
        }

        protected virtual ScreenShaker AssignShaker()
        {
            if (cachedCameraShaker != null)
            {
                return cachedCameraShaker;
            }
            else
            {
                if (camera == null)
                {
                    Camera getCamera = Camera.main;

                    if (getCamera != null)
                    {
                        camera = getCamera.transform;
                    }
                }

                if (camera != null)
                {
                    ScreenShaker shaker =  camera.gameObject.GetComponent<ScreenShaker>() as ScreenShaker;

                    if (shaker == null)
                    {
                        shaker = camera.gameObject.AddComponent<ScreenShaker>() as ScreenShaker;
                    }

                    return shaker;
                }
            }

            return null;
        }

        // Ensure the static reference gets destroyed when there are no Camera Shake health effects.
        private void OnDestroy()
        {
            totalCameraShakerEffects--;

            if (totalCameraShakerEffects == 0)
            {
                cachedCameraShaker = null;
            }
        }

        public void OnDamaged(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnDamaged);
        }

        public void OnDeath(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnDeath);
        }

        public void OnHealed(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnHealed);
        }

        public void OnHealthRestored(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnRestored);
        }

        public void OnHealDone(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnHealCallback, info.Owner.transform);
        }

        public void OnDamageDone(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                OnEvent(EEffectCondition.OnDamageCallback, info.Owner.transform);
            }
            else
            {
                OnEvent(EEffectCondition.OnDamageCallbackKill, info.Owner.transform);
            }
        }

        public virtual void OnEvent(EEffectCondition condition, Transform target = null)
        {
            for (int i = 0; i < shakeConfigs.Length; i++)
            {
                if (shakeConfigs[i].Condition == condition)
                {
                    if (shakeConfigs[i].FilterTargetTag)
                    {
                        if (condition == EEffectCondition.OnDamageCallback || condition == EEffectCondition.OnDamageCallbackKill)
                        {
                            if (!shakeConfigs[i].HasMatchingTag(target))
                                continue;
                        }
                    }

                    cameraShaker.Shake(shakeConfigs[i].PositionShakeIntensity, shakeConfigs[i].RotationShakeIntensity, shakeConfigs[i].ShakeDuration);
                }
            }
        }
    }
}