// Source: https://forum.unity.com/threads/drag-threshold-default-10-now.792879/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Vashta.Entropy.Util
{
    // Attach this to the object with your EventSystem and it will overwrite the Drag Threshold
    // on Start to change it relative to the device's current DPI, allowing it to be specified
    // in terms of physical distance instead of pixels
    public class PhysicalDragThreshold : MonoBehaviour
    {
        public float inchesDragThreshold = .05f;
        public float defaultDPI = 100;    // DPI to assume if we can't detect the device's
 
        // if touchThreshMultiplier > 1, then the threshold will be higher for touches than for
        // mouse input.  This works by checking the number of active touches every frame...
        public float touchThreshMultiplier = 4;
 
        private EventSystem evSys;
 
 
        void Start ()
        {
            evSys = GetComponent<EventSystem>();
            SetDragThreshold();
        }
 
        private void Update()
        {
            if (!Mathf.Approximately(touchThreshMultiplier, 1))
            {
                SetDragThreshold();
            }
        }
 
        public void SetDragThreshold()
        {
            if (evSys != null)
            {
                float dpi = Screen.dpi;
                if (dpi < 10) dpi = defaultDPI;
 
                if (Input.touchCount > 0) dpi *= touchThreshMultiplier;
 
                evSys.pixelDragThreshold = Mathf.RoundToInt(dpi * inchesDragThreshold);
            }
            else
            {
                Debug.LogWarning("PhysicalDragThreshold component should be on an object with EventSystem", gameObject);
            }
        }
    }
}