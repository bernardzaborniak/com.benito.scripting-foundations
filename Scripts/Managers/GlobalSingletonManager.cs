using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benitos.ScriptingFoundations.Managers
{
    public class GlobalSingletonManager : AbstractSingletonManager
    {
        public static GlobalSingletonManager Instance;

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

            for (int i = 0; i < Instance.singletons.Count; i++)
            {
                Instance.singletonDictionary.Add(singletons[i].GetType(), singletons[i]);
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
    }
}
