using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.Models
{
    /// <summary>
    /// can be derived to make game specific saves
    /// </summary>
    [System.Serializable]
    public class SceneSaveInfoCreateModel
    {
        public string savegameName;
        public SceneSaveType savegameType;
        public string unitySceneName;
        public string missionName;

        public Texture2D previewImage;

        /// <summary>
        /// Override this method for your custom models
        /// </summary>
        public virtual T MapToJsonModel<T>() where T : SceneSaveInfoJsonModel
        {
            return new SceneSaveInfoJsonModel
            {
                savegameName = this.savegameName,
                savegameType = this.savegameType,
                unitySceneName = this.unitySceneName,
                missionName = this.missionName,
            } as T;
        }
    }
}
