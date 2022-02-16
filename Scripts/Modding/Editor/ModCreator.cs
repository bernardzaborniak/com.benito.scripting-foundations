using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Benito.ScriptingFoundations.Modding.Editor
{
    public static class ModCreator
    {


        public static void CreateCustomLevelMod(ModInfo modInfo)
        {
            string modExportsDirectory = Path.Combine(Application.dataPath,  "Mod Exports");

            // Create Mod Exports Folder
            if (!Directory.Exists(modExportsDirectory))
            {       
                Directory.CreateDirectory(modExportsDirectory);
            }

            string modDirectory = Path.Combine(modExportsDirectory, modInfo.name);
            Directory.CreateDirectory(modDirectory);


            // Create Mod Info
            string modInfoJson = JsonUtility.ToJson(modInfo);
            File.WriteAllText(Path.Combine(modDirectory, "ModInfo.json"), modInfoJson);

            

            // Create Asset Bundle
            //TODO
            //BuildPipeline.BuildAssetBundles(modPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}


