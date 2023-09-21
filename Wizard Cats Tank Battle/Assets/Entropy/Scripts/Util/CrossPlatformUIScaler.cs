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
#if UNITY_STANDALONE
            CanvasScaler.referenceResolution = DesktopResolutionScale;
#endif

#if UNITY_IPHONE || UNITY_ANDROID
            CanvasScaler.referenceResolution = MobileResolutionScale;
#endif
        }
    }
}