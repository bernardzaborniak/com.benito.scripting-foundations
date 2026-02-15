using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.NaughtyAttributes;
using Benito.ScriptingFoundations.IdSystem;

namespace Benito.ScriptingFoundations.Saving.SceneObjects
{
    [RequireComponent(typeof(IdReference))]

    public abstract class SaveableSceneObject : MonoBehaviour
    {
        private IdReference id;

        public IdReference Id
        {
            get
            {
                if (id == null)
                    id = GetComponent<IdReference>();

                return id;
            }
            private set
            {
                id = value;
            }
        }

        [Tooltip("Saves will saved and loaded in the priority order, lower priority first: For example: 0 before 3")]
        public int Priority = 0;
       
        /// <summary>
        /// May also return null in order to optimise, when saving something is only necessary during a certain stage
        /// </summary>
        public abstract SaveableSceneObjectData Save();

        public abstract void Load(SaveableSceneObjectData dataToLoad);
    }
}
