using Benito.ScriptingFoundations.Managers;
using UnityEngine;

namespace Benito.ScriptingFoundations.CameraHelpers
{

    /// <summary>
    /// Put an disabled Screenshot camera as this objects child
    /// </summary>
    public class ScreenshotCreator : SingletonManagerGlobal
    {
        [SerializeField] Camera screenShotCamera;


        public override void InitialiseManager()
        {
            if (screenShotCamera == null)
            {
                GameObject camObject = new GameObject("Screenshot Camera");
                camObject.transform.parent = transform;
                screenShotCamera = camObject.AddComponent<Camera>();
            }

            screenShotCamera.enabled = false;            
        }

        public override void UpdateManager()
        {
        }

        public Texture2D CreateScreenshot(Camera cameraToCopyFrom, int pixelWidth, int pixelHeight)
        {
            return CreateScreenshot(cameraToCopyFrom.transform.position, cameraToCopyFrom.transform.rotation, cameraToCopyFrom.fieldOfView, pixelWidth, pixelHeight);
        }

        public Texture2D CreateScreenshot(Vector3 position, Quaternion rotation, float fieldOfView, int pixelWidth, int pixelHeight)
        {
            screenShotCamera.transform.position = position;
            screenShotCamera.transform.rotation = rotation;
            screenShotCamera.fieldOfView = fieldOfView;

            int width = pixelWidth;
            int height = pixelHeight;
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            screenShotCamera.targetTexture = renderTexture;

            var previousRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            screenShotCamera.Render();

            Texture2D image = new Texture2D(width, height);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();

            RenderTexture.ReleaseTemporary(renderTexture);

            RenderTexture.active = previousRenderTexture;

            return image;
        }
    }
}
