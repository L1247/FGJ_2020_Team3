using System;
using System.Collections.Generic;
using MonsterLove.HealthPro.Collections;
using UnityEngine;

/*

 Modifications have been made to the original:
 Added an out parameter to indicate if the object has already been recycled
 This is useful when you want to add logic on first creation of an object.

 Renmaed namespace to prevent overlap issues for users that potentially might use this pooling solution already.

 Source : https://github.com/thefuntastic/unity-object-pool
 Licence : MIT

*/

namespace MonsterLove.HealthPro.Pooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        public bool logStatus;
        public Transform root;

        private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
        private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

        private static bool closingGame = false;

        void Awake()
        {
            prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
            instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        }

        private void OnDisable()
        {
            PoolManager.closingGame = true;
        }

        public void warmPool(GameObject prefab, int size)
        {
            if (prefabLookup.ContainsKey(prefab))
            {
                throw new Exception("Pool for prefab " + prefab.name + " has already been created");
            }
            var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
            prefabLookup[prefab] = pool;
        }

        public GameObject spawnObject(GameObject prefab, out bool isRecycled)
        {
            return spawnObject(prefab, Vector3.zero, Quaternion.identity, out isRecycled);
        }

        public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation, out bool isRecycled)
        {
            bool createdPool = false;

            if (!prefabLookup.ContainsKey(prefab))
            {
                WarmPool(prefab, 1);
                createdPool = true;
            }

            var pool = prefabLookup[prefab];

            var clone = pool.GetItem(out isRecycled);

            if (createdPool)
                isRecycled = false;

            clone.transform.position = position;
            clone.transform.rotation = rotation;
            clone.SetActive(true);

            instanceLookup.Add(clone, pool);

            return clone;
        }

        public void releaseObject(GameObject clone)
        {
            clone.SetActive(false);

            if (instanceLookup.ContainsKey(clone))
            {
                instanceLookup[clone].ReleaseItem(clone);
                instanceLookup.Remove(clone);
            }
            else
            {
                Debug.LogWarning("No pool contains the object: " + clone.name);
            }
        }

        private GameObject InstantiatePrefab(GameObject prefab)
        {
            var go = Instantiate(prefab) as GameObject;
            if (root != null) go.transform.parent = root;
            return go;
        }

        public void PrintStatus()
        {
            foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in prefabLookup)
            {
                Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name, keyVal.Value.CountUsedItems, keyVal.Value.Count));
            }
        }

        #region Static API

        public static void WarmPool(GameObject prefab, int size)
        {
            Instance.warmPool(prefab, size);
        }

        public static GameObject SpawnObject(GameObject prefab)
        {
            bool temp;
            return SpawnObject(prefab, out temp);
        }

        public static GameObject SpawnObject(GameObject prefab, out bool isRecycled)
        {
            return Instance.spawnObject(prefab, out isRecycled);
        }

        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            bool temp;
            return SpawnObject(prefab, position, rotation, out temp);
        }

        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, out bool isRecycled)
        {
            return Instance.spawnObject(prefab, position, rotation, out isRecycled);
        }

        public static void ReleaseObject(GameObject clone)
        {
            if (closingGame)
                return;

            Instance.releaseObject(clone);
        }

        #endregion
    }
}


