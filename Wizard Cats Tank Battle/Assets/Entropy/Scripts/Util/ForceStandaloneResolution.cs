using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class ForceStandaloneResolution: MonoBehaviour
    {
        public int Width;
        public int Height;
        
        #if UNITY_STANDALONE
        private void Start()
        {
            Screen.SetResolution(Width, Height, false);
        }
        #endif
    }
}