using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Benito.ScriptingFoundations.Saving
{
    public static class SaveableSceneObjectsIdAssigner
    {
        public static void AssignMissingIdsInCurrentScene()
        {
            List<SaveableSceneObject> saveableObjects  = new List<SaveableSceneObject>(GameObject.FindObjectsOfType<SaveableSceneObject>(true));
            HashSet<int> usedIds = new HashSet<int>();
            foreach (SaveableSceneObject item in saveableObjects)
            {
                if (item.HasId())
                {
                    if (!usedIds.Contains(item.GetId()))
                    {
                        usedIds.Add(item.GetId());
                        continue;
                    }
                    else
                    {
                        Debug.LogWarning("Current Scene had duplicate SaveIDs, isssue was fixed, may cause incompatibility of savegames");
                    }
                }

                int newID = CreateNewId(usedIds);
                item.SetId(newID);
                usedIds.Add(newID);
            }
        }

        public static void ReassignAllIdsInCurrentScene()
        {
            List<SaveableSceneObject> saveableObjects = new List<SaveableSceneObject>(GameObject.FindObjectsOfType<SaveableSceneObject>(true));
            HashSet<int> usedIds = new HashSet<int>();
            foreach (SaveableSceneObject item in saveableObjects)
            {                
                int newID = CreateNewId(usedIds);
                item.SetId(newID);
                usedIds.Add(newID);
            }
        }

        public static int CreateNewId(HashSet<int> usedIds)
        {
            int id = Random.Range(1, 200000000);

            // Recreate if it already exists
            while (usedIds.Contains(id))
            {
                id = Random.Range(1, 200000000);
            }

            return id;
        }
    }
}
