using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;
using Benito.ScriptingFoundations.Optimisation;
using System;
using Debug = UnityEngine.Debug;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Manages all saveable objects inside a scene.
    /// You Dont need to call any Methods from this class, just place it in the scene and call GlobalSavesManager.Sace/Load
    /// </summary>
    public class SaveableObjectsSceneManager : SingletonManagerLocalScene
    {
        [SerializeField] List<SaveableSceneObject> saveableObjects;
        [SerializeField] int[] saveableObjectIds;

        public Action OnLoadingFinished;
        public Action<List<SaveableSceneObjectData>> OnSavingFinished;

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
            List<SaveableSceneObject> saveableObjects;
            int[] saveableObjectIds;
            List<SaveableSceneObjectData> objectsData;
            int creatingDictionaryStoppedAtIndex;
            int lastStoppedIndex;


            Dictionary<int, SaveableSceneObject> saveableObjectsIdDictionary;

            public LoadingSceneSaveBudgetedOperation(List<SaveableSceneObject> saveableObjects, int[] saveableObjectIds, List<SaveableSceneObjectData> objectsData , float timeBudget)
            {
                this.saveableObjects = saveableObjects;
                this.saveableObjectIds = saveableObjectIds;
                this.objectsData = objectsData;
                this.TimeBudget = timeBudget;

                creatingDictionaryStoppedAtIndex = 0;
                lastStoppedIndex = 0;
                stage = Stage.CreatingDictionary;

                saveableObjectsIdDictionary = new Dictionary<int, SaveableSceneObject>();
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
                    for (int i = lastStoppedIndex; i < objectsData.Count; i++)
                    {
                        saveableObjectsIdDictionary[objectsData[i].saveableObjectID].Load(objectsData[i]);
                        
                        if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                        {
                            lastStoppedIndex = i+1;
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

            List<SaveableSceneObject> saveableObjects;
            List<SaveableSceneObjectData> objectsData;
            public Action<List<SaveableSceneObjectData>> OnSavingFinished;
            int lastStoppedIndex;

            public SavingSceneSaveBudgetedOperation(List<SaveableSceneObject> saveableObjects, float timeBudget)
            {
                this.saveableObjects = saveableObjects;
                this.TimeBudget = timeBudget;
                lastStoppedIndex = 0;
                Finished = false;
                objectsData = new List<SaveableSceneObjectData>();

            }

            public void Update(float deltaTime)
            {
                float startUpdateTime = Time.realtimeSinceStartup;

                for (int i = lastStoppedIndex; i < saveableObjects.Count; i++)
                {
                    SaveableSceneObjectData data = saveableObjects[i].Save();
                    if (data != null)
                    {
                        objectsData.Add(data);
                    }

                    if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                    {
                        Debug.Log("stopped budgeted save operation after : " + ((Time.realtimeSinceStartup - startUpdateTime) / 1000) + " ms");
                        lastStoppedIndex = i+1;
                        Progress = (1f * i) / (1f * saveableObjects.Count);
                        return;
                    }
                }
                Progress = 1;
                Finished = true;
                Debug.Log("invoke on saving fisnished");
                OnSavingFinished?.Invoke(objectsData);
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
            saveableObjects = new List<SaveableSceneObject>(FindObjectsOfType<SaveableSceneObject>(true));

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
            ManagerState = State.SavingSceneSave;
            savingSceneOperation = new SavingSceneSaveBudgetedOperation(saveableObjects, SavingSettings.GetOrCreateSettings().savingSceneSaveMsBudgetPerFrame /1000);
            savingSceneOperation.OnSavingFinished += OnSavingOperationFinished;
        }

        void OnSavingOperationFinished(List<SaveableSceneObjectData> objectsData)
        {
            savingSceneOperation.OnSavingFinished -= OnSavingOperationFinished;
            OnSavingFinished?.Invoke(objectsData);
        }

       

        public void LoadFromSaveData(List<SaveableSceneObjectData> objectsData)
        {
            ManagerState = State.LoadingSceneSave;

            loadingSceneOperation = new LoadingSceneSaveBudgetedOperation(saveableObjects, saveableObjectIds, objectsData, SavingSettings.GetOrCreateSettings().loadingSceneSaveMsBudgetPerFrame/1000);
        }
    }
}
