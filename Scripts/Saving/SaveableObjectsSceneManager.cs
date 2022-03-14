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

        public float loadingSceneSaveBudgetPerFrame = 0.083f;

        public class LoadingSceneSaveBudgetedOperation
        {
            public enum Stage
            {
                CreatingDictionary,
                CallingLoadMethod,
                Finished
            }

            public bool Finished { get => stage == Stage.Finished; }

            public Stage stage;
            List<SaveableObject> saveableObjects;
            int[] saveableObjectIds;
            List<SaveableObjectData> objectsData;
            int creatingDictionaryStoppedAtIndex;
            int callingLoadMethodStoppedAtIndex;

            float timeBudget;

            Dictionary<int, SaveableObject> saveableObjectsIdDictionary;

            public LoadingSceneSaveBudgetedOperation(List<SaveableObject> saveableObjects, int[] saveableObjectIds, List<SaveableObjectData> objectsData , float timeBudget)
            {
                this.saveableObjects = saveableObjects;
                this.saveableObjectIds = saveableObjectIds;
                this.objectsData = objectsData;
                this.timeBudget = timeBudget;

                creatingDictionaryStoppedAtIndex = 0;
                callingLoadMethodStoppedAtIndex = 0;
                stage = Stage.CreatingDictionary;

                saveableObjectsIdDictionary = new Dictionary<int, SaveableObject>();
            }

            public void Update(float deltaTime)
            {
                float startUpdateTime = Time.realtimeSinceStartup;

                if(stage == Stage.CreatingDictionary)
                {
                    for (int i = creatingDictionaryStoppedAtIndex; i < saveableObjects.Count; i++)
                    {
                        saveableObjectsIdDictionary.Add(saveableObjectIds[i], saveableObjects[i]);
                        
                        if (Time.realtimeSinceStartup - startUpdateTime > timeBudget)
                        {
                            creatingDictionaryStoppedAtIndex = i;
                            //Debug.Log("reached  time budget");
                            return;
                        }
                    }

                    stage = Stage.CallingLoadMethod;
                }
                else if(stage == Stage.CallingLoadMethod)
                {
                    for (int i = callingLoadMethodStoppedAtIndex; i < objectsData.Count; i++)
                    {
                        saveableObjectsIdDictionary[objectsData[i].saveableObjectID].Load(objectsData[i]);
                        
                        if (Time.realtimeSinceStartup - startUpdateTime > timeBudget)
                        {
                            callingLoadMethodStoppedAtIndex = i;
                            //Debug.Log("reached  time budget");
                            return;
                        }
                    }
                    stage = Stage.Finished;
                }
            }

        }

        LoadingSceneSaveBudgetedOperation loadingSceneOperation;



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
            LoadFromSaveData(save.GetSavedObjectsFromSave());

            //stopwatch.Stop();
            //UnityEngine.Debug.Log("coroutine LoadFromSaveData took " + stopwatch.Elapsed.TotalSeconds + " s");

        }

        public void LoadFromSaveData(List<SaveableObjectData> objectsData)
        {
            state = State.LoadingSceneSave;
            loadingSceneOperation = new LoadingSceneSaveBudgetedOperation(saveableObjects, saveableObjectIds, objectsData, loadingSceneSaveBudgetPerFrame);
        }


        public override void InitialiseManager()
        {

        }

        public override void UpdateManager()
        {
            if(state == State.LoadingSceneSave)
            {
                loadingSceneOperation.Update(Time.deltaTime);

                if (loadingSceneOperation.Finished)
                {
                    state = State.Idle;
                    loadingSceneOperation = null;
                    OnLoadingFinished?.Invoke();
                }
            }
        }
    }
}
