using TanksMP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.UI.MapSelection;

namespace Entropy.Scripts.Audio
{
    public class MusicController : MonoBehaviour
    {
        public AudioSource AudioSource;
        public MapDefinition MapDefinition;

        private int _currentTrackIndex = 0;

        private void Awake()
        {
            AudioSource.clip = MapDefinition.MusicTracks[_currentTrackIndex];
            SceneManager.sceneLoaded += OnSceneLoaded;

            AudioSource.enabled = bool.Parse(PlayerPrefs.GetString(PrefsKeys.playMusic));
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(AudioSource != null)
                AudioSource.Stop();
        }

        private void Update()
        {
            if (!AudioSource.enabled || AudioSource.isPlaying)
                return;

            _currentTrackIndex++;
            if (_currentTrackIndex >= MapDefinition.MusicTracks.Length)
                _currentTrackIndex = 0;

            AudioSource.clip = MapDefinition.MusicTracks[_currentTrackIndex];
            AudioSource.Play();
        }

        public void PlayMusic()
        {
            if(AudioSource.enabled)
                AudioSource.Play();
        }
    }
}