using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using Benito.ScriptingFoundations.Saving.SceneObjects;

namespace Benito.ScriptingFoundations.Saving.Editor
{
    public class OnSaveSceneAssignsSaveablesAndIdsHook : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if(path == SceneManager.GetActiveScene().path)
                {
                    //Debug.Log("AssignMissingIdsInCurrentScene on scene Save");
                    SaveableSceneObjectsIdAssigner.AssignMissingIdsInCurrentScene();

                    SaveableObjectsSceneManager manager = GameObject.FindObjectOfType<SaveableObjectsSceneManager>();

                    if (manager != null)
                    {
                        manager.ScanSceneForSaveableObjects();
                    }
                }
            }

            return paths;
        }
    }
}

