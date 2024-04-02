using System.Collections;

using UnityEngine;
namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class control the time to wait for seconds duration
    /// of the Slide and return to the main loop
    /// </summary>
    public class VRG_Slide : VRG_Base
    {
        /// <summary>
        /// The delay in seconds to wait before it try to load the scene
        /// 0 seconds is infinite delay time
        /// </summary>
        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private float m_Delay = 5.0f;



        /// <summary>
        /// constructor,
        /// </summary>
        public VRG_Slide()
        {
            m_PlayOnEnable = true;
            m_NextFrame = true;
            m_SelfTurnOff = false;
        }

        protected override IEnumerator Do()
        {
            VRG_SlideShow.SetDelay(this.m_Delay);

            // return, it is like a void and wait a frame
            yield return null;
        }

    }
}