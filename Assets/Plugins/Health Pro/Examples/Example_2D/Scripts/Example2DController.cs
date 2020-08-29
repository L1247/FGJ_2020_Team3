using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.HealthPro.Pooling;

namespace Lowscope.HealthPro.Examples.Example2D
{
    public class Example2DController : MonoBehaviour, IKillable
    {
        [SerializeField]
        private Vector2 monsterSpawnTime;

        [SerializeField]
        private int maxMonsterCount;

        [SerializeField]
        private BoxCollider2D monsterSpawnBounds;

        [SerializeField]
        private GameObject monsterPrefab;

        [SerializeField]
        private List<Example2DCharacter> characters;

        private float currentMonsterSpawnTime = 0;

        [SerializeField]
        private List<Health> aliveEnemies = new List<Health>();

        [SerializeField]
        private List<Health> deadEnemies = new List<Health>();

        private void Update()
        {
            if (currentMonsterSpawnTime < 0 && aliveEnemies.Count < maxMonsterCount)
            {
                bool isRecycled = false;

                GameObject spawnedMonster = PoolManager.SpawnObject(monsterPrefab, ReturnPointInBounds(monsterSpawnBounds), monsterPrefab.transform.rotation, out isRecycled);

                Health getHealth = spawnedMonster.GetComponent<Health>();

                if (!isRecycled)
                {
                    spawnedMonster.AddComponent<ReturnToPoolOnDisable>();

                    if (getHealth != null)
                    {
                        getHealth.AddListener(this as IKillable);
                    }
                }

                aliveEnemies.Add(getHealth);

                currentMonsterSpawnTime = Random.Range(monsterSpawnTime.x, monsterSpawnTime.y);
            }
            else
            {
                currentMonsterSpawnTime -= Time.deltaTime;
            }

            if (aliveEnemies.Count > 0)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    characters[i].TryShoot((Vector2)(aliveEnemies[Random.Range(0, aliveEnemies.Count - 1)].transform.position));
                }
            }
        }

        private Vector2 ReturnPointInBounds(BoxCollider2D col)
        {
            Vector2 colliderSize = col.size / 2;

            float x = col.transform.position.x
                + Random.Range(-colliderSize.x, colliderSize.x);

            float y = col.transform.position.y
                + Random.Range(-colliderSize.y, colliderSize.y);

            return new Vector2(x, y);
        }

        public void OnDeath(HealthInfo info)
        {
            Health health = info.Owner;

            // I'm using the Finish tag because I don't want to add additional tags
            // And mess with your project. Since this is example code.
            if (health.CompareTag("Finish"))
            {
                // We add the enemy to the list of dead enemies and remove it from the alive one.
                deadEnemies.Add(health);
                aliveEnemies.Remove(health);
            }
        }
    }

}