using Benito.ScriptingFoundations.IdSystem;
using UnityEngine;


namespace Benito.ScriptingFoundations.Saving.SceneObjects
{
    /// <summary>
    /// Same as  SaveableSceneObject, but handles automatic data assignment.
    /// Automatically fills field alled "saveData" , use it
    /// </summary>
    public abstract class SaveableSceneObjectWithData<T> : SaveableSceneObject where T: SaveableSceneObjectData, new()
    {
        protected T saveData;


        public override void LoadCalledByManager(SaveableSceneObjectData dataToLoad)
        {
            saveData = (T)dataToLoad;
            Load(saveData);
        }

        public override SaveableSceneObjectData SaveCalledByManager()
        {
            saveData = new T();
            saveData.SetSerializationInfos<T>(Id.GetId());

            return Save();
        }

        public abstract T Save();

        public abstract void Load(T dataToLoad);
    }
}
