using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.SceneSaves
{
    /// <summary>
    /// Can be derived and expanded to make game specific saves
    /// </summary>
    [System.Serializable]
    public class SceneSaveInfo
    {
        public string saveName;
        public SceneSaveType saveType;
        public string unitySceneName;
        public string levelName; // level / mission name, smth inbetween

        public string lastSavedTimeString;    // used for storing

        [System.NonSerialized]
        public DateTime lastSavedTime; // only used during runtime

        [System.NonSerialized]
        public Texture2D previewImage;

        //[System.NonSerialized]
        public string filePathInSavesFolder;
    }
}
