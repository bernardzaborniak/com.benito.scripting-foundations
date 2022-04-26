using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Reflection;
using System;

namespace Benito.ScriptingFoundations.Saving
{
    public static class SceneSavegameUtility
    {
        public static SceneSavegame CreateSavegameFromJsonString(string jsonString)
        {
            string[] seperatedString = jsonString.Split("\n");
            string sceneName = seperatedString[0];

            List<SaveableSceneObjectData> saveableObjects = new List<SaveableSceneObjectData>();

            Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblyDictionary.Add(assembly.GetName().ToString(), assembly);
            }
;
            for (int i = 1; i < seperatedString.Length - 1; i++)
            {
                SaveableSceneObjectData genericData = (SaveableSceneObjectData)JsonUtility.FromJson<SaveableSceneObjectData>(seperatedString[i]);

                Type saveableDataType = assemblyDictionary[genericData.assemblyName].GetType(genericData.typeName);
                saveableObjects.Add((SaveableSceneObjectData)JsonUtility.FromJson(seperatedString[i], saveableDataType));
            }

            SceneSavegame newSaveGame = new SceneSavegame(sceneName, saveableObjects);

            return newSaveGame;
        }

        public static async Task<SceneSavegame> CreateSavegameFromJsonStringAsync(string jsonString, IProgress<float> progress = null)
        {
            var result = await Task.Run(() =>
            {
                string[] seperatedString = jsonString.Split("\n");
                string sceneName = seperatedString[0];
                progress?.Report(0.05f);
                List<SaveableSceneObjectData> saveableObjects = new List<SaveableSceneObjectData>();

                Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    assemblyDictionary.Add(assembly.GetName().ToString(), assembly);
                }
                progress?.Report(0.1f);

                for (int i = 1; i < seperatedString.Length - 1; i++)
                {
                    SaveableSceneObjectData genericData = (SaveableSceneObjectData)JsonUtility.FromJson<SaveableSceneObjectData>(seperatedString[i]);

                    Type saveableDataType = assemblyDictionary[genericData.assemblyName].GetType(genericData.typeName);
                    saveableObjects.Add((SaveableSceneObjectData)JsonUtility.FromJson(seperatedString[i], saveableDataType));

                    progress?.Report(0.1f + 1f * i / seperatedString.Length);
                }

                SceneSavegame newSaveGame = new SceneSavegame(sceneName, saveableObjects);

                return newSaveGame;
            });

            return result;
        }

        /// <summary>
        /// This ist actually Json but a kind of custom format
        /// </summary>
        public static string ConvertSaveGameToJsonString(SceneSavegame savegame)
        {
            // Workaround, because erializing one by one works, but whole list doesnt :(
            string jsonString = savegame.SceneName + "\n";

            for (int i = 0; i < savegame.SavedObjects.Count; i++)
            {
                jsonString += JsonUtility.ToJson(savegame.SavedObjects[i], false) + "\n";
            }

            return jsonString;
        }

        /// <summary>
        /// This ist actually Json but a kind of custom format
        /// WARNING: This method does not seem to work in other threads - JSON Utulity does not seem to be trhead safe :(
        /// </summary>
        public static async Task<string> ConvertSaveGameToJsonStringAsync(SceneSavegame savegame, IProgress<float> progress = null)
        {
            //List<SaveableObjectData> savedObjectsUsed = new List<SaveableObjectData>(savedObjects);

            var result = await Task.Run(() =>
            {
                // Workaround, because erializing one by one works, but whole list doesnt :(
                string jsonString = savegame.SceneName + "\n";

                for (int i = 0; i < savegame.SavedObjects.Count; i++)
                {
                    /*jsonString += " wwdwddwwdwdwdwdwd";
                    for (int j = 0; j < 50000; j++)
                    {
                        float y = Mathf.Sqrt(3);
                    }*/
                    jsonString += JsonUtility.ToJson(savegame.SavedObjects[i], false) + "\n";
                    progress?.Report((1.0f * i) / savegame.SavedObjects.Count);
                }

                return jsonString;
            });

            //savedObjects = savedObjectsUsed;

            return result;
        }
    }
}
