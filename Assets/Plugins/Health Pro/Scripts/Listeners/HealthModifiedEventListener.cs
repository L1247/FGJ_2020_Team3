using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Modified Listener")]
    public class HealthModifiedEventListener : MonoBehaviour, IDamageable, IHealable, IKillable, IReviveable
    {
        public UnityEventFloat TotalHealth;
        public UnityEventFloat CurrentHealth;

        public void OnDamaged(HealthInfo info) { Invoke(info); }

        public void OnDeath(HealthInfo info){ Invoke(info); }

        public void OnHealed(HealthInfo info){ Invoke(info); }

        public void OnRevive(HealthInfo info) { Invoke(info); }

        private void Invoke(HealthInfo info)
        {
            TotalHealth.Invoke(info.TotalHealth);
            CurrentHealth.Invoke(info.CurrentHealth);
        }
    }
}