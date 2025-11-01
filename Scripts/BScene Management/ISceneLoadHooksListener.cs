using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public interface ISceneLoadHooksListener
    {
        void Start_h1_OnFinishedLoadTargetScene();
        void Start_h2_OnFinishedLoadSceneLoadSaveAndInitialize();
        void Start_h3_OnFinishedStillPlayingLastFadeIn();
        void Start_h4_OnFinished();
    }
}
