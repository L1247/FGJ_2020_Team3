using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.Shooter
{
    [AddComponentMenu("")]
    public class ShooterEnemy : MonoBehaviour, IKillable, IReviveable
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private float minSpeed = 0;

        [SerializeField]
        private float maxSpeed = 0;

        private float curSpeed;

        private void Reset()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            curSpeed = Random.Range(minSpeed, maxSpeed);
        }

        public void Update()
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, Vector3.zero, Time.deltaTime * curSpeed);

            if (this.transform.position.z < 0.2f)
            {
                RaycastHit[] hit = Physics.SphereCastAll(this.transform.position, 1, Vector3.up);

                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].transform.CompareTag("Player"))
                    {
                        hit[i].transform.GetComponent<Health>().Damage(1, hit[i].point, health);
                    }
                }

                this.gameObject.SetActive(false);
            }
        }

        public void OnDeath(HealthInfo healthInfo)
        {
            this.enabled = false;
        }

        public void OnRevive(HealthInfo info)
        {
            this.enabled = true;
        }
    }
}