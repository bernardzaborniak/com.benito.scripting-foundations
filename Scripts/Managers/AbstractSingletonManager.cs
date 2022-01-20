using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benitos.ScriptingFoundations.Managers
{
    public abstract class AbstractSingletonManager : MonoBehaviour
    {
        [SerializeField] protected List<Singleton> singletons = new List<Singleton>();

        void Start()
        {
            for (int i = 0; i < singletons.Count; i++)
            {
                singletons[i].InitialiseSingleton();
            }
        }

        void Update()
        {
            for (int i = 0; i < singletons.Count; i++)
            {
                singletons[i].UpdateSingleton();
            }
        }

        #region Validation 

        void OnValidate()
        {
            // Check if the singleton Array contains duplicates.
            Dictionary<Type,Singleton> singletonsAlreadyRegistered = new Dictionary<Type,Singleton>();
            HashSet<Singleton> duplicatesToRemove = new HashSet<Singleton>();

            foreach(Singleton singleton in singletons)
            {
                if (singletonsAlreadyRegistered.ContainsKey(singleton.GetType()) || singletonsAlreadyRegistered.ContainsValue(singleton))
                {
                    duplicatesToRemove.Add(singleton);
                }
                else
                {
                    singletonsAlreadyRegistered.Add(singleton.GetType(), singleton);
                }      
            }

            foreach (Singleton item in duplicatesToRemove)
            {
                singletons.Remove(item);
            }

        }

        #endregion
    }
}
