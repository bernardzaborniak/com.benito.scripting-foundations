//#define ENABLE_LOGS

using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Pools;
using TMPro;


namespace Benito.ScriptingFoundations.BDebug
{
    public class BDebugManager : Singleton
    {
        ComponentPool<TextMeshPro> textMeshPool = new ComponentPool<TextMeshPro>();

        public override void InitialiseSingleton()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
#endif
            // Spawn the texxtemsh object pool
        }

        public override void UpdateSingleton()
        {
            throw new System.NotImplementedException();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public void Draw(Transform  transform)
        {
            transform.position += Vector3.up * Time.deltaTime * 0.1f;
           // UnityEngine.Debug.Log("amma draw 2");
            //UnityEngine.Debug.DrawLine(transform.position ,Vector3.forward * 2, Color.blue);
        }

        public void AddMeshDrawCommand(BDebugMeshDrawCommand command)
        {

        }

        public void AddTextDrawCommand(BDebugTextDrawCommand command)
        {

        }

    }


}
