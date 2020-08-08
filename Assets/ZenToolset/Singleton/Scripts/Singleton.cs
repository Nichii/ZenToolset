using UnityEngine;

namespace ZenToolset
{
    /// <summary>
    /// Makes sure there's only one instance of this script running at the same time. Inherit from this script to create a new singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = false;

        public static T Instance
        {
            get
            {
                return instance;
            }

            private set
            {
                if (instance != null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Singleton has already existed! Destroying {value.gameObject.name}...");
#endif
                    Destroy(value);
                    return;
                }
                
                instance = value;
            }
        }

        private static T instance = default;

        /// <summary>
        /// Determines if current script is the current singleton instance
        /// </summary>
        protected bool IsCurrentlySingletonInstance => instance == this as T;

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this as T;

                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
