using System;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine;

namespace Benito.ScriptingFoundations.IdSystem
{
    /// <summary>
    /// For now only used by the saving system to save references to objects and also between objects
    /// </summary>
    public class IdReference : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField] protected string id;

        [Tooltip("Set true for stuff like prefabs etc, that wont be set inside scene")]
        [SerializeField] bool reacreateIdOnAwake = false;

        public string GetId() { return id; }

        /// <summary>
        /// Should be called by the editor before entering playmode and before build. 
        /// </summary>
        public void CreateNewId()
        {
            SetId(Guid.NewGuid().ToString());

        }

        public bool HasId()
        {
            return !String.IsNullOrEmpty(id);
        }

        public void Awake()
        {
            if (reacreateIdOnAwake)
            {
                CreateNewId();
            }
        }

        // useful
        public void SetId(string newId)
        {
            string oldId = id;
            id = newId;
            LocalSceneManagers.Get<IdReferenceManager>().OnIdChanged(oldId, newId,this);
        }
    }
}
