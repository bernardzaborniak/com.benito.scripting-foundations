using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Gets serialized as Json, a small object containing information about a scene savagame, but not the actual save itself
    /// </summary>
    [System.Serializable]
    public class SceneSavegameInfo
    {
        public string savegameName;
        public SceneSavegameType savegameType;
        public string unitySceneName;
        public string missionName;
        public string dateCreated;
        public string timeCreated;

        public SceneSavegameInfo(string savegameName, SceneSavegameType savegameType, string unitySceneName, string missionName, string dateCreated, string timeCreated)
        {
            this.savegameName = savegameName;
            this.savegameType = savegameType;
            this.unitySceneName = unitySceneName;
            this.missionName = missionName;
            this.dateCreated = dateCreated;
            this.timeCreated = timeCreated;
        }
    }
}
