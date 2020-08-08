using System.Collections.Generic;
using UnityEngine;

namespace ZenToolset
{
    /// <summary>
    /// Auto spawns a pool of game object that can be continuously spawned and despawned for better performance
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private PoolInfo[] objectsToPool = null;

        private readonly Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();
        
        [System.Serializable]
        private struct PoolInfo
        {
            public string poolName;
            public GameObject prefab;
            public int initialPoolAmount;
            public bool autoIncreasePool;

            public PoolInfo(string poolName, GameObject prefab, int initialPoolAmount, bool autoIncreasePool)
            {
                this.poolName = poolName;
                this.prefab = prefab;
                this.initialPoolAmount = initialPoolAmount;
                this.autoIncreasePool = autoIncreasePool;
            }
        }

        private struct Pool
        {
            public PoolInfo poolInfo;
            public Queue<PoolInstance> poolQueue;

            public Pool(PoolInfo poolInfo, Queue<PoolInstance> poolQueue)
            {
                this.poolInfo = poolInfo;
                this.poolQueue = poolQueue;
            }
        }

        /// <summary>
        /// Try to spawn from the object pool with a prefab reference
        /// </summary>
        /// <param name="prefab">Same prefab reference on the ObjectPooler</param>
        /// <returns>A GameObject if it's available on the pool, null if not available</returns>
        public GameObject TryToSpawn(GameObject prefab)
        {
            if (prefab == null) return null;
            
            if (!pools.ContainsKey(prefab)) return null;

            // If the pool is empty, check if we can instantiate more
            if (pools[prefab].poolQueue.Count <= 0)
            {
                if (pools[prefab].poolInfo.autoIncreasePool)
                {
                    // Instantiate more to the pool
                    InstantiateAndQueue(pools[prefab].poolQueue, prefab);
                    return TryToSpawn(prefab);
                }
                else
                {
                    // We can't instantiate anymore
                    return null;
                }
            }

            // Dequeue it and activate the game object
            PoolInstance instance = pools[prefab].poolQueue.Dequeue();
            GameObject obj = instance.gameObject;
            obj.SetActive(true);

            // Invoke the onSpawned event on PoolInstance
            instance.OnSpawned();

            return obj;
        }

        /// <summary>
        /// Despawn this game object back to the object pool.
        /// Only successful if it contains PoolInstance and it's original prefab currently being pooled.
        /// </summary>
        /// <param name="obj">GameObject to despawn back to object pool</param>
        public void Despawn(GameObject obj)
        {
            if (obj == null) return;

            PoolInstance poolInstance = obj.GetComponent<PoolInstance>();
            Despawn(poolInstance);
        }

        /// <summary>
        /// Despawns this game object back to the object pool.
        /// Only successful if its original prefab is currently being pooled.
        /// </summary>
        /// <param name="instance">PoolInstance whose GameObject will be despawned back to object pool</param>
        public void Despawn(PoolInstance instance)
        {
            if (instance == null) return;
            if (!pools.ContainsKey(instance.OriginalPrefab)) return;

            // Disable game object
            instance.gameObject.SetActive(false);

            // Call the onDespawned event on PoolInstance
            instance.OnDespawned();

            // Queue it back to pool
            pools[instance.OriginalPrefab].poolQueue.Enqueue(instance);
        }

        /// <summary>
        /// Initialize the object pool and spawn all of the game objects
        /// </summary>
        private void InitializePools()
        {
            if (objectsToPool == null) return;

            for (int i = 0; i < objectsToPool.Length; i++)
            {
                if (objectsToPool[i].prefab == null) continue;

                if (pools.ContainsKey(objectsToPool[i].prefab))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"{objectsToPool[i].prefab.name} has already been pooled! Skipping to the next prefab...");
#endif
                    continue;
                }

                Queue<PoolInstance> queue = new Queue<PoolInstance>(objectsToPool[i].initialPoolAmount);

                for (int q = 0; q < objectsToPool[i].initialPoolAmount; q++)
                {
                    InstantiateAndQueue(queue, objectsToPool[i].prefab);
                }

                // Add the queues to pools
                pools.Add(objectsToPool[i].prefab, new Pool(objectsToPool[i], queue));
            }
        }

        /// <summary>
        /// Instantiate one GameObject from prefab and add it to the queue
        /// </summary>
        /// <param name="queue">PoolInstance queue</param>
        /// <param name="prefab">Prefab to be instantiated</param>
        private void InstantiateAndQueue(Queue<PoolInstance> queue, GameObject prefab)
        {
            // Spawn the game object and deactivate it
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);

            // Check if PoolInstance script has been added, otherwise add it by code
            PoolInstance instance = obj.GetComponent<PoolInstance>();

            if (instance == null)
            {
                instance = obj.AddComponent<PoolInstance>();
            }

            // Set reference to the pool and original prefabs
            instance.Pool = this;
            instance.OriginalPrefab = prefab;

            // Despawn it
            Despawn(instance);

            // Add to queue
            queue.Enqueue(instance);
        }

        private void Awake()
        {
            InitializePools();
        }
    }
}
