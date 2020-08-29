using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Directly disables the object if the root object has died.
/// Useful for hiding specific game object elements such as UI.
/// </summary>

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Disable On Death")]
    public class HealthEffectDisableOnDeath : MonoBehaviour, IKillable
    {
        [SerializeField]
        private float time = 0;

        public void OnDeath(HealthInfo info)
        {
            StartCoroutine(DisableCoroutine());
        }

        IEnumerator DisableCoroutine()
        {
            if (time > 0)
            {
                yield return new WaitForSeconds(time);
                this.gameObject.SetActive(false);
                yield break;
            }
            else
            {
                yield return null;
                this.gameObject.SetActive(false);
            }
        }
    }
}