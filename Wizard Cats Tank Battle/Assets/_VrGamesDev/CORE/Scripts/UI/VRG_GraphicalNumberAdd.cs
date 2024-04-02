using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

///#IGNORE
namespace VrGamesDev
{
    public class VRG_GraphicalNumberAdd : VRG_Base
    {
        /// <summary>
        /// Set here the graphical Numbers to be affected.
        /// </summary>
        [Tooltip("Set here the graphical Numbers to be affected.")]
        [SerializeField] private VRG_GraphicalNumber m_GraphicalNumber = null;

        /// <summary>
        /// The duration between every random number.
        /// </summary>
        [Tooltip("The duration between every random number.")]
        [SerializeField] private int m_Add = 1;

        protected override IEnumerator Do()
        {
            if (this.m_GraphicalNumber != null)
            {
                this.m_GraphicalNumber.Add(this.m_Add);
            }

            // next frame
            yield return null;
        }

    }
}