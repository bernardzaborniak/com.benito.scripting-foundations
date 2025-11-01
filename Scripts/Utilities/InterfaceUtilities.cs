using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class InterfaceUtilities
    {
        public static List<T> FindInterfacesInScene<T>()
        {
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                T rootInterface = rootGameObject.GetComponent<T>();
                if (rootInterface != null) interfaces.Add(rootInterface);


                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }

            return interfaces;
        }
    }
}