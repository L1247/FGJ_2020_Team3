
namespace Lowscope.HealthPro
{
    /// <summary>
    /// Gets called once you have healed another health component.
    /// </summary>
    public interface IHealCallback : IHealthInterface
    {
        /// <summary>
        /// Gets called once you have healed another health component.
        /// </summary>
        /// <param name="targetInfo"> All information regarding your effect. In this case the owner is the target </param>
        void OnHealDone(HealthInfo info);
    }
}