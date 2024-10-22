using System.Collections.Generic;
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
            TeammateKilled,
            UltimateReady,
            UltimateNotReady,
            CantShoot;

        private Dictionary<AudioClip, float> _clipLastPlayedDict;
        private bool _hasInit;
        
        public AudioSource Source;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;

            _clipLastPlayedDict = new Dictionary<AudioClip, float>();

            _hasInit = true;
        }

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

        public void PlayUltimateReady()
        {
            PlaySound(UltimateReady);
        }

        public void PlayUltimateNotReady()
        {
            PlaySound(UltimateNotReady);
        }

        public void PlayCantShoot(float minTimeSinceLastPlayed = 0f)
        {
            if (ShouldPlay(CantShoot, minTimeSinceLastPlayed))
            {
                PlaySound(CantShoot);
                
                SetLastPlayed(CantShoot);
            }
        }

        private void SetLastPlayed(AudioClip audioClip)
        {
            Init();

            _clipLastPlayedDict[audioClip] = Time.time;
        }

        private bool ShouldPlay(AudioClip audioClip, float minTimeSinceLastPlayed)
        {
            Init();
            
            if (minTimeSinceLastPlayed < .01)
                return true;

            if (!_clipLastPlayedDict.ContainsKey(audioClip))
                return false;

            float lastPlayedTime = _clipLastPlayedDict[audioClip];

            return (Time.time - lastPlayedTime) > minTimeSinceLastPlayed;
        }
    }
}