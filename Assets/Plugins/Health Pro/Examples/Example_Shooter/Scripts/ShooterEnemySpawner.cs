using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro.Examples.Shooter
{
    [AddComponentMenu("")]
    public class ShooterEnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider spawnVolume = null;

        // Example of how you could subscribe to a health component.
        [SerializeField]
        private Health playerHealth = null;

        [SerializeField]
        private GameObject enemyPrefab = null;

        [SerializeField]
        private float spawnTimeOffset = 0;

        [SerializeField]
        private AnimationCurve spawnTimeCurve = null;

        [SerializeField]
        private float maximumGameTime = 0;

        private float currentSpawnTime;

        // Lets listen to the player, so we get it's health notifications
        // So we can stop spawning once the player has died. and start again when he is revived.
        private void Awake()
        {
            playerHealth.AddGameObjectInterfaces(this.gameObject, false);
            currentSpawnTime = 1;
        }

        private void Update()
        {
            if (currentSpawnTime > 0)
            {
                currentSpawnTime -= Time.deltaTime;
                return;
            }

            Vector3 randomPoint = RandomPointInBox(spawnVolume.transform.position, spawnVolume.size);

            bool isRecycled;
            GameObject spawnedEnemy = PoolManager.SpawnObject(enemyPrefab, randomPoint, enemyPrefab.transform.rotation, out isRecycled);

            if (!isRecycled)
            {
                spawnedEnemy.AddComponent<ReturnToPoolOnDisable>();
                spawnedEnemy.name = "Cube";
            }

            currentSpawnTime = spawnTimeCurve.Evaluate(maximumGameTime / Time.time) + Random.Range(-spawnTimeOffset, spawnTimeOffset);
        }

        private Vector3 RandomPointInBox(Vector3 center, Vector3 size)
        {
            return center + new Vector3(
               (Random.value - 0.5f) * size.x,
               (Random.value - 0.5f) * size.y,
               (Random.value - 0.5f) * size.z
            );
        }
    }
}