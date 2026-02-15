using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Benito.ScriptingFoundations.IdSystem
{
    public static class SceneObjectsIdAssigner
    {
        public static void AssignMissingIdsInCurrentScene()
        {
            List<IdReference> idObjects = new List<IdReference>(GameObject.FindObjectsByType<IdReference>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            HashSet<string> usedIds = new HashSet<string>();

            foreach (IdReference item in idObjects)
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
                        item.CreateNewId();
                        Debug.LogWarning("Current Scene had duplicate SaveIDs, isssue was fixed, may cause incompatibility of savegames");
                        continue;
                    }
                }
                Debug.LogWarning("AssignMissingIdsInCurrentScene, assigned new Id, as item didnt had one");
                item.CreateNewId();
            }
        }
    }
}
