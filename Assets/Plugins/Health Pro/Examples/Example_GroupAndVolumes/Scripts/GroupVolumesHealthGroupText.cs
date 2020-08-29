using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro.Examples.GroupsAndVolumes
{
    [AddComponentMenu("")]
    public class GroupVolumesHealthGroupText : MonoBehaviour
    {
        [SerializeField]
        private TextMesh text = null;

        private int aliveEntities;
        private int totalEntities;

        public void SetAliveEntities(int amount)
        {
            aliveEntities = amount;
            UpdateText();
        }

        public void SetTotalEntities(int amount)
        {
            totalEntities = amount;
            UpdateText();
        }

        private void UpdateText()
        {
            text.text = string.Format("{0} / {1}", aliveEntities, totalEntities);
        }
    }
}