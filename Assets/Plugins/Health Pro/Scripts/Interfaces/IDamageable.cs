

namespace Lowscope.HealthPro
{
    /// <summary>
    /// Gets called from a health component if it has taken damage
    /// </summary>
    public interface IDamageable : IHealthInterface
    {
        void OnDamaged(HealthInfo info);
    }
}