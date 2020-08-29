using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.GroupsAndVolumes
{
    [AddComponentMenu("")]
    public class GroupVolumesObjectMover : MonoBehaviour
    {
        [SerializeField]
        private Transform wayPointOne = null;

        [SerializeField]
        private Transform wayPointTwo = null;

        [SerializeField]
        private float speed = 1;

        private float t;

        void Update()
        {
            t += Time.deltaTime * speed;

            this.transform.position = Vector3.Lerp(wayPointOne.position, wayPointTwo.position, Mathf.Abs(Mathf.Sin(t)));
        }
    }
}