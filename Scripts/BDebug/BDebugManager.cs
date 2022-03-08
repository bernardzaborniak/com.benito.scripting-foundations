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
    public class BDebugManager : SingletonManagerGlobal
    {
        List<TextMeshPro> textMeshPool = new List<TextMeshPro>();
        List<TextMeshPro> usedTextMeshes = new List<TextMeshPro>();

        List<BDebugDrawMeshCommand> drawMeshCommands = new List<BDebugDrawMeshCommand>();
        List<BDebugDrawTextCommand> drawTextCommands = new List<BDebugDrawTextCommand>();
        List<BDebugDrawMultipleLinesCommand> drawLinesCommands = new List<BDebugDrawMultipleLinesCommand>();

        Material debugMaterial;
        Material debugTransparentMaterial;
        Material debugWireframeMaterial;
        Material debugLineMaterial;

        Material defaultTextMeshMaterial;
        Material overlayTextMeshMaterial;

        Mesh lineMesh;


        public override void InitialiseManager()
        {

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            BDebugSettings settings = BDebugSettings.GetOrCreateSettings();
            debugMaterial = settings.debugMeshMaterial;
            debugTransparentMaterial = settings.debugMeshTransparentMaterial;
            debugWireframeMaterial = settings.debugMeshWireframeMaterial;
            debugLineMaterial = settings.debugLineMaterial;
            defaultTextMeshMaterial = settings.defaultTextMeshMaterial;
            overlayTextMeshMaterial = settings.overlayTextMeshMaterial;

            lineMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            // this code is causing the crashing somehow
            GameObject textPrefab = new GameObject("textObject");
            TextMeshPro textMesh = textPrefab.AddComponent<TextMeshPro>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = "example text";

            for (int i = 0; i < settings.maxDebugTextsOnScreen; i++)
            {
                TextMeshPro text = Instantiate(textPrefab, transform).GetComponent<TextMeshPro>();
                textMeshPool.Add(text);
                textMeshPool[i].enabled = false;
            }

            Destroy(textPrefab);
#endif
        }

        public override void UpdateManager()
        {

        }

        public void LateUpdate()
        {
            float distanceToCameraSquared = 0;
            float distanceToCamera = 0;
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraPosition = cameraTransform.position;
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            int colorPropertyID = Shader.PropertyToID("_BaseColor");

            // Go through all commands and execute them 
            foreach (BDebugDrawMeshCommand command in drawMeshCommands)
            {
                distanceToCameraSquared = (command.matrix.GetPosition() - cameraPosition).sqrMagnitude;
                if (distanceToCameraSquared > command.maxDrawDistance * command.maxDrawDistance)
                    continue;

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
                distanceToCameraSquared = (command.lines[0].start - cameraPosition).sqrMagnitude;
                if (distanceToCameraSquared > command.maxDrawDistance * command.maxDrawDistance)
                    continue;

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




            //this code crashes with uCOntext - ive wrote to the dev and asked him to fix it... which he did at the same ay +1
            for (int i = 0; i < usedTextMeshes.Count; i++)
            {
                usedTextMeshes[i].enabled = false;
            }

            textMeshPool.AddRange(usedTextMeshes);
            usedTextMeshes.Clear();

            // Sort the texts via distance
            drawTextCommands.Sort(delegate (BDebugDrawTextCommand a, BDebugDrawTextCommand b)
            {
                return Vector3.Distance(cameraPosition, a.position)
                .CompareTo(
                  Vector3.Distance(cameraPosition, b.position));
            });

            foreach (BDebugDrawTextCommand command in drawTextCommands)
            {
                distanceToCamera = Vector3.Distance(command.position, cameraPosition);
                if (distanceToCamera > command.maxDrawDistance)
                    continue;

                if (textMeshPool.Count <= 0)
                    break;


                TextMeshPro textMesh = textMeshPool[0];

                textMeshPool.RemoveAt(0);
                usedTextMeshes.Add(textMesh);

                textMesh.enabled = true;
                textMesh.transform.position = command.position;
                textMesh.text = command.text;
                textMesh.fontSize = command.size * (distanceToCamera*command.scaleWithDistanceRatio);
                textMesh.color = command.color;
                textMesh.transform.forward = cameraTransform.forward;

                if (command.overlay)
                {
                    textMesh.fontMaterial = overlayTextMeshMaterial;
                }
                else
                {
                    textMesh.fontMaterial = defaultTextMeshMaterial;
                }


            }

            // Clear the lists
            drawMeshCommands.Clear();
            drawTextCommands.Clear();
            drawLinesCommands.Clear();
        }

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



