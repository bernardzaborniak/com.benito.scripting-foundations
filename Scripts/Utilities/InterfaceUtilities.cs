using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class InterfaceUtilities
    {
        public static List<T> FindInterfacesInActiveScene<T>()
        {
            return FindInterfacesInScene<T>(SceneManager.GetActiveScene());
        }

        public static List<T> FindInterfacesInScene<T>(Scene scene)
        {
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = scene.GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    Debug.Log($"found interface: {childInterface} with interface id: {childInterface.GetHashCode()}");
                    interfaces.Add(childInterface);
                }
            }

            return interfaces;
        }
    }
}