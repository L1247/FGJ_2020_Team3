using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Lowscope.HealthPro
{
    /// <summary>
    /// Usefull if you want to track the health of a group of entities that have health.
    /// For instance when you need to know if a room is cleared.
    /// </summary>
    [AddComponentMenu("Health/Utility/Health Group", -1), DisallowMultipleComponent]
    public class HealthGroup : Health, IDamageable, IHealable, IKillable, IReviveable
    {
        [SerializeField]
        private List<Health> healthComponents = null;

        [SerializeField]
        private bool obtainFromChildren = false;

        [SerializeField, Tooltip("Will actively keep scanning and subscribing if new entities enter the volume")]
        private bool obtainFromVolume = false;
        public bool ObtainFromVolume { set { obtainFromVolume = value; } get { return obtainFromVolume; } }

        private int totalAliveEntities;

        [System.Serializable]
        public class UnityEventInt : UnityEvent<int> { }

        [SerializeField]
        private UnityEventInt onGroupAliveCountChange = null;
        public UnityEventInt OnGroupAliveCountChange { get { return onGroupAliveCountChange; } }

        [SerializeField]
        private UnityEventInt onGroupTotalCountChange = null;
        public UnityEventInt OnGroupTotalCountChange { get { return onGroupTotalCountChange; } }

        [SerializeField]
        private UnityEvent onGroupEmpty = null;
        public UnityEvent OnGroupEmpty { get { return onGroupEmpty; } }

        [SerializeField]
        private UnityEvent onGroupEntered = null;
        public UnityEvent OnGroupEntered { get { return onGroupEntered; } }

        private bool groupCleared;

        private void Start()
        {
            if (obtainFromChildren)
            {
                healthComponents.AddRange(GetComponentsInChildren<Health>());
            }

            for (int i = 0; i < healthComponents.Count; i++)
            {
                StartListening(healthComponents[i]);
            }

            UpdateGroupData();
        }

        private void StartListening(Health target)
        {
            target.AddListener(this as IDamageable);
            target.AddListener(this as IHealable);
            target.AddListener(this as IKillable);
            target.AddListener(this as IReviveable);
        }
        
        private void StopListening(Health target)
        {
            target.RemoveListener(this as IDamageable);
            target.RemoveListener(this as IHealable);
            target.RemoveListener(this as IKillable);
            target.RemoveListener(this as IReviveable);
        }

        public void AddToGroup(Health health)
        {
            healthComponents.Add(health);

            StartListening(health);

            UpdateGroupData();
        }

        public void RemoveFromGroup(Health health)
        {
            healthComponents.Remove(health);

            StopListening(health);

            UpdateGroupData();
        }

        private void UpdateGroupData()
        {
            float groupCurrentHealth = 0;
            float groupTotalHealth = 0;
            int totalAliveCount = 0;

            for (int i = 0; i < healthComponents.Count; i++)
            {
                groupCurrentHealth += healthComponents[i].CurrentHealth;
                groupTotalHealth += healthComponents[i].TotalHealth;
                totalAliveCount += healthComponents[i].IsAlive ? 1 : 0;
            }

            onGroupAliveCountChange.Invoke(totalAliveCount);
            onGroupTotalCountChange.Invoke(healthComponents.Count);

            totalAliveEntities = totalAliveCount;


            if (groupCleared && totalAliveEntities > 0)
            {
                groupCleared = false;
                onGroupEntered.Invoke();
            }
            else
            {
                if (!groupCleared && totalAliveEntities <= 0)
                {
                    groupCleared = true;
                    onGroupEmpty.Invoke();
                }
            }

            Configure(groupCurrentHealth, groupTotalHealth);
        }

        public void OnDamaged(HealthInfo info)
        {
            Damage(info.Amount, info.HitLocation, info.Cause);
        }

        public void OnDeath(HealthInfo info)
        {
            int totalAliveCount = 0;

            for (int i = 0; i < healthComponents.Count; i++)
            {
                totalAliveCount += healthComponents[i].IsAlive ? 1 : 0;
            }

            if (totalAliveEntities != totalAliveCount)
            {
                onGroupAliveCountChange.Invoke(totalAliveCount);

                totalAliveEntities = totalAliveCount;
            }
        }

        public void OnHealed(HealthInfo info)
        {
            Heal(info.Amount, info.HitLocation, info.Cause);
        }

        public void OnRevive(HealthInfo info)
        {
            Heal(info.CurrentHealth, info.HitLocation, info.Cause);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (obtainFromVolume)
            {
                Health health = other.GetComponent<Health>();
                if (health != null && !healthComponents.Contains(health))
                {
                    AddToGroup(health);
                    Heal(0, Vector3.zero, null);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (obtainFromVolume)
            {
                Health health = other.GetComponent<Health>();
                if (health != null && healthComponents.Contains(health))
                {
                    RemoveFromGroup(health);
                    Heal(0, Vector3.zero, null);
                }
            }
        }
    }
}