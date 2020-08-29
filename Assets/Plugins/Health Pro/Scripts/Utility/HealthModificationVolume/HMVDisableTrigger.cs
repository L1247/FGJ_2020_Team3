using UnityEngine;
using System.Collections.Generic;

namespace Lowscope.HealthPro
{
    // Ensures a Health Modification Volume gets notified if a registered objects gets disabled or destoryed.
    // This is required since Unity does not give any callbacks to the collision system.

    [HideInInspector, AddComponentMenu("")]
    public class HMVDisableTrigger : MonoBehaviour
    {
        private List<HealthModificationVolumeBase> references = new List<HealthModificationVolumeBase>();

        public void AddReference(HealthModificationVolumeBase volume)
        {
            if (!references.Contains(volume))
                references.Add(volume);
        }

        private void OnDisable()
        {
            int referenceCount = references.Count;
            for (int i = referenceCount - 1; i >= 0; i--)
            {
                if (references[i] != null)
                {
                    references[i].RemoveTarget(this.gameObject);
                }

                references.RemoveAt(i);
            }
        }
    }
}