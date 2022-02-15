using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.Linq;

namespace Benito.ScriptingFoundations.LayerMaskOrganiser.Editor
{
    public class LayerMaskOrganiser : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/LayerMask Organiser";

        public static LayerMaskOrganiser GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<LayerMaskOrganiser>(DefaultSettingsPathInResourcesFolder);
        }

        public enum LayerType
        {
            Hybrid,
            Visual,
            Physical
        }

        //public List<int> layerInfoDictKeyList = new List<int>();
        //public List<LayerInfo> layerInfoDictValueList = new List<LayerInfo>();
        public LayerInfo[] layerInfosList = new LayerInfo [32];
        //public Dictionary<int, LayerInfo> layerInfoDict = new Dictionary<int, LayerInfo>();

        [System.Serializable]
        public class LayerInfo
        {
            public LayerType type;
            [TextArea]
            public string description;
        }

        /* [System.Serializable]
         public class LayerInfoObject
         {
             [HideInInspector]
             public string displayLayerName;
             [HideInInspector]
             public string layerName;
             public LayerType layerType;
             [TextArea]
             public string layerDescription;
         }

         [SerializeField] List<LayerInfoObject> layerInfos = new List<LayerInfoObject>();

         private void OnValidate()
         {
             // Get list of all layers
             List<string> layerNamesToAddToTheOrganiser = new List<string>();
             for (int i = 0; i < 31; i++)
             {
                 string layerName = LayerMask.LayerToName(i);
                 if (layerName.Length > 0)
                     layerNamesToAddToTheOrganiser.Add(layerName);
             }

             // Decide which to add, which to remove
             List<LayerInfoObject> layerInfosToRemove = new List<LayerInfoObject>();

             foreach(LayerInfoObject layerInfo in layerInfos)
             {
                 if (!layerNamesToAddToTheOrganiser.Contains(layerInfo.layerName))
                 {
                     layerInfosToRemove.Add(layerInfo);
                 }
                 else
                 {
                     layerNamesToAddToTheOrganiser.Remove(layerInfo.layerName);
                 }
             }

             // Remove
             foreach (LayerInfoObject layerInfo in layerInfosToRemove)
             {
                 layerInfos.Remove(layerInfo);
             }

             // Add
             foreach (string layerName in layerNamesToAddToTheOrganiser)
             {
                 LayerInfoObject layerInfo = new LayerInfoObject();
                 layerInfo.displayLayerName = $"{LayerMask.NameToLayer(layerName).ToString()} - {layerName}";
                 layerInfo.layerName = layerName;

                 layerInfos.Add(layerInfo);
             }

             layerInfos.Sort(delegate (LayerInfoObject a, LayerInfoObject b)
             {
                 return LayerMask.NameToLayer(a.layerName).CompareTo(LayerMask.NameToLayer(b.layerName));
             }
             );
         }*/
    }
}
