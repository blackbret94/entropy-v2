using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Start the timer and activate the map to play the collect game and update the slider UI component
    /// </summary>
    public class VRG_5sSlider : VRG_Base
    {
        /// <summary>
        /// The Play component that trigger the game over
        /// </summary>
        [Tooltip("The Play component that trigger the game over")]
        [SerializeField] private VRG_5sPlay m_Play = null;

        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>        
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// Objects to trigger when you win
        /// </summary>
        [Tooltip("Objects to trigger when you win")]
        [SerializeField] private Slider m_Slider = null;


        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        /// #IGNORE
        public VRG_5sSlider()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = true;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Start the timer and activate the map to play the collect game and update the slider UI component
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            if (this.m_Play != null && this.m_Timer != null && this.m_Slider != null)
            {
                if (this.m_Timer.winRound == 0)
                {
                    this.m_Slider.gameObject.SetActive(false);
                }
                else
                {
                    this.m_Slider.gameObject.SetActive(true);
                }

                this.m_Slider.value = (float)((float)(this.m_Play.round) / (float)this.m_Timer.winRound);
            }

            // next frame
            yield return null;
        }


    }
}