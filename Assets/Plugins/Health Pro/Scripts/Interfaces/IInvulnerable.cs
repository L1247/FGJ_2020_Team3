
namespace Lowscope.HealthPro
{
    // This gets invoked from a Health component, if it has been set invulnerable
    public interface IInvulnerable : IHealthInterface
    {
        void BecameInvulnerable(bool isInvulnerable);
    }
}