using UnityEngine;
using Vashta.Entropy.IO;

namespace Entropy.Scripts.Audio
{
    public class AudioListenerSettingsLoader : MonoBehaviour
    {
        private void Start()
        {
            AudioListener.volume = SettingsReader.GetVolume();
        }
    }
}