using UnityEngine;
using System.Collections;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro.Examples.Example2D
{
    public class Example2DCharacter : MonoBehaviour
    {
        [SerializeField]
        private Transform weapon;

        [SerializeField]
        private Transform shootPoint;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private ParticleSystem shootParticle;

        [SerializeField]
        private Vector2 shootDelay = new Vector2 (0.3f,0.5f);

        [SerializeField]
        private Vector2 damage;

        [SerializeField]
        private float minimumShootDistance = 1.5f;

        private float currentShootDelay;

        public void TryShoot(Vector3 target)
        {
            if (currentShootDelay <= 0)
            {
                if (target.x - this.transform.position.x > minimumShootDistance)
                    return;

                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.Play();

                shootParticle.Play();

                animator.PlayInFixedTime("Character_Shoot");

                bool isRecycled;

                Vector3 direction = (target - weapon.transform.position).normalized;
                float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

                GameObject bullet = PoolManager.SpawnObject(bulletPrefab, shootPoint.transform.position, weapon.transform.rotation, out isRecycled);

                if (!isRecycled)
                {
                    bullet.AddComponent<ReturnToPoolOnDisable>();
                }

                if (bullet != null)
                {
                    HealthModificationVolumeBase modificationBase = bullet.GetComponent<HealthModificationVolumeBase>();
                    modificationBase.SetAmount((int)Random.Range(damage.x, damage.y));
                }

                currentShootDelay = Random.Range(shootDelay.x, shootDelay.y);
            }
            else
            {
                currentShootDelay -= Time.deltaTime;
            }
        }
    }
}