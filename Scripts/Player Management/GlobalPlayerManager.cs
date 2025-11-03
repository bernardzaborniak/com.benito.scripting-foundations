using Benito.ScriptingFoundations.Managers;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.PlayerManagement
{
    /// <summary>
    /// Handles possession of localPlayerManagers
    /// Handles player data like progress , stats, id , etc... ?
    /// </summary>
    public class GlobalPlayerManager : SingletonManagerGlobal
    {      
        public LocalPlayerManager PossessedLocalManager { get; private set; }

        public override void InitialiseManager()
        {
            
        }

        public override void UpdateManager()
        {
            
        }

        public void PossesLocalManager(LocalPlayerManager manager)
        {
            if (PossessedLocalManager != null)
            {
                PossessedLocalManager.StopPossessingByGlobalManager();
                Debug.Log($"[PlayerManager] Stopped Possessing Local Player Manager: {PossessedLocalManager?.GetType().Name}");
            }
           
            PossessedLocalManager = manager;
            if (PossessedLocalManager != null)
            {
                Debug.Log($"[PlayerManager] Started Possessing Local Player Manager: {PossessedLocalManager?.GetType().Name}");
                PossessedLocalManager.PossessByGlobalManager(this);
            }
        }

        public void OnLocalManagerDestroyed(LocalPlayerManager manager)
        {
            if(PossessedLocalManager == manager)
            {
                PossesLocalManager(null);
            }
        }
    }
}
