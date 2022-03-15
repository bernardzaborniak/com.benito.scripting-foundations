using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Reflection;
using Debug = UnityEngine.Debug;
using System.IO;

namespace Benito.ScriptingFoundations.Saving
{
    public class SceneSavegame
    {
        string sceneName;
        List<SaveableObjectData> savedObjects;

        public SceneSavegame(string sceneName, List<SaveableObjectData> savedObjects)
        {
            this.sceneName = sceneName;
            this.savedObjects = savedObjects;
        }

        /// <summary>
        /// Has to be done in a special way to properly serialise the savedObjectsas derived classes.
        /// </summary>
        /// <returns></returns>
        public string GetJsonString()
        {
            // Workaround, because erializing one by one works, but whole list doesnt :(
            string jsonString = sceneName + "\n";

            for (int i = 0; i < savedObjects.Count; i++)
            {
                jsonString += JsonUtility.ToJson(savedObjects[i], false) + "\n";
            }

            return jsonString;
        }

        public async Task<string> GetJsonStringAsync(IProgress<float> progress = null)
        {
            var result = await Task.Run(() =>
            {
                // Workaround, because erializing one by one works, but whole list doesnt :(
                string jsonString = sceneName + "\n";

                for (int i = 0; i < savedObjects.Count; i++)
                {
                    jsonString += JsonUtility.ToJson(savedObjects[i], false) + "\n";
                    progress?.Report((1.0f* i)/ savedObjects.Count);
                }

                return jsonString;
            });

            return result;
        }

        public List<SaveableObjectData> GetSavedObjectsFromSave()
        {
            return savedObjects;
        }

        public string GetSceneName()
        {
            return sceneName;
        }

        public static SceneSavegame CreateSavegameFromJsonString(string jsonString)
        {
            string[] seperatedString = jsonString.Split("\n");
            string sceneName = seperatedString[0];

            List<SaveableObjectData> saveableObjects = new List<SaveableObjectData>();

            Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblyDictionary.Add(assembly.GetName().ToString(), assembly);
            }
;
            for (int i = 1; i < seperatedString.Length - 1; i++)
            {
                SaveableObjectData genericData = (SaveableObjectData)JsonUtility.FromJson<SaveableObjectData>(seperatedString[i]);

                Type saveableDataType = assemblyDictionary[genericData.assemblyName].GetType(genericData.typeName);
                saveableObjects.Add((SaveableObjectData)JsonUtility.FromJson(seperatedString[i], saveableDataType));
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
                List<SaveableObjectData> saveableObjects = new List<SaveableObjectData>();

                Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    assemblyDictionary.Add(assembly.GetName().ToString(), assembly);
                }
                progress?.Report(0.1f);

                for (int i = 1; i < seperatedString.Length - 1; i++)
                {
                    SaveableObjectData genericData = (SaveableObjectData)JsonUtility.FromJson<SaveableObjectData>(seperatedString[i]);

                    Type saveableDataType = assemblyDictionary[genericData.assemblyName].GetType(genericData.typeName);
                    saveableObjects.Add((SaveableObjectData)JsonUtility.FromJson(seperatedString[i], saveableDataType));

                    progress?.Report( 0.1f + 1f * i / seperatedString.Length);
                }

                SceneSavegame newSaveGame = new SceneSavegame(sceneName, saveableObjects);

                return newSaveGame;
            });

            return result;
        }

        public static string GetTargetSceneFromSceneSavegamePath(string path)
        {
            string name;

            using (StreamReader reader = new StreamReader(path))
            {
                name = reader.ReadLine();
                reader.Close();
            }

            return name;

        }
    }
}



