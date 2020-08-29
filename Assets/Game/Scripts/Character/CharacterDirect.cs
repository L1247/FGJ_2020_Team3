using System;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3
{
    public class CharacterDirect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite         UpSprite , DownSprite;
        private void Start()
        {
            GetComponent<Character>().GetDirectionChange
                                     .Subscribe(isUp => ChangeDirection(isUp))
                                     .AddTo(gameObject);
        }

        private void ChangeDirection(bool isUp)
        {
            _spriteRenderer.sprite = isUp ? UpSprite : DownSprite;
        }
    }
}