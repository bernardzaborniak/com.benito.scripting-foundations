using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Benito.ScriptingFoundations.NaughtyAttributes;

namespace Benito.ScriptingFoundations.Managers
{
    public abstract class AbstractManagersManager<T> : MonoBehaviour where T : ISingletonManager
    {
        [Tooltip("Automatically asigned to all children in OnValidate")]
        [SerializeField] protected List<T> managers = new List<T>();

        void Start()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].InitialiseManager();
            }
        }

        void Update()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].UpdateManager();
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].LateUpdateManager();
            }
        }


#region Validation 

        protected void OnValidate()
        {
            // Check if the singleton Array contains duplicates.
            Dictionary<Type,T> singletonsAlreadyRegistered = new Dictionary<Type,T>();
            HashSet<T> duplicatesToRemove = new HashSet<T>();

            foreach(T singletonManager in managers)
            {
                if (singletonsAlreadyRegistered.ContainsKey(singletonManager.GetType()) || singletonsAlreadyRegistered.ContainsValue(singletonManager))
                {
                    duplicatesToRemove.Add(singletonManager);
                }
                else
                {
                    singletonsAlreadyRegistered.Add(singletonManager.GetType(), singletonManager);
                }      
            }

            foreach (T item in duplicatesToRemove)
            {
                managers.Remove(item);
            }
        }


#if UNITY_EDITOR
        [Button("Scan Children for Managers")]
        protected void ScanChildrenForManagers()
        {
            UnityEditor.Undo.RecordObject(this, "Scan Children for managers");

            managers.Clear();
            foreach (ISingletonManager manager in GetComponentsInChildren<ISingletonManager>())
            {
                try
                {
                    managers.Add((T)manager);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Could not cast the child {manager} to type {typeof(T)}", (manager as Component).gameObject);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif


        #endregion
    }
}
