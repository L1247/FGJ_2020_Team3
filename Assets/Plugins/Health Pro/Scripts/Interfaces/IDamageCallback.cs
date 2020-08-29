
namespace Lowscope.HealthPro
{
    /// <summary>
    /// Gets called from a health component once you have damaged another health component.
    /// </summary>
    public interface IDamageCallback : IHealthInterface
    {
        /// <summary>
        /// Gets called once you have damaged another health component.
        /// </summary>
        /// <param name="targetInfo"> All information regarding your effect. In this case the owner is the target </param>
        void OnDamageDone(HealthInfo targetInfo);
    }
}