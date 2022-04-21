using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;
using Benito.ScriptingFoundations.Optimisation;
using System.IO;
using System;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// You Dont need to call any Methods from this class, just place it in the scene and call GlobalSavesManager.Sace/Load
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerLocalScene
    {
        [SerializeField] List<SaveableObject> saveableObjects;
        [SerializeField] int[] saveableObjectIds;

        public Action OnLoadingFinished;
        public Action<List<SaveableObjectData>> OnSavingFinished;

        public enum State
        {
            Idle,
            LoadingSceneSave,
            SavingSceneSave
        }

        public State ManagerState { get; private set; }

        public float LoadingProgress { get => loadingSceneOperation.Progress; }
        public float SavingProgress { get => savingSceneOperation.Progress; }

        #region Budgeted Operations 

        public class LoadingSceneSaveBudgetedOperation : IBudgetedOperation
        {
            public enum Stage
            {
                CreatingDictionary,
                CallingLoadMethod,
                Finished
            }

            public bool Finished { get => stage == Stage.Finished; }
            public float Progress { get; private set; }
            public float TimeBudget { get; private set; }


            public Stage stage;
            List<SaveableObject> saveableObjects;
            int[] saveableObjectIds;
            List<SaveableObjectData> objectsData;
            int creatingDictionaryStoppedAtIndex;
            int callingLoadMethodStoppedAtIndex;


            Dictionary<int, SaveableObject> saveableObjectsIdDictionary;

            public LoadingSceneSaveBudgetedOperation(List<SaveableObject> saveableObjects, int[] saveableObjectIds, List<SaveableObjectData> objectsData , float timeBudget)
            {
                this.saveableObjects = saveableObjects;
                this.saveableObjectIds = saveableObjectIds;
                this.objectsData = objectsData;
                this.TimeBudget = timeBudget;

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
                        
                        if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                        {
                            creatingDictionaryStoppedAtIndex = i;
                            Progress = (1f*i) / (1f * saveableObjects.Count + objectsData.Count);
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
                        
                        if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                        {
                            callingLoadMethodStoppedAtIndex = i;
                            Progress = (1f * saveableObjects.Count + i) / (1f * saveableObjects.Count + objectsData.Count);
                            return;
                        }
                    }
                    stage = Stage.Finished;
                }

                Progress = 1;
            }

        }

        public class SavingSceneSaveBudgetedOperation : IBudgetedOperation
        {
            public bool Finished  {get; private set;}
            public float Progress { get; private set; }
            public float TimeBudget { get; private set; }


            List<SaveableObject> saveableObjects;

            public Action<List<SaveableObjectData>> OnSavingFinished;


            int lastStoppedIndex;

            public SavingSceneSaveBudgetedOperation(List<SaveableObject> saveableObjects, float timeBudget)
            {
                this.saveableObjects = saveableObjects;
                this.TimeBudget = timeBudget;
                lastStoppedIndex = 0;
                Finished = false;
            }

            public void Update(float deltaTime)
            {
                float startUpdateTime = Time.realtimeSinceStartup;

                List<SaveableObjectData> objectsData = new List<SaveableObjectData>();

                for (int i = lastStoppedIndex; i < saveableObjects.Count; i++)
                {
                    SaveableObjectData data = saveableObjects[i].Save();
                    if (data != null)
                    {
                        objectsData.Add(data);
                    }

                    if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                    {
                        lastStoppedIndex = i;
                        Progress = (1f * i) / (1f * saveableObjects.Count);
                        return;
                    }
                }
                Progress = 1;
                Finished = true;
                Debug.Log("invoke on saving fisnished");
                OnSavingFinished?.Invoke(objectsData);

                //GlobalManagers.Get<GlobalSavesManager>().CreateSceneSave(objectsData);
            }
        }

        #endregion

        LoadingSceneSaveBudgetedOperation loadingSceneOperation;
        SavingSceneSaveBudgetedOperation savingSceneOperation;


        public override void InitialiseManager()
        {

        }

        public override void UpdateManager()
        {
            if (ManagerState == State.LoadingSceneSave)
            {
                loadingSceneOperation.Update(Time.deltaTime);

                if (loadingSceneOperation.Finished)
                {
                    ManagerState = State.Idle;
                    loadingSceneOperation = null;
                    OnLoadingFinished?.Invoke();
                }
            }
            else if(ManagerState == State.SavingSceneSave)
            {
                savingSceneOperation.Update(Time.deltaTime);

                if (savingSceneOperation.Finished)
                {
                    ManagerState = State.Idle;
                    savingSceneOperation = null;
                }
            }
        }

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
#endif
        /*[Button("Save")]
        public void TempCallSave()
        {
            GlobalManagers.Get<GlobalSavesManager>().CreateSceneSaveForCurrentScene("", "test");
        }*/

        /*[Button("Load")]
       public async void LoadSaveFileWithoutSceneTransition()
       {
           SceneSavegame save = await GlobalManagers.Get<GlobalSavesManager>().ReadSceneSaveFileAsync("test");
           LoadFromSaveData(save.SavedObjects);
       }*/

        public void SaveAllObjects()
        {
            Debug.Log("SaveableObjectsSceneManager.SaveAllObjects called");
            ManagerState = State.SavingSceneSave;
            savingSceneOperation = new SavingSceneSaveBudgetedOperation(saveableObjects, SavingSettings.GetOrCreateSettings().savingSceneSaveBudgetPerFrame);
            savingSceneOperation.OnSavingFinished += OnSavingOperationFinished;
        }

        void OnSavingOperationFinished(List<SaveableObjectData> objectsData)
        {
            Debug.Log("SaveableObjectsSceneManager.OnSavingOperationFinished called, data length: " + objectsData.Count);

            OnSavingFinished?.Invoke(objectsData);
        }

       

        public void LoadFromSaveData(List<SaveableObjectData> objectsData)
        {
            ManagerState = State.LoadingSceneSave;

            loadingSceneOperation = new LoadingSceneSaveBudgetedOperation(saveableObjects, saveableObjectIds, objectsData, SavingSettings.GetOrCreateSettings().loadingSceneSaveBudgetPerFrame);
        }
    }
}
