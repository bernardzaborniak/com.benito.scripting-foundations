using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.NaughtyAttributes;

namespace Benito.ScriptingFoundations.Saving.SceneObjects
{
    public abstract class SaveableSceneObject : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField] protected int saveID;

        public int GetId()
        {
            return saveID;
        }
       
        /// <summary>
        /// Should be called by the editor before entering playmode and before build. 
        /// </summary>
        public void SetId(int saveID)
        {
            this.saveID = saveID;
        }

        public bool HasId()
        {
            return saveID != 0;
        }

        /// <summary>
        /// May also return null in order to optimise, when saving something is only necessary during a certain stage
        /// </summary>
        public abstract SaveableSceneObjectData Save();

        public abstract void Load(SaveableSceneObjectData dataToLoad);
    }
}
