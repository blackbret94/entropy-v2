using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.Util
{
    public class VersionText : MonoBehaviour
    {
        public Text Text;

        private void Start()
        {
            Text.text = "Build " + Application.version;
        }
    }
}