using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class MouseDragRotate : MonoBehaviour
    {
        public bool RotateX = true;
        public bool RotateY = true;
        
        public float rotationSpeed = 0.2f;
 
        void OnMouseDrag()
        {
            if (RotateX)
            {
                float XaxisRotation = Input.GetAxis("Mouse X")*rotationSpeed;
                transform.RotateAround (Vector3.down, XaxisRotation);
            }

            if (RotateY)
            {
                float YaxisRotation = Input.GetAxis("Mouse Y")*rotationSpeed;
                transform.RotateAround (Vector3.right, YaxisRotation);
            }
        }
    }
}
