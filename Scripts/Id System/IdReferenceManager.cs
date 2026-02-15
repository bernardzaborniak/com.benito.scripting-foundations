using System.Collections.Generic;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.NaughtyAttributes;
using Benito.ScriptingFoundations.Saving.SceneObjects;
using UnityEngine;

namespace Benito.ScriptingFoundations.IdSystem
{
    public class IdReferenceManager : SingletonManagerLocalScene
    {
        // Dictionary is flattened to be able to be saved
        [SerializeField] string[] saveableObjectIds;
        [SerializeField] IdReference[] idReferenceObjects;

        private Dictionary<string, IdReference> runtimeIdLookup;

        public override void InitialiseManager()
        {
            runtimeIdLookup = new Dictionary<string, IdReference>();

            for (int i = 0; i < idReferenceObjects.Length; i++)
            {
                runtimeIdLookup.Add(saveableObjectIds[i], idReferenceObjects[i]);
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

#if UNITY_EDITOR
        [Button("ScanSceneForIdReferenceObjects")]
        public void ScanSceneForIdReferenceObjects()
        {
            idReferenceObjects = FindObjectsByType<IdReference>( FindObjectsInactive.Include, FindObjectsSortMode.None);
            saveableObjectIds = new string[idReferenceObjects.Length];

            for (int i = 0; i < idReferenceObjects.Length; i++)
            {
                saveableObjectIds[i] = idReferenceObjects[i].GetId();
            }
        }
#endif
    }
}
