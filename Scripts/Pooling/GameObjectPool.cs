using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Pools
{
    [System.Serializable]
    public class GameObjectPool : AbstractPool<GameObject>
    {
        [Space]
        [SerializeField] GameObject prefab;

        Transform spawnParent;

        public void Initialize(Transform spawnParent)
        {
            this.spawnParent = spawnParent;
            base.Initialize();   
        }

        protected override GameObject CreatePoolObject()
        {
            return GameObject.Instantiate(prefab, spawnParent);
        }

        protected override void DestroyPoolObject(GameObject objectToDestroy)
        {
            GameObject.Destroy(objectToDestroy.gameObject);
        }     
    }
}
