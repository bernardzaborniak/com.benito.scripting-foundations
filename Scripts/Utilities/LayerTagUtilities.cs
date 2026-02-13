using Benito.ScriptingFoundations.Managers;
using System.Collections;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public class LayerTagUtilities
    {
        public static void SetLayerRecursively(Transform parent, int layer)
        {
            parent.gameObject.layer = layer;

            foreach (Transform child in parent)
            {
                SetLayerRecursively(child, layer);
            }
        }

        /// <summary>
        /// Made safe, in case stuff gets destroyed in the meantime
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tag"></param>
        public static void SetTagRecursively(Transform parent, string tag) 
        {
            if (parent == null) return;
            parent.gameObject.tag = tag;

            foreach (Transform child in parent)
            {
                if(child==null ) continue;
                SetTagRecursively(child, tag);
            }
        }

        public static void SetTagRecursivelyUnsafe(Transform parent, string tag)
        {
            parent.gameObject.tag = tag;

            foreach (Transform child in parent)
            {
                SetTagRecursivelyUnsafe(child, tag);
            }
        }

        /// <summary>
        /// Might be usefull to change some asynchronous errors, like for example cinemachine deoccluder
        /// </summary>
        public static Coroutine OverrideTagForSetTime(Transform parent, string tag, float time = 1f)
        {
            return GlobalManagers.Get<GlobalCoroutineHost>().StartCoroutine(OverrideTagForSetTimeCoroutine(parent,tag,time));
        }

        static IEnumerator OverrideTagForSetTimeCoroutine(Transform parent, string tag, float time = 1f)
        {
            string originalTag = parent.tag;
            SetTagRecursively(parent,tag);
            yield return new WaitForSeconds(time);
            SetTagRecursively(parent,originalTag);
        }
    }
}
