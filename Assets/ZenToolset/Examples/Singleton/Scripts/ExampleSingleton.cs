using UnityEngine;

namespace Junnav.Zen.Toolset.Singleton.Example
{
    public class ExampleSingleton : Singleton<ExampleSingleton>
    {
        public void SpawnGameObject(GameObject obj)
        {
#if UNITY_EDITOR
            Debug.Log($"<b><color=#2ecc71>Singleton method called!</color></b>");
#endif

            if (obj)
            {
                Instantiate(obj).SetActive(true);
            }
        }

        protected override void Awake()
        {
            // You must call the base Awake function first
            base.Awake();

#if UNITY_EDITOR
            Debug.Log($"<b><color=#34495e>Awake called!</color></b>");
#endif
        }

        protected override void OnDestroy()
        {
            // You must call the base onDestroy function first
            base.OnDestroy();

#if UNITY_EDITOR
            Debug.Log($"<b><color=#34495e>Destroy called!</color></b>");
#endif
        }
    }
}
