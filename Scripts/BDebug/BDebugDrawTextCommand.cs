using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BDebug
{
    public struct BDebugDrawTextCommand
    {
        public string text;
        public Vector3 position;
        public float size;
        public Color color;
        public float maxDrawDistance;
        public float scaleWithDistanceRatio;
        public bool overlay;

        public BDebugDrawTextCommand(string text, Vector3 position, float size, Color color, bool overlay, float scaleWithDistanceRatio, float maxDrawDistance)
        {
            this.text = text;
            this.position = position;
            this.size = size;
            this.color = color;
            this.maxDrawDistance = maxDrawDistance;
            this.scaleWithDistanceRatio = scaleWithDistanceRatio;
            this.overlay = overlay;
        }
    }
}
