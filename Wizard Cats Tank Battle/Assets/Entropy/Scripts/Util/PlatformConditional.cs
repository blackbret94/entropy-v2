using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class PlatformConditional : MonoBehaviour
    {
        public bool AllowOnDesktop;
        public bool AllowOnMobile;


        private void Start()
        {
            // Handle Desktop
            #if UNITY_STANDALONE
            if (!AllowOnDesktop)
            {
                gameObject.SetActive(false);
            }
            #endif
            
            // Handle Mobile
            #if UNITY_ANDROID || UNITY_IPHONE
            if(!AllowOnMobile)
            {
                gameObject.SetActive(false);
            }
            #endif
        }

    }
}