using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lowscope.HealthPro.Listeners
{
    [AddComponentMenu("Health/Listeners/Health Heal Listener")]
    public class HealthHealEventListener : MonoBehaviour, IHealable
    {
        [SerializeField]
        public UnityEventFloat HealAmount;

        public void OnHealed(HealthInfo healInfo)
        {
            HealAmount.Invoke(healInfo.Amount);
        }
    }
}