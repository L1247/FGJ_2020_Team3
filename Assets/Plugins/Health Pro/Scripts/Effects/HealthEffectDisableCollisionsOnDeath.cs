using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Disable Collisions On Death")]
    public class HealthEffectDisableCollisionsOnDeath : MonoBehaviour, IKillable, IReviveable
    {
        [SerializeField]
        private new Collider collider;

        [SerializeField]
        private Rigidbody rigidBody;

        private void Reset()
        {
            collider = GetComponent<Collider>();
            rigidBody = GetComponent<Rigidbody>();
        }

        public void OnDeath(HealthInfo info)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (rigidBody != null)
            {
                rigidBody.useGravity = false;
            }
        }

        public void OnRevive(HealthInfo info)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (rigidBody != null)
            {
                rigidBody.useGravity = true;
            }
        }
    }
}