using System;
using TransformService;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class Character_Base : MonoBehaviour
    {
        public  IObservable<bool> GetDirectionChange => OnDirectionChange; 
        private Subject<bool>     OnDirectionChange = new Subject<bool>();
        
        [SerializeField] private float             moveSpeed = 3;
        private                  UnityInputService _unityInputService;
        private                  Movement          _movement;
        private                  Transform         _transform;
        

        private void Start()
        {
            _unityInputService = new UnityInputService();
            _movement          = new Movement(moveSpeed);
            _transform         = transform;
        }

        private void Update()
        {
            var vertical = _unityInputService.GetVertical();
            var moveVector = 
                _movement.TwoDCalculateWithoutDirection(_unityInputService.GetHorizontal() ,
                                                           vertical ,
                                                           _unityInputService.GetDeltaTime());
            if (vertical != 0)
            {
                var isUp = vertical >0;
                OnDirectionChange.OnNext(isUp);
            }
            _transform.position += moveVector;
        }
    }
}