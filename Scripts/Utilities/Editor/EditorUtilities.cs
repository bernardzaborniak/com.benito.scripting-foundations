using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;

namespace Benito.ScriptingFoundations.Utilities.Editor
{
    public static class EditorUtilities
    {
        /// <summary>
        /// Returns the path the User is currently in in the project window.
        /// </summary>
        /// <returns></returns>
        public static string TryGetActiveProjectWindowFolderPath()
        {
            string path = "Assets";
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            if (found) path = (string)args[0];

            return path;
        }

        public static void DrawDefaultInspectorForSerializedObject(SerializedObject so)
        {
            so.Update();

            SerializedProperty prop = so.GetIterator();
            prop.NextVisible(true);
            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop);
            }

            so.ApplyModifiedProperties();
        }
    }
}
