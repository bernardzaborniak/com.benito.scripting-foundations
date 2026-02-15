using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.SceneObjects
{
    public class SaveableSceneObjectData 
    {
        public string typeName;
        public string assemblyName;

        public string saveableObjectID;

        public void SetSerializationInfos<T>(string saveableObjectID)
        {
            this.typeName = typeof(T).FullName;
            this.assemblyName = typeof(T).Assembly.GetName().ToString();
            this.saveableObjectID = saveableObjectID;
        }
    }

}