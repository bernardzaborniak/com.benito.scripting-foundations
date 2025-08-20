using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers
{

    /// <summary>
    /// Gets called on the scene when we enter from Unit's playmode.
    /// Doesn't get called if you enter from another scene
    /// </summary>
    public abstract class AbstractEnterPlayModeSceneInitializer : MonoBehaviour
    {
        public abstract void OnEnteredPlayModeViaEditor();
    }
}