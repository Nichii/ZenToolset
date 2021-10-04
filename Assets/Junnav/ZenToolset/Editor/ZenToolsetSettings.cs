using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZenToolset
{
    class ZenToolsetSettings : ScriptableObject
    {
        public const string SettingsFolder = "Assets/Settings/Editor/ZenToolset/";
        public const string AssetName = "ZenToolsetSettings.asset";
        public static string SettingsPath => $"{SettingsFolder}{AssetName}";

        private static readonly char[] pathSeparator = new char[] { '/' };
        
        [SerializeField]
        private bool useStartingScene = false;
        public bool UseStartingScene => useStartingScene;

        [SerializeField]
        private SceneAsset startingScene = null;
        public SceneAsset StartingScene => startingScene;

        internal static ZenToolsetSettings GetOrCreateSettings()
        {
            ZenToolsetSettings settings = AssetDatabase.LoadAssetAtPath<ZenToolsetSettings>(SettingsPath);

            // Create settings if it doesn't exist
            if (settings == null)
            {
                settings = CreateInstance<ZenToolsetSettings>();
                
                // Check to make sure the folder exists
                if (!AssetDatabase.IsValidFolder(SettingsFolder))
                {
                    // Make sure each parent folders are properly created
                    string[] parentFolders = SettingsFolder.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries);
                    string currentPath = parentFolders[0];

                    for (int i = 1; i < parentFolders.Length; i++)
                    {
                        if (string.IsNullOrEmpty(parentFolders[0]))
                        {
                            continue;
                        }

                        string newPath = $"{currentPath}/{parentFolders[i]}";
                        Debug.Log(newPath);

                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, parentFolders[i]);
                        }

                        currentPath = newPath;
                    }
                }

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}