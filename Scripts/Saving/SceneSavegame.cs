using System.Collections.Generic;
using System.IO;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// The class storing all information about a loaded Savegame.
    /// </summary>
    public class SceneSavegame
    {
        public string SceneName { get; private set; }
        public List<SaveableSceneObjectData> SavedObjects { get; private set; }

        public SceneSavegame(string sceneName, List<SaveableSceneObjectData> savedObjects)
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



