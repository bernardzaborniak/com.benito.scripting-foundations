using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Validator;


namespace Benito.ScriptingFoundations.BSceneManagement
{

    /// <summary>
    /// Sits under the Scene Manager.
    /// Spawns the required fade prefab and manages it. H
    /// </summary>
    /*public class BSceneFadeSettings : MonoBehaviour
    {
        public class BSceneFadeSettingsObject
        {
            public string fadeName;
            [Validate("The assigned Gameobject must have a BSceneFade Component on its root", nameof(ValidateGameObject))]
            public GameObject fadeGameObject;

            bool ValidateGameObject(GameObject fadeGameObject)
            {
                return fadeGameObject.GetComponent<BSceneFade>() != null;
            }
        }

        public BSceneFadeSettingsObject[] fades;
    }*/
}
