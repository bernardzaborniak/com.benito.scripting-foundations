using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Benito.ScriptingFoundations.BDebug
{
    public struct BDebugLineDrawingParams
    {
        public Vector3 start;
        public Vector3 end;

        public BDebugLineDrawingParams(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
