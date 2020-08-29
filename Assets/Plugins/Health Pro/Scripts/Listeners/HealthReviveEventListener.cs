using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Revive Listener")]
    public class HealthReviveEventListener : MonoBehaviour, IReviveable
    {
        [SerializeField]
        private UnityEvent ReviveNotification = null;

        public void OnRevive(HealthInfo info)
        {
            ReviveNotification.Invoke();
        }
    }
}