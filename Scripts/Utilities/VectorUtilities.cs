using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Utilities
{
    public static class VectorUtilities
    {
        #region Vector3
        public static Vector3 ToVector3_x0z(this Vector3 vector)
        {
            vector.y = 0;
            return vector;
        }
        public static Vector3 ToVector3_0y0(this Vector3 vector)
        {
            vector.x = vector.z = 0;
            return vector;
        }

        public static Vector2 ToVector2_xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        #endregion

        #region Vector2

        public static Vector3 ToVector3_x0z(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        #endregion
    }
}
