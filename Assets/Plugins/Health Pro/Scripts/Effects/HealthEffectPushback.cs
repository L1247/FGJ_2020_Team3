using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Pushback"), DisallowMultipleComponent]
    public class HealthEffectPushback : MonoBehaviour, IDamageable
    {
        private enum PushType { NavmeshAgent, Rigidbody, RigidBody2D, CharacterController };

        private Rigidbody rigidBody;
        private Rigidbody2D rigidBody2D;
        private NavMeshAgent navMeshAgent;
        private CharacterController characterController;
        private RaycastHit[] hitResults;

        [SerializeField]
        private float distance = 0;

        [SerializeField, Tooltip("Set this according tp the scaling to your game, to prevent warping through objects")]
        private float stepSize = 0.15f;

        [SerializeField, Tooltip("What should this component try to push?")]
        private PushType pushType = PushType.Rigidbody;

        [SerializeField, Tooltip("This will multiply the result, giving you the possibility to zero out specific axis.")]
        private Vector3 MultiplyMovement = new Vector3(1, 1, 1);

        private void Awake()
        {
            switch (pushType)
            {
                case PushType.NavmeshAgent:
                    navMeshAgent = GetComponent<NavMeshAgent>();
                    break;
                case PushType.Rigidbody:
                    rigidBody = GetComponent<Rigidbody>();
                    break;
                case PushType.RigidBody2D:
                    rigidBody2D = GetComponent<Rigidbody2D>();
                    break;
                case PushType.CharacterController:
                    characterController = GetComponent<CharacterController>();
                    break;
                default:
                    break;
            }
        }

        public void OnDamaged(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                StopAllCoroutines();

                switch (pushType)
                {
                    case PushType.CharacterController:

                        if (characterController != null)
                        {
                            Vector3 target = GetMovementVelocity(info.HitLocation, true);

                            characterController.Move(target);
                        }

                        break;
                    case PushType.NavmeshAgent:

                        if (navMeshAgent != null)
                            StartCoroutine(EaseNavMeshAgentCoroutine(info.HitLocation));

                        break;
                    case PushType.Rigidbody:

                        if (rigidBody != null)
                            StartCoroutine(EaseRigidBodyCoroutine(info.HitLocation, true));
                        break;

                    case PushType.RigidBody2D:

                        if (rigidBody2D != null)
                            StartCoroutine(EaseRigidBodyCoroutine(info.HitLocation, false));
                        break;


                    default:
                        break;
                }
            }
        }

        private Vector3 GetMovementVelocity(Vector3 hitLocation, bool is3D)
        {
            if (is3D)
            {
                hitLocation.y = this.transform.position.y;
            }
            else
            {
                hitLocation.z = this.transform.position.z;
            }

            Vector3 moveDirection = (this.transform.position - hitLocation).normalized * distance;

            if (MultiplyMovement.x != 1)
            {
                moveDirection.x *= MultiplyMovement.x;
            }

            if (MultiplyMovement.y != 1)
            {
                moveDirection.y *= MultiplyMovement.y;
            }

            if (MultiplyMovement.z != 1)
            {
                moveDirection.z *= MultiplyMovement.z;
            }

            return moveDirection;
        }

        private IEnumerator EaseNavMeshAgentCoroutine(Vector3 hitLocation)
        {
            Vector3 startPostion = this.transform.position;
            Vector3 totalDistance = GetMovementVelocity(hitLocation, true);

            while (totalDistance.sqrMagnitude > 0.0005f)
            {
                Vector3 moveDistance = totalDistance * stepSize;

                if (navMeshAgent.Warp(this.transform.position + moveDistance))
                {
                    this.transform.position = navMeshAgent.nextPosition;

                    yield return null;
                }
                else
                {
                    yield break;
                }

                totalDistance -= moveDistance;
            }
        }

        private IEnumerator EaseRigidBodyCoroutine(Vector3 hitLocation, bool isRigidBody3D)
        {
            // Rigidbody velocity gets executed in units per second in the physics system. Which is why it is devided by the duration
            // Results may vary depending on how you have your rigidbodies set up, and what kind of shape and size they are.

            Vector3 totalDistance = GetMovementVelocity(hitLocation, isRigidBody3D);

            while (totalDistance.sqrMagnitude > 0.0005f)
            {
                yield return new WaitForFixedUpdate();

                Vector3 moveDistance = totalDistance * stepSize;

                if (isRigidBody3D)
                {
                    this.transform.position = this.transform.position + moveDistance;
                }
                else
                {
                    this.transform.position = this.transform.position + moveDistance;
                }

                totalDistance -= moveDistance;

            }
        }

        public void OnDeath()
        {
            StopAllCoroutines();
        }
    }

}