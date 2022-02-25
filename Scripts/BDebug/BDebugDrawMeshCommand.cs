using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BDebug
{
    public struct BDebugDrawMeshCommand
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public Color color;
        public bool useWireFrameShader;
        public float maxDrawDistance;

        public BDebugDrawMeshCommand(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool useWireFrameShader, float maxDrawDistance)
        {
            this.mesh = mesh;
            this.color = color;
            this.matrix = new Matrix4x4();
            matrix.SetTRS(position, rotation, scale);
            this.useWireFrameShader = useWireFrameShader;
            this.maxDrawDistance = maxDrawDistance;
        }
    }

 }
