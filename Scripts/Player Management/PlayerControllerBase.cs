using UnityEngine;

namespace Benito.ScriptingFoundations.PlayerManagement
{
    public abstract class PlayerControllerBase: MonoBehaviour
    {
        public abstract void Initialize();

        /// <summary>
        /// Player Controllers can later cast this manager into the type they need.
        /// </summary>
        /// <param name="manager"></param>
        public abstract void Possess(LocalPlayerManager manager);

        public abstract void StopPossessing();

    }
}