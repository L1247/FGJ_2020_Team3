
namespace Lowscope.HealthPro
{
    /// <summary>
    /// Gets called from a health component if it has been healed
    /// </summary>
    public interface IHealable : IHealthInterface
    {
        void OnHealed(HealthInfo info);
    }
}