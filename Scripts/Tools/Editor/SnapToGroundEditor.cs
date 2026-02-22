using UnityEditor;
using UnityEngine;

namespace Benito.ScriptingFoundations.Tools
{
    // Got it from here https://gist.github.com/s4lt3d/57d6d6740f092ce36de0a9d4622727c5
    // From yt channel: Walter's Bits https://www.youtube.com/watch?v=VvR4Dg2dVks
    // But modified by me a bit
    public class SnapToGroundEditor : Editor
    {
        private const string UNDO_GROUP_NAME = "Snap to Ground";

        [MenuItem("Tools/Walter's Bits/Snap to Ground Collider %g")]
        static void SnapToGroundCollider()
        {
            Undo.SetCurrentGroupName(UNDO_GROUP_NAME);

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Collider collider = gameObject.GetComponent<Collider>();
                if (!collider)
                {
                    Debug.LogWarning("GameObject " + gameObject.name + " does not have a collider.", gameObject);
                    continue;
                }

                Vector3 lowestPoint = collider.bounds.min;
                RaycastHit hit;
                if (Physics.Raycast(lowestPoint + Vector3.up * 0.1f, Vector3.down, out hit))
                {
                    Undo.RecordObject(gameObject.transform, UNDO_GROUP_NAME);

                    float distanceToMoveDown = Vector3.Distance(lowestPoint, hit.point);
                    gameObject.transform.position -= Vector3.up * distanceToMoveDown;
                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                }
            }
        }


        [MenuItem("Tools/Walter's Bits/Snap to Ground Renderer %h")]
        static void SnapToGroundMesh()
        {
            Undo.SetCurrentGroupName(UNDO_GROUP_NAME);

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
                if (!renderer)
                {
                    Debug.LogWarning("GameObject " + gameObject.name + " does not have a renderer.", gameObject);
                    continue;
                }

                Vector3 lowestPoint = renderer.bounds.min;
                RaycastHit hit;
                if (Physics.Raycast(lowestPoint + Vector3.up * 0.1f, Vector3.down, out hit))
                {
                    Undo.RecordObject(gameObject.transform, UNDO_GROUP_NAME);

                    float distanceToMoveDown = Vector3.Distance(lowestPoint, hit.point);
                    gameObject.transform.position -= Vector3.up * distanceToMoveDown;
                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                }
            }
        }
    }
}
