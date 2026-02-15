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

        private Dictionary<string, IdReference> runtimeIdLookup;// = new Dictionary<string, IdReference>();

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
            Debug.Log($"Dict Size {runtimeIdLookup.Count}");
            Debug.Log($"GetIdByString for id {id} -> returned ref {reference}", reference?.gameObject);
            return reference;
        }

#if UNITY_EDITOR
        [Button("ScanSceneForIdReferenceObjects")]
        public void ScanSceneForIdReferenceObjects()
        {
            Debug.Log("ScanSceneForIdReferenceObjects called");
            //runtimeIdLookup = new Dictionary<string, IdReference>();

            idReferenceObjects = FindObjectsByType<IdReference>( FindObjectsInactive.Include, FindObjectsSortMode.None);
            saveableObjectIds = new string[idReferenceObjects.Length];

            for (int i = 0; i < idReferenceObjects.Length; i++)
            {
                saveableObjectIds[i] = idReferenceObjects[i].GetId();
            }
            
               // runtimeIdLookup.Add(id.GetId(), id);
            

            Debug.Log($"ScanSceneForIdReferenceObjects filled dict, now {idReferenceObjects.Length}");
            //saveableObjectIds = runtimeIdLookup.Keys;
            //idReferenceObjects = runtimeIdLookup.Values;
        }
#endif
    }
}
