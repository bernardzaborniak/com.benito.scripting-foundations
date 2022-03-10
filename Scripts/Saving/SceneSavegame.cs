using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

// how to get unitys newtonsoft?
//https://stackoverflow.com/questions/63955593/how-do-we-parse-json-in-unity3d

namespace Benito.ScriptingFoundations.Saving 
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SceneSavegame 
    {
        [JsonProperty]
        string sceneName;

        [JsonProperty()]
        List<SaveableObjectData> savedObjects;

        public SceneSavegame(string sceneName, List<SaveableObjectData> savedObjects)
        {
            this.sceneName = sceneName;
            this.savedObjects = savedObjects;
        }

        /// <summary>
        /// Has to be done in a special way to properly serialise the savedObjectsas derived classes
        /// </summary>
        /// <returns></returns>
        public string GetJsonString()
        {
            //string jsonString = JsonUtility.ToJson(this, true);

            // serializing one by one works :(

            /*string jsonString = "{\n\"sceneName\": \"Saving\",\n\"savedObjects\": [\n";

            for (int i = 0; i < savedObjects.Count; i++)
            {
                jsonString += "\t\t" + JsonUtility.ToJson(savedObjects[i], true);

                if (i != savedObjects.Count - 1)
                    jsonString += ",\n";

            }

            jsonString += "\n]\n}";*/

            Debug.Log("convert");

            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return jsonString;
        }

        public static SceneSavegame CreateFromJsonString(string jsonString)
        {
            /*SceneSavegame savegame = JsonUtility.FromJson<SceneSavegame>(jsonString);


            string[] saveableObjectsStrings = jsonString.Split("[")

            // Recreate the list again using correct derived Types
            List<SaveableObjectData> newList = new List<SaveableObjectData>();

            for (int i = 0; i < savegame.savedObjects.Count; i++)
            {



            }

            return savegame;*/
            //SceneSavegame savegame = new SceneSavegame()


            SceneSavegame savegame =  JsonConvert.DeserializeObject<SceneSavegame>(jsonString);

            for (int i = 0; i < savegame.savedObjects.Count; i++)
            {
                Debug.Log("loaded object: " + savegame.savedObjects[i].GetType());
            }
            return savegame;
        }

        public List<SaveableObjectData> GetSavedObjectsFromSave()
        {
            return savedObjects;
        }


    }
}



