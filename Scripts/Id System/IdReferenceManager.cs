using System.Collections.Generic;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.NaughtyAttributes;
using Benito.ScriptingFoundations.Saving.SceneObjects;
using UnityEngine;

namespace Benito.ScriptingFoundations.IdSystem
{
    public class IdReferenceManager : SingletonManagerLocalScene
    {
        // Dictionary of scene objects is flattened to be able to be saved
        [SerializeField] string[] ids;
        [SerializeField] IdReference[] idReferenceSceneObjects;

        private Dictionary<string, IdReference> runtimeIdLookup;

        public override void InitialiseManager()
        {
            runtimeIdLookup = new Dictionary<string, IdReference>();

            for (int i = 0; i < idReferenceSceneObjects.Length; i++)
            {
                runtimeIdLookup.Add(ids[i], idReferenceSceneObjects[i]);
            }
        }

        public override void UpdateManager()
        {

        }

        public IdReference GetByIdString(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            runtimeIdLookup.TryGetValue(id, out var reference);
            return reference;
        }

        public void OnIdChanged(string oldId, string newId, IdReference reference)
        {
            if (runtimeIdLookup.ContainsKey(oldId))
            {
                runtimeIdLookup.Remove(oldId);
            }
            if (runtimeIdLookup.ContainsKey(newId))
            {
                runtimeIdLookup.Remove(newId);
            }

            runtimeIdLookup.Add(newId, reference);
        }

#if UNITY_EDITOR
        [Button("ScanSceneForIdReferenceObjects")]
        public void ScanSceneForIdReferenceObjects()
        {
            idReferenceSceneObjects = FindObjectsByType<IdReference>( FindObjectsInactive.Include, FindObjectsSortMode.None);
            ids = new string[idReferenceSceneObjects.Length];

            for (int i = 0; i < idReferenceSceneObjects.Length; i++)
            {
                ids[i] = idReferenceSceneObjects[i].GetId();
            }
        }
#endif
    }
}
