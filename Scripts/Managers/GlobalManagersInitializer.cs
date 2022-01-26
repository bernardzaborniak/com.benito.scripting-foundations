using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Benito.ScriptingFoundations.Managers
{
    /// <summary>
    /// Makes sure a DontDestroyOnLoad Collection with Global Managers is present in every Scene
    /// </summary>

    public class GlobalManagersInitializer
    {
        static GameObject Instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeGlobalManagers()
        {
            if(Instance == null)
            {
                GameObject dontDestroyWrapper = new GameObject("Dont Destroy On Load Wrapper", typeof(DontDestroyOnLoadWrapper));
                GameObject.Instantiate(GlobalManagersSettings.GetOrCreateSettings().globalManagersPrefab, dontDestroyWrapper.transform);
               
                Instance = dontDestroyWrapper;

            }
        }   
    }
}
