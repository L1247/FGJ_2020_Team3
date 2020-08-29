
namespace Lowscope.HealthPro
{
    // This gets invoked from a Health component, when it is killed
    public interface IKillable : IHealthInterface
    {
        void OnDeath(HealthInfo info);
    }
}