using Benito.ScriptingFoundations.BSceneManagement;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the seperate steps of playing fades to switch between 2 scenes.
/// 
/// Requires an already preloaded Scene. This change is there to allow the game to control how and when to start preloading
/// as this transition type will be used more creatively
/// </summary>
public class TransitionExecutorDefaultRequiresPreloadedScene : TransitionExecuter
{
    // Fades
    GameObject exitCurrentSceneFadePrefab;
    GameObject enterNextSceneFadePrefab;

    // Refs
    MonoBehaviour coroutineHost;
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

    public TransitionExecutorDefaultRequiresPreloadedScene(MonoBehaviour coroutineHost, Transform sceneManagerTransform, BSceneLoader sceneLoader, GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
    {
        stage = Stage.Idle;

        this.coroutineHost = coroutineHost;
        this.sceneLoader = sceneLoader;
        this.sceneManagerTransform = sceneManagerTransform;
        this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
        this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
    }

    public override void StartTransition()
    {
        coroutineHost.StartCoroutine(TransitionCoroutine());
    }

    public override void UpdateTransition()
    {
    }

    IEnumerator TransitionCoroutine()
    {
        // 1 play ExitCurrentScene Fade
        BSceneFade exitCurrentSceneFade = null;
        if (exitCurrentSceneFadePrefab != null)
        {
            stage = Stage.PlayingExitCurrentSceneFade;
            exitCurrentSceneFade = CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
            exitCurrentSceneFade.StartFade();

            yield return new WaitUntil(() => exitCurrentSceneFade.HasFinished);
        }

        // 2 Switch to preloaded scene
        bool switchDone = false;
        Action switchDoneHandler = () =>
        {
            switchDone = true;
            OnFinishedLoadingTargetScene?.Invoke();
        };

        sceneLoader.OnSwitchedToPreloadedScene += switchDoneHandler;
        sceneLoader.SwitchToPreloadedScene();

        yield return new WaitUntil(() => switchDone);
        sceneLoader.OnSwitchedToPreloadedScene -= switchDoneHandler;

        if (exitCurrentSceneFade)
            GameObject.Destroy(exitCurrentSceneFade.gameObject);


        // 3 Play enter next scene fade
        BSceneFade enterNextSceneFade = null;
        if (enterNextSceneFadePrefab != null)
        {
            stage = Stage.PlayingEnterNextSceneFade;
            enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
            enterNextSceneFade.StartFade();

            yield return new WaitUntil(() => enterNextSceneFade.HasFinished);
        }

        if (enterNextSceneFade)
            GameObject.Destroy(enterNextSceneFade.gameObject);

        stage = Stage.Finished;
        OnFinished?.Invoke();
    }

    public override float GetProgress()
    {
        return (int)stage;
    }

    public override string GetCurrentStageDebugString()
    {
        return stage.ToString();
    }
}
