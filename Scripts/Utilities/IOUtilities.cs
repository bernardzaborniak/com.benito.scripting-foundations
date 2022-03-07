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
    }
}

