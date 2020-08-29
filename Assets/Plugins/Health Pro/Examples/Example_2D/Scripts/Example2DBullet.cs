using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.Example2D
{
    public class Example2DBullet : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private void OnEnable()
        {
            StartCoroutine(MuzzleFlashCoroutine());
        }

        IEnumerator MuzzleFlashCoroutine()
        {
            this.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            int frameWait = 2;
            while (frameWait > 0)
            {
                yield return null;
                frameWait--;
            }

            this.transform.localScale = Vector3.one;
        }

        void Update()
        {

            this.transform.Translate((transform.right * speed) * Time.deltaTime, Space.World);

            if (this.transform.position.x > 2 || this.transform.position.x < -2)
                this.gameObject.SetActive(false);
        }
    }
}