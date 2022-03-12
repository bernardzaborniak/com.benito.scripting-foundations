using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Managers.Editor
{
    public class ManagersGameObjectMenuExtension
    {
        [MenuItem("GameObject/ScriptingFoundations/Scene Singleton Manager")]
        public static void CreateSceneSingletonManager()
        {
            GameObject obj = new GameObject("SCENE SINGLETON MANAGER");
            obj.AddComponent<LocalSceneManagers>();
        }
    }
}

