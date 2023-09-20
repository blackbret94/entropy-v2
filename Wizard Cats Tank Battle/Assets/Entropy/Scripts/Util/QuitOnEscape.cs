using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class QuitOnEscape : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}