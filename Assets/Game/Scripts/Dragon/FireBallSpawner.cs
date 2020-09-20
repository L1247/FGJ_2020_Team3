using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Dragon
{
    public class FireBallSpawner : MonoBehaviour
    {
        [SerializeField] private int         defaultSpawnCount = 5;
        [SerializeField] private Animator    animator;
        [SerializeField] private GameObject  fireBallPrefab;
        [SerializeField] private AudioClip   _audioClip;
        [SerializeField] private AudioSource _audioSource;
        private                  int         spawnCount;
        private                  void        Start() => spawnCount = defaultSpawnCount;

        public float Excute()
        {
            spawnCount = defaultSpawnCount;
            CallFireBallAnimation();
            return defaultSpawnCount * 1f + 1f;
        }
        public void CallFireBallAnimation()
        {
            animator.Play("Fire");
        }

        public void SpawnFireBall()
        {
            _audioSource.PlayOneShot(_audioClip);
            spawnCount--;
            Instantiate(fireBallPrefab , transform.position,Quaternion.identity);
            if (spawnCount > 0) CallFireBallAnimation();
        }
    }
}