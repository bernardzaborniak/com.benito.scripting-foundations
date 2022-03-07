using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
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
            Debug.Log("initialize singleton");
            // Load all exisiting settings from folder.
            pathToLoadSettingsFrom = InGameSettingsSettings.GetOrCreateSettings().GetInGameSettingsFolderPath();
            DirectoryInfo directoryInfo = new DirectoryInfo(pathToLoadSettingsFrom);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.json"))
            {
                Debug.Log("fileinfo in folder: " + fileInfo.FullName);
                StreamReader reader = new StreamReader(fileInfo.FullName);
                string fileContent = reader.ReadToEnd();
                reader.Close();

                InGameSettings settingsGeneric = JsonUtility.FromJson<InGameSettings>(fileContent);
                //Debug.Log("settingsTypeName: "  + settings.settingsTypeName);

               
                Type settingsType = null;

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().ToString() == settingsGeneric.settingsAssemblyName)
                    {
                        settingsType = assembly.GetType(settingsGeneric.settingsTypeName);
                    }
                }

                InGameSettings settings = (InGameSettings)JsonUtility.FromJson(fileContent, settingsType);

                /*Debug.Log("looking for type: " + Path.GetFileNameWithoutExtension(fileInfo.FullName));

                Type settingsType = null;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    settingsType = assembly.GetType(Path.GetFileNameWithoutExtension(fileInfo.FullName));
                    if (settingsType != null)
                        break;

                }

                if (settingsType == null)
                    Debug.LogError($"Check the name of the InGameSettings.json file {fileInfo.Name} needs to have the same name as a type");
                */

                /*InGameSettingsJsonWrapper<InGameSettings> settingsWrapper = JsonUtility.FromJson<InGameSettingsJsonWrapper<InGameSettings>>(fileContent);
                Debug.Log("content: " + settingsWrapper.settingsTypeName);
                Debug.Log("content: " + settingsWrapper.settingsAssemblyName);
                Debug.Log("content: " + settingsWrapper.settings);

                Type correctedType = null;
                
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if(assembly.FullName == settingsWrapper.settingsAssemblyName)
                    {
                        correctedType = assembly.GetType(settingsWrapper.settingsTypeName);
                    }
                }


                InGameSettings settings = JsonUtility.FromJson<InGameSettingsJsonWrapper<correctedType>>(fileContent);*/

                //InGameSettingsJsonWrapper settingsWrapper = JsonUtility.FromJson<InGameSettingsJsonWrapper>(fileContent);
                //InGameSettings settings = (InGameSettings) JsonUtility.FromJson(fileContent, settingsType);
                //typeof(type) settings



                settingsDictionary.Add(settings.GetType(), settings);
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
                File.WriteAllText(Path.Combine(pathToLoadSettingsFrom, settingsToSave.RelativeSettingsPath), jsonFileContents);
            }
            else
            {
                Debug.LogError(settingsToSave + " are not present in the collection of loaded IngameSettings - could not be saved");
            }
        }

        public T GetSettings<T>() where T: InGameSettings, new()
        {
            // If settings dont exist yet - create them in the path and add to dictionary
            if (!settingsDictionary.ContainsKey(typeof(T)))
            {
                T newSettings = CreateNewSettingsFile<T>();
                settingsDictionary.Add(newSettings.GetType(), newSettings);
                UpdateDebugInfoOnLoadedSettings();
            }

            return (T)settingsDictionary[typeof(T)];
        }

        T CreateNewSettingsFile<T>() where T: InGameSettings, new()
        {
            T newSettings = new T();
            newSettings.settingsTypeName = typeof(T).FullName;
            newSettings.settingsAssemblyName = typeof(T).Assembly.GetName().ToString();
            //InGameSettingsJsonWrapper<T> settingsWrapper = new InGameSettingsJsonWrapper<T>(newSettings);

            string jsonFileContents = JsonUtility.ToJson(newSettings);
            File.WriteAllText(Path.Combine(pathToLoadSettingsFrom, newSettings.RelativeSettingsPath), jsonFileContents);  

            Debug.Log($"File {typeof(T)}.json was not present in the IngameSettings Folder, so it was automatically created");
            return newSettings;
        }
    }
}


