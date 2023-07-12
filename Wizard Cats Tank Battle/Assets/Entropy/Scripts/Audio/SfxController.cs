using UnityEngine;

namespace Entropy.Scripts.Audio
{
    public class SfxController : MonoBehaviour
    {
        public AudioClip
            BasicButtonClick,
            DramaticButtonClick,
            GoBackButtonClick,
            Purchase,
            CoinEarned,
            NoCoins,
            TeammateKilled;
        
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

        public void PlayNoCoins()
        {
            PlaySound(NoCoins);
        }

        public void PlayCoinEarnedSound()
        {
            PlaySound(CoinEarned);
        }

        public void PlayTeammateKilledSound()
        {
            PlaySound(TeammateKilled);
        }

        public void PlayGoBackSoundEffect()
        {
            PlaySound(GoBackButtonClick);
        }
    }
}