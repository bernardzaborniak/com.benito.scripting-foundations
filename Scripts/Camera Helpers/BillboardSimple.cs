using Benito.ScriptingFoundations.Managers;
using UnityEngine;
using static Benito.ScriptingFoundations.CameraHelpers.BillboardSimpleManager;

namespace Benito.ScriptingFoundations.CameraHelpers
{
    public class BillboardSimple : MonoBehaviour
    {
        public BillboardSimpleManager.RotationType rotationType;
        public BillboardSimpleManager.VerticalRotationType verticalRotationType;

        // will be added to position for creating rotation to camera -> let it be on eye height roughly
        public float yRotYOffset = 0;

        [Range(0.0f, 1.0f)]
        public float partialYRotationAmount = 0.2f;

        [Range(0.0f, 1.0f)]
        public float hybridRotationBlend = 0.2f;


        void OnEnable()
        {
            LocalSceneManagers.Get<BillboardSimpleManager>().RegisterBillboard(this);
        }

        void OnDisable()
        {
            LocalSceneManagers.Get<BillboardSimpleManager>().UnregisterBillboard(this);
        }
    }
}
