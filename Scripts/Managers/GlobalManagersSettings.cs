using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Benito.ScriptingFoundations.Managers
{
    // https://docs.unity3d.com/2019.1/Documentation/ScriptReference/SettingsProvider.html

    //[CreateAssetMenu(fileName = "Global Managers Settings", menuName = "Scripting Foundations/Global Managers Settings", order = 100)]
    public class GlobalManagersSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Global Managers Settings";

        [Space(10)]
        [Tooltip("Prefab Containing all the Global Managers, use \"Create/Scripting Foundations/Global Managers\" Prefab to create.")]
        public GameObject globalManagersPrefab;

        public static GlobalManagersSettings GetOrCreateSettings()
        {
            GlobalManagersSettings settings = Resources.Load<GlobalManagersSettings>(DefaultSettingsPathInResourcesFolder);
            Debug.Log("loaded from ressources folder: " + settings);
            Debug.Log("paths resources: " +DefaultSettingsPathInResourcesFolder);
            Debug.Log("paths resources: " + "Assets/Resources/" + DefaultSettingsPathInResourcesFolder);

            if(settings == null)
            {
                Debug.Log("Create");
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
                settings = ScriptableObject.CreateInstance<GlobalManagersSettings>();
                UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/Resources/" + DefaultSettingsPathInResourcesFolder + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
#else
                throw new System.Exception("No Global Managers Settings aviable in Ressources Folder, make sure it was created in the editor by just opening up Project Settings/Global Managers once");
#endif
            }
            else
            {
                Debug.Log("Get");
            }

            return settings;

        }
    }  
}
