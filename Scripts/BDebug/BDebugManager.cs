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

        List<BDebugDrawMeshCommand> drawMeshCommands = new List<BDebugDrawMeshCommand>();
        List<BDebugDrawTextCommand> drawTextCommands = new List<BDebugDrawTextCommand>();
        List<BDebugDrawMultipleLinesCommand> drawLinesCommands = new List<BDebugDrawMultipleLinesCommand>();

        Material debugMaterial;
        Material debugTransparentMaterial;
        Material debugWireframeMaterial;
        Material debugLineMaterial;

        Mesh lineMesh;

        public override void InitialiseSingleton()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            BDebugSettings settings = BDebugSettings.GetOrCreateSettings();
            debugMaterial = settings.debugMeshMaterial;
            debugTransparentMaterial = settings.debugMeshTransparentMaterial;
            debugWireframeMaterial = settings.debugMeshWireframeMaterial;
            debugLineMaterial = settings.debugLineMaterial;

            lineMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

#endif
            // Spawn the texxtemsh object pool
        }

        public override void UpdateSingleton()
        {
            //throw new System.NotImplementedException();
        }

        public void LateUpdate()
        {
            float distanceToCameraSquared = 0;
            float angleTowardsCamera;
            Vector3 cameraPosition = Camera.main.transform.position;
            //Vector3 cameraForward = Camera.current.transform.forward;
            //Vector3 directionFromCameraToMesh;
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            int colorPropertyID = Shader.PropertyToID("_BaseColor");

            // Go through all commands and execute them 

            foreach (BDebugDrawMeshCommand command in drawMeshCommands)
            {
                // 1 Check Distance
                distanceToCameraSquared = (command.matrix.GetPosition() - cameraPosition).sqrMagnitude;
                if (distanceToCameraSquared > command.maxDrawDistance * command.maxDrawDistance)
                    continue;


                // 2 Check Angle?

                propertyBlock.SetColor(colorPropertyID, command.color);

                Material materialToUse = debugMaterial;
                if (command.useWireFrameShader)
                    materialToUse = debugWireframeMaterial;
                else if (command.color.a < 1)
                    materialToUse = debugTransparentMaterial;

                propertyBlock.SetColor(colorPropertyID, command.color);
  
                Graphics.DrawMesh(
                    command.mesh,
                    command.matrix,
                    materialToUse,
                    0,
                    null,
                    0,
                    propertyBlock);               
            }

            foreach (BDebugDrawMultipleLinesCommand command in drawLinesCommands)
            {
                propertyBlock.SetColor(colorPropertyID, command.color);

                


                foreach (BDebugLineDrawingParams lineParams in command.lines)
                {
                    
                    Vector3 directionVector = lineParams.end - lineParams.start;
                    float distance = directionVector.magnitude;
                    Vector3 scale = new Vector3(command.thickness, command.thickness, distance);
                    Vector3 position = lineParams.start + directionVector * 0.5f;
                    Quaternion rotation = Quaternion.LookRotation(directionVector);

                    Matrix4x4 matrix = new Matrix4x4();
                    matrix.SetTRS(position, rotation, scale);

                    Graphics.DrawMesh(
                        lineMesh,
                        matrix,
                        debugLineMaterial,
                        0,
                        null,
                        0,
                        propertyBlock);
                    
                }
            }


            // Clear the lists
            drawMeshCommands.Clear();
            drawTextCommands.Clear();
            drawLinesCommands.Clear();
        }

        /* [Conditional("UNITY_EDITOR")]
         [Conditional("DEVELOPMENT_BUILD")]
         public void Draw(Transform  transform)
         {
             transform.position += Vector3.up * Time.deltaTime * 0.1f;
            // UnityEngine.Debug.Log("amma draw 2");
             //UnityEngine.Debug.DrawLine(transform.position ,Vector3.forward * 2, Color.blue);
         }*/

        public void AddDrawMeshCommand(BDebugDrawMeshCommand command)
        {
            drawMeshCommands.Add(command);
        }

        public void AddDrawTextCommand(BDebugDrawTextCommand command)
        {
            drawTextCommands.Add(command);
        }

        public void AddDrawLinesCommand(BDebugDrawMultipleLinesCommand command)
        {
            drawLinesCommands.Add(command);
        }
    }
}



