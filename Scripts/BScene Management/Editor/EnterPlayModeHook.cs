using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement.Editor
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
                SceneLoadHooks.Instance?.OnEnteredPlayModeViaEditor();
            }
        }
    }

}