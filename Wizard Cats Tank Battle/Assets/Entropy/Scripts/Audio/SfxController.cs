using UnityEngine;

namespace Entropy.Scripts.Audio
{
    public class SfxController : MonoBehaviour
    {
        public AudioClip
            BasicButtonClick,
            DramaticButtonClick,
            Purchase;
        
        public AudioSource Source;

        public void PlaySound(AudioClip clip)
        {
            Source.PlayOneShot(clip);
        }

        public void PlayBasicButtonClick()
        {
            PlaySound(BasicButtonClick);
        }

        public void PlayDramaticButtonClick()
        {
            PlaySound(DramaticButtonClick);
        }

        public void PlayPurchase()
        {
            PlaySound(Purchase);
        }
    }
}