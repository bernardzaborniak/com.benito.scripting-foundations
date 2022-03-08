using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;


namespace Benito.ScriptingFoundations.Managers
{
    public class GlobalManagersSettings : ScriptableObject
    {
        public const string DefaultSettingsPathInResourcesFolder = "Settings/Global Managers Settings";

        [Space(10)]
        [Tooltip("Prefab Containing all the Global Managers, use \"Create/Scripting Foundations/Global Managers\" Prefab to create.")]
        public GameObject globalManagersPrefab;

        public static GlobalManagersSettings GetOrCreateSettings()
        {
            GlobalManagersSettings settings = RessourceSettingsUtilities.GetOrCreateSettingAsset<GlobalManagersSettings>(DefaultSettingsPathInResourcesFolder);

#if UNITY_EDITOR           
            if (settings.globalManagersPrefab == null)
            {
                if (UnityEditor.AssetDatabase.IsValidFolder("Packages/com.benito.scripting-foundations/Prefabs"))
                {
                    GameObject globalManagersPrefab = (GameObject) UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.benito.scripting-foundations/Prefabs/Global Managers Prefab.prefab",typeof(GameObject));
                    if(globalManagersPrefab != null)
                    {
                        settings.globalManagersPrefab = globalManagersPrefab;
                        UnityEditor.EditorUtility.SetDirty(settings);
                        return settings;
                    }
                   
                }
                else
                {
                    UnityEditor.AssetDatabase.CreateFolder("Packages/com.benito.scripting-foundations", "Prefabs");
                }

                Debug.Log("Created new GlobalManagersPrefab at Packages/com.benito.scripting-foundations/Prefabs, as none was assigned inside ProjectSettings/Global Managers Settings");
                GameObject newGlobalManagersPrefab = new GameObject("Global Managers Prefab", typeof(GlobalManagers), typeof(DontDestroyOnLoadWrapper));
                UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(newGlobalManagersPrefab, "Packages/com.benito.scripting-foundations/Prefabs/Global Managers Prefab.prefab", UnityEditor.InteractionMode.UserAction);
                settings.globalManagersPrefab = newGlobalManagersPrefab;
                UnityEditor.EditorUtility.SetDirty(settings);
                GameObject.DestroyImmediate(newGlobalManagersPrefab);
            }
#endif

            return settings;
        }
    }
}
