using UnityEngine;
using System.Collections;

namespace Lowscope.HealthPro
{
    public struct HealthInfo
    {
        public float PreviousHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public float TotalHealth { get; private set; }
        public Vector3 HitLocation { get; private set; }
        public int CauseIdentifier { get; private set; }
        public int OwnerIdentifier { get; private set; }
        public float Amount { get; private set; }

        /// <summary>
        /// Obtains the cause through Health.GetInstance(CauseIdentifier).
        /// It is reccomended to cache these results.
        /// </summary>
        public Health Cause { get { return Health.GetInstance(CauseIdentifier); } }

        /// <summary>
        /// Obtains the owner through Health.GetInstance(OwnerIdentifier).
        /// It is reccomended to cache these results.
        /// </summary>
        public Health Owner { get { return Health.GetInstance(OwnerIdentifier); } }

        /// <summary>
        /// Will check based on the health change if it has been fully restored.
        /// </summary>
        public bool IsHealthRestored()
        {
            return PreviousHealth < TotalHealth && (PreviousHealth + Amount) >= TotalHealth;
        }

        /// <summary>
        /// Checks for the additional damage you have done compared to the total health.
        /// </summary>
        /// <param name="tresholdPercentage"> Returns true if the damage is higher than a % of the total health. </param>
        public bool IsHealthDamageOverkill(float tresholdPercentage)
        {
            if (PreviousHealth > 0 || Amount < 0)
            {
                return false;
            }

            float modifiedHealth = Mathf.Abs(PreviousHealth - Amount);

            return modifiedHealth / TotalHealth > tresholdPercentage;
        }

        public HealthInfo(float previousHealth, float currentHealth, float totalHealth,
            Vector3 hitLocation, int causeIdentifier, int ownerIdentifier, float amount)
        {
            this.PreviousHealth = previousHealth;
            this.CurrentHealth = currentHealth;
            this.TotalHealth = totalHealth;
            this.HitLocation = hitLocation;
            this.CauseIdentifier = causeIdentifier;
            this.OwnerIdentifier = ownerIdentifier;
            this.Amount = amount;
        }
    }
}