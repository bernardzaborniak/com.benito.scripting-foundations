using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Reflection;
using Debug = UnityEngine.Debug;

using System.Diagnostics;

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

            return jsonString;
        }

        //public static async Task<SceneSavegame> CreateFromJsonString(string saveString)
        public static SceneSavegame CreateFromJsonString(string saveString)
        {
            //var result = await Task.Run(() =>
            //{
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();


                //this makes the performance worse
                for (int i = 0; i < 500000000; i++)
                {
                    float f = Mathf.Sqrt(5);
                }

                string[] seperatedString = saveString.Split("\n");
                string sceneName = seperatedString[0];

                stopwatch.Stop();
                UnityEngine.Debug.Log("Split string took " + stopwatch.Elapsed.TotalSeconds + " s");

                stopwatch.Reset();
                stopwatch.Start();
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

                stopwatch.Stop();
                UnityEngine.Debug.Log("read string took" + stopwatch.Elapsed.TotalSeconds + " s");


                stopwatch.Reset();
                stopwatch.Start();
                SceneSavegame newSaveGame = new SceneSavegame(sceneName, saveableObjects);
                stopwatch.Stop();
                UnityEngine.Debug.Log("new SceneSavegame tool " + stopwatch.Elapsed.TotalSeconds + " s");

                return newSaveGame;
           //});

            //return result;
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



