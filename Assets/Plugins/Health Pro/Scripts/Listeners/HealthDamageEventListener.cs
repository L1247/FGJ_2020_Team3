using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Damage Listener")]
    public class HealthDamageEventListener : MonoBehaviour, IDamageable
    {
        [SerializeField]
        public UnityEventFloat DamageAmount;

        public void OnDamaged(HealthInfo healthInfo)
        {
            DamageAmount.Invoke(healthInfo.Amount);
        }
    }
}