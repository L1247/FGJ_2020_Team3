using System;
using CodeMonkey.Utils;
using FGJ2020_Team3.Utility;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterAttack : MonoBehaviour , IAattack
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
                Attack();
        }

        private void Attack()
        {
            Disable();
            SetStateAttacking();
            var     isUp      = _moveTransformVelocity.IsUp;
            Vector3 v         = isUp ? Vector3.up * 5 : Vector3.zero;
            Vector3 attackDir = (_transform.position + v/*UtilsClass.GetMouseWorldPosition()*/ - GetPosition()).normalized;
            float   angle     = isUp ? 90f : -90f;
            Transform swordSlashTransform =
                Instantiate(GameAssets.Instance.pfSwordSlash ,
                            GetPosition() + attackDir * 2f ,
                            Quaternion.Euler(0 , 0 , angle /*UtilsClass.GetAngleFromVector(attackDir)*/)).transform;
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