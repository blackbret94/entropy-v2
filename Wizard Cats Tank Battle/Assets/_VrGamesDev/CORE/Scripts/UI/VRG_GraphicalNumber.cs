using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;



namespace VrGamesDev
{
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// It shows a graphical number with images
    /// </summary>
    public class VRG_GraphicalNumber : VRG_SessionData
    {
        [Header("From: Custom if needed")]
        /// <summary>
        /// Stretch the number trying to fill the container
        /// </summary>
        [Tooltip("Stretch the number trying to fill the container")]
        [SerializeField] private bool m_Stretch = false;

        /// <summary>
        /// The maximum digit, if it is greater than the m_Digits, the maximum is m_Digits.length, set 0 for auto calculate
        /// </summary>
        [Tooltip("The maximum digit, if it is greater than the m_Digits, the maximum is m_Digits.length, set 0 for auto calculate")]
        [SerializeField]
        private int m_MaxDigit = 0;




        [Header("From: Components")]
        /// <summary>
        /// The container to display the number
        /// </summary>
        [Tooltip("The container to display the number")]
        [SerializeField] private RectTransform m_RectTransform;

        /// <summary>
        /// The digits to display
        /// </summary>
        [Tooltip("The digits to display")]
        [SerializeField] private Image[] m_Digits = null;

        /// <summary>
        /// Graphical numbers
        /// </summary>
        [Tooltip("Graphical numbers")]
        [SerializeField] private Sprite[] m_Numbers = null;




        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("The width of container")]
        //[SerializeField]
        private float m_Width = 0.0f;

        [Tooltip("The width of each number")]
        //[SerializeField]
        private float m_WidthNumber = 0.0f;




        // set the RectTransform
        private void Awake()
        {
            // if no rectTransform, this can't work, it needs a container to display
            this.m_RectTransform = this.FindMy(this.m_RectTransform);

            // It, is just an Int
            this.m_DataType = ENUM_DataType.INT;

            // Load the data from session
            this.m_Load = true;

            // ... and save it
            this.m_Save = true;

            // save the width from parent rect
            this.m_Width = this.m_RectTransform.rect.width;
        }

        protected new void OnEnable()
        {
            // clean the graphical digits
            foreach (Image child in this.m_Digits)
            {
                child.gameObject.SetActive(false);
                child.sprite = null;
            }

            base.OnEnable();
        }

        // Enumerator proxy
        protected override IEnumerator Do()
        {
            // clean the graphical digits
            foreach (Image child in this.m_Digits)
            {
                child.gameObject.SetActive(false);
                child.sprite = null;
            }

            // load the data if available
            this.Load();

            // in case no value, since its an INT, default it to zero
            this.value = this.value.Trim() == "" ? "0" : this.value;

            // set the width and consider the stretching
            int iWidthDigit = this.m_MaxDigit == 0 ? this.m_Digits.Length : this.m_MaxDigit;

            // get the maximum digits to use
            int iMaxDigit = this.m_Value.Length > iWidthDigit ? iWidthDigit : this.m_Value.Length;

            // the default width is the original
            float fWidthContainer = this.m_Width;

            this.m_WidthNumber = this.m_Width / iWidthDigit;

            if (iMaxDigit < iWidthDigit)
            {
                fWidthContainer = this.m_WidthNumber * (iMaxDigit + Convert.ToInt32(this.m_Stretch));
            }
            
            // modify the width
            this.m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fWidthContainer);
            



            // fill the sprites of digits
            int ii = 0;
            for (int i = iMaxDigit; i > 0; i--)
            {
                this.m_Digits[ii].gameObject.SetActive(true);

                this.m_Digits[ii].sprite = this.m_Numbers[int.Parse(this.m_Value.Substring(i - 1, 1))];

                ii++;
            }

            // next frame
            yield return null;
        }

        /// <summary>
        /// Modify the number to display
        /// </summary>
        /// <param name="valueLocal">The number to set into the Graphical Digital Number</param>
        public void SetNumber(int valueLocal)
        {
            // set the number
            this.m_Value = valueLocal.ToString();

            // just save it
            this.Save();
               
            // Start next frame, just in case you need at iterator
            this.Play();
        }

        /// #IGNORE
        public void Add() => this.Add(1);

        /// <summary>
        /// Add valueLocal to the number to display
        /// </summary>
        /// <param name="valueLocal">(Optional) The amount that will be added, by default is 1</param>
        public void Add(int valueLocal)
        {
            this.SetNumber(int.Parse(this.m_Value) + valueLocal);
        }





    }
}