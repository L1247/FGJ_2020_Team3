using System;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3
{
    public class CharacterDirect : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Character>().GetDirectionChange
                                     .Subscribe(isUp => ChangeDirection(isUp))
                                     .AddTo(gameObject);
        }

        private void ChangeDirection(bool isUp)
        {
            
        }
    }
}