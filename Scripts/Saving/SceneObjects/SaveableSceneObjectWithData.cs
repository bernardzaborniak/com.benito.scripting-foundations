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

        protected virtual void Awake()
        {
            if (saveData == null)
            {
                saveData = new T();

                saveData.SetSerializationInfos<T>(Id.GetId());
            }
        }
    }
}
