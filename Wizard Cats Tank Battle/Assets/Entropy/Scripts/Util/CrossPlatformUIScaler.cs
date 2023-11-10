using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.Util
{
    public class CrossPlatformUIScaler : MonoBehaviour
    {
        public CanvasScaler CanvasScaler;
        public Vector2 DesktopResolutionScale = new Vector2(1920, 1080);
        public Vector2 MobileResolutionScale = new Vector2(1280, 720);
        
        private void Start()
        {
            CanvasScaler.referenceResolution = IsLargeScreen() ? DesktopResolutionScale : MobileResolutionScale;
        }

        private bool IsLargeScreen()
        {
            bool isLargeScreen = false;

#if UNITY_STANDALONE
            isLargeScreen = true;
#endif
            
#if UNITY_IPHONE || UNITY_ANDROID
            string identifier = SystemInfo.deviceModel;
            if(identifier.StartsWith("iPhone"))
            {
                isLargeScreen = false;
            }
            else if(identifier.StartsWith("iPad"))
            {
                isLargeScreen = true;
            }
#endif

            return isLargeScreen;
        }
    }
}