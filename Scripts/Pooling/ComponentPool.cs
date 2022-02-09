using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Pools
{
    [System.Serializable]
    public class ComponentPool<T> : AbstractPool<T> where T:MonoBehaviour
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

        protected override T CreatePoolObject()
        {
            return GameObject.Instantiate(prefabWithComponent, spawnParent).GetComponent<T>(); ;
        }

        protected override void DestroyPoolObject(T objectToDestroy)
        {
            GameObject.Destroy(objectToDestroy.gameObject);
        }
    }
}
