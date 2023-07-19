/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TanksMP
{
    /// <summary>
    /// Handles playback of background music, 2D and 3D one-shot clips during the game.
    /// Makes use of the PoolManager for activating 3D AudioSources at desired world positions.
    /// </summary>
	public class AudioManager : MonoBehaviour
	{
        //reference to this script instance
		private static AudioManager instance;
		
        /// <summary>
        /// AudioSource for playing back one-shot 2D clips.
        /// </summary>
		public AudioSource audioSource;

        /// <summary>
        /// Prefab instantiated for playing back one-shot 3D clips.
        /// </summary>
        public GameObject oneShotPrefab;


        // Sets the instance reference, if not set already,
        // and keeps listening to scene changes.
		void Awake()
		{
            if (instance != null)
                return;

            instance = this;
		}

        /// <summary>
        /// Play sound clip passed in in 2D space.
        /// </summary>
        public static void Play2D(AudioClip clip)
        {
            instance.audioSource.PlayOneShot(clip);
        }


        /// <summary>
        /// Play sound clip passed in in 3D space, with optional random pitch (0-1 range).
        /// Automatically creates an audio source for playback using our PoolManager.
        /// </summary>
        public static void Play3D(AudioClip clip, Vector3 position, float pitch = 0f)
        {
            //cancel execution if clip wasn't set
            if (clip == null) return;
            //calculate random pitch in the range around 1, up or down
            pitch = UnityEngine.Random.Range(1 - pitch, 1 + pitch);

            //activate new audio gameobject from pool
            GameObject audioObj = PoolManager.Spawn(instance.oneShotPrefab, position, Quaternion.identity);
            //get audio source for later use
            AudioSource source = audioObj.GetComponent<AudioSource>();
            
            //assign properties, play clip
            source.clip = clip;
            source.pitch = pitch;
            source.Play();
            
            //deactivate audio gameobject when the clip stops playing
            PoolManager.Despawn(audioObj, clip.length);
        }
    }
}

