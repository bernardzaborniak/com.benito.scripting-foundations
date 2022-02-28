using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

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
            //GUIStyle boldFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
            //boldFoldoutStyle.

            so.Update();

            SerializedProperty prop = so.GetIterator();
            prop.NextVisible(true);
            while (prop.NextVisible(false))
            {
                
                // I dont know why but I have to do this woraround or changes in arrays wont be saved :/
                if(prop.isArray && prop.isExpanded)
                {
                    prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName, EditorStyles.foldoutHeader);

                    EditorGUI.indentLevel++;
                    for (int i = 0; i < prop.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.PropertyField(prop);
                }
               

            }

            so.ApplyModifiedProperties();
        }

        public static Texture2D TakeEditModeScreenshot()
        {
            // source https://stackoverflow.com/questions/48663073/editor-window-screenshot#:~:text=After%20importing%20it%20to%20your,of%20the%20currently%20active%20EditorWindow.

            // Get actvive EditorWindow
            SceneView sceneWindow = EditorWindow.GetWindow<SceneView>();

            // Get screen position and sizes
            var vec2Position = sceneWindow.position.position;
            var sizeX = sceneWindow.position.width;
            var sizeY = sceneWindow.position.height;

            Debug.Log("width: " + sizeX);
            Debug.Log("height: " + sizeY);

            // Take Screenshot at given position sizes
            Color[] colors = InternalEditorUtility.ReadScreenPixel(vec2Position, (int)sizeX, (int)sizeY);

            // write result Color[] data into a temporal Texture2D
            Texture2D result = new Texture2D((int)sizeX, (int)sizeY);
            result.SetPixels(colors);

            return result;
        }
    }
}
