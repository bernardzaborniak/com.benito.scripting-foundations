using System.Collections.Generic;
using System.IO;
using Benito.ScriptingFoundations.Saving.SceneObjects;

namespace Benito.ScriptingFoundations.Saving.SceneSaves
{
    /// <summary>
    /// The class storing all information about a loaded Savegame.
    /// </summary>
    public class SceneSave
    {
        public string SceneName { get; private set; }
        public List<SaveableSceneObjectData> SavedObjects { get; private set; }

        public SceneSave(string sceneName, List<SaveableSceneObjectData> savedObjects)
        {
            this.SceneName = sceneName;
            this.SavedObjects = savedObjects;
        }
      
        public static string GetTargetSceneFromSceneSavePath(string path)
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



