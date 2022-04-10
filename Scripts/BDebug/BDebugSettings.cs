using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using Benito.ScriptingFoundations.Validator;
using System.IO;

namespace Benito.ScriptingFoundations.BDebug
{
    public class BDebugSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/BDebug Settings";
        const string DebugMaterialsFolderLocation = "Packages/com.benito.scripting-foundations/Materials/BDebug URP";

        [Header("Materials")]
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

        [Header("Text")]
        public int maxDebugTextsOnScreen = 50;
        public enum OrientTextMode
        {
            AlongCameraForward,
            TowardsCamera
        }
        public OrientTextMode orientTextMode  = OrientTextMode.AlongCameraForward;

        [Space(10)]
        [SerializeField] bool[] drawingLayers = new bool[24] { true, true , true , true , true , true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        public bool[] DrawingLayers { get => drawingLayers; }



        public static BDebugSettings GetOrCreateSettings()
        {
            BDebugSettings settings = RessourceSettingsUtilities.GetOrCreateSettingAsset<BDebugSettings>(DefaultSettingsPathInResourcesFolder);

#if UNITY_EDITOR
            if (settings.debugMeshMaterial == null)        
                settings.debugMeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug.mat"), typeof(Material)) as Material;

            if (settings.debugMeshTransparentMaterial == null)
                settings.debugMeshTransparentMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug Transparent.mat"), typeof(Material)) as Material;

            if (settings.debugMeshWireframeMaterial == null)
                settings.debugMeshWireframeMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug Wireframe.mat"), typeof(Material)) as Material;

            if (settings.debugLineMaterial == null)
                settings.debugLineMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug Line.mat"), typeof(Material)) as Material;

            if (settings.defaultTextMeshMaterial == null)
                settings.defaultTextMeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug TextMesh.mat"), typeof(Material)) as Material;

            if (settings.defaultTextMeshMaterial == null)
                settings.defaultTextMeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug TextMesh.mat"), typeof(Material)) as Material;

            if (settings.defaultTextMeshMaterial == null)
                settings.defaultTextMeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug TextMesh.mat"), typeof(Material)) as Material;

            if (settings.overlayTextMeshMaterial == null)
                settings.overlayTextMeshMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(Path.Combine(DebugMaterialsFolderLocation, "Debug Overlay TextMesh.mat"), typeof(Material)) as Material;

#endif

            return settings;
        }

    }
}

