using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Pools
{
    [System.Serializable]
    public class ComponentPool<T> : AbstractPool<T> where T:Component
    {
        [Space]
        [Tooltip("Make sure the prefab contains the desired Component on its root object")]
        [SerializeField] GameObject prefabWithComponent;

        Transform spawnParent;

        public void Initialize(Transform spawnParent)
        {
            this.spawnParent = spawnParent;
            base.Initialize();
        }
        public void InitializeWithParams(Transform spawnParent, GameObject prefabWithComponent, int defaultSize, bool expandSize = false, int expandSizeInterval = 5, int maxExpandedSize = 50, bool reduceSize = true, float reduceSizeThreshold = 0.7f)
        {
            this.prefabWithComponent = prefabWithComponent;
            this.spawnParent = spawnParent;
            base.InitializeWithParams(defaultSize, expandSize ,  expandSizeInterval, maxExpandedSize, reduceSize, reduceSizeThreshold);
        }

        protected override T CreatePoolObject()
        {
            return GameObject.Instantiate(prefabWithComponent, spawnParent).GetComponent<T>(); ;
        }

        protected override void DestroyPoolObject(T objectToDestroy)
        {
            GameObject.Destroy(objectToDestroy.gameObject);
        }

        public GameObject GetPoolPrefabComponent()
        {
            return prefabWithComponent;
        }
    }
}
