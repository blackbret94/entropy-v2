using System.Collections;

using UnityEngine;

namespace VrGamesDev.BHEL
{
    /// <summary>
    /// This class send a log text OnEnable
    /// </summary>
    public class VRH_Bhel_OpenUrl : VRG_Base
    {
        /// <summary>
        /// Save this string into the logs
        /// </summary>
        [Tooltip("Save this string into the logs")]
        [SerializeField] private bool m_SendToLogs = true;

        /// <summary>
        /// Constructor, set to true the Next frame and the verbose level to Logs
        /// </summary>
        public VRH_Bhel_OpenUrl()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = true;

            this.verbose = ENUM_Verbose.LOGS;
        }

        protected override IEnumerator Do()
        {
            // ask and wait for teh remote singleton
            yield return VRG_Bhel.IsValid();

            // in case the singleton is real
            if (VRG_Bhel.Instance != null)
            {
                if (this.m_SendToLogs)
                {
                    this.Logs("Opening the Bhel URL in the default browser", "VRH_Bhel_OpenUrl->Do()", this.m_Verbose);
                }

                yield return null;

                VRG_Bhel.Instance.OpenUrl();
            }

            yield return null;
        }

    }
}