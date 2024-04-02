using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Easy call to the OpenURL funcionality, open one of the 3 predefined or a custom one
    /// </summary>
    public class VRG_OpenUrl : VRG_Base
    {
        [Tooltip("The custom field load any URL you need")]
        [SerializeField] private string m_Url = string.Empty;

        //        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        public VRG_OpenUrl()
        {
            m_PlayOnEnable = true;
            m_NextFrame = true;
            m_SelfTurnOff = true;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Call an external browser with the provided URL
        /// </summary>
        protected override IEnumerator Do()
        {
            if (!VRG.OpenUrl(this.m_Url))
            {
                this.Logs("VRG_OpenUrl custom is empty, please fill the data in the inspector", ENUM_Verbose.ERROR);
            }

            // return, it is like a void
            yield return null;
        }

    }
}