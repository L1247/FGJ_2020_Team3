namespace Lowscope.HealthPro
{
    public interface IHealthCorrection : IHealthInterface
    {
        /// <summary>
        /// The order of when specific corrections take place.
        /// Lower means that it gets invoked earlier.
        /// </summary>
        /// <param name="ownerIndex"> Owner health component instance ID, obtain by calling Health.GetInstance(index) </param>
        /// <returns></returns>
        int OnCorrectionOrder(int ownerIndex);

        /// <summary>
        /// Corrects damage dealt, modify the amount in the correction data. Or stop
        /// the execution. Stopping the execution means no effects or extra logic get executed.
        /// </summary>
        /// <param name="amount"> Amount of recieved damage </param>
        /// <param name="attackerIndex"> Attacker health component instance, obtain by calling Health.GetInstance(index) </param>
        /// <returns></returns>
        HealthCorrectionData OnCorrectDamage(float amount, int attackerIndex);

        /// <summary>
        /// Corrects the amount healed, modify the amount in the correction data. Or stop
        /// the execution. Stopping the execution means no effects or extra logic get executed.
        /// </summary>
        /// <param name="amount"> Amount of recieved damage </param>
        /// <param name="attackerIndex"> Attacker health component instance, obtain by calling Health.GetInstance(index) </param>
        /// <returns></returns>
        HealthCorrectionData OnCorrectHeal(float amount, int attackerIndex);

        /// <summary>
        /// Corrects the percentage you can revive towards. Also possible to stop the execution of a revive.
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="attackerIndex"></param>
        /// <returns></returns>
        HealthCorrectionData OnCorrectRevive(float percentage, int attackerIndex);
    }
}