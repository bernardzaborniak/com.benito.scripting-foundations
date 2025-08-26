using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;

namespace Benito.ScriptingFoundations.Modding
{
    /// <summary>
    /// Takes Care of managing, loading and unloading Mods.
    /// Should be together with the Global Managers.
    /// </summary>
    [AddComponentMenu("Benitos Scripting Foundations/ModManager")]
    public class ModManager : SingletonManagerGlobal
    {
        List<ModInfo> availabeMods = new List<ModInfo>();
        List<ModInfo> loadedMods = new List<ModInfo>();

        public List<ModInfo> GetAvailableMods()
        {
            return availabeMods;
        }

        public List<ModInfo> GetLoadedMods()
        {
            return loadedMods;
        }

        public override void InitialiseManager()
        {
            availabeMods = ModLoader.GetAllMods();
        }

        public override void UpdateManager()
        {

        }

        public void LoadMod(ModInfo modInfo)
        {
            ModLoader.LoadMod(modInfo);
            loadedMods.Add(modInfo);
        }

        public void UnloadMod(ModInfo modInfo)
        {
            if (loadedMods.Contains(modInfo))
            {
                ModLoader.UnloadMod(modInfo);
                loadedMods.Remove(modInfo);
            }
            else
            {
                Debug.LogError($"can't unload mod {modInfo.name} -> this mod was not loaded");
            }
        }

       
    }
}

