using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers
{
    public abstract class AbstractEnterPlayModeSceneInitializer : MonoBehaviour
    {
        public abstract void OnEnteredPlayModeViaEditor();
    }
}