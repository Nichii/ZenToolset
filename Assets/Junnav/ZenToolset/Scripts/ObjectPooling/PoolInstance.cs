using UnityEngine;

namespace ZenToolset
{
    /// <summary>
    /// Attach this to any game objects that will be managed by the ObjectPooler.
    /// Will be automatically added if its missing from the game object.
    /// </summary>
    public class PoolInstance : MonoBehaviour
    {
        [Tooltip("Set 'true' to auto return to pool when this game object is disabled")]
        public bool returnToPoolOnDisable = true;

        private IPoolInstanceEvent[] poolInstanceEvents = null;

        private ObjectPooler pool = null;
        private GameObject originalPrefab = null;

        /// <summary>
        /// Pool reference can only be set once by the ObjectPooler
        /// </summary>
        public ObjectPooler Pool
        {
            set
            {
                if (pool != null) return;
                pool = value;
            }
        }

        /// <summary>
        /// Original prefab reference can only be set once by the ObjectPooler
        /// </summary>
        public GameObject OriginalPrefab
        {
            get
            {
                return originalPrefab;
            }
            
            set
            {
                if (originalPrefab != null) return;
                originalPrefab = value;
            }
        }

        /// <summary>
        /// Called by ObjectPooler when this GameObject has been spawned from the pool
        /// </summary>
        public void OnSpawned()
        {
            if (poolInstanceEvents == null) return;

            for (int i = 0; i < poolInstanceEvents.Length; i++)
            {
                poolInstanceEvents[i].OnPoolSpawned();
            }
        }

        /// <summary>
        /// Called by ObjectPooler when this GameObject has been despawned back to the pool
        /// </summary>
        public void OnDespawned()
        {
            if (poolInstanceEvents == null) return;

            for (int i = 0; i < poolInstanceEvents.Length; i++)
            {
                poolInstanceEvents[i].OnPoolDespawned();
            }
        }

        /// <summary>
        /// Call this to manually despawn this game object
        /// </summary>
        public void Despawn()
        {
            if (pool == null) return;

            pool.Despawn(this);
        }

        /// <summary>
        /// Call this if you need it to refresh PoolInstance's list of interfaces to call
        /// </summary>
        public void UpdatePoolInstanceEvents()
        {
            poolInstanceEvents = GetComponentsInChildren<IPoolInstanceEvent>();
        }

        private void Awake()
        {
            UpdatePoolInstanceEvents();
        }

        private void OnDisable()
        {
            if (returnToPoolOnDisable)
            {
                Despawn();
            }
        }
    }
}
