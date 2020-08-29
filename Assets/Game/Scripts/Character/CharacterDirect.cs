using System;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterDirect : MonoBehaviour
    {
        [SerializeField] private Animator       _animator;
        private void Start()
        {
            GetComponent<Character_Base>().GetDirectionChange
                                     .Subscribe(isUp => ChangeDirection(isUp))
                                     .AddTo(gameObject);
        }

        private void ChangeDirection(bool isUp)
        {
            _animator.SetBool("TowardBack" ,isUp);
        }
    }
}