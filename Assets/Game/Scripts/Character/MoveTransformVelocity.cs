using System;
using TransformService;
using UniRx;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace FGJ2020_Team3.Character
{
    public class MoveTransformVelocity : MonoBehaviour , IMoveVelocity
    {
        [SerializeField] private float moveSpeed = 3;
        [SerializeField] public  bool  IsUp;

        public  IObservable<bool> GetDirectionChange => OnDirectionChange;
        private Subject<bool>     OnDirectionChange = new Subject<bool>();

        private Vector3           moveVector;
        private Transform         _transform;
        private Movement          _movement;
        private UnityInputService _unityInputService;

        private void Start()
        {
            _transform         = transform;
            _movement          = new Movement(moveSpeed);
            _unityInputService = new UnityInputService();
        }

        public void SetVelocity(Vector3 velocityVector)
        {
        }

        private void Update()
        {
            var vertical = _unityInputService.GetVertical();

            var moveVector =
                _movement.TwoDCalculateWithoutDirection(_unityInputService.GetHorizontal() ,
                                                        vertical ,
                                                        _unityInputService.GetDeltaTime());
            _unityInputService.GetDeltaTime();
            if (vertical != 0)
            {
                var isUp = vertical > 0;
                OnDirectionChange.OnNext(isUp);
                IsUp = isUp;
            }

            _transform.position += moveVector;
            // characterBase.PlayMoveAnim(velocityVector);
        }


        public void Disable()
        {
            this.enabled = false;
        }

        public void Enable()
        {
            this.enabled = true;
        }
    }
}