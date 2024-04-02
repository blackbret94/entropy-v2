using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace VrGamesDev
{
    /// <summary>
    /// It unparents the GameObject and makes the <em>m_Parent</em> gameObject the new parent
    /// </summary>
    public class VRG_EnableImage : VRG_Base
    {
        [Tooltip("The future parent when this object enables")]
        [SerializeField] private bool m_Value = true;

        /// <summary>
        /// The future parent when this object enables
        /// </summary>
        [Tooltip("The future parent when this object enables")]
        [SerializeField] private Image m_Image = null;


        protected override IEnumerator Do()
        {
            if (this.m_Image != null)
            {
                this.m_Image.enabled = this.m_Value;
            }

            yield return null;
        }

    }
}
