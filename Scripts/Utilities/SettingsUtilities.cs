using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/2019.1/Documentation/ScriptReference/SettingsProvider.html

namespace Benito.ScriptingFoundations.Utilities
{
    public static class SettingsUtilities
    {
        public static T GetOrCreateSettingAsset<T>(string defaultSettingsPathInRessourceFolder) where T : ScriptableObject
        {
            T settings = Resources.Load<T>(defaultSettingsPathInRessourceFolder);

            if (settings == null)
            {
                //Debug.Log($"Create {typeof(T)} Asset");
#if UNITY_EDITOR
                // Create Folder if none is present
                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                }

                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Settings"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "Settings");
                }

                // Create Asset
                settings = ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/Resources/" + defaultSettingsPathInRessourceFolder + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
#else
                throw new System.Exception("No {typeof(T)} aviable in Ressources Folder, make sure it was created in the editor by just opening up Project Settings/Global Managers once");
#endif
            }
            else
            {
                //Debug.Log($"Get {typeof(T)} Asset");
            }

            return settings;
        }
    }
}