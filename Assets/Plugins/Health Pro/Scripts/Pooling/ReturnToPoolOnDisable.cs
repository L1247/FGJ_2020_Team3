using UnityEngine;
using System.Collections;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("")]
    public class ReturnToPoolOnDisable : MonoBehaviour
    {
        private void OnDisable()
        {
            PoolManager.ReleaseObject(this.gameObject);
        }
    }
}