using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro
{
    public struct ModificationVolumeConfiguration
    {
        /// <summary>
        /// The effect that this volume should apply.
        /// </summary>
        public EHealthModificationEffect Effect;

        /// <summary>
        /// Amount of affect to apply
        /// </summary>
        public float Amount;

        /// <summary>
        /// Should the volume deactivate automatically? Based on the DeactivationTime
        /// </summary>
        public bool AutoDeactivate;

        /// <summary>
        /// Disable the volume after set time, ensure AutoDeactivate is true to make it work
        /// </summary>
        public float DeactivationTime;

        /// <summary>
        /// Specify tags which can be damaged by this volume, if null, it will not check for any tags.
        /// </summary>
        public string[] TargetTags;

        /// <summary>
        /// Owner instance ID, this must resemble to the InstanceID of an object
        /// That has a Health component. Example of how to obtain it: health.gameObject.GetInstanceID()
        /// </summary>
        public int Owner;

        /// <summary>
        /// Should this volume stop once a effect has been applied?
        /// </summary>
        public bool DeactivateOnEffect;

        public ModificationVolumeConfiguration
            (
            EHealthModificationEffect effect = EHealthModificationEffect.Damage,
            float amount = 1,
            bool autoDeactivate = false,
            float deactivationTime = 0,
            int owner = -1,
            bool deactivateOnEffect = false,
            string[] targetTags = null
            )
        {
            this.Effect = effect;
            this.Amount = amount;
            this.AutoDeactivate = autoDeactivate;
            this.DeactivationTime = deactivationTime;
            this.Owner = owner;
            this.DeactivateOnEffect = deactivateOnEffect;
            this.TargetTags = targetTags;
        }
    }
}