using System.Collections.Generic;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving.SceneObjects;
using EditorAttributes;
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
                // temp
                if (runtimeIdLookup.ContainsKey(ids[i])) Debug.Log($"runtime lookup already contains key :{ids[i]}: for this object: ", idReferenceSceneObjects[i].gameObject);
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
            // this might cause bugs if we delete used ones :0, but so far works :)
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
        /// Is also called automatically by on scene save hook
        public void ScanSceneForIdReferenceObjects()
        {
            List<IdReference> potentialReferences = new List<IdReference>(FindObjectsByType<IdReference>(FindObjectsInactive.Include, FindObjectsSortMode.None));

            // Remove ids which are set to recreate on awake, they will add themselves later on
            HashSet<IdReference> idsWithRecreateOnAwake = new HashSet<IdReference>();
            for (int i = 0; i < potentialReferences.Count; i++)
            {
                if (potentialReferences[i].reacreateIdOnAwake) idsWithRecreateOnAwake.Add(potentialReferences[i]);
            }
            foreach (IdReference toRemove in idsWithRecreateOnAwake)
            {
                potentialReferences.Remove(toRemove);
            }

            idReferenceSceneObjects = potentialReferences.ToArray();
            ids = new string[idReferenceSceneObjects.Length];
            Debug.Log($"[IdSceneManager] ScanSceneForIdReferenceObjects, found {idReferenceSceneObjects.Length}");

            for (int i = 0; i < idReferenceSceneObjects.Length; i++)
            {
                ids[i] = idReferenceSceneObjects[i].GetId();
            }
        }
#endif
    }
}
