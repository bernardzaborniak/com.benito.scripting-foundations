using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.NaughtyAttributes;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    [DefaultExecutionOrder(-4)]
    public class SceneInitializers : MonoBehaviour
    {
        public static SceneInitializers Instance;

        [SerializeField] protected List<AbstractSceneInitializer> initializers = new List<AbstractSceneInitializer>();


        // Dictionary used to simplify the get by type process internaly.
        protected Dictionary<Type, object> singletonDictionary = new Dictionary<Type, object>();

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
            for (int i = 0; i < initializers.Count; i++)
            {
                initializers[i].Initialize();
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
