using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.Linq;

namespace Benito.ScriptingFoundations.LayerMaskOrganiser.Editor
{
    public class LayerMaskOrganiser : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Layer Mask Organiser";

        public static LayerMaskOrganiser GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<LayerMaskOrganiser>(DefaultSettingsPathInResourcesFolder);
        }

        public enum LayerType
        {
            Hybrid,
            Visual,
            Physical
        }

        public LayerInfo[] layerInfosList = new LayerInfo [32];

        [System.Serializable]
        public class LayerInfo
        {
            public LayerType type;
            [TextArea]
            public string description;
        }
    }
}
