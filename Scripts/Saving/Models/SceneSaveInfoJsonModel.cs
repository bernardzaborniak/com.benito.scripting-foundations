using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.Models
{
    /// <summary>
    /// Gets serialized as Json, a small object containing information about a scene savagame, but not the actual save itself
    /// TODO allow every game to add extra stuff here
    /// </summary>
    [System.Serializable]
    public class SceneSaveInfoJsonModel
    {
        public string savegameName;
        public SceneSaveType savegameType;
        public string unitySceneName;
        public string missionName;

        public string dateTimeCreated;

        /// <summary>
        /// Override this method for your custom models
        /// </summary>
        public virtual T MapToReadModel<T>() where T : SceneSaveInfoReadModel
        {
            return new SceneSaveInfoReadModel
            {
                savegameName = this.savegameName,
                savegameType = this.savegameType,
                unitySceneName = this.unitySceneName,
                missionName = this.missionName,
                dateTimeCreated = this.dateTimeCreated
            } as T;
        }
    }
}
