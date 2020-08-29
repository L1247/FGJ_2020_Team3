using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro
{
    public interface IHealthConfigure : IHealthInterface
    {
        void OnHealthConfigure(HealthInfo info);
    }
}