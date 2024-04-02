using System.Collections;

using UnityEngine;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
using Sirenix.OdinInspector;
#endif


namespace VrGamesDev
{
    /// <summary>
    /// It unparents the GameObject and makes the <em>m_Parent</em> gameObject the new parent
    /// </summary>
    public class VRG_Unparent : VRG_Base
    {
        /// <summary>
        /// The future parent when this object enables
        /// </summary>
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("The future parent when this object enables")]
        [SerializeField] protected GameObject m_Parent = null;

        protected override IEnumerator Do()
        {
            if (this.m_Parent == null)
            {
                this.transform.SetParent(null);
            }
            else
            {
                this.transform.SetParent(this.m_Parent.transform);
            }

            yield return null;
        }

    }
}
