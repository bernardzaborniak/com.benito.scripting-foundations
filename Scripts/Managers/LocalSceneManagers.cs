using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Managers
{
    /// <summary>
    /// Local Scene Mangaers Locator, not using Locator in the name to shorten writing a bit
    /// </summary>
    [DefaultExecutionOrder(-5)]
    [AddComponentMenu("Benitos Scripting Foundations/LocalSceneManagers")]
    public class LocalSceneManagers : AbstractManagersManager<SingletonManagerLocalScene>
    {
        public static LocalSceneManagers Instance;

        protected Dictionary<Type, object> managersDictionary = new Dictionary<Type, object>();

        protected override void Awake()
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
                Instance.managersDictionary.Add(managers[i].GetType(), managers[i]);
            }

            InitializeManagers();
        }

        public static T Get<T>()
        {
            if (!Instance.managersDictionary.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Type: {typeof(T)} could not be found in the SceneSingletonManager");
                return default(T);
            }

            return (T)Instance.managersDictionary[typeof(T)];
        }

        public static bool HasInstance()
        {
            return Instance != null;
        }
    }
}
