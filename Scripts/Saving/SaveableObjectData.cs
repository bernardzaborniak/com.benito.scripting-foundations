using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;

namespace Benito.ScriptingFoundations.Saving
{
    // [System.Serializable]
    //[JsonObject(MemberSerialization.Fields)]
    public class SaveableObjectData 
    {
        public string typeName;
        public string assemblyName;

        public int saveableObjectID;

        public void SetSerializationInfos(string typeName, string assemblyName, int saveableObjectID)
        {
            this.typeName = typeName;
            this.assemblyName = assemblyName;
            this.saveableObjectID = saveableObjectID;
        }
    }

}