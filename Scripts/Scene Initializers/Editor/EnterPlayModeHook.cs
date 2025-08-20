using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.SceneInitializers.Editor
{
    public class EnterPlayModeHook
    {

        [RuntimeInitializeOnLoadMethod]
        static void InitializeHook()
        {
            EditorApplication.playModeStateChanged += ModeStateChanged;
        }

        static void ModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EnterPlayModeSceneInitializersManager.Instance?.OnEnteredPlayModeViaEditor();
            }
        }
    }

}