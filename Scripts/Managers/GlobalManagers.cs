using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Managers
{
    [DefaultExecutionOrder(-6)]
    [AddComponentMenu("Benitos Scripting Foundations/GlobalManagers")]
    public class GlobalManagers : AbstractManagersManager<SingletonManagerGlobal>
    {
        public static GlobalManagers Instance;

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

            for (int i = 0; i < Instance.managers.Count; i++)
            {
                Instance.singletonDictionary.Add(managers[i].GetType(), managers[i]);
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
    }
}
