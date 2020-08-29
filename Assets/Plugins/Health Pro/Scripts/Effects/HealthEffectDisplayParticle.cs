using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Display Particle")]
    public class HealthEffectDisplayParticle : MonoBehaviour, IDamageable, IHealable, IKillable, IReviveable, IDamageCallback, IHealCallback
    {
        [System.Serializable]
        private class ParticleConfiguration
        {
            [SerializeField]
            private EEffectCondition eEffectCondition = EEffectCondition.OnDamaged;
            public EEffectCondition EffectCondition
            {
                get { return eEffectCondition; }
            }

            [SerializeField]
            private EEffectLocation eEffectLocation = EEffectLocation.Hit;
            public EEffectLocation EEffectLocation
            {
                get { return eEffectLocation; }
            }

            [SerializeField]
            private GameObject[] particlePrefabs = null;

            public void Spawn(HealthInfo info, Transform transform)
            {
                for (int i = 0; i < particlePrefabs.Length; i++)
                {
                    bool isRecycled = false;

                    Quaternion rot = (info.HitLocation != transform.position) ?
                        Quaternion.LookRotation(info.HitLocation - transform.position) : transform.rotation;

                    Vector3 spawnLocation = info.HitLocation;

                    switch (eEffectLocation)
                    {
                        case EEffectLocation.Hit:
                            spawnLocation = info.HitLocation;

                            break;
                        case EEffectLocation.Owner:

                            Health getOwner = info.Owner;
                            spawnLocation = (getOwner != null) ? info.Owner.transform.position : info.HitLocation;

#if UNITY_EDITOR
                            if (getOwner == null)
                            Debug.LogWarning("No owner found for Display Particle");
#endif

                            break;
                        default:
                            break;
                    }

                    GameObject spawnedParticle = PoolManager.SpawnObject(particlePrefabs[i], spawnLocation, rot, out isRecycled);

                    if (!isRecycled)
                    {
                        spawnedParticle.AddComponent<ReturnToPoolOnDisable>();
                    }
                }
            }
        }

        [SerializeField]
        private ParticleConfiguration[] particleConfigs = null;

        public void OnDamaged(HealthInfo info)
        {
            SpawnParticle(EEffectCondition.OnDamaged, info);
        }

        public void OnDeath(HealthInfo info)
        {
            SpawnParticle(EEffectCondition.OnDeath, info);
        }

        public void OnRevive(HealthInfo info)
        {
            SpawnParticle(EEffectCondition.OnRevive, info);
        }

        public void OnHealed(HealthInfo info)
        {
            if (info.IsHealthRestored())
            {
                SpawnParticle(EEffectCondition.OnRestored, info);
            }
            else
            {
                SpawnParticle(EEffectCondition.OnHealed, info);
            }
        }

        public void OnHealDone(HealthInfo info)
        {
            SpawnParticle(EEffectCondition.OnHealCallback, info);
        }

        public void OnDamageDone(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                SpawnParticle(EEffectCondition.OnDamageCallback, info);
            }
            else
            {
                SpawnParticle(EEffectCondition.OnDamageCallbackKill, info);
            }
        }

        private void SpawnParticle(EEffectCondition eEffectCondition, HealthInfo info)
        {
            for (int i = 0; i < particleConfigs.Length; i++)
            {
                if (eEffectCondition == particleConfigs[i].EffectCondition)
                {
                    particleConfigs[i].Spawn(info, this.transform);
                }
            }
        }
    }

}