using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.BDebug
{
    public class BDebugSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/BDebug Settings";

        public Material debugMeshMaterial;
        public int maxDebugTextsOnScreen;

        public class BDebugDrawingLayer
        {
            public string name;
            public bool visible;
        }

        public List<BDebugDrawingLayer> debugDrawingLayers;

        public static BDebugSettings GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<BDebugSettings>(DefaultSettingsPathInResourcesFolder);
        }
    }
}

