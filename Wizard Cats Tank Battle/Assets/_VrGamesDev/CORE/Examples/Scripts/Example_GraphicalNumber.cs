using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    ///#IGNORE
    public class Example_GraphicalNumber : VRG_Base
    {
        /// <summary>
        /// Set here the graphical Numbers to be affected.
        /// </summary>
        [Tooltip("Set here the graphical Numbers to be affected.")]
        [SerializeField] private VRG_GraphicalNumber[] m_GraphicalNumber = null;

        /// <summary>
        /// The duration between every random number.
        /// </summary>
        [Tooltip("The duration between every random number.")]
        [SerializeField] private float m_Duration = 1.0f;

        /// <summary>
        /// The minimum random number.
        /// </summary>
        [Tooltip("The minimum random number.")]
        [SerializeField] private int m_NumberMin = 0;

        /// <summary>
        /// The maximum random number.
        /// </summary>
        [Tooltip("The maximum random number.")]
        [SerializeField] private int m_NumberMax = 99999;

        protected new void OnEnable()
        {
            base.OnEnable();
        }

        protected override IEnumerator Do()
        {
            bool bContinue = true;

            // do it while it is ready
            while (bContinue)
            {
                // Cycle through all the VRG_GraphicalNumber
                foreach (VRG_GraphicalNumber child in this.m_GraphicalNumber)
                {
                    if (child != null)
                    {
                        // set a random number
                        child.SetNumber(Random.Range(this.m_NumberMin, this.m_NumberMax));
                    }
                }

                // just do it if the duration is above 0
                if (this.m_Duration > 0)
                {
                    // wait the duration time
                    yield return new WaitForSeconds(this.m_Duration);
                }
                else
                {
                    // disconnect
                    bContinue = false;
                }
            }

            // next frame
            yield return null;
        }

    }
}