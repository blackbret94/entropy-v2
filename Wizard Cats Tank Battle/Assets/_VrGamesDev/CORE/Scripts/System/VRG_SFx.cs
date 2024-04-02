using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// #IGNORE
    /// <summary>
    /// Play the sound as a fire and forget single use sound
    /// It unparents itself so it isn't affected by the parent
    /// </summary>
    public class VRG_SFx : VRG_Base
    {
        /// <summary>
        /// The clip to play
        /// </summary>
        [Tooltip("The clip to play")]
        [SerializeField] private AudioClip m_AudioClip = null;

        /// <summary>
        /// The source of the audio to play, usually it is my own child
        /// </summary>
        [Tooltip("The source of the audio to play, usually it is my own child")]
        [SerializeField] private AudioSource m_AudioSource;

        // make sure you have an audio source
        private void Awake()
        {
            // make sure we have an Audio Source
            this.m_AudioSource = this.FindMy(this.m_AudioSource);
        }

        
        protected override IEnumerator Do()
        {
            if (this.m_AudioClip != null)
            {
                // update the clip
                this.m_AudioSource.clip = this.m_AudioClip;

                // activate the audio source
                this.m_AudioSource.gameObject.SetActive(true);

                // wait until sound is played
                yield return new WaitForSeconds(this.m_AudioClip.length);

                // may the force be with you
                Destroy(this.gameObject);
            }
            else
            {
                this.Logs("The m_AudioClip is null, please provide a valid audio clip file in the inspector", ENUM_Verbose.WARNING);
            }

            // go to next frame
            yield return null;
        }


    }
}