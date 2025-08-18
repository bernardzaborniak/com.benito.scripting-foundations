using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.NaughtyAttributes;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    [DefaultExecutionOrder(-4)]
    public class EnterPlayModeSceneInitializers : MonoBehaviour
    {
        public static EnterPlayModeSceneInitializers Instance;
        [SerializeField] protected List<AbstractEnterPlayModeSceneInitializer> initializers = new List<AbstractEnterPlayModeSceneInitializer>();


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
        }

        public void OnEnteredPlayModeViaEditor()
        {
            for (int i = 0; i < initializers.Count; i++)
            {
                initializers[i].OnEnteredPlayModeViaEditor();
            }
        }

        public static bool HasInstance()
        {
            return Instance != null;
        }

        #region Validation 

        protected void OnValidate()
        {
            // Check if the singleton Array contains duplicates.
            Dictionary<Type, AbstractEnterPlayModeSceneInitializer> singletonsAlreadyRegistered = new Dictionary<Type, AbstractEnterPlayModeSceneInitializer>();
            HashSet<AbstractEnterPlayModeSceneInitializer> duplicatesToRemove = new HashSet<AbstractEnterPlayModeSceneInitializer>();

            foreach (AbstractEnterPlayModeSceneInitializer initializer in initializers)
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

            foreach (AbstractEnterPlayModeSceneInitializer item in duplicatesToRemove)
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
            foreach (AbstractEnterPlayModeSceneInitializer manager in GetComponentsInChildren<AbstractEnterPlayModeSceneInitializer>())
            {
                try
                {
                    initializers.Add((AbstractEnterPlayModeSceneInitializer)manager);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Could not cast the child {manager} to type {typeof(AbstractEnterPlayModeSceneInitializer)}", (manager as Component).gameObject);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
        #endregion
    }
}
