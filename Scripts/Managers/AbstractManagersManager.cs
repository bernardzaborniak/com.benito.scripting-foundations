using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Managers
{
    public abstract class AbstractManagersManager<T> : MonoBehaviour where T : ISingletonManager
    {
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

        #region Validation 

        void OnValidate()
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

        #endregion
    }
}
