using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.BSceneManagement.Editor
{
    

    [CustomEditor(typeof(BSceneTransitionManager))]
    public class BSceneTransitionManagerEditor : UnityEditor.Editor
    {
        BSceneTransitionManager manager;

        void OnEnable()
        {
            manager = (target as BSceneTransitionManager);
        }

        public override void OnInspectorGUI()
        {
            if (manager == null)
                return;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Transition: ");
                EditorGUILayout.LabelField(manager.GetCurrentTransitionName());
            }
            EditorGUILayout.EndHorizontal();

            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Transition Stage: ");
                EditorGUILayout.LabelField(manager.GetCurrentTransitionStage());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Transition Progress: ");
                EditorGUILayout.LabelField(manager.TransitionProgress.ToString());
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}
