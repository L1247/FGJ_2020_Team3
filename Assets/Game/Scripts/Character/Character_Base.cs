using System;
using TransformService;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class Character_Base : MonoBehaviour
    {
        private                  Transform _transform;
        [SerializeField] private Animator  _animator;

        private void Start()
        {
            _transform = transform;
            GetComponent<MoveTransformVelocity>()
                .GetDirectionChange
                .Subscribe(isUp => ChangeDirection(isUp))
                .AddTo(gameObject);
        }

        private void ChangeDirection(bool isUp)
        {
            _animator.SetBool("TowardBack" , isUp);
        }

        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);
        }
    }
}