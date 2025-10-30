using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Benito.ScriptingFoundations.NaughtyAttributes;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    [DefaultExecutionOrder(-4)]
    [AddComponentMenu("Benitos Scripting Foundations/Initializers/SceneInitializersManager")]
    public class SceneInitializersManager : MonoBehaviour
    {
        public static SceneInitializersManager Instance;

        [SerializeField] protected List<AbstractSceneInitializer> initializers = new List<AbstractSceneInitializer>();

        // Dictionary used to simplify the get by type process internaly.
        protected Dictionary<Type, object> singletonDictionary = new Dictionary<Type, object>();


        // ----- Execution Variables -------

        [Tooltip("Initializers are only allowed to use a maximum of this amount of miliseconds per frame " +
       "- to prevent stuttering in the loading bar. values around 5-10 seem appropriate")]
        [SerializeField] float initializersBudgetInMs = 7;


        public bool IsFinished { get; private set; }
        public Action OnFinished;
        /// <summary>
        /// Says what the initializers are currently doing
        /// </summary>.
        public string ProgressString { get; private set; }
        /// <summary>
        /// Value between 0 and 1, gives back the initialization progress.
        /// </summary>
        public float Progress { get; private set; }

        int currentInitializerIndex;

        // -----------------------------


        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
            else
            {
                Instance = this;
            }

            for (int i = 0; i < Instance.initializers.Count; i++)
            {
                Instance.singletonDictionary.Add(initializers[i].GetType(), initializers[i]);
            }
        }

        void Start()
        {
            if(initializers.Count == 0)
            {
                IsFinished = true;
                return;
            }

            currentInitializerIndex = 0;
            IsFinished = false;

            initializers[currentInitializerIndex].StartInitialization();
        }

        private void Update()
        {
            if (IsFinished)
                return;

            initializers[currentInitializerIndex].UpdateInitialization(initializersBudgetInMs);

            Progress = (float)currentInitializerIndex / initializers.Count + initializers[currentInitializerIndex].Progress;
            ProgressString = initializers[currentInitializerIndex].ProgressString;

            if (initializers[currentInitializerIndex].InitializationFinished)
            {
                if(currentInitializerIndex == initializers.Count-1)
                {
                    IsFinished = true;
                    OnFinished?.Invoke();
                }
                else
                {
                    currentInitializerIndex++;
                    initializers[currentInitializerIndex].StartInitialization();
                }
            }
        }

        public static T Get<T>()
        {
            if (!Instance.singletonDictionary.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Type: {typeof(T)} could not be found in the GlobalSingletonManager");
                return default(T);
            }

            return (T)Instance.singletonDictionary[typeof(T)];
        }

        public static bool HasInstance()
        {
            return Instance != null;
        }

        #region Validation 

        protected void OnValidate()
        {
            // Check if the singleton Array contains duplicates.
            Dictionary<Type, AbstractSceneInitializer> singletonsAlreadyRegistered = new Dictionary<Type, AbstractSceneInitializer>();
            HashSet<AbstractSceneInitializer> duplicatesToRemove = new HashSet<AbstractSceneInitializer>();

            foreach (AbstractSceneInitializer initializer in initializers)
            {
                if (singletonsAlreadyRegistered.ContainsKey(initializer.GetType()) || singletonsAlreadyRegistered.ContainsValue(initializer))
                {
                    duplicatesToRemove.Add(initializer);
                }
                else
                {
                    singletonsAlreadyRegistered.Add(initializer.GetType(), initializer);
                }
            }

            foreach (AbstractSceneInitializer item in duplicatesToRemove)
            {
                initializers.Remove(item);
            }
        }


#if UNITY_EDITOR
        [Button("Scan Children for Managers")]
        protected void ScanChildrenForManagers()
        {
            UnityEditor.Undo.RecordObject(this, "Scan Children for managers");

            initializers.Clear();
            foreach (AbstractSceneInitializer manager in GetComponentsInChildren<AbstractSceneInitializer>())
            {
                try
                {
                    initializers.Add((AbstractSceneInitializer)manager);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Could not cast the child {manager} to type {typeof(AbstractSceneInitializer)}", (manager as Component).gameObject);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
        #endregion
    }
}
