using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;
using System.IO;
using System;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

using System.Diagnostics;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerLocalScene
    {
        Stopwatch stopwatch = new Stopwatch();


        [SerializeField] List<SaveableObject> saveableObjects;
        [SerializeField] int[] saveableObjectIds;

        public Action OnLoadingFinished;
        public Action OnSavingFinished;

        public enum State
        {
            Idle,
            LoadingSceneSave,
            Saving
        }

        [SerializeField]
        State state;

        /*public enum LoadingState
        {
            ReadingFile,
            InterpretingFile,
            LoadingInScene
        }

        [SerializeField]
        LoadingState loadingState;*/


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
            stopwatch.Start();

            List<SaveableObjectData> objectsData = new List<SaveableObjectData>();

            for (int i = 0; i < saveableObjects.Count; i++)
            {
                SaveableObjectData data = saveableObjects[i].Save();
                if(data != null)
                {
                    objectsData.Add(data);
                }
            }

            stopwatch.Stop();
            Debug.Log("stopwatch SaveableObjectsManager.SaveAllObjects took " + stopwatch.Elapsed.TotalSeconds + " s");

            stopwatch.Stop();

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
        public async void TempLoadSaveFile()
        {
            stopwatch.Start();

            /*StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, "Saves/test.json"));
            string fileContent = reader.ReadToEnd();
            reader.Close();

            stopwatch.Stop();
            UnityEngine.Debug.Log("stopwatch SaveableObjectsManager.TempLoadSaveFile reader took " + stopwatch.Elapsed.TotalSeconds + " s");

            stopwatch.Reset();
            stopwatch.Start();
            System.Threading.Tasks.Task task = SceneSavegame.CreateFromJsonString(fileContent);
            //task.*/
            SceneSavegame save = await GlobalManagers.Get<GlobalSavesManager>().ReadSceneSaveFile(Path.Combine(Application.persistentDataPath, "Saves/test.json"));

            stopwatch.Stop();
            UnityEngine.Debug.Log("async ReadSceneSaveFile took " + stopwatch.Elapsed.TotalSeconds + " s");
            //stopwatch.Reset();
            //stopwatch.Start();
            stopwatch.Reset();

            stopwatch.Start();
            StartCoroutine(LoadFromSaveData(save.GetSavedObjectsFromSave()));
            
            //stopwatch.Stop();
            //UnityEngine.Debug.Log("coroutine LoadFromSaveData took " + stopwatch.Elapsed.TotalSeconds + " s");

        }

        /*void OnCreatingSceneSaveFromFileFinished()
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log("stopwatch SceneSavegame.CreateFromJsonString reader took " + stopwatch.Elapsed.TotalSeconds + " s");
            stopwatch.Reset();
            stopwatch.Start();
            LoadFromSaveData(save.GetSavedObjectsFromSave());
            stopwatch.Stop();
            UnityEngine.Debug.Log("stopwatch LoadFromSaveData took " + stopwatch.Elapsed.TotalSeconds + " s");
        }*/

        // Do this one as an coroutine with progress , as maybe the load function will not be thread safe
        public IEnumerator LoadFromSaveData(List<SaveableObjectData> objectsData)
        {
            float startTime = Time.time;
            float maxTime = 0.008333f;
            state = State.LoadingSceneSave;

            // stopwatch.Start();
            Dictionary<int, SaveableObject> saveableObjectsIdDictionary = new Dictionary<int, SaveableObject>();

            for (int i = 0; i < saveableObjects.Count; i++)
            {
                saveableObjectsIdDictionary.Add(saveableObjectIds[i],saveableObjects[i]);
                
                if (Time.time - startTime > maxTime)
                {
                    Debug.Log("yield return");
                    yield return null;
                }
            }

            foreach (SaveableObjectData data in objectsData)
            {
                saveableObjectsIdDictionary[data.saveableObjectID].Load(data);
                Debug.Log("Time.time - startTime " + (Time.time - startTime) + "maxTime" + maxTime);
                if (Time.time - startTime > maxTime)
                {
                    Debug.Log("yield return");
                    yield return null;
                }
                   
            }

           // stopwatch.Stop();
           // UnityEngine.Debug.Log("stopwatch SaveableObjectsManager.LoadFromSaveData took " + stopwatch.Elapsed.TotalSeconds + " s");

            OnLoadingFinished?.Invoke();
            stopwatch.Stop();
            UnityEngine.Debug.Log("coroutine took  " + stopwatch.Elapsed.TotalSeconds + " s");
            state = State.Idle;
            yield return null;
        }

       
 
        public override void InitialiseManager()
        {

        }

        public override void UpdateManager()
        {

        }
    }
}
