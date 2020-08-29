using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drop item on death or damage
/// </summary>

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Drop Item")]
    public class HealthEffectDropItem : MonoBehaviour, IDamageable, IKillable
    {
        public enum DropRequirement { Death, Damaged };

        [System.Serializable]
        public class DroppableObject
        {
            [SerializeField]
            public GameObject prefab;

            public int minDropCount;
            public int maxDropCount;

            public int weight;
        }

        [System.Serializable]
        public class DropObjectCategory
        {
            [Range(0, 100), Tooltip("The percentage chance of dropping object on requirement.")]
            public int dropChance = 1;

            public DropRequirement requirement;

            [SerializeField]
            public DroppableObject[] droppableObjects = null;

            private int totalWeight = 0;

            public DroppableObject ReturnRandomObject()
            {
                if (totalWeight == 0)
                {
                    foreach (DroppableObject obj in droppableObjects)
                    {
                        totalWeight += obj.weight;
                    }
                }

                int randomWeight = Random.Range(0, totalWeight + 1);

                foreach (var obj in droppableObjects)
                {
                    randomWeight -= obj.weight;

                    if (randomWeight <= 0)
                    {
                        return obj;
                    }
                }

                return null;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            for (int i = 0; i < dropItemCategory.Length; i++)
            {
                if (dropItemCategory[i].dropChance <= 0)
                    dropItemCategory[i].dropChance = 1;

                for (int i2 = 0; i2 < dropItemCategory[i].droppableObjects.Length; i2++)
                {
                    if (dropItemCategory[i].droppableObjects[i2].weight <= 0)
                    {
                        dropItemCategory[i].droppableObjects[i2].weight = 1;
                    }

                    if (dropItemCategory[i].droppableObjects[i2].minDropCount <= 0)
                    {
                        dropItemCategory[i].droppableObjects[i2].minDropCount = 1;
                    }
                    if (dropItemCategory[i].droppableObjects[i2].maxDropCount < dropItemCategory[i].droppableObjects[i2].minDropCount)
                    {
                        dropItemCategory[i].droppableObjects[i2].maxDropCount = dropItemCategory[i].droppableObjects[i2].minDropCount;
                    }
                }

            }
        }

#endif

        [SerializeField]
        public DropObjectCategory[] dropItemCategory = new DropObjectCategory[0] { };

        public void OnDamaged(HealthInfo info)
        {
            Drop(DropRequirement.Damaged);
        }

        public void OnDeath(HealthInfo info)
        {
            Drop(DropRequirement.Death);
        }

        private void Drop(DropRequirement requirement)
        {
            for (int i = 0; i < dropItemCategory.Length; i++)
            {
                if (dropItemCategory[i].requirement != requirement)
                    continue;

                int rnd = Random.Range(1, 101);

                if (rnd < dropItemCategory[i].dropChance)
                {
                    DroppableObject randomObject = dropItemCategory[i].ReturnRandomObject();

                    if (randomObject.prefab != null)
                    {
                        int dropAmount = Random.Range(randomObject.minDropCount, randomObject.maxDropCount + 1);

                        for (int i2 = 0; i2 < dropAmount; i2++)
                        {
                            GameObject.Instantiate(randomObject.prefab, this.transform.position, randomObject.prefab.transform.rotation);
                        }
                    }
                    else
                    {
                        Debug.Log("HealthEffectDropItem: Contains null reference prefabs");
                    }
                }
            }
        }
    }


}