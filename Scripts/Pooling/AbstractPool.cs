using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.Pools
{
    public abstract class AbstractPool<T>
    {
        [SerializeField] protected int defaultSize;
        [Space]
        [SerializeField] protected bool expandSize;
        [SerializeField] protected int expandSizeInterval;
        [SerializeField] protected int maxExpandedSize;
        [Space]
        [Tooltip("Only working if expand Size is true. Controls weather to go back to normal size in expandSizeIntervals when less than 70% of the current size is used")]
        [SerializeField] protected bool reduceSize;
        [Range(0.1f, 0.9f)]
        [SerializeField] protected float reduceSizeThreshold = 0.7f;
        [Space]
        //Readonly
        [SerializeField] protected int currentSize;
        [SerializeField] protected float percentageUsed;

        protected Queue<T> unusedObjectQueue;
        protected HashSet<T> usedObjects;

        public HashSet<T> UsedObjects { get => usedObjects; }

        public Action<T> OnAddObjectToPool;
        public Action<T> OnRemoveObjectFromPool;


        public virtual void Initialize()
        {
            unusedObjectQueue = new Queue<T>();
            usedObjects = new HashSet<T>();


            for (int i = 0; i < defaultSize; i++)
            {
                T obj = CreatePoolObject();
                unusedObjectQueue.Enqueue(obj);
                OnAddObjectToPool?.Invoke(obj);
            }

            UpdatePoolStats();
        }

        public T Get()
        {
            if (unusedObjectQueue.Count == 0)
            {
                if (expandSize && currentSize != maxExpandedSize)
                {
                    ExpandPoolSize();
                }
                else
                {
                    Debug.LogError($"Could not retrieve Object from Pool - All objects are in Use or Pool size is 0");
                }
            }

            T dequeuedObject = unusedObjectQueue.Dequeue();
            OnRemoveObjectFromPool?.Invoke(dequeuedObject);
            usedObjects.Add(dequeuedObject);
            UpdatePoolStats();
            return dequeuedObject;
        }

        public void Return(T objectToReturn)
        {
            if (!usedObjects.Contains(objectToReturn))
            {
                Debug.LogError($"Object is not part of the pool you are trying to return it to");
                return;
            }

            usedObjects.Remove(objectToReturn);
            OnAddObjectToPool?.Invoke(objectToReturn);
            unusedObjectQueue.Enqueue(objectToReturn);
            UpdatePoolStats();

            if (expandSize && percentageUsed < reduceSizeThreshold && currentSize > defaultSize)
            {
                ReducePoolSize();
                UpdatePoolStats();
            }
        }

        protected abstract T CreatePoolObject();

        protected abstract void DestroyPoolObject(T objectToDestroy);

        void UpdatePoolStats()
        {
            currentSize = unusedObjectQueue.Count + usedObjects.Count;
            percentageUsed = currentSize > 0 ? (float)usedObjects.Count / currentSize : -1;
        }

        void ExpandPoolSize()
        {
            int sizeToExpand = Mathf.Min(expandSizeInterval, maxExpandedSize - currentSize);
            currentSize += sizeToExpand;

            for (int i = 0; i < sizeToExpand; i++)
            {
                T obj = CreatePoolObject();
                unusedObjectQueue.Enqueue(obj);
                OnAddObjectToPool?.Invoke(obj);
            }
        }

        void ReducePoolSize()
        {
            int sizeToReduce = Mathf.Min(expandSizeInterval, currentSize - defaultSize);

            if (currentSize - sizeToReduce < usedObjects.Count)
                return;

            currentSize -= sizeToReduce;

            for (int i = 0; i < sizeToReduce; i++)
            {
                T removedObject = unusedObjectQueue.Dequeue();
                OnRemoveObjectFromPool?.Invoke(removedObject);
                DestroyPoolObject(removedObject);
            }
        }
    }
}
