using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZenToolset
{
    class ZenToolsetSettingsRegister : MonoBehaviour
    {
        [SettingsProvider]
        public static SettingsProvider CreateZenToolsetSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/Zen Toolset", SettingsScope.Project)
            {
                label = "Zen Toolset",
                guiHandler = (searchContext) =>
                {
                    SerializedObject settings = ZenToolsetSettings.GetSerializedSettings();
                    
                    EditorGUILayout.LabelField("Scene Options", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty("useStartingScene"), new GUIContent("Use Starting Scene"));
                    EditorGUILayout.PropertyField(settings.FindProperty("startingScene"), new GUIContent("Starting Scene"));

                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = new HashSet<string>(new[] { "Use Starting Scene", "Starting Scene" }),
            };

            return provider;
        }
    }
}
