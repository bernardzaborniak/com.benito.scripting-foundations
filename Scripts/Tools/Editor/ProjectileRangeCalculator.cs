using Benito.ScriptingFoundations.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.Tools.Editor
{
    public class ProjectileRangeCalculator : EditorWindow
    {
        float buttonHeight = 28f;

        float launchAngle = 0f;
        float launchVelocity = 0;
        float heightDifference = 0;
        float gravity = 9.81f;

        float currentRange;

        float[] allAngles = new float[] {90,85,80,75,70,65,60,55,50,45,40,35,30,25,20,15,10,5,0,-5,-10,-15,-20,-25,-30,-35,-40,-45,-50,-55,-60,-65,-70,-75,-80,-85,-90};
        float[] allAngleRanges;

        bool initialized = false;



        [MenuItem("Tools/Benito/Projectile Range Calculator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectileRangeCalculator>("Projectile Range Calculator");
            window.Show();
        }

        private void OnGUI()
        {
            if(!initialized)
            {
                initialized = true;
                allAngleRanges = new float[allAngles.Length];
            }

            GUILayout.BeginVertical();
            {
                launchAngle = EditorGUILayout.FloatField("launchAngle", launchAngle);
                launchVelocity = EditorGUILayout.FloatField("launchVelocity", launchVelocity);
                heightDifference = EditorGUILayout.FloatField("heightDifference", heightDifference);
                gravity = EditorGUILayout.FloatField("gravity", gravity);

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontStyle = FontStyle.Bold;
                GUILayout.Label("resulting range: " + currentRange.ToString(), labelStyle);
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            if (GUILayout.Button("Calculate", GUILayout.Width(position.width), GUILayout.Height(buttonHeight)))
            {
                currentRange = ProjectileUtilities.CalculateProjectileRange(launchAngle, launchVelocity, heightDifference, gravity);

                for (int i = 0; i < allAngles.Length; i++)
                {
                    allAngleRanges[i] = ProjectileUtilities.CalculateProjectileRange(allAngles[i], launchVelocity, heightDifference, gravity);
                }
            }

            GUILayout.Space(25);
            GUILayout.Label("alternative angles:");
            for (int i = 0; i < allAngles.Length; i++)
            {
                GUILayout.Label(allAngles[i] + ": " + allAngleRanges[i].ToString("F2"));
            }
        }

    }
}
