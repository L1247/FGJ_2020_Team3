using MonsterLove.HealthPro.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.GroupsAndVolumes
{
    [AddComponentMenu("")]
    public class GroupVolumesPlayer : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 2;

        [SerializeField]
        private Health health = null;

        [SerializeField]
        private CharacterController characterController = null;

        [SerializeField]
        private new Camera camera = null;

        [SerializeField]
        private GameObject bulletPrefab = null;

        ModificationVolumeConfiguration damageEffectConfiguration;
        ModificationVolumeConfiguration healEffectConfiguration;

        private void Awake()
        {
            damageEffectConfiguration = new ModificationVolumeConfiguration()
            {
                Effect = EHealthModificationEffect.Damage,
                Amount = 1,
                DeactivateOnEffect = true,
                DeactivationTime = 2,
                Owner = health.gameObject.GetInstanceID(),
                AutoDeactivate = true
            };

            healEffectConfiguration = new ModificationVolumeConfiguration()
            {
                Effect = EHealthModificationEffect.Damage,
                Amount = 1,
                DeactivateOnEffect = true,
                DeactivationTime = 2,
                Owner = health.gameObject.GetInstanceID(),
                AutoDeactivate = true
            };
        }

        void FixedUpdate()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            characterController.SimpleMove((input.normalized * moveSpeed) * Time.fixedDeltaTime);
        }

        private void Update()
        {
            bool leftClick = Input.GetMouseButtonDown(0);
            bool rightClick = Input.GetMouseButtonDown(1);

            if (leftClick || rightClick)
            {
                Vector3 clickPosition = Input.mousePosition;
                clickPosition.z = 1;
                Vector3 worldClickPoint = camera.ScreenToWorldPoint(clickPosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(camera.transform.position, (worldClickPoint - camera.transform.position).normalized, out hitInfo, 100))
                {
                    Vector3 shootDirection = hitInfo.point - this.transform.position;
                    shootDirection.y = this.transform.position.y;

                    bool isRecycled;

                    // GroupVolumes is the name of this example.
                    GroupVolumesBullet bullet = PoolManager.SpawnObject(bulletPrefab, out isRecycled).GetComponent<GroupVolumesBullet>();

                    if (!isRecycled)
                    {
                        bullet.gameObject.AddComponent<ReturnToPoolOnDisable>();
                    }

                    bullet.transform.position = this.transform.position;
                    bullet.Configure(shootDirection.normalized, 15);

                    bullet.GetComponent<HealthModificationVolumeBase>().Configure
                        ((leftClick) ? damageEffectConfiguration : healEffectConfiguration);

                    bullet.gameObject.SetActive(true);
                }

            }
        }
    }
}