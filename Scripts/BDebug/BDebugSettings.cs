using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using Benito.ScriptingFoundations.Validator;

namespace Benito.ScriptingFoundations.BDebug
{
    public class BDebugSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/BDebug Settings";
        [Space(10)]
        [Validate("cant be null")]
        public Material debugMeshMaterial;
        [Validate("cant be null")]
        public Material debugMeshTransparentMaterial;
        [Validate("cant be null")]
        public Material debugMeshWireframeMaterial;
        [Validate("cant be null")]
        public Material debugLineMaterial;
        [Validate("cant be null")]
        public Material defaultTextMeshMaterial;
        [Validate("cant be null")]
        public Material overlayTextMeshMaterial;

        public int maxDebugTextsOnScreen = 50;


        [Space(10)]
        [SerializeField] bool[] drawingLayers = new bool[24];
        public bool[] DrawingLayers { get => drawingLayers; }



        public static BDebugSettings GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<BDebugSettings>(DefaultSettingsPathInResourcesFolder);
        }

    }
}

