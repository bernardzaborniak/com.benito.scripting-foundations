using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class GameObjectUtilities
    {
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
