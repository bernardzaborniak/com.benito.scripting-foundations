using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Benito.ScriptingFoundations.Modding 
{
    public static class ModLoader
    {
        public static List<ModInfo> GetAllMods()
        {
            List<ModInfo> modInfos = new List<ModInfo>();

            string modsDirectory = ModdingSettings.GetOrCreateSettings().GetModFolderPath();
            if (!Directory.Exists(modsDirectory))
            {
                Debug.LogError($"Mod Directory: {modsDirectory} does not exist :(");
                Debug.Log("path: " + modsDirectory);
                return modInfos;
            }

            string[] directories = Directory.GetDirectories(modsDirectory);
            
            foreach (string directory in directories)
            {
                string modInfoPath = Path.Combine(directory, "ModInfo.json");
                string modBundlePath = Path.Combine(directory, "modbundle");

                if (File.Exists(modInfoPath))
                {
                    ModInfo modInfo = JsonUtility.FromJson<ModInfo>(File.ReadAllText(modInfoPath));
                    modInfo.modInfoFileInfo = new FileInfo(modInfoPath);
                    modInfo.modBundleFileInfo = new FileInfo(modBundlePath);

                    modInfos.Add(modInfo);
                }
                else
                {
                    Debug.LogError($"Mod at path {directory} does not have a ModInfo.json file -> cant be loaded");
                }
            }
            
            return modInfos;
        }

        public static void LoadMod(ModInfo modInfo)
        {
            AssetBundle loadedBundle = AssetBundle.LoadFromFile(modInfo.modBundleFileInfo.FullName);
            string sceneName = Path.GetFileNameWithoutExtension(loadedBundle.GetAllScenePaths()[0]);

            modInfo.loadedModBundle = loadedBundle;
            modInfo.customSceneName = sceneName;
            modInfo.loaded = true;
        }

        public static void UnloadMod(ModInfo modInfo)
        {
            modInfo.loadedModBundle.Unload(true);
            modInfo.customSceneName = string.Empty;
            modInfo.loaded = false;
        }
    }
}


