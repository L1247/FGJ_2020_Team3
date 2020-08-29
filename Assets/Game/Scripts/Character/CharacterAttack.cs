using System;
using CodeMonkey.Utils;
using FGJ2020_Team3.Utility;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterAttack : MonoBehaviour
    {
        private enum State
        {
            Normal ,
            Attacking
        }

        private Transform             _transform;
        private State                 state;
        private MoveTransformVelocity _moveTransformVelocity;
        private Character_Base        _characterBase;

        private void Awake()
        {
            SetStateNormal();
            _moveTransformVelocity = GetComponent<MoveTransformVelocity>();
            _characterBase         = GetComponent<Character_Base>();
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
            var isUp = _moveTransformVelocity.IsUp;
            Transform swordSlashTransform =
                Instantiate(GameAssets.Instance.pfSwordSlash ,
                            GetPosition() + attackDir * 1 ,
                            Quaternion.Euler(0 , 0 , UtilsClass.GetAngleFromVector(attackDir))).transform;
            var localScale = swordSlashTransform.localScale;

            if (!isUp) // front
            {
                _characterBase.SetTrigger("Atk_Front");
                localScale.y = -1;
            }
            else
            {
                _characterBase.SetTrigger("Atk_Back");
            }

            swordSlashTransform.localScale = localScale;
        }

        private Vector3 GetPosition()
        {
            return _transform.position;
        }

        private void SetStateAttacking()
        {
            state = State.Attacking;
            GetComponent<IMoveVelocity>().Disable();
        }

        private void SetStateNormal()
        {
            state = State.Normal;
            GetComponent<IMoveVelocity>().Enable();
        }
    }
}