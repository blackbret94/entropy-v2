using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Set the VRG_Campaign UI data into a canvas elements
    /// </summary>
    public class VRG_UICustom : VRG_Base
    {
        /// <summary>
        /// If you need this to get colorized with the Background
        /// </summary>
        [Tooltip("If you need to except this item")]
        [SerializeField] private ENUM_VRG_UIExcept m_Except = ENUM_VRG_UIExcept.SELF;
        [SerializeField] public ENUM_VRG_UIExcept except { get { return this.m_Except; } }


        /// <summary>
        /// If you need this to get colorized with the Background
        /// </summary>
        [Tooltip("If you need to except this item")]
        [SerializeField] private ENUM_VRG_UIColor m_Color = ENUM_VRG_UIColor.NONE;
        [SerializeField] public ENUM_VRG_UIColor color { get { return this.m_Color; } }


        /// <summary>
        /// If you need to mark this font with the secondary color
        /// </summary>
        [Tooltip("If you need to mark this font with the secondary color")]
        [SerializeField] private bool m_Font = false;
        [SerializeField] public bool font { get { return this.m_Font; } }



        public VRG_UICustom()
        {
            this.m_SelfTurnOff = false;
            this.m_NextFrame = false;
        }


        protected override IEnumerator Do()
        {
            if (this.m_Color != ENUM_VRG_UIColor.NONE && this.m_Except != ENUM_VRG_UIExcept.NONE)
            {
                this.Logs("You assigned a color, and you are excepting this component", ENUM_Verbose.WARNING);
            }

            yield return null;
        }

    }
}