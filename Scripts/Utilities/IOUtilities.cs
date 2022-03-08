using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class IOUtilities 
    {
        public static void EnsurePathExists(string absolutePath)
        {
            if (Directory.Exists(absolutePath))
                return;

            string[] directories = absolutePath.Split(Path.DirectorySeparatorChar);

            string allPreviousDirectoriesCombined = "";
            foreach (string directory in directories)
            {
                if (allPreviousDirectoriesCombined == "")
                {
                    allPreviousDirectoriesCombined = directory;
                }
                else
                {
                    allPreviousDirectoriesCombined = Path.Combine(allPreviousDirectoriesCombined,directory);
                }

                if (!Directory.Exists(allPreviousDirectoriesCombined))
                {
                    Directory.CreateDirectory(allPreviousDirectoriesCombined);
                }
            }
        }

        [System.Serializable]
        public class AssigneableGameDataPath
        {
            public enum PathPrefix
            {
                PersistendData,
                GameData,
                None
            }
           
            [Tooltip("Combines persistent or game data path with the following string.")]
            [SerializeField] PathPrefix pathPrefix;
            [Tooltip("Leave out the / or \\ at the start of this string, use \\ for subfolders")]
            [SerializeField] string relativePath = "Mods";

            public AssigneableGameDataPath(PathPrefix defaultPathPrefix, string defaultRelativePath)
            {
                pathPrefix = defaultPathPrefix;
                relativePath = defaultRelativePath;

            }

            public string GetPath()
            {
                string path = null;

                if (relativePath == string.Empty)
                    return path;

                switch (pathPrefix)
                {
                    case PathPrefix.PersistendData:
                        {
                            path = Path.Combine(Application.persistentDataPath, relativePath);
                            break;
                        }

                    case PathPrefix.GameData:
                        {
                            path = Path.Combine(Application.dataPath, relativePath);
                            break;
                        }

                    case PathPrefix.None:
                        {
                            path = relativePath;
                            break;
                        }
                }

                return path;
            }
        }
    }
}

