using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    /// <summary>
    /// Gets called on entering a scene, load for the initializers to finish before 
    /// proceding with saving or enabling certaing game components.
    /// 
    /// Use those initializers for stuff like generating landscapes or other procedurally generated stuff.
    /// </summary>
    public abstract class AbstractSceneInitializer : MonoBehaviour
    {
        public bool InitializationFinished {  get; protected set; }
        public Action OnInitializationFinished;
        public float Progress {  get; protected set; }
        public string ProgressString {  get; protected set; }

        public abstract void StartInitialization();

        public abstract void UpdateInitialization(float frameBudgetInMs);
    }
}