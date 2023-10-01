using System;
using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class LockRotation : MonoBehaviour
    {
        public Vector3 Rotation;

        // Could optimize by copying the quaternion each time instead of creating a new one from Euler
        private void LateUpdate()
        {
            transform.localRotation = Quaternion.Euler(Rotation);
        }
    }
}