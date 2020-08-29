using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.GroupsAndVolumes
{
    [AddComponentMenu("")]
    public class GroupVolumesCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform target = null;

        [SerializeField]
        private float cameraSpeed = 2;

        [SerializeField]
        private Vector3 offset;

        [SerializeField]
        private bool useStartPositionAsOffset = true;

        [SerializeField]
        private bool ignoreX = false;

        [SerializeField]
        private bool ignoreY = true;

        [SerializeField]
        private bool ignoreZ = false;

        private void Start()
        {
            if (useStartPositionAsOffset)
            {
                offset = this.transform.position - target.transform.position;
            }
        }

        void FixedUpdate()
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.x = (ignoreX) ? this.transform.position.x : targetPosition.x + offset.x;
            targetPosition.y = (ignoreY) ? this.transform.position.y : targetPosition.y + offset.y;
            targetPosition.z = (ignoreZ) ? this.transform.position.z : targetPosition.z + offset.z;

            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * cameraSpeed);
        }
    }
}