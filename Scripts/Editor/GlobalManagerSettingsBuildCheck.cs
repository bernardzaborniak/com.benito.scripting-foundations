using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Benitos.ScriptingFoundations.Managers.Editor
{
    /// <summary>
    /// Makes sure a Settings asset exists before Build.
    /// This code is executed before the build beginns contrary to the "PostProcessBuild" Attribute
    /// </summary>
    public class GlobalManagerSettingsBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder => throw new System.NotImplementedException();

        public void OnPreprocessBuild(BuildReport report)
        {
            //Make sure settings file exists before build
            GlobalManagersSettings.GetOrCreateSettings();
        }
    }
}
