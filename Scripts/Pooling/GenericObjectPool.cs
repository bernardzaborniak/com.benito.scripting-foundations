using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Pools
{
    [System.Serializable]
    public class GenericObjectPool<T> : AbstractPool<T> where T : new()
    {
        protected override T CreatePoolObject()
        {
            return new T();
        }

        protected override void DestroyPoolObject(T objectToDestroy)
        {
            //We cant destroy normal objects in C#
        }

    }
}
