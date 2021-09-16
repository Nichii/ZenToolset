using System.Collections;
using UnityEngine;

namespace ZenToolset.Example
{
    public class ObjectPoolSpawnExample : MonoBehaviour
    {
        [SerializeField] private GameObject poolPrefab = null;
        [SerializeField] private float spawnRate = 0.5f;
        [SerializeField] private ObjectPooler pooler = null;
        
        private void Start()
        {
            StartCoroutine(Spawning());
        }

        private IEnumerator Spawning()
        {
            WaitForSeconds wait = new WaitForSeconds(spawnRate);
            
            while (true)
            {
                pooler.TryToSpawn(poolPrefab);
                yield return wait;
            }
        }
    }
}
