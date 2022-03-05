using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public abstract class BSceneTransition
    {
        public abstract void StartTransition();

        public abstract void UpdateTransition();

        protected BSceneFade CreateFade(GameObject fadePrefab, Transform fadeParent)
        {
            BSceneFade fade = GameObject.Instantiate(fadePrefab, fadeParent).GetComponent<BSceneFade>();

            if (fade == null)
            {
                Debug.LogError("fadePrefab needs to have a BSceneFade Component at its root");
            }
            return fade;
        }
    }
}
