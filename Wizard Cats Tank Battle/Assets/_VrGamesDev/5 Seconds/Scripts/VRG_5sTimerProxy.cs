using System.Collections;

using UnityEngine;

using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Create a proxy from the Timer and move it to the target
    /// </summary>
    public class VRG_5sTimerProxy : VRG_Move
    {
        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>
        [Tooltip("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// The proxy text
        /// </summary>
        [Tooltip("The proxy text")]
        [SerializeField] private Text m_MyText = null;

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Create a proxy text and copy the value
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Fill the proxy data
            this.m_MyText.text = this.m_Timer.time.ToString("F2");

            // Move the proxy to the target
            yield return base.Do();
        }
    }
}