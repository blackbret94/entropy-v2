using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Toogle between 2 sprites On / Off
    /// </summary>
    public class VRG_ToogleIcon : VRG_SessionData
    {
        [Header("From: Icon")]
        /// <summary>
        /// If it is true, the image m_On; If it is false, the image is m_Off
        /// </summary>
        [Tooltip("If it is true, the image m_On; If it is false, the image is m_Off")]
        [SerializeField] private bool m_IsItOn = true;

        /// <summary>
        /// The icon image that will toogle.
        /// </summary>
        [Tooltip("The icon image that will toogle.")]
        [SerializeField] private Image m_Image = null;

        /// <summary>
        /// The sprite image to toogle when the object is On
        /// </summary>
        [Tooltip("The sprite image to toogle when the object is On")]
        [SerializeField] private Sprite m_On = null;

        /// <summary>
        /// The sprite image to toogle when the object is off
        /// </summary>
        [Tooltip("The sprite image to toogle when the object is off")]
        [SerializeField] private Sprite m_Off = null;

        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]



        public VRG_ToogleIcon()
        {
            // customize for this kind of objects
            this.m_DataType = ENUM_DataType.BOOL;
            this.m_SelfTurnOff = true;
        }

        private void Awake()
        {
        }

        protected override IEnumerator Do()
        {
            // just try if the object is complete
            if (
                this.m_On != null &&
                this.m_Off != null &&
                this.m_Image != null
                )
            {
                if (this.m_Value.Trim() == "")
                {
                    this.m_IsItOn = VRG_Session.GetBool(this.m_SessionObject, this.m_SessionData, this.m_IsItOn);
                }
                else
                {
                    // toogle
                    this.m_IsItOn = !this.m_IsItOn;
                }

                // Use On or Off from this.m_IsItOn
                this.m_Image.sprite = this.m_IsItOn ? this.m_On : this.m_Off;

                // Set the value in the value
                this.m_Value = this.m_IsItOn.ToString();

                // if the session is set and need to be saved
                this.Save();

                // check if it needs to trigger the value data
                this.OnValue();
            }

            // go to next frame
            yield return null;
        }

    }
}