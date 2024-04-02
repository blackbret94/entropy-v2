using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Trigger the Stop function from timer
    /// </summary>
    public class VRG_5sTimerStop : VRG_Base
    {
        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// The map of the checks, X's and star
        /// </summary>
        [Tooltip("The map of the checks, X's and star")]
        [SerializeField] private VRG_5sMap m_Map = null;


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Hide the map and stop the timer.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // hide the checks, X's and star and unenable them
            this.m_Map.Hide();

            // Just do it if it is declared the timer
            if (this.m_Timer != null)
            {
                // stop the timer
                this.m_Timer.Stop();
            }

            // next frame
            yield return null;
        }
    }
}