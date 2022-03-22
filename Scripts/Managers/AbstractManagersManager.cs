using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Benito.ScriptingFoundations.InspectorAttributes;

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


//#if UNITY_ENGINE
        [Button("Scan Children for Managers")]
        protected void ScanChildrenForManagers()
        {
            managers.Clear();
            foreach (ISingletonManager manager in GetComponentsInChildren<ISingletonManager>())
            {
                managers.Add((T)manager);
            }
        }

//#endif


        #endregion
    }
}
