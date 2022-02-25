using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BDebug
{
    public struct BDebugDrawMultipleLinesCommand
    {
        public BDebugLineDrawingParams[] lines;
        public Color color;
        public float thickness;
        public float maxDrawDistance;


        public BDebugDrawMultipleLinesCommand(BDebugLineDrawingParams[] lines, Color color, float thickness, float maxDrawDistance)
        {
            thickness *= 0.01f;

            this.lines = lines;
            this.color = color;
            this.thickness = thickness;
            this.maxDrawDistance = maxDrawDistance;
        }

        public BDebugDrawMultipleLinesCommand(BDebugLineDrawingParams line, Color color, float thickness, float maxDrawDistance)
        {
            thickness *= 0.01f;

            lines = new BDebugLineDrawingParams[] { line };

            this.color = color;
            this.thickness = thickness;
            this.maxDrawDistance = maxDrawDistance;
        }
    }
}
