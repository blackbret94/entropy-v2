using System.Collections;

using UnityEngine;

using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Add the time left when you collect the green check
    /// </summary>
    public class VRG_5sBonus : VRG_Base
    {
        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// The UI component to display the current bonus amount
        /// </summary>
        [Tooltip("The UI component to display the current bonus amount")]
        [SerializeField] private Text m_Bonus = null;

        /// <summary>
        /// The detail of the floating point, by default is 2
        /// </summary>
        [Tooltip("The detail of the floating point, by default is 2")]
        [SerializeField] private int m_Decimals = 2;

        /// <summary>
        /// The time in seconds to complete the scaling, by default is 0.5 seconds
        /// </summary>
        [Tooltip("The time in seconds to complete the scaling, by default is 0.5 seconds")]
        [SerializeField] private float m_Duration = 0.5f;


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Calculate the bonus times the speed and the time and update the Bonus text
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // all the data to use to count the bonus
            float fNumberFinal, fNumberCurrent, fSpeed;

            // just on valid timer
            if (this.m_Timer != null)
            {
                // get the current number 
                fNumberCurrent = float.Parse(this.m_Bonus.text);

                // calculete the current mas the next
                fNumberFinal = fNumberCurrent + this.m_Timer.time;

                // the speed by the duration
                fSpeed = this.m_Timer.time / this.m_Duration;

                // cycle until the number is reached
                while (fNumberCurrent < fNumberFinal)
                {
                    // set it with the floating point
                    this.m_Bonus.text = fNumberCurrent.ToString("F" + this.m_Decimals.ToString());

                    // update it by the speed defined
                    fNumberCurrent += Time.deltaTime * fSpeed;

                    // next frame
                    yield return null;
                }

                // make sure the value is equal to the defined final value
                this.m_Bonus.text = fNumberFinal.ToString("F" + this.m_Decimals.ToString());
            }

            // inform the error
            else
            {
                this.Logs(this.name + " needs a VRG_5sBonus component, assign it in the inspector", ENUM_Verbose.WARNING);
            }

            // next frame
            yield return null;
        }


    }
}