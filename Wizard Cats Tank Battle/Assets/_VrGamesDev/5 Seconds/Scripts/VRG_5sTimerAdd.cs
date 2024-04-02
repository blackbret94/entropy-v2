using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Trigger the Add function, increment the current time
    /// </summary>
    public class VRG_5sTimerAdd : VRG_Base
    {
        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// #IGNORE
        [Tooltip("The amount to add to the current time in the timer")]
        [SerializeField] private float m_Amount = -0.10f;
        /// <summary>
        /// The amount to add to the current time in the timer
        /// </summary>
        public float amount { get { return this.m_Amount; } set { this.m_Amount = value; } }


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Add the amount seconds to the timer
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Just do it if it is declared the timer
            if (this.m_Timer != null)
            {
                // add the amount declared to the timer
                this.m_Timer.Add(this.m_Amount);
            }

            // next frame
            yield return null;
        }
    }
}