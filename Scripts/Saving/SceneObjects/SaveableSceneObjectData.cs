using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Saving.SceneObjects
{
    public class SaveableSceneObjectData 
    {
        public string typeName;
        public string assemblyName;

        public int saveableObjectID;

        public void SetSerializationInfos<T>(int saveableObjectID)
        {
            this.typeName = typeof(T).FullName;
            this.assemblyName = typeof(T).Assembly.GetName().ToString();
            this.saveableObjectID = saveableObjectID;
        }
    }

}