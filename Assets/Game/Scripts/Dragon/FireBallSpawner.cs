using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Dragon
{
    public class FireBallSpawner : MonoBehaviour
    {
        [SerializeField] private int        defaultSpawnCount = 5;
        [SerializeField] private Animator   animator;
        [SerializeField] private GameObject fireBallPrefab;
        private                  int        spawnCount;

        private void Start() => spawnCount = defaultSpawnCount;

        public void Excute()
        {
            spawnCount = defaultSpawnCount;
            CallFireBallAnimation();
        }
        public void CallFireBallAnimation()
        {
            animator.Play("Fire");
        }

        public void SpawnFireBall()
        {
            spawnCount--;
            Instantiate(fireBallPrefab , transform.position,Quaternion.identity);
            if (spawnCount > 0) CallFireBallAnimation();
        }
    }
}