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
        public Texture2D previewImage;

        // Non Serializable, only used at runtime
        public FileInfo modInfoFileInfo;
        public FileInfo modBundleFileInfo;

        public bool loaded;
        public AssetBundle loadedModBundle;
        public string customSceneName;
    }
}


