using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro
{
    public interface IHealthModificationListener
    {
        void OnOwnerSet(int ownerInstanceID);
        void OnEffectApplied(float amount, Vector3 hitLocation, EHealthModificationEffect effect);
        void OnEffectRemoved();
    }
}