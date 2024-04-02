using System.Collections;

using UnityEngine;

namespace VrGamesDev.BHEL
{
    /// <summary>
    /// This class send a log text OnEnable
    /// </summary>
    public class VRG_Bhel_Log : VRG_Base
    {
        /// <summary>
        /// Save this string into the logs
        /// </summary>
        [Tooltip("Save this string into the logs")]
        [SerializeField] private string  m_Value = string.Empty;

        /// <summary>
        /// Constructor, set to true the Next frame and the verbose level to Logs
        /// </summary>
        public VRG_Bhel_Log()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = false;

            this.verbose = ENUM_Verbose.LOGS;
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            if (this.m_Value.Trim() == string.Empty)
            {
                this.m_Value = this.name;
            }

            this.Logs(this.m_Value, "VRG_Bhel_Log->Do()", this.m_Verbose);

            yield return null;
        }

    }
}