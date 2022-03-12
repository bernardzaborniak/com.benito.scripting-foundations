using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.BSceneManagement.Editor
{
    [CustomEditor(typeof(BSceneManager))]
    public class BSceneManagerEditor : UnityEditor.Editor
    {
        BSceneManager manager;

        void OnEnable()
        {
            manager = (target as BSceneManager);
        }

        public override void OnInspectorGUI()
        {
            if (manager == null)
                return;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Manager State: ");
                EditorGUILayout.LabelField(manager.GetCurrentState());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Transition: ");
                EditorGUILayout.LabelField(manager.GetCurrentTransitionName());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current State: ");
                EditorGUILayout.LabelField(manager.GetCurrentTransitionStage());
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
