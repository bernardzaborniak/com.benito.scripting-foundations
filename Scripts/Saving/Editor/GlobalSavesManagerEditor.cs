using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Saving.Editor
{
    [CustomEditor(typeof(GlobalSavesManager))]
    public class GlobalSavesManagerEditor : UnityEditor.Editor
    {
        GlobalSavesManager manager;

        void OnEnable()
        {
            manager = (target as GlobalSavesManager);
        }

        public override void OnInspectorGUI()
        {
            if (manager == null)
                return;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Manager State: ");
                EditorGUILayout.LabelField(manager.ManagerState.ToString());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Progress: ");

                if(manager.ManagerState == GlobalSavesManager.State.LoadingSceneSave)
                {
                    EditorGUILayout.LabelField(manager.ReadSceneSaveFileProgress.ToString("F2"));
                }
                else if(manager.ManagerState == GlobalSavesManager.State.CreatingSceneSave)
                {
                    EditorGUILayout.LabelField(manager.CreateSceneSaveFileProgress.ToString("F2"));
                }
            }
            EditorGUILayout.EndHorizontal();

            // Draw the default inspector 
            base.OnInspectorGUI();
        }
    }
}
