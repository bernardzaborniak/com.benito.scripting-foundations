using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// https://docs.unity3d.com/2019.1/Documentation/ScriptReference/SettingsProvider.html

namespace Benito.ScriptingFoundations.Utilities
{
    public static class RessourceSettingsUtilities
    {
        public static T GetOrCreateSettingAsset<T>(string defaultSettingsPathInRessourceFolder) where T : ScriptableObject
        {
            T settings = Resources.Load<T>(defaultSettingsPathInRessourceFolder);

            if (settings == null)
            {
#if UNITY_EDITOR
                IOUtilities.EnsurePathExists(Path.Combine(Application.dataPath,"Resources", defaultSettingsPathInRessourceFolder));

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
