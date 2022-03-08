using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerScene
    {
        List<ISaveableObject> saveableObjects = new List<ISaveableObject>();

        public void RegisterSaveableObject(ISaveableObject saveable)
        {

        }

        public void UnregisterSaveableObject(ISaveableObject saveable)
        {

        }

        public void SaveAllObjects()
        {
            for (int i = 0; i < saveableObjects.Count; i++)
            {

            }
        }

        public void LoadFromSaveData(List<SaveableObjectData> objectsData)
        {

        }

        public override void InitialiseManager()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateManager()
        {
            throw new System.NotImplementedException();
        }
    }
}
