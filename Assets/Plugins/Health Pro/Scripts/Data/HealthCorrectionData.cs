using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro
{
    public struct HealthCorrectionData
    {
        public bool stopExecution;
        public float amount;

        public HealthCorrectionData(bool stopExecution, float amount)
        {
            this.stopExecution = stopExecution;
            this.amount = amount;
        }
    }
}