using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.BDebug
{
    public class BDebugSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/BDebug Settings";
        [Space(10)]
        public Material debugMeshMaterial;
        public Material debugMeshTransparentMaterial;
        public Material debugMeshWireframeMaterial;
        public Material debugLineMaterial;
        public int maxDebugTextsOnScreen = 50;

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

