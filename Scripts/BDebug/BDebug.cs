using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Benito.ScriptingFoundations.Managers;

namespace Benito.ScriptingFoundations.BDebug
{
    public static class BDebug
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawText(string text, Vector3 position, float size, Color color, bool overlay, float scaleWithDistanceRatio = 0, float maxDrawDistance = 50)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawTextCommand(new BDebugDrawTextCommand(text, position, size, color, overlay, scaleWithDistanceRatio, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float thickness = 1, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawLinesCommand(new BDebugDrawMultipleLinesCommand(new BDebugLineDrawingParams(start, end), color, thickness, maxDrawDistance));
        }


        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawLineWithDistance(Vector3 start, Vector3 end, Color lineColor, float lineThickness, float textSize, Color textColor, bool overlayText, float scaleTextWithDistanceRatio = 0, float maxDrawDistance = 100)
        {
            DrawLine(start, end, lineColor, lineThickness, maxDrawDistance);
            Vector3 lineVector = end - start;
            DrawText(lineVector.magnitude.ToString("F2") + " m", start + lineVector * 0.5f, textSize, textColor, overlayText, scaleTextWithDistanceRatio, maxDrawDistance);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(mesh, position, rotation, scale, color, false, maxDrawDistance));

        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            UnityEngine.Profiling.Profiler.BeginSample("AddDrawMeshCommand");

            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(mesh, position, rotation, scale, color, true, maxDrawDistance));
            UnityEngine.Profiling.Profiler.EndSample();

        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawSphere(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), position, rotation, scale, color, false, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireSphere(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            // TODO Draw with lines instead of wiremesh shader?
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), position, rotation, scale, color, true, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), position, rotation, scale, color, false, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), position, rotation, scale, color, true, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Cylinder.fbx"), position, rotation, scale, color, false, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float maxDrawDistance = 100)
        {
            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(Resources.GetBuiltinResource<Mesh>("Cylinder.fbx"), position, rotation, scale, color, true, maxDrawDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawPoint(Vector3 position, Color color, float maxDrawingDistance = 50)
        {
            BDebugLineDrawingParams[] lines = new BDebugLineDrawingParams[]
                {
                    new BDebugLineDrawingParams(position + Vector3.right * 0.5f, position + Vector3.left * 0.5f),
                    new BDebugLineDrawingParams(position + Vector3.up * 0.5f, position + Vector3.down * 0.5f),
                    new BDebugLineDrawingParams(position + Vector3.forward * 0.5f, position + Vector3.back * 0.5f),
                };

            GlobalSingletonManager.Get<BDebugManager>().AddDrawLinesCommand(new BDebugDrawMultipleLinesCommand(lines, color, 1, maxDrawingDistance));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireCircle(Vector3 position, Vector3 normal, float radius, Color color, int verticesNumber = 24, float maxDrawingDistance = 50, float lineThickness = 1)
        {
            BDebugLineDrawingParams[] lines = new BDebugLineDrawingParams[verticesNumber];

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position,Quaternion.LookRotation(Vector3.Slerp(normal, -normal, 0.5f),normal),Vector3.one*radius);

            Vector3 lastPoint = matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 nextPoint = Vector3.zero;

            float scalar =  2*Mathf.PI / verticesNumber;
            for (int i = 0; i < verticesNumber; i++)
            {
                nextPoint =  matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos((i+1)* scalar), 0, Mathf.Sin((i+1)* scalar)));

                lines[i] = new BDebugLineDrawingParams(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }

            GlobalSingletonManager.Get<BDebugManager>().AddDrawLinesCommand(new BDebugDrawMultipleLinesCommand(lines, color, lineThickness, maxDrawingDistance));

        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireViewCone(Vector3 position, Vector3 direction, float distance, float angle, Color color, int verticesNumber = 24, float maxDrawingDistance = 50, float lineThickness = 1)
        {
            // TODO just draw 2 arcs?
            DrawWireArc(position, Vector3.up, direction, distance, angle, color, verticesNumber, maxDrawingDistance, lineThickness);
            DrawWireArc(position, Vector3.right, direction, distance, angle, color, verticesNumber, maxDrawingDistance, lineThickness);
            float circleRadius = Mathf.Sin(angle * 0.5f * Mathf.Deg2Rad) * distance;
            float circleCenterDistance = Mathf.Cos(angle* 0.5f * Mathf.Deg2Rad) * distance;
            DrawWireCircle(position + direction * circleCenterDistance, direction, circleRadius, color, verticesNumber, maxDrawingDistance, lineThickness);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawWireArc(Vector3 position, Vector3 normal, Vector3 arcDirection, float arcLength, float arcAngle, Color color, int verticesNumber = 24, float maxDrawingDistance = 50, float lineThickness = 1)
        {
            BDebugLineDrawingParams[] lines = new BDebugLineDrawingParams[verticesNumber + 1];

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, Quaternion.LookRotation(arcDirection, normal), Vector3.one*arcLength);

            float scalar = (arcAngle * Mathf.Deg2Rad) / (verticesNumber-1);
            float startAngle = Mathf.PI / 2 - (arcAngle * Mathf.Deg2Rad) / 2;
            float endAngle = Mathf.PI / 2 + (arcAngle * Mathf.Deg2Rad) / 2;

            Vector3 lastPoint = matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(startAngle), 0, Mathf.Sin(startAngle)));
            Vector3 nextPoint = Vector3.zero;

            for (int i = 0; i < verticesNumber-1; i++)
            {
                nextPoint =  matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(startAngle + (i + 1) * scalar), 0, Mathf.Sin(startAngle + (i + 1) * scalar)));

                lines[i] = new BDebugLineDrawingParams(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }

            // Add the last 2 lines for the arc
            lines[verticesNumber-1] = new BDebugLineDrawingParams(position,  matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(startAngle), 0, Mathf.Sin(startAngle))));
            lines[verticesNumber] = new BDebugLineDrawingParams(position, matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(endAngle), 0, Mathf.Sin(endAngle))));

            GlobalSingletonManager.Get<BDebugManager>().AddDrawLinesCommand(new BDebugDrawMultipleLinesCommand(lines, color, lineThickness, maxDrawingDistance));

        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DrawArc(Vector3 position, Vector3 normal, Vector3 arcDirection, float arcLength, float arcAngle, Color color, int verticesNumber = 24, float maxDrawingDistance = 50)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[verticesNumber + 1];
            int[] triangles = new int[(verticesNumber -1)*3];

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, Quaternion.LookRotation(arcDirection, normal), Vector3.one * arcLength);

            float scalar = (arcAngle * Mathf.Deg2Rad) / (verticesNumber-1);
            float startAngle = Mathf.PI / 2 + (arcAngle * Mathf.Deg2Rad) / 2;
            float endAngle = Mathf.PI / 2 - (arcAngle * Mathf.Deg2Rad) / 2;

            // add the start vertex at the end
            vertices[verticesNumber] = position;

            vertices[0] = matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(startAngle), 0, Mathf.Sin(startAngle)));

            int triangleIndex = 0;

            for (int i = 1; i < verticesNumber; i++)
            {
                vertices[i] = matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(startAngle - i * scalar), 0, Mathf.Sin(startAngle - i * scalar)));
                triangles[triangleIndex] = verticesNumber;
                triangles[triangleIndex+1] = i - 1;
                triangles[triangleIndex+2] = i;

                triangleIndex += 3;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            GlobalSingletonManager.Get<BDebugManager>().AddDrawMeshCommand(new BDebugDrawMeshCommand(mesh, Vector3.zero, Quaternion.identity, Vector3.one, color, false, maxDrawingDistance));
        }
    }
}
