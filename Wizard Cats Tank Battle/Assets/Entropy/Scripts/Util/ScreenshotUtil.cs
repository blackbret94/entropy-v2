using System;
using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class ScreenshotUtil: MonoBehaviour
    {
        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                string filename = $"{Application.persistentDataPath}/{Screen.width}x{Screen.height}-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
                
                ScreenCapture.CaptureScreenshot(filename);
                Debug.Log("Screenshot taken: " +  filename);
            }
        }
        #endif
    }
}