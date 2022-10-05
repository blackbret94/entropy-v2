using UnityEngine;

namespace Entropy.Scripts.Audio
{
    public class SfxController : MonoBehaviour
    {
        public AudioSource Source;

        public void PlaySound(AudioClip clip)
        {
            Source.PlayOneShot(clip);
        }
    }
}