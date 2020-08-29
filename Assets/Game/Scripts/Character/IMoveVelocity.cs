using System.Numerics;

namespace FGJ2020_Team3.Character
{
    public interface IMoveVelocity {

        void SetVelocity(Vector3 velocityVector);
        void Disable();
        void Enable();
    }
}