using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
static class SceneAutoLoader
{
    // Editor preference keys
    private const string prefLoadMasterOnPlay = "ZenToolset.SceneAutoLoader.LoadMasterOnPlay";
    private const string prefMasterScene = "ZenToolset.SceneAutoLoader.MasterScene";
    private const string prefPreviousScene = "ZenToolset.SceneAutoLoader.PreviousScene";

    private static bool LoadMasterOnPlay
    {
        get { return EditorPrefs.GetBool(prefLoadMasterOnPlay, false); }
        set { EditorPrefs.SetBool(prefLoadMasterOnPlay, value); }
    }

    private static string MasterScene
    {
        get { return EditorPrefs.GetString(prefMasterScene, "Master.unity"); }
        set { EditorPrefs.SetString(prefMasterScene, value); }
    }

    private static string PreviousScene
    {
        get { return EditorPrefs.GetString(prefPreviousScene, EditorSceneManager.GetActiveScene().path); }
        set { EditorPrefs.SetString(prefPreviousScene, value); }
    }

    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!LoadMasterOnPlay) return;

        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                PreviousScene = EditorSceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    try
                    {
                        EditorSceneManager.OpenScene(MasterScene);
                    }
                    catch
                    {
                        Debug.LogError(string.Format("error: scene not found: {0}", MasterScene));
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
                    Debug.LogError(string.Format("error: scene not found: {0}", PreviousScene));
                }
                break;
        }
    }

    [MenuItem("Tools/ZenToolset/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");

        // Make sure the path is relative instead of absolute
        masterScene = masterScene.Replace(Application.dataPath, "Assets");

        if (!string.IsNullOrEmpty(masterScene))
        {
            MasterScene = masterScene;
            LoadMasterOnPlay = true;
        }
    }

    [MenuItem("Tools/ZenToolset/Scene Autoload/Load Master On Play", true)]
    private static bool ShowLoadMasterOnPlay()
    {
        return !LoadMasterOnPlay;
    }
    [MenuItem("Tools/ZenToolset/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = true;
    }

    [MenuItem("Tools/ZenToolset/Scene Autoload/Don't Load Master On Play", true)]
    private static bool ShowDontLoadMasterOnPlay()
    {
        return LoadMasterOnPlay;
    }
    [MenuItem("Tools/ZenToolset/Scene Autoload/Don't Load Master On Play")]
    private static void DisableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = false;
    }
}
