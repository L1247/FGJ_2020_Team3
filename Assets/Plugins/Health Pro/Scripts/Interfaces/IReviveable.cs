namespace Lowscope.HealthPro
{
    // This gets invoked from a Health component, when it is revived
    public interface IReviveable : IHealthInterface
    {
        void OnRevive(HealthInfo info);
    }
}