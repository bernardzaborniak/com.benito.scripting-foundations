using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.Models
{
    /// <summary>
    /// can be derived to make game specific saves
    /// </summary>
    [System.Serializable]
    public class SceneSaveInfoReadModel
    {
        public string savegameName;
        public SceneSaveType savegameType;
        public string unitySceneName;
        public string missionName;

        public string dateTimeCreated;

        public string filePathInSavesFolder;
        public Texture2D previewImage;
    }
}
