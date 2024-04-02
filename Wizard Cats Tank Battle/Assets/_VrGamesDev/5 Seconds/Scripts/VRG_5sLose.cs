using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Trigger the Stop function
    /// </summary>
    public class VRG_5sLose : VRG_Base
    {
        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// The map of the checks, X's and star, where the action happens
        /// </summary>
        [Tooltip("The map of the checks, X's and star, where the action happens")]
        [SerializeField] private VRG_5sMap m_Map = null;


        /// <summary>
        /// To it's thing: hide and trigger the timer lose
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Hide the checks, x and stars
            this.m_Map.Hide();

            // Just do it if it is declared the timer
            if (this.m_Timer != null)
            {
                // and you lose the game
                this.m_Timer.Lose();
            }

            // next frame
            yield return null;
        }


    }
}