using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class DrawSimpleGizmo : MonoBehaviour
    {
        public Color color = Color.white;
        public float radius = 1;
            
        
        /// -- EDITOR --
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
#endif
    }
}