using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Benito.ScriptingFoundations.InspectorAttributes.Editor
{
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ButtonDrawer : UnityEditor.Editor
    {
        List<Button> buttons = new List<Button>();

        private void OnEnable()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = target.GetType().GetMethods(flags);

            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute != null)
                {
                    buttons.Add(new Button(method,buttonAttribute.displayName));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawButtons();
        }

        void DrawButtons()
        {
            foreach (Button button in buttons)
            {
                if (GUILayout.Button(button.Name))
                {
                    button.Method.Invoke(target , null);
                }
            }
        }
    }
}
