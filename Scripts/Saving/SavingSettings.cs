using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.Saving
{
    [CreateAssetMenu(menuName = "My Assets/SavingSettings")]
    public class SavingSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Saving Settings";

        [SerializeField]
        IOUtilities.AssigneableGameDataPath path = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData,"Default");
    }
}
