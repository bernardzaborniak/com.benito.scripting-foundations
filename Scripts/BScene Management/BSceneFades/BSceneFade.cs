using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public abstract class BSceneFade : MonoBehaviour
    {
        public Action OnFadeFinished;

        public abstract void StartFade();

    }
}
