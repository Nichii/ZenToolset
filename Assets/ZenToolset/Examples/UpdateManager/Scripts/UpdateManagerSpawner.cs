using UnityEngine;

namespace ZenToolset.Example
{
    public class UpdateManagerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject spawnObj = null;
        [SerializeField] private float spawnDistance = 1f;
        [SerializeField] private int spawnX = 100;
        [SerializeField] private int spawnZ = 100;

        private void Start()
        {
            for (int x = 0; x < spawnX; x++)
            {
                for (int z = 0; z < spawnZ; z++)
                {
                    Instantiate(spawnObj, new Vector3(x * spawnDistance, 0, z * spawnDistance), Quaternion.identity);
                }
            }
        }
    }
}
