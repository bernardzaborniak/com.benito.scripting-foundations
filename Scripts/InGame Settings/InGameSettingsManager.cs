using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.InspectorAttributes;

namespace Benito.ScriptingFoundations.InGameSettings
{
    public class InGameSettingsManager : Singleton
    {
        Dictionary<Type, InGameSettings> settingsDictionary = new Dictionary<Type, InGameSettings>();
        string pathToLoadSettingsFrom;


        // Debug Only
        [ReadOnly]
        [SerializeField] List<InGameSettings> loadedSettings;


        public override void InitialiseSingleton()
        {
            // Load all exisiting settings from folder.
            pathToLoadSettingsFrom = InGameSettingsSettings.GetOrCreateSettings().GetInGameSettingsFolderPath();
            DirectoryInfo directoryInfo = new DirectoryInfo(pathToLoadSettingsFrom);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("json"))
            {
                StreamReader reader = new StreamReader(fileInfo.FullName);
                string fileContent = reader.ReadToEnd();
                reader.Close();
                InGameSettings obj = JsonUtility.FromJson<InGameSettings>(fileContent);
            }

            UpdateDebugInfoOnLoadedSettings();
        }

        public override void UpdateSingleton()
        {
            //throw new NotImplementedException();
        }

        void UpdateDebugInfoOnLoadedSettings()
        {
            loadedSettings = new List<InGameSettings>(settingsDictionary.Values);
        }

        public void SaveChangedSettings(InGameSettings settingsToSave)
        {
            if (settingsDictionary.ContainsValue(settingsToSave))
            {
                string jsonFileContents = JsonUtility.ToJson(settingsToSave);
                File.WriteAllText(Path.Combine(pathToLoadSettingsFrom, settingsToSave.GetSettingsPath()), jsonFileContents);
            }
            else
            {
                Debug.LogError(settingsToSave + " are not present in the collection of loaded IngameSettings - could not be saved");
            }
        }

        public T GetSettings<T>() where T: InGameSettings
        {
            // If settings dont exist yet - create them in the path and add to dictionary
            if (!settingsDictionary.ContainsKey(typeof(T)))
            {
                T newSettings = ScriptableObject.CreateInstance<T>();
                string jsonFileContents = JsonUtility.ToJson(newSettings);
                File.WriteAllText(Path.Combine(pathToLoadSettingsFrom, newSettings.GetSettingsPath()), jsonFileContents);
                UpdateDebugInfoOnLoadedSettings();
            }

            return (T)settingsDictionary[typeof(T)];
        }
    }
}


