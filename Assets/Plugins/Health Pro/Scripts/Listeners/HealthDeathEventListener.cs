using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Death Listener")]
    public class HealthDeathEventListener : MonoBehaviour, IKillable
    {
        [SerializeField]
        private UnityEvent onDeath = null;

        public void OnDeath(HealthInfo healthInfo)
        {
            onDeath.Invoke();
        }
    }
}