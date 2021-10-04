using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZenToolset
{
    [InitializeOnLoad]
    static class ZenSceneAutoLoader
    {
        private const string prefPreviousScene = "ZenToolset.SceneAutoLoader.PreviousScene";

        private static string PreviousScene
        {
            get { return EditorPrefs.GetString(prefPreviousScene, EditorSceneManager.GetActiveScene().path); }
            set { EditorPrefs.SetString(prefPreviousScene, value); }
        }

        static ZenSceneAutoLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            ZenToolsetSettings settings = ZenToolsetSettings.GetOrCreateSettings();

            if (!settings.UseStartingScene) return;

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    PreviousScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        string scenePath = AssetDatabase.GetAssetPath(settings.StartingScene);

                        try
                        {
                            EditorSceneManager.OpenScene(scenePath);
                        }
                        catch
                        {
                            Debug.LogError($"[ZenToolset] Unable to load starting scene '{settings.StartingScene.name}'");
                            EditorApplication.isPlaying = false;
                        }
                    }
                    else
                    {
                        // User cancelled the save operation, cancel play as well
                        EditorApplication.isPlaying = false;
                    }
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    try
                    {
                        EditorSceneManager.OpenScene(PreviousScene);
                    }
                    catch
                    {
                        Debug.LogError($"[ZenToolset] Unable to load previous scene '{PreviousScene}'");
                    }
                    break;
            }
        }
    }
}
