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
        /// This ist actually Json but a kind of custom format, i think this logic was moved into the budgeted Operation
        /// </summary>
        public static string ConvertSaveGameToJsonString(SceneSavegame savegame)
        {
            // Workaround, because Serializing one by one works, but whole list doesnt :(
            string jsonString = savegame.SceneName + "\n";

            for (int i = 0; i < savegame.SavedObjects.Count; i++)
            {
                jsonString += JsonUtility.ToJson(savegame.SavedObjects[i], false) + "\n";
            }

            return jsonString;
        }   
    }
}
