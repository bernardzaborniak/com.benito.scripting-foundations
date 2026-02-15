using Benito.ScriptingFoundations.Saving.SceneObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Benito.ScriptingFoundations.IdSystem.Editor
{
    public class OnSaveSceneAssignsIdsHook : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if (path == SceneManager.GetActiveScene().path)
                {
                    Debug.Log("OnSaveSceneAssignsIdsHook save scene ");

                    //Debug.Log("AssignMissingIdsInCurrentScene on scene Save");
                    SceneObjectsIdAssigner.AssignMissingIdsInCurrentScene();

                    IdReferenceManager manager = GameObject.FindFirstObjectByType<IdReferenceManager>();

                    if (manager != null) manager.ScanSceneForIdReferenceObjects();
                }
            }

            return paths;
        }
    }
}
