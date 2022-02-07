using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Pools
{

    [Serializable]
    public class GameObjectPool
    {
        [Tooltip("Prefab must have 1 IPooleable script at the root")]
        [SerializeField] GameObject prefab;

        [SerializeField] int defaultSize;

        [Tooltip("Should the Pool be prefilled with some instances in awake?")]
        [SerializeField] bool prefill;
        [SerializeField] int prefillSize;

        [SerializeField] bool expandSize;
        [SerializeField] int expandSizeInterval;
        [SerializeField] int maxExpandedSize;

        [Tooltip("Only working if expand Size is true. Controls weather to go back to normal size in expandSizeIntervals when less than 70% of the current size is used")]
        [SerializeField] bool reduceSize;
        [Range(0.1f, 0.9f)]
        [SerializeField] float reduceSizeThreshold = 0.7f;

        //Readonly
        [SerializeField] int currentSize;
        [SerializeField] float percentageUsed;


        Queue<GameObject> unusedObjectQueue;
        HashSet<GameObject> usedObjects;
        Dictionary<GameObject, IPooleable> pooleableCache;

        //public Action<GameObject> OnGetObjectFromPool;
        //public Action<GameObject> OnAddOrReturnObjectToPool;

        public void Initialize()
        {
            unusedObjectQueue = new Queue<GameObject>();
            usedObjects = new HashSet<GameObject>();
            pooleableCache = new Dictionary<GameObject, IPooleable>();

            if (prefill)
            {
                for (int i = 0; i < prefillSize; i++)
                {
                    CreateAndAddObjectToPool();
                    //OnAddOrReturnObjectToPool(newObject);
                }
            }

            UpdatePoolStats();
        }

        void CreateAndAddObjectToPool()
        {
            GameObject newObject = GameObject.Instantiate(prefab);
            unusedObjectQueue.Enqueue(newObject);

            IPooleable pooleable = newObject.GetComponent<IPooleable>();
            pooleableCache.Add(newObject, pooleable);
            pooleable.OnAddToPool();
        }

        void DestroyPoolObject(GameObject go)
        {
            pooleableCache.Remove(go);
            GameObject.Destroy(go);
        }

        void UpdatePoolStats()
        {
            currentSize = unusedObjectQueue.Count + unusedObjectQueue.Count;
            percentageUsed = currentSize > 0 ? unusedObjectQueue.Count / currentSize : -1;
        }

        public GameObject Get()
        {
            if (unusedObjectQueue.Count == 0)
            {
                if (expandSize)
                {
                    ExpandPoolSize();
                }
                else
                {
                    Debug.LogError($"Could not retrieve Object from Pool - All objects are in Use or Pool size is 0");
                }
            }

            GameObject dequeuedObject = unusedObjectQueue.Dequeue();
            usedObjects.Add(dequeuedObject);
            pooleableCache[dequeuedObject].OnGetFromPool();
            UpdatePoolStats();
            // OnGetObjectFromPool(dequeuedObject);
            return dequeuedObject;
        }

        public void Return(GameObject objectToReturn)
        {
            if (!usedObjects.Contains(objectToReturn))
            {
                Debug.LogError($"Object is not part of the pool you are trying to return it to");
                return;
            }

            usedObjects.Remove(objectToReturn);
            unusedObjectQueue.Enqueue(objectToReturn);
            UpdatePoolStats();

            if (expandSize && percentageUsed < reduceSizeThreshold && currentSize > defaultSize)
            {
                ReducePoolSize();
                UpdatePoolStats();
            }
        }

        void ExpandPoolSize()
        {
            int sizeToExpand = Mathf.Min(expandSizeInterval, maxExpandedSize - currentSize);
            currentSize += sizeToExpand;

            for (int i = 0; i < sizeToExpand; i++)
            {
                CreateAndAddObjectToPool();
                //OnAddOrReturnObjectToPool(newObject);
            }
        }

        void ReducePoolSize()
        {
            int sizeToReduce = Mathf.Min(expandSizeInterval, currentSize - defaultSize);
            currentSize -= sizeToReduce;

            for (int i = 0; i < sizeToReduce; i++)
            {
                GameObject removedObject = unusedObjectQueue.Dequeue();
                DestroyPoolObject(removedObject);               
            }
        }

       
    }
}
