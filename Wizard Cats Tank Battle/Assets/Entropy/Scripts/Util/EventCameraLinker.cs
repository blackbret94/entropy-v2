using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class EventCameraLinker : MonoBehaviour
    {
        private void Awake()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("No canvas attached to EventCameraLinker");
                return;
            }
            
            canvas.worldCamera = Camera.main;
        }
    }
}