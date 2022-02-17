using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Modding.Editor
{
    public class CustomLevelModCreationWindow : EditorWindow
    {
        ModInfo createdModInfo = new ModInfo();

        [MenuItem("Tools/Benito/Create Custom Level Mod")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomLevelModCreationWindow>("Create Custom Level Mod");
            window.minSize = new Vector2(300, 500);
            window.Show();           
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            createdModInfo.name = EditorGUILayout.TextField("Mod Name", createdModInfo.name);
            createdModInfo.description = EditorGUILayout.TextField("Mod Description", createdModInfo.description, GUILayout.Height(64));
            createdModInfo.modType = ModType.CustomScenario;

            createdModInfo.previewImage = (Texture2D)EditorGUILayout.ObjectField("Mod Preview Image", createdModInfo.previewImage, typeof(Texture2D),false, GUILayout.Height(position.width * 0.7f));

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(createdModInfo.name))
            {
                if (GUILayout.Button("Create Mod from currently opened Scene"))
                {
                    string path = EditorUtility.OpenFolderPanel("Create Custom Level Mod", ModdingSettings.GetOrCreateSettings().lastExportModPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        ModCreator.CreateCustomLevelMod(createdModInfo, path);

                        SerializedObject serializedModdingSettings = new SerializedObject(ModdingSettings.GetOrCreateSettings());
                        serializedModdingSettings.FindProperty("lastExportModPath").stringValue = path;
                        serializedModdingSettings.ApplyModifiedProperties();
                    }
                }  
            }
            else
            {
                EditorGUILayout.HelpBox("The Mod needs a name", MessageType.Error);
            }
            

            if(!string.IsNullOrEmpty(ModdingSettings.GetOrCreateSettings().lastExportModPath))
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Open last exported Mod path"))
                {
                    EditorUtility.RevealInFinder(ModdingSettings.GetOrCreateSettings().lastExportModPath);
                }
            }
        }
    }
}

