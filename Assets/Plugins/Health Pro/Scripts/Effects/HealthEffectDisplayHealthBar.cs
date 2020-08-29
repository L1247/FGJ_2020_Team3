using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.HealthPro.Pooling;
using Lowscope.HealthPro.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Display Health Bar"), DisallowMultipleComponent]
    public class HealthEffectDisplayHealthBar : MonoBehaviour, IDamageable, IKillable, IHealable, IHealthBarListener, IReviveable
    {
        [SerializeField]
        private GameObject healthBarPrefab = null;

        [SerializeField]
        private EObtainHeightMethod obtainHeightMethodology = EObtainHeightMethod.ColliderHeightWithHeightOffset;

        [SerializeField]
        private float heightOffset = 0;

        [SerializeField]
        private float scale = 1;

        [SerializeField]
        private new Collider collider;

        // We want to keep the instance as long as it is displayed.
        // So we would need to check if it has been released to the pool or not

        private HealthBar healthBarInstance { set; get; }

        private HealthBar GetHealthBarInstance(HealthInfo info)
        {
            if (healthBarInstance == null)
            {
                bool isRecycled;

                healthBarInstance = PoolManager.SpawnObject(healthBarPrefab, out isRecycled).GetComponent<HealthBar>();
                healthBarInstance.transform.rotation = healthBarPrefab.transform.rotation;

                if (!isRecycled)
                {
                    healthBarInstance.gameObject.AddComponent<ReturnToPoolOnDisable>();
                }

                if (collider == null)
                {
                    collider = this.GetComponent<Collider>();
                }

                healthBarInstance.Configure(this.transform, collider, this, obtainHeightMethodology, heightOffset, scale * 0.01f, info);
            }

            return healthBarInstance;
        }

        public void OnDamaged(HealthInfo info)
        {
            GetHealthBarInstance(info).OnDamaged(info);
        }

        public void OnDeath(HealthInfo info)
        {
            GetHealthBarInstance(info).OnDeath(info);
        }

        public void OnHealed(HealthInfo info)
        {
            GetHealthBarInstance(info).OnHealed(info);
        }

        public void OnRevive(HealthInfo info)
        {
            GetHealthBarInstance(info).OnRevive(info);
        }

        public void OnHealthBarClear()
        {
            healthBarInstance = null;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(HealthEffectDisplayHealthBar), true)]
    public class HealthEffectHealthBarInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("You can modify global settings within the health bar prefab.", MessageType.Info);

            base.DrawDefaultInspector();
        }
    }

#endif

}