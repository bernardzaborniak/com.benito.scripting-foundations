using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Can be derived and expanded to make game specific saves
    /// </summary>
    [System.Serializable]
    public class SceneSaveInfo
    {
        public string savegameName;
        public SceneSaveType savegameType;
        public string unitySceneName;
        public string missionName;

        public string dateTimeCreated;

        [System.NonSerialized]
        public Texture2D previewImage;

        [System.NonSerialized]
        public string filePathInSavesFolder;
    }
}
