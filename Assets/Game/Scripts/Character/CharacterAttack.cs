using System;
using CodeMonkey.Utils;
using FGJ2020_Team3.Utility;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterAttack : MonoBehaviour
    {
        private enum State {
            Normal,
            Attacking
        }
        private Transform _transform;
        private State     state;

        private void Awake()
        {
            SetStateNormal();
        }

        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Click");
                Attack();
            }
        }

        private void Attack()
        {
            SetStateAttacking();
            Vector3 attackDir = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;
            GameObject swordSlashTransform =
                Instantiate(GameAssets.Instance.pfSwordSlash ,
                            GetPosition() + attackDir *1 ,
                            Quaternion.Euler(0 , 0 , UtilsClass.GetAngleFromVector(attackDir)));
        }

        private Vector3 GetPosition()
        {
            return _transform.position;
        }
        
        private void SetStateAttacking() {
            state = State.Attacking;
            GetComponent<IMoveVelocity>().Disable();
        }

        private void SetStateNormal() {
            state = State.Normal;
            GetComponent<IMoveVelocity>().Enable();
        }
    }
}