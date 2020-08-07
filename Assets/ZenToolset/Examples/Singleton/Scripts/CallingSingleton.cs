using UnityEngine;

namespace Junnav.Zen.Toolset.Singleton.Example
{
    public class CallingSingleton : MonoBehaviour
    {
        [SerializeField] private GameObject exampleObj = null;
        
        private void Start()
        {
            // Check if the singleton exists first before calling the function
            if (ExampleSingleton.Instance)
            {
                ExampleSingleton.Instance.SpawnGameObject(exampleObj);
            }
        }
    }
}
