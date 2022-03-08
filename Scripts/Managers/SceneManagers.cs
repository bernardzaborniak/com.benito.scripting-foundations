using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Managers
{
    public class SceneManagers : AbstractManagersManager<SingletonManagerScene>
    {
        public static SceneManagers Instance;

        protected Dictionary<Type, object> managersDictionary = new Dictionary<Type, object>();

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
                Instance.managersDictionary.Add(managers[i].GetType(), managers[i]);
            }
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
    }
}
