using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class GameObjectUtilities
    {
        /// <summary>
        ///  Makes sure that the spawned object has the required component and returns it, if the component is not there, the instantiated Prefab gets destroyed again.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="spawnParent"></param>
        /// <returns></returns>
        public static T SpawnPrefabWithComponent<T>(GameObject prefab, Transform spawnParent)
        {
            GameObject spawnedObject = GameObject.Instantiate(prefab, spawnParent);
            T spawnedComponent = spawnedObject.GetComponent<T>();

            if (spawnedComponent == null)
            {
                Debug.LogError($"[GameObjectUtilities] The Prefab you want to instantiate: {prefab} doesn't have the required component: {typeof(T)} - It needs it at its root");
                GameObject.Destroy(spawnedObject);
            }
            return spawnedComponent;
        }
    }
}
