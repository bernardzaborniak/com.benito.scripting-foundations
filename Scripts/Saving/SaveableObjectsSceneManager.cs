using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;
using System.IO;
using System;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerLocalScene
    {
       [SerializeField] List<SaveableObject> saveableObjects;
       [SerializeField] int[] saveableObjectIds;

        public Action OnLoadingFinished;
        public Action OnSavingFinished;

#if UNITY_EDITOR
        [Button("ScanSceneForSaveableObjects")]
        public void ScanSceneForSaveableObjects()
        {
            saveableObjects = new List<SaveableObject>(FindObjectsOfType<SaveableObject>(true));

            saveableObjectIds = new int[saveableObjects.Count];
            for (int i = 0; i < saveableObjects.Count; i++)
            {
                saveableObjectIds[i] = saveableObjects[i].GetId();
            }
        }

        [Button("Assign IDS")]
        public void AssignIds()
        {
            SaveableObjectsIdAssigner.AssignMissingIdsInCurrentScene();
        }

#endif

        [Button("Save")]
        public void SaveAllObjects()
        {
            List<SaveableObjectData> objectsData = new List<SaveableObjectData>();

            for (int i = 0; i < saveableObjects.Count; i++)
            {
                SaveableObjectData data = saveableObjects[i].Save();
                if(data != null)
                {
                    objectsData.Add(data);
                }
            }

            GlobalManagers.Get<GlobalSavesManager>().CreateSceneSave(objectsData);
            //TempCreateSaveFile(objectsData);

            OnSavingFinished?.Invoke();
        }

        /*public void TempCreateSaveFile(List<SaveableObjectData> objectsToSave)
        {
            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsToSave);

            string contents = save.GetJsonString();

            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);
        }*/

        [Button("Load")]
        public void TempLoadSaveFile()
        {
            Debug.Log("path: " + Path.Combine(Application.persistentDataPath, "Saves/test.json"));
            StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, "Saves/test.json"));
            string fileContent = reader.ReadToEnd();
            reader.Close();

            SceneSavegame save = SceneSavegame.CreateFromJsonString(fileContent);

            LoadFromSaveData(save.GetSavedObjectsFromSave());
        }

        public void LoadFromSaveData(List<SaveableObjectData> objectsData)
        {
            Dictionary<int, SaveableObject> saveableObjectsIdDictionary = new Dictionary<int, SaveableObject>();

            for (int i = 0; i < saveableObjects.Count; i++)
            {
                saveableObjectsIdDictionary.Add(saveableObjectIds[i],saveableObjects[i]);
            }

            foreach (SaveableObjectData data in objectsData)
            {
                Debug.Log("loaded data type: " + data.GetType());
                saveableObjectsIdDictionary[data.saveableObjectID].Load(data);
            }

            Debug.Log("try to call on loading finished");
            Debug.Log("OnLoadingFinished: " + OnLoadingFinished);
            OnLoadingFinished?.Invoke();
        }

       

        public override void InitialiseManager()
        {

        }

        public override void UpdateManager()
        {

        }
    }
}
