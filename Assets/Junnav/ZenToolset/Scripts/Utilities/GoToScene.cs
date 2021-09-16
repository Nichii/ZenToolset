using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ZenToolset
{
    /// <summary>
    /// Basic helper component to customize loading scene experience
    /// </summary>
    public class GoToScene : MonoBehaviour
    {
        [Header("Load Scene Behaviours")]
        [Tooltip("Automatically loads next scene on start")]
        [SerializeField] protected bool changeOnStart = false;
        [Tooltip("What happens when currently in last/first scene and failed to load next scene")]
        [SerializeField] protected LastSceneBehaviourAction lastSceneBehaviour = LastSceneBehaviourAction.BackToFirst;
        [Tooltip("Whether to interrupt previous scene loading when loading again")]
        [SerializeField] protected bool isInterruptible = false;

        [Header("Asynchronous Behaviours")]
        [Tooltip("Whether or not to load scene asynchronously")]
        [SerializeField] protected bool isAsync = false;
        [Tooltip("Allow Scenes to be activated as soon as it is ready.")]
        [SerializeField] protected bool allowSceneActivation = true;

        [Header("Delays")]
        [Tooltip("Add delays before loading scene")]
        [SerializeField] protected float loadDelay = 0f;

        [Header("Scene Names")]
        [Tooltip("Override the name of the next scene that will be loaded")]
        [SerializeField] protected string overrideNextSceneName = string.Empty;
        [Tooltip("Default scene to load if Last Scene Behaviour is set to 'Load Default'")]
        [SerializeField] protected string defaultSceneName = string.Empty;

        [Header("Events")]
        [Tooltip("Called when scene loading has started")]
        [SerializeField] protected UnityEvent onStartLoading = new UnityEvent();
        [Tooltip("Called upon scene loading finished")]
        [SerializeField] protected UnityEvent onFinishedLoading = new UnityEvent();
        [Tooltip("Called while Last Scene Behaviour is set to 'Stop'")]
        [SerializeField] protected UnityEvent onLastSceneStopped = new UnityEvent();
        [Tooltip("Called upon failing to load default scene")]
        [SerializeField] protected UnityEvent onLoadDefaultSceneFailed = new UnityEvent();

        [Header("Async Events")]
        [Tooltip("Called while asynchronously loading scene, containing loading progress amount")]
        [SerializeField] protected AsyncLoadProgressEvent onAsyncLoadProgress = new AsyncLoadProgressEvent();
        [Tooltip("Called when Allow Scene Activation is off and async loading progress is at 0.9")]
        [SerializeField] protected AsyncWaitActivationEvent onAsyncWaitActivation = new AsyncWaitActivationEvent();

        /// <summary>
        /// Checks if currently in progress of loading scene
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
        }
        protected bool isLoading = false;

        /// <summary>
        /// Determines what do to when trying to auto-load the next scene while in the last scene
        /// </summary>
        protected enum LastSceneBehaviourAction
        {
            /// <summary>
            /// Does nothing, onLastSceneStopped event will be called
            /// </summary>
            Stop,
            /// <summary>
            /// Goes back to the first scene in the build settings
            /// </summary>
            BackToFirst,
            /// <summary>
            /// Jumps to the last scene in the build settings
            /// </summary>
            JumpToLast,
            /// <summary>
            /// Reloads the current scene
            /// </summary>
            ReloadCurrent,
            /// <summary>
            /// Loads default scene as set in 'defaultSceneName'
            /// </summary>
            LoadDefault,
        }
        
        /// <summary>
        /// Event for returning asynchronous scene loading progress
        /// </summary>
        [System.Serializable] public class AsyncLoadProgressEvent : UnityEvent<float> { }

        /// <summary>
        /// Event upon finished async loading, but allowSceneActivation was turned off.
        /// This allows player to customize when their scene shall be loaded.
        /// </summary>
        [System.Serializable] public class AsyncWaitActivationEvent : UnityEvent<AsyncOperation> { }

        /// <summary>
        /// Automatically handles scene loading (will use OverrideNextSceneName)
        /// </summary>
        public virtual void LoadScene()
        {
            if (!TryLoadingSceneByName(overrideNextSceneName))
            {
                LoadNextScene();
            }
        }

        /// <summary>
        /// Loads next scene in build index
        /// </summary>
        public virtual void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = Mathf.Min(currentSceneIndex + 1, SceneManager.sceneCountInBuildSettings - 1);

            // If the current scene is the last
            if (nextSceneIndex == currentSceneIndex)
            {
                HandleLastSceneBehaviour();
            }
            else
            {
                HandleSceneLoading(nextSceneIndex);
            }
        }

        /// <summary>
        /// Loads previous scene in build index
        /// </summary>
        public virtual void LoadPreviousScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int previousSceneIndex = Mathf.Max(currentSceneIndex - 1, 0);

            // If the current scene is the first
            if (previousSceneIndex == currentSceneIndex)
            {
                HandleLastSceneBehaviour();
            }
            else
            {
                HandleSceneLoading(previousSceneIndex);
            }
        }

        /// <summary>
        /// Reloads the current scene
        /// </summary>
        public virtual void ReloadCurrentScene()
        {
            HandleSceneLoading(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Tries to load a scene by its name
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns>True if scene exists, false if otherwise</returns>
        public virtual bool TryLoadingSceneByName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;
            
            Scene scene = SceneManager.GetSceneByName(sceneName);

            if (scene == null) return false;
            if (!scene.IsValid()) return false;

            HandleSceneLoading(scene.buildIndex);

            return true;
        }

        protected virtual void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void Start()
        {
            if (changeOnStart)
            {
                LoadNextScene();
            }
        }

        protected virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnValidate()
        {
            if (loadDelay < 0f)
            {
                loadDelay = 0f;
            }
        }

        /// <summary>
        /// Handles what happened when trying to load non-existent next/previous scene
        /// </summary>
        protected virtual void HandleLastSceneBehaviour()
        {
            switch (lastSceneBehaviour)
            {
                case LastSceneBehaviourAction.BackToFirst:
                    HandleSceneLoading(0);
                    break;

                case LastSceneBehaviourAction.JumpToLast:
                    HandleSceneLoading(SceneManager.sceneCountInBuildSettings - 1);
                    break;

                case LastSceneBehaviourAction.ReloadCurrent:
                    HandleSceneLoading(SceneManager.GetActiveScene().buildIndex);
                    break;

                case LastSceneBehaviourAction.LoadDefault:
                    if (!TryLoadingSceneByName(defaultSceneName))
                    {
#if UNITY_EDITOR
                        Debug.LogError("Failed to load default scene by name: " +
                            "Default scene does not exist!");
#endif

                        onLoadDefaultSceneFailed.Invoke();
                    }
                    break;

                default:
                    onLastSceneStopped.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Loads scene by name. Will load asynchronously when isAsync is enabled.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        protected virtual void HandleSceneLoading(string sceneName)
        {
            HandleSceneLoading(SceneManager.GetSceneByName(sceneName).buildIndex);
        }

        /// <summary>
        /// Loads scene by its build index. Will load asynchronously when isAsync is enabled.
        /// </summary>
        /// <param name="buildIndex">Build index of the scene to load</param>
        protected virtual void HandleSceneLoading(int buildIndex)
        {
            if (IsLoading && isInterruptible)
            {
                StopAllCoroutines();
            }
            
            if (!isAsync)
            {
                StartCoroutine(DelayedSceneLoading(buildIndex));
            }
            else
            {
                StartCoroutine(HandleSceneLoadingAsync(buildIndex));
            }
        }

        /// <summary>
        /// Loads scene by its build index with load delay (if any)
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <returns></returns>
        protected virtual IEnumerator DelayedSceneLoading(int buildIndex)
        {
            isLoading = true;
            
            if (loadDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(loadDelay);
            }
            
            onStartLoading.Invoke();

            SceneManager.LoadScene(buildIndex);
        }

        /// <summary>
        /// Loads scene asynchronously by name
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns></returns>
        protected virtual IEnumerator HandleSceneLoadingAsync(string sceneName)
        {
            return HandleSceneLoadingAsync(SceneManager.GetSceneByName(sceneName).buildIndex);
        }

        /// <summary>
        /// Loads scene asynchronously by its build index
        /// </summary>
        /// <param name="buildIndex">Build index of the scene to load</param>
        /// <returns></returns>
        protected virtual IEnumerator HandleSceneLoadingAsync(int buildIndex)
        {
            isLoading = true;

            if (loadDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(loadDelay);
            }

            onStartLoading.Invoke();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

            asyncLoad.allowSceneActivation = allowSceneActivation;

            while (!asyncLoad.isDone)
            {
                onAsyncLoadProgress.Invoke(asyncLoad.progress);

                if (allowSceneActivation && asyncLoad.progress >= 0.9f)
                {
                    onAsyncWaitActivation.Invoke(asyncLoad);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Called when scene has finished loading
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isLoading = false;

            onFinishedLoading.Invoke();
        }
    }
}
