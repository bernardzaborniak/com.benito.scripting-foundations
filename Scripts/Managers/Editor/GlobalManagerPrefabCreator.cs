using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;
using Benito.ScriptingFoundations.BDebug;
using Benito.ScriptingFoundations.Modding;

namespace Benito.ScriptingFoundations.Managers.Editor
{
    /// <summary>
    /// Simple Script to allow quick creation of the required Prefab.
    /// </summary>
    public class GlobalManagerPrefabCreator
    {
        [MenuItem("Assets/Create/Scripting Foundations/Global Managers Prefab", isValidateFunction: false, priority: -20)]
        public static void CreateGlobalManagersPrefab()
        {
            GameObject globalManagersPrefab = new GameObject("Global Managers Prefab", typeof(GlobalSingletonManager));

            PrefabUtility.SaveAsPrefabAssetAndConnect(globalManagersPrefab, EditorUtilities.TryGetActiveProjectWindowFolderPath() + "/Global Managers Prefab.prefab", InteractionMode.UserAction);

            GameObject.DestroyImmediate(globalManagersPrefab);
        }
    }

}
