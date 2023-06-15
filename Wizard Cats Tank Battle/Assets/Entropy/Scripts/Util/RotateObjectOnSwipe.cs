using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class RotateObjectOnSwipe : MonoBehaviour
    {
        public Transform TargetTransform;
        public Transform CameraTransform;
        public float Multiplier = 1f;
        
        private void Update()
        {
            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);
                if (touch.deltaPosition.x > touch.deltaPosition.y)
                {
                    TargetTransform.Rotate(CameraTransform.right * touch.deltaPosition.x * Multiplier, Space.World);
                }
                else
                {
                    TargetTransform.Rotate(CameraTransform.up * touch.deltaPosition.y * Multiplier, Space.World);
                }
            }
        }
    }
}