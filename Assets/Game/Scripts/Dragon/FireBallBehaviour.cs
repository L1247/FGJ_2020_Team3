using System;
using FGJ2020_Team3.Character;
using UnityEngine;

namespace Game.Scripts.Dragon
{
    public class FireBallBehaviour : MonoBehaviour
    {
        [SerializeField] private float     moveSpeed = 5;
        private                  Transform target;
        private                  Vector3   _direction;
        private                  Vector3   _targetPosition;
        private                  Vector3   endPosition;

        private void Start()
        {
            Destroy(gameObject , 5);
            target          = FindObjectOfType<Character_Base>().transform;
            _targetPosition = target.position;
            _direction      = _targetPosition - transform.position;
            endPosition     = _targetPosition + _direction * 10;
        }

        private void Update()
        {
            float step = moveSpeed * Time.deltaTime;
            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(transform.position , endPosition , step);
        }
    }
}