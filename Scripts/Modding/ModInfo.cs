using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Benito.ScriptingFoundations.Modding
{   
    [System.Serializable]
    public class ModInfo
    {
        public string name;
        public string description;
        public ModType modType;

        // Non Serializable, only used at runtime
        public FileInfo modInfoFileInfo;
        public FileInfo modBundleFileInfo;
    }
}


