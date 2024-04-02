using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace VrGamesDev.DDuA
{
    public class VRG_SlideButton : VRG_Base
    {
        [Tooltip("")]
        [SerializeField] private Button m_Button = null;

        [Tooltip("")]
        [SerializeField] private Image m_Image = null;

        [Tooltip("")]
        [SerializeField] private Color32 m_ColorUnSelected = new Color32(160, 160, 160, 128);

        [Tooltip("")]
        [SerializeField] private Color32 m_ColorSelected = new Color32(255, 255, 255, 255);

        [Range(0.25f, 1.25f)]
        [Tooltip("")]
        [SerializeField] private float m_Scale = 0.75f;




        private void Awake()
        {
            this.m_Button = this.FindMy(this.m_Button);
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            yield return null;
        }

        // the listener function
        public void ActionOnClick()
        {
            // call my Number Slide
            VRG_SlideShow.Play(int.Parse(this.name));
        }

        // the listener function
        public void Selected(int valueLocal)
        {
            // assign the default value
            float fScale = this.m_Scale;
            Color32 c32Color = this.m_ColorUnSelected;

            // if i am the one selected
            if (valueLocal == int.Parse(this.name))
            {
                // the default data is assigned
                fScale = 1.0f;
                c32Color = this.m_ColorSelected;
            }

            // set the data
            if (this.m_Image != null)
            {
                this.m_Image.color = c32Color;
            }

            // set the scale according to the selection
            this.transform.localScale = new Vector3(fScale, fScale, fScale);
        }
    }
}