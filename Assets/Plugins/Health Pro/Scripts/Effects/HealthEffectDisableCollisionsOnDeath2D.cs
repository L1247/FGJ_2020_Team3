using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Disable Collisions On Death 2D")]
    public class HealthEffectDisableCollisionsOnDeath2D : MonoBehaviour, IKillable, IReviveable
    {
        [SerializeField]
        private new Collider2D collider;

        private void Reset()
        {
            collider = GetComponent<Collider2D>();
        }

        public void OnDeath(HealthInfo info)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        public void OnRevive(HealthInfo info)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
}