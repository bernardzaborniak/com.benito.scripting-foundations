using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Modding 
{
    public static class ModLoader
    {
        public static List<ModInfo> GetModInfos()
        {
            return null;
        }

        public static void LoadMod(ModInfo modInfo)
        {

        }

        public static void UnloadMod(ModInfo modInfo)
        {

        }

        public static string GetCustomLevelModsScene(ModInfo modInfo)
        {
            // checkif modSettings.loadedMods contains mod else throw error

            return string.Empty;
        } 
    }
}

/*
int sceneCount;
//Dictionary<string, ModInfo> modDictionary = new Dictionary<string, ModInfo>(); //Maps mods to a name so it can be easily loaded[Header("Mod Path : /Assets/")]

string bundleName = "testbundle";
string assetName = "SampleScene";

string bundlePath;



void Awake()
{
   sceneCount = SceneManager.sceneCountInBuildSettings;
   // Addressables.InitializeAsync().Completed += OnInitializationAdressCompleted;
   bundlePath = Path.Combine(Application.persistentDataPath, "Mods", bundleName);


}

private void OnGUI()
{
   for (int i = 0; i < sceneCount; i++)
   {
       GUI.Label(new Rect(10, 10, 300, 50), "Default Scenes");
       //SceneManager.GetSceneByBuildIndex(i).name)
       if (GUI.Button(new Rect(10, 50 + (40 * i), 200, 30), SceneUtility.GetScenePathByBuildIndex(i)))
       {
           SceneManager.LoadScene(i);
       }
   }

   GUI.Label(new Rect(10, 100 + (40 * sceneCount), 300, 50), "Modded Scenes");
   int posYHelper = 100 + (40 * sceneCount);

   if (GUI.Button(new Rect(10, posYHelper, 200, 30), "try load mod"))
   {
       AssetBundle localAssetBundle = AssetBundle.LoadFromFile(bundlePath);
       //SceneAsset scene = localAssetBundle.LoadAsset<SceneAsset>(assetName);
       string sceneName = Path.GetFileNameWithoutExtension(localAssetBundle.GetAllScenePaths()[0]);
       Debug.Log("scene name: " + sceneName);

       SceneManager.LoadScene(sceneName);
   }


}*/

