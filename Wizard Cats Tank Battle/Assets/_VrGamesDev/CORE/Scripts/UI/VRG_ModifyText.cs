using System.Collections;

using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Modify a TExt UI component from outside, It can add or set a number, useful on Enabled objects
    /// </summary>
    public class VRG_ModifyText : VRG_Base
    {
        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        /// <summary>
        /// Text Componennt from a UI Canvas
        /// </summary>
        [Tooltip("Text Componennt from a UI Canvas")]
        [SerializeField] private Text m_MyText = null;

        /// <summary>
        /// The value to hardcore set it
        /// </summary>
        [Tooltip("The value to hardcore set it")]
        [SerializeField] private string m_Set = "";

        /// <summary>
        /// Increase the text value by this amount
        /// </summary>
        [Tooltip("Increase the text value by this amount")]
        [SerializeField] private float m_Add = 0;

        /// <summary>
        /// The level of detail of the float number
        /// </summary>
        [Tooltip("The level of detail of the float number")]
        [SerializeField] private int m_Decimals = 0;

        /// <summary>
        /// If the scene name is the one to fill
        /// </summary>
        [Tooltip("The level of detail of the float number")]
        [SerializeField] private bool m_Scene = false;


        protected override IEnumerator Do()
        {
            // define it first
            if (this.m_MyText != null)
            {
                string sText = "";

                // if a set value is hardocred
                if (this.m_Set.Trim() != "")
                {
                    // set it
                    sText = this.m_Set;
                }

                // if a set value is hardocred
                if (this.m_Add != 0)
                {
                    // add it, it try to float it
                    sText = (float.Parse(this.m_MyText.text) + this.m_Add).ToString("F" + this.m_Decimals.ToString());
                }

                // if a set value is hardocred
                if (this.m_Scene)
                {
                    // add it, it try to float it
                    sText = SceneManager.GetActiveScene().name;
                }

                this.m_MyText.text = sText;
            }

            yield return null;
        }


    }
}