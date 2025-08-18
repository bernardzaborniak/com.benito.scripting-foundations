using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    public abstract class AbstractSceneInitializer : MonoBehaviour
    {
        public bool InitializationFinished {  get; protected set; }
        public Action OnInitializationFinished;

        public abstract void Initialize();
    }
}