using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Pools
{
    // TODO make similar to gameObject pool afte GO Pool was tested
    public class GenericObjectPool<T> where T : IPooleable, new()
    {
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


        Queue<T> unusedObjectQueue;
        HashSet<T> usedObjects;

        //public Action<T> OnGetObjectFromPool;
        //public Action<T> OnAddOrReturnObjectToPool;

        public void Initialize()
        {
            unusedObjectQueue = new Queue<T>();
            usedObjects = new HashSet<T>();

            if (prefill)
            {
                for (int i = 0; i < prefillSize; i++)
                {
                    T newObject = new T();
                    unusedObjectQueue.Enqueue(newObject);
                    (newObject as IPooleable).OnAddToPool();
                    //OnAddOrReturnObjectToPool(newObject);
                }
            }

            UpdatePoolStats();
        }

        void UpdatePoolStats()
        {
            currentSize = unusedObjectQueue.Count + unusedObjectQueue.Count;
            percentageUsed = currentSize > 0 ? unusedObjectQueue.Count / currentSize : -1;
        }

        public T Get()
        {
            if (unusedObjectQueue.Count == 0)
            {
                if (expandSize)
                {
                    ExpandPoolSize();
                }
                else
                {
                    Debug.LogError($"Could not retrieve Object of type {typeof(T)} from Pool - All objects are in Use or Pool size is 0");
                }
            }

            T dequeuedObject = unusedObjectQueue.Dequeue();
            UpdatePoolStats();
            usedObjects.Add(dequeuedObject);
            (dequeuedObject as IPooleable).OnGetFromPool();
            // OnGetObjectFromPool(dequeuedObject);
            return dequeuedObject;
        }

        void ExpandPoolSize()
        {
            int sizeToExpand = Mathf.Min(expandSizeInterval, maxExpandedSize - currentSize);
            currentSize += sizeToExpand;

            for (int i = 0; i < sizeToExpand; i++)
            {
                T newObject = new T();
                unusedObjectQueue.Enqueue(newObject);
                (newObject as IPooleable).OnAddToPool();
                // OnAddOrReturnObjectToPool(newObject);
            }
        }

        void ReducePoolSize()
        {
            int sizeToReduce = Mathf.Min(expandSizeInterval, currentSize - defaultSize);
            currentSize -= sizeToReduce;

            for (int i = 0; i < sizeToReduce; i++)
            {
                unusedObjectQueue.Dequeue();
            }
        }



        public void Return(T objectToReturn)
        {
            if (!usedObjects.Contains(objectToReturn))
            {
                Debug.LogError($"Object of Type {typeof(T)} is not part of the pool you are trying to return it to");
                return;
            }

            usedObjects.Remove(objectToReturn);
            unusedObjectQueue.Enqueue(objectToReturn);
            (objectToReturn as IPooleable).OnAddToPool();

            //OnAddOrReturnObjectToPool(objectToReturn);

            UpdatePoolStats();

            if (expandSize && percentageUsed < reduceSizeThreshold && currentSize > defaultSize)
            {
                ReducePoolSize();
                UpdatePoolStats();
            }
        }
    }
}
