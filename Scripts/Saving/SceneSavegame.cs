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
        public string SceneName { get; private set; }
        public List<SaveableObjectData> SavedObjects { get; private set; }

        public SceneSavegame(string sceneName, List<SaveableObjectData> savedObjects)
        {
            this.SceneName = sceneName;
            this.SavedObjects = savedObjects;
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



