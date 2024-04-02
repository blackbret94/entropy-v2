using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Trigger the result of the mission in the VRG_Campaign, next, pass, star
    /// </summary>
    public class VRG_MissionFinish : VRG_Base
    {
        /// <summary>
        /// <a href="#VrGamesDev.ENUM_Mission">Enumerator Mission</a>: ENUM_Mission.FAIL by default
        /// </summary>
        [Tooltip("Enumerator Mission: ENUM_Mission.FAIL by default")]
        [SerializeField]
        private ENUM_Mission m_Mission = ENUM_Mission.FAIL;

        /// #IGNORE
        public VRG_MissionFinish()
        {
            this.m_SelfTurnOff = true;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Call the <a href="#VrGamesDev.VRG_Campaign">VRG_Campaign</a> methods, Next, pass or star
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Campaign.IsValid();

            // is it?
            if (VRG_Campaign.Instance != null)
            { 
                switch (this.m_Mission)
                {
                    case ENUM_Mission.FAIL:
                        break;
                    case ENUM_Mission.NEXT:
                        VRG_Campaign.Next();
                        break;
                    case ENUM_Mission.PASS:
                        VRG_Campaign.Pass();
                        break;
                    case ENUM_Mission.STAR:
                        VRG_Campaign.Star();
                        break;
                }
            }

            yield return null;
        }
    }
}