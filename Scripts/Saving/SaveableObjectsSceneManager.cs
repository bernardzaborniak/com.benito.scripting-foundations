using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;
using System.IO;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerScene
    {
       [SerializeField] List<SaveableObject> saveableObjects;
       [SerializeField] int[] saveableObjectIds;

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
            SaveableObjectsIdAssigner.AssignIdsInCurrentScene();
        }
        
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

            TempCreateSaveFile(objectsData);
        }

        public void TempCreateSaveFile(List<SaveableObjectData> objectsToSave)
        {
            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsToSave);

            string contents = save.GetJsonString();

            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);
        }

        [Button("Load")]

        public void TempLoadSaveFile()
        {
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
        }

       

        public override void InitialiseManager()
        {

        }

        public override void UpdateManager()
        {

        }
    }
}
