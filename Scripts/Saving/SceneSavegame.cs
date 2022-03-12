using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

// how to get unitys newtonsoft?
//https://stackoverflow.com/questions/63955593/how-do-we-parse-json-in-unity3d

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
            // serializing one by one works :(

            string jsonString = sceneName + "\n";

            for (int i = 0; i < savedObjects.Count; i++)
            {            
                jsonString +=  JsonUtility.ToJson(savedObjects[i], false) + "\n";
            }

            Debug.Log("convert");

            return jsonString;
        }

        public static SceneSavegame CreateFromJsonString(string saveString)
        {
            string[] seperatedString = saveString.Split("\n");
            string sceneName = seperatedString[0];

            List<SaveableObjectData> saveableObjects = new List<SaveableObjectData>();

            for (int i = 1; i < seperatedString.Length-1; i++)
            {
                SaveableObjectData genericData = (SaveableObjectData)JsonUtility.FromJson<SaveableObjectData>(seperatedString[i]);

                Type saveableDataType = null;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {                    
                    if (assembly.GetName().ToString() == genericData.assemblyName)
                    {
                        saveableDataType = assembly.GetType(genericData.typeName);
                    }
                }
                saveableObjects.Add((SaveableObjectData)JsonUtility.FromJson(seperatedString[i], saveableDataType));
            }

            SceneSavegame newSaveGame = new SceneSavegame(sceneName, saveableObjects);
            return newSaveGame;
        }

        public List<SaveableObjectData> GetSavedObjectsFromSave()
        {
            return savedObjects;
        }

        public string GetSceneName()
        {
            return sceneName;
        }
    }
}



