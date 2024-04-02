using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    [RequireComponent(typeof(RectTransform))]

    /// <summary>
    /// Scroll a RectTransform to produce a credits scrolling effect
    /// </summary>
    public class VRG_Credits : VRG_Base
    {
        //[SerializeField]
        [Tooltip("From UI: The Body text container as a RectTransform, the information from credits will be displayed here")]
        /// <summary>
        /// From UI: The Body text container as a RectTransform, the information from credits will be displayed here
        /// </summary>
        private RectTransform m_Content = null;

        /// <summary>
        /// The speed of scrolling
        /// </summary>
        [Tooltip("The speed of scrolling")]
        [SerializeField] private float m_Speed = 10.0f;

        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]


        //[SerializeField]
        [Tooltip("The y height that is displaying")]
        /// <summary>
        /// The y height that is displaying
        /// </summary>
        private float m_YCurrent = 0.0f;

        //[SerializeField]
        [Tooltip("The height includig all the content")]
        /// <summary>
        /// The height includig all the content
        /// </summary>
        private float m_YMax = 0.0f;

        /// #IGNORE
        private void Awake()
        {
            this.m_Content = this.FindMy(this.m_Content, false);

            // get all the childs and calculate the max childs lenght as height
            this.m_YMax = this.transform.GetComponentsInChildren<Transform>().Length * 50;

            // set it
            this.m_Content.sizeDelta = new Vector2(this.m_Content.sizeDelta.x, this.m_YMax);
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Scroll the credits
        /// </summary>
        protected override IEnumerator Do()
        {
            // start from the begginng
            this.m_YCurrent = 0.0f;

            // send the content to the beggining
            this.m_Content.anchoredPosition = new Vector2(0, this.m_YCurrent);

            // while there are still content
            while (this.m_Content.offsetMin.y < 0 )
            {
                // move it 
                this.m_Content.anchoredPosition = new Vector2(0, this.m_YCurrent++ * this.m_Speed);

                // return, it is like a void
                yield return null;
            }

            // return, it is like a void
            yield return null;
        }

    }
}