using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Handles the seperate steps of playing fades to switch between 2 scenes.
    /// 
    /// This executor preloads the next scene himself
    /// </summary>
    public class BTransitionExecutorDefault : BTransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterNextSceneFade;

        // Refs
        Transform sceneManagerTransform;
        BSceneLoader sceneLoader;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorDefault(string targetScene, 
            Transform sceneManagerTransform, BSceneLoader sceneLoader, 
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

            this.sceneLoader = sceneLoader;
            this.sceneManagerTransform = sceneManagerTransform;
            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override string GetCurrentStageDebugString()
        {
            throw new System.NotImplementedException();
        }

        public override float GetProgress()
        {
            throw new System.NotImplementedException();
        }

        public override void StartTransition()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateTransition()
        {
            throw new System.NotImplementedException();
        }
    }
}