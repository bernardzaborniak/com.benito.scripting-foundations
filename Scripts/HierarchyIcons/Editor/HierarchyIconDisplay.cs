using UnityEngine;
using UnityEditor;
using System;
using NUnit.Framework.Constraints;
using System.Linq;


namespace Benito.ScriptingFoundations.HierarchyIcons
{
    /// <summary>
    /// Draws Hierarchy icons based on the icon of the upper most Component.
    /// Impemented based on Warped Imaginations tutorial https://www.youtube.com/watch?v=EFh7tniBqkk&t=5s
    /// </summary>
    [InitializeOnLoad] // Calls the constructor of this class when Unity Starts
    public static class HierarchyIconDisplay
    {
        static bool _hierarchyHasFocus = false;
        static EditorWindow _hierarchyEditorWindow;

        static HierarchyIconDisplay()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if(_hierarchyEditorWindow == null)
                _hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

            _hierarchyHasFocus = EditorWindow.focusedWindow != null 
                && EditorWindow.focusedWindow == _hierarchyEditorWindow;

        }

        private static void OnHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject; // Get the object of the icon
            if (obj == null)
                return;

            // If its a prefab, draw the default version
            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null)
                return;

            Component[]components = obj.GetComponents<Component>();
            if(components == null || components.Length == 0) 
                return;

            Component component = components.Length > 1 ? components[1] : components[0]; // components [0] is transform // adjust priority list if needed here

            if(component == null) return; // additional checks as sometimes editor became stuck

            Type type = component.GetType();

            // Create an optional tooltip
            GUIContent content = EditorGUIUtility.ObjectContent(component, type);
            content.text = null;
            content.tooltip = type.Name;

            if (content.image == null) // Some components dont have imagages?
                return;

            // Overdraw the original rectangle, this is quite difficult
            bool isSelected = Selection.instanceIDs.Contains(instanceId);
            bool isHovering = selectionRect.Contains(Event.current.mousePosition);

            Color color = UnityEditorBackgroundColor.Get(isSelected, isHovering, _hierarchyHasFocus);
            Rect backgroundRect = selectionRect;
            backgroundRect.width = 18.5f;
            EditorGUI.DrawRect(backgroundRect,color);

            EditorGUI.LabelField(selectionRect, content);
        }

    }
}


