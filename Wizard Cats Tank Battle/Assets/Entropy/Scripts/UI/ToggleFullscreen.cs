using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class ToggleFullscreen : MonoBehaviour
    {
        public Toggle Toggle;

        private void Start()
        {
            Toggle.isOn = Screen.fullScreen;
        }

        public void SetFullscreen(bool b)
        {
            Screen.fullScreen = b;
        }
    }
}