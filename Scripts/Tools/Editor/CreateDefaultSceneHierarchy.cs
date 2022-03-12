using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Managers;
using UnityEngine.SceneManagement;

namespace Benito.ScriptingFoundations.Tool.Editor
{
    public class CreateDefaultSceneHierarchy
    {
        [MenuItem("GameObject/ScriptingFoundations/Create Default Scene Hierarchy (Delete Existing)")]
        public static void CreateSceneHierarchyDeleteExisting()
        {
            CreateSceneHierarchy(true);
        }

        [MenuItem("GameObject/ScriptingFoundations/Create Default Scene Hierarchy (Keep Existing)")]
        public static void CreateSceneHierarchyKeepExisting()
        {
            CreateSceneHierarchy(false);
        }

        static void CreateSceneHierarchy(bool deleteExisting)
        {
            // Delete all existing Items
            if (deleteExisting)
            {
                GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

                foreach (GameObject go in objects)
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }

            GameObject managers = new GameObject("-- Managers --");
            
            GameObject singletonManager = new GameObject("SCENE SINGLETON MANAGER");
            singletonManager.transform.parent = managers.transform;
            singletonManager.AddComponent<LocalSceneManagers>();


            GameObject globalLighting = new GameObject("-- Global Lighting --");
            GameObject directionLight = new GameObject("Directional Light");
            directionLight.transform.parent = globalLighting.transform;

            Light light = directionLight.AddComponent<Light>();
            light.type = LightType.Directional;

            Undo.RegisterCreatedObjectUndo(managers, "CreateDefaultSceneHierarchy");
            Undo.RegisterCreatedObjectUndo(globalLighting, "CreateDefaultSceneHierarchy");
        }
    }
}
