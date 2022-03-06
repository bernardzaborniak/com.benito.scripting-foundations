using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.InGameSettings
{
    public abstract class InGameSettings : ScriptableObject
    {
        public abstract string GetSettingsPath();
    }
}
