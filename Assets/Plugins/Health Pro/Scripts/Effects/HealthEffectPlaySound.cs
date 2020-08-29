using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Play Sound")]
    public class HealthEffectPlaySound : MonoBehaviour, IDamageable, IKillable, IReviveable, IHealable, IDamageCallback, IHealCallback
    {
        [System.Serializable]
        private class SoundConfig
        {
            [SerializeField]
            private EEffectCondition eEffectCondition = EEffectCondition.OnDamaged;
            public EEffectCondition EffectCondition
            {
                get { return eEffectCondition; }
            }

            [SerializeField]
            private AudioClip[] sounds = null;

            [SerializeField]
            private float minPitch = 0.9f;

            [SerializeField]
            private float maxPitch = 1.1f;

            public void SetPitch(float min, float max)
            {
                minPitch = min;
                maxPitch = max;
            }

            public AudioClip ReturnRandomSound()
            {
                if (sounds.Length > 0)
                {
                    return sounds[Random.Range(0, sounds.Length)];
                }
                else
                {
                    return null;
                }
            }

            public float ReturnRandomPitch()
            {
                return Random.Range(minPitch, maxPitch);
            }
        }

        [SerializeField]
        private SoundConfig[] soundConfig;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField, Tooltip("Will create a global audio instance")]
        private bool useGlobalAudio;

        private static AudioSource globalAudioInstance;

        private void Start()
        {
            if (!useGlobalAudio)
            {
                if (audioSource == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(string.Format("No audiosource attached to :{0}", this.gameObject.name));
#endif
                    // We remove this component in case it has no audio source to play from.
                    GameObject.Destroy(this);
                }
            }
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            soundConfig = new SoundConfig[1] { new SoundConfig() };
            soundConfig[0].SetPitch(0.9f, 1.1f);
        }

        public void OnDamaged(HealthInfo info)
        {
            PlaySound(EEffectCondition.OnDamaged);
        }

        public void OnDeath(HealthInfo info)
        {
            PlaySound(EEffectCondition.OnDeath);
        }

        public void OnHealed(HealthInfo info)
        {
            if (info.IsHealthRestored())
            {
                PlaySound(EEffectCondition.OnRestored);
            }
            else
            {
                PlaySound(EEffectCondition.OnHealed);
            }
        }

        public void OnRevive(HealthInfo info)
        {
            PlaySound(EEffectCondition.OnRevive);
        }

        public void OnHealthRestored(HealthInfo info)
        {
            PlaySound(EEffectCondition.OnRestored);
        }

        public void OnHealDone(HealthInfo info)
        {
            PlaySound(EEffectCondition.OnHealCallback);
        }

        public void OnDamageDone(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                PlaySound(EEffectCondition.OnDamageCallback);
            }
            else
            {
                PlaySound(EEffectCondition.OnDamageCallbackKill);
            }
        }

        private void PlaySound(EEffectCondition eEffectCondition)
        {
            for (int i = 0; i < soundConfig.Length; i++)
            {
                if (soundConfig[i].EffectCondition == eEffectCondition)
                {
                    if (!useGlobalAudio)
                    {
                        audioSource.pitch = soundConfig[i].ReturnRandomPitch();
                        audioSource.PlayOneShot(soundConfig[i].ReturnRandomSound());
                    } else
                    {
                        if (globalAudioInstance == null)
                        {
                            globalAudioInstance = new GameObject("Global Audio Source").AddComponent<AudioSource>();
                        }

                        globalAudioInstance.pitch = soundConfig[i].ReturnRandomPitch();
                        globalAudioInstance.PlayOneShot(soundConfig[i].ReturnRandomSound());
                    }
                }
            }
        }
    }

}