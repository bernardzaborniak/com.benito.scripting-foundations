using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Benito.ScriptingFoundations.NaughtyAttributes;

namespace Benito.ScriptingFoundations.Pools
{
    public abstract class AbstractPool<T>
    {
        [SerializeField] protected int defaultSize;
        [Space]
        [Tooltip("Should the pool expand its size when more Items are requested than inside the pool?")]
        [SerializeField] protected bool expandSize;
        [Tooltip("The pool will be increased by this size, when more items are needed")]
        [SerializeField] protected int expandSizeInterval;
        [Tooltip("The maximum size the pool can have after expanding")]
        [SerializeField] protected int maxExpandedSize;
        [Space]
        [Tooltip("Only working if expandSize is true. Controls weather to go back to normal size in expandSizeIntervals when less than 70% (reduceSizeThreshold) of the current size is used")]
        [SerializeField] protected bool reduceSize;
        [Range(0.1f, 0.9f)]
        [SerializeField] protected float reduceSizeThreshold = 0.7f;
        //[Space]
        //Readonly
        [NaughtyAttributes.ReadOnly][SerializeField][AllowNesting]
        protected int currentSize;
        [NaughtyAttributes.ReadOnly][SerializeField][AllowNesting]
        protected float percentageUsed;

        protected Queue<T> unusedObjectQueue;
        protected HashSet<T> usedObjects;

        public HashSet<T> UsedObjects { get => usedObjects; }

        public Action<T> OnAddObjectToPool;
        public Action<T> OnRemoveObjectFromPool;

        public Action<AbstractPool<T>,T> OnCreatePoolObject;
        public Action<AbstractPool<T>,T> OnBeforeDestroyPoolObject;


        public virtual void Initialize()
        {
            unusedObjectQueue = new Queue<T>();
            usedObjects = new HashSet<T>();


            for (int i = 0; i < defaultSize; i++)
            {
                T obj = CreatePoolObject();
                OnCreatePoolObject?.Invoke(this, obj);
                unusedObjectQueue.Enqueue(obj);
                OnAddObjectToPool?.Invoke(obj);
            }

            UpdatePoolStats();
        }

        public virtual void InitializeWithParams(int defaultSize, bool expandSize = false, int expandSizeInterval = 5, int maxExpandedSize = 50, bool reduceSize = true, float reduceSizeThreshold = 0.7f)
        {
            this.defaultSize = defaultSize;
            this.expandSize = expandSize;
            this.expandSizeInterval = expandSizeInterval;
            this.maxExpandedSize = maxExpandedSize;
            this.reduceSize = reduceSize;
            this.reduceSizeThreshold = reduceSizeThreshold;


            unusedObjectQueue = new Queue<T>();
            usedObjects = new HashSet<T>();


            for (int i = 0; i < defaultSize; i++)
            {
                T obj = CreatePoolObject();
                OnCreatePoolObject?.Invoke(this, obj);
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
            usedObjects.Add(dequeuedObject);
            UpdatePoolStats();
            OnRemoveObjectFromPool?.Invoke(dequeuedObject);
            return dequeuedObject;
        }

        public void Return(T objectToReturn)
        {
            if (!usedObjects.Contains(objectToReturn))
            {
                Debug.LogWarning($"Object is not part of the pool you are trying to return it to");

                if(unusedObjectQueue.Contains(objectToReturn))
                    Debug.LogWarning($"unusedObjectQueue already has this object, it was returned before");

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
            
            OnAddObjectToPool?.Invoke(objectToReturn);
        }

        protected abstract T CreatePoolObject();

        protected abstract void DestroyPoolObject(T objectToDestroy);

        public bool HasUnusedObjects()
        {
            return unusedObjectQueue.Count > 0;
        }

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
                OnCreatePoolObject?.Invoke(this, obj);
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
                OnBeforeDestroyPoolObject?.Invoke(this, removedObject);
                DestroyPoolObject(removedObject);
            }
        }

        public System.Type GetPoolObjectType()
        {
            return typeof(T);
        }
    }
}
