using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Benito.ScriptingFoundations.Utilities.UI
{
    public static class UIUtilities
    {
        public static List<RaycastResult> GetUIObjectsUnderMouse()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastsResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastsResultList);
            return raycastsResultList;
        }

        /// <summary>
        /// Ignores objects that have the MouseUIClickthrough Component
        /// </summary>
        /// <returns></returns>
        public static bool IsMouseOverUIWithIgnores()
        {
            List<RaycastResult> raycastsResultList = GetUIObjectsUnderMouse();

            for (int i = 0; i < raycastsResultList.Count; i++)
            {
                if (raycastsResultList[i].gameObject.GetComponent<MouseUIClickthrough>() != null)
                {
                    raycastsResultList.RemoveAt(i);
                    i--;
                }
            }

            return raycastsResultList.Count > 0;
        }
    }
}
