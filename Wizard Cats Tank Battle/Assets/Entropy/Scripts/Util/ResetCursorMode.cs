using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class ResetCursorMode : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;  
        }
    }
}