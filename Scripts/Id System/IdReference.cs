using System;
using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine;

namespace Benito.ScriptingFoundations.IdSystem
{
    /// <summary>
    /// For now only used by the saving system to save references to objects and also between objects
    /// </summary>
    public class IdReference : MonoBehaviour
    {
        //[ReadOnly]
        //[SerializeField] protected Guid id;
        [ReadOnly]
        [SerializeField] protected string id;

        //public Guid GetId() { return id; }
        public string GetId() { return id; }

        /// <summary>
        /// Should be called by the editor before entering playmode and before build. 
        /// </summary>
        public void CreateNewId()
        {
            //this.id = Guid.NewGuid();
            this.id = Guid.NewGuid().ToString();

        }

        public bool HasId()
        {
            return !String.IsNullOrEmpty(id);
        }
    }
}
