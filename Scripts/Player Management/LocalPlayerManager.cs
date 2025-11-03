using UnityEngine;

namespace Benito.ScriptingFoundations.PlayerManagement
{
    /// <summary>
    /// Every scene has one of those, handles all the switching between the sub player controllers.
    /// They can get quite complex for some games with lots of switching and data saved
    /// between controller
    /// </summary>
    public abstract class LocalPlayerManager : MonoBehaviour
    {
        [SerializeField] protected PlayerControllerBase initialControllerToPossessOnSceneLoad;

        public PlayerControllerBase PossessedController { get; protected set; }

        GlobalPlayerManager globalManager;

        public virtual void PossessByGlobalManager(GlobalPlayerManager globalManager)
        {
            this.globalManager = globalManager;
            gameObject.SetActive(true);
            PossessPlayerController(initialControllerToPossessOnSceneLoad);
        }

        public virtual void StopPossessingByGlobalManager()
        {
            gameObject.SetActive(false); // maybe this one unnecesary?
            PossessPlayerController(null);
        }

        public virtual void PossessPlayerController(PlayerControllerBase controller)
        {
            if(PossessedController != null)
            {
                PossessedController.StopPossessing();
                Debug.Log($"[PlayerManager] Stopped Possessing Player Controller: {PossessedController?.GetType().Name}");
            }
            
            PossessedController = controller;

            if (PossessedController != null)
            {
                Debug.Log($"[PlayerManager] Started Possessing Player Controller: {PossessedController?.GetType().Name}");
                PossessedController.Possess(this);
            }
        }

        private void OnDestroy()
        {
            if (globalManager == null)
                return;

            globalManager.OnLocalManagerDestroyed(this);
        }
    }
}
