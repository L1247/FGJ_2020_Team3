using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.Example2D
{
    public class Example2DMonster : MonoBehaviour , IKillable, IReviveable
    {
        [SerializeField]
        private Rigidbody2D rigidBody;

        [SerializeField]
        private float speed;

        [SerializeField]
        private Health health;

        public void OnDeath(HealthInfo info)
        {
            this.enabled = false;
        }

        public void OnRevive(HealthInfo info)
        {
            this.enabled = true;
        }

        private void FixedUpdate()
        {
            if (this.gameObject.transform.position.x < -1.5f)
            {
                health.Kill(false, health.transform.position, null);
            }
            else
            {
                rigidBody.MovePosition(this.transform.position + ((Vector3.left * speed) * Time.fixedDeltaTime));
            }
        }
    }
}