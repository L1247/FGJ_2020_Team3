using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro.Examples.GroupsAndVolumes
{
    [AddComponentMenu("")]
    public class GroupVolumesBullet : MonoBehaviour
    {
        private Vector3 direction;
        private float speed;

        public void Configure(Vector3 direction, float speed)
        {
            this.direction = direction.normalized;
            this.speed = speed;
        }

        private void FixedUpdate()
        {
            this.transform.Translate((direction * speed) * Time.deltaTime);
        }
    }
}