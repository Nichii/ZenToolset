using UnityEngine;

namespace ZenToolset.Example
{
    /// <summary>
    /// Implement the IPoolInstanceEvent to listen for spawn and despawn event from PoolInstance
    /// </summary>
    public class PoolInstanceExampleMover : MonoBehaviour, IPoolInstanceEvent
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float spawnX = -10f;
        [SerializeField] private float despawnX = 10f;

        public void OnPoolDespawned()
        {
            // This will be called by PoolInstance when this game object has been despawned
#if UNITY_EDITOR
            Debug.Log("Despawned!");
#endif
        }

        public void OnPoolSpawned()
        {
            // This will be called by PoolInstance when this game object has been spawned
            transform.position = new Vector3(spawnX, 0f);
        }

        private void Update()
        {
            // Perform your usual logic here
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);

            if (transform.position.x >= despawnX)
            {
                // Disabling this object will automatically return it back to the pool (unless this setting is off)
                gameObject.SetActive(false);
            }
        }
    }
}
