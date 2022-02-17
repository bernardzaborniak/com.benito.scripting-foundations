using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

namespace Benito.ScriptingFoundations.Modding.Editor
{
    public static class ModCreator
    {
        public static void CreateCustomLevelMod(ModInfo modInfo, string modExportPath)
        {
            string modPath = Path.Combine(modExportPath, modInfo.name);
            Directory.CreateDirectory(modPath);

            // Create Mod Info
            string modInfoJson = JsonUtility.ToJson(modInfo);
            File.WriteAllText(Path.Combine(modPath, "ModInfo.json"), modInfoJson);

            // Create Preview Image 
            if (modInfo.previewImage != null)
            {
                byte[] imageBytes = modInfo.previewImage.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(modPath, "PreviewImage.png"), imageBytes);
            }

            // Create Asset Bundle - Has to come after creating all the other files
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = "modbundle";
            buildMap[0].assetNames = new string[] { SceneManager.GetActiveScene().path};
            BuildPipeline.BuildAssetBundles(modPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            Debug.Log("Mod image while exporting: " + modInfo.previewImage);
           

            // Delete the additional created assets - I dont know how to prevent their creation :/
            if (File.Exists(Path.Combine(modPath,"modbundle.manifest")))
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(modPath, "modbundle.manifest"));
            }
            if (File.Exists(Path.Combine(modPath, modInfo.name)))
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(modPath, modInfo.name));
            }
            if (File.Exists(Path.Combine(modPath, modInfo.name + ".manifest")))
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(modPath, modInfo.name + ".manifest"));
            }
        }
    }
}


