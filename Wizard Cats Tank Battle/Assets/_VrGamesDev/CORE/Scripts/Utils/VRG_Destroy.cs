using System.Collections;using UnityEngine;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
using Sirenix.OdinInspector;
#endif



namespace VrGamesDev
{
    /// <summary>
    /// Destroy a GameObject after some time, it could destroy itself or its parent
    /// </summary>
    public class VRG_Destroy : VRG_Base
    {
        /// <summary>
        /// The time it will wait to perform the actions
        /// </summary>
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("The time it will wait to perform the actions")]
        [SerializeField] private float m_Delay = 0.0f;
        public float delay
        {
            get
            {
                return this.m_Delay;
            }
            set
            {
                this.m_Delay = value;
            }
        }

        /// <summary>
        /// If false, i will destroy myself, if true, i will destroy my parent (myself included)
        /// </summary>
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("If false, i will destroy myself, if true, i will destroy my parent (myself included)")]
        [SerializeField] private bool m_Parent = false;
        public bool parent
        {
            get
            {
                return this.m_Parent;
            }
            set
            {
                this.m_Parent = value;
            }
        }

        /// <summary>
        /// Before destroy itself, it realizes all its childs
        /// </summary>
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("If false, i will destroy myself, if true, i will destroy my parent (myself included)")]
        [SerializeField] private bool m_ReleaseChilds = false;
        /*
        public bool releaseChilds
        {
            get
            {
                return this.m_ReleaseChilds;
            }
            set
            {
                this.m_ReleaseChilds = value;
            }
        }
        */










        public VRG_Destroy()
        {
            this.m_NextFrame = true;
        }

        // Coroutine funciont of Download
        protected override IEnumerator Do()
        {
            // release my childs?
            if (this.m_ReleaseChilds)
            {
                while (this.transform.childCount > 0)
                {
                    this.transform.GetChild(0).SetParent(null);
                }
            }

            // call my parent
            if (this.m_Parent)
            {
                if (this.transform.parent)
                {
                    Object.Destroy(this.transform.parent.gameObject, this.m_Delay);
                }
                else
                {
                    Object.Destroy(this.gameObject, this.m_Delay);
                }
            }

            // or myself
            else
            {
                Object.Destroy(this.gameObject, this.m_Delay);
            }

            // finish next frame
            yield return null;
        }
    }
}