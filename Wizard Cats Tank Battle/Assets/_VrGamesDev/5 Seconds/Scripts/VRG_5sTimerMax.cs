using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Trigger the Max function, increment the Maximum time from timer
    /// </summary>
    public class VRG_5sTimerMax : VRG_Base
    {
        /// #IGNORE
        [Tooltip("The amount to add to the Max time in the timer")]
        [SerializeField] private float m_Amount = -0.10f;
        /// <summary>
        /// The amount to add to the Max time in the timer
        /// </summary>
        public float amount { get { return this.m_Amount; } set { this.m_Amount = value; } }



        [Header("FROM Components: ")]
        /// <summary>
        /// FROM Components: The CountdownTimer component to modify
        /// </summary>
        [Tooltip("FROM Components: The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// FROM Components: The CountdownTimer Text UI
        /// </summary>
        [Tooltip("FROM Components: The CountdownTimer Text UI")]
        [SerializeField] private Text m_Text= null;


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Set the max amount seconds possible 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Just do it if it is declared the timer
            if (this.m_Timer != null)
            {
                // add the amount declared to the timer
                this.m_Timer.Max(this.m_Amount);

                if (this.m_Text != null)
                {
                    this.m_Text.text = this.m_Amount.ToString();
                }

            }

            // next frame
            yield return null;
        }
    }
}