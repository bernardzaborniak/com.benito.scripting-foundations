using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.LayerMaskOrganiser
{
    public class LayerMaskOrganiser : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/LayerMask Organiser";

        public string soString;

        public static LayerMaskOrganiser GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<LayerMaskOrganiser>(DefaultSettingsPathInResourcesFolder);
        }
    }
}
