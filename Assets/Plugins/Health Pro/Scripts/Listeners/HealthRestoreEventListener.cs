using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Restore Listener")]
    public class HealthRestoreEventListener : MonoBehaviour, IHealable
    {
        [SerializeField]
        private UnityEvent healthRestoreNotification = null;

        public void OnHealed(HealthInfo info)
        {
            if (info.IsHealthRestored())
            {
                healthRestoreNotification.Invoke();
            }
        }
    }
}