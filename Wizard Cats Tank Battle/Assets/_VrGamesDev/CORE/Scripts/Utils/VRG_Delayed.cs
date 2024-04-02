using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// This class allows to activate, deactivate, toogle or destroy many items of the scene
    /// it will be delayed for some seconds, instant or the next frame
    /// </summary>
    public class VRG_Delayed : VRG_Base
    {
        /// <summary>
        /// The time it will wait to perform the actions
        /// </summary>
        [Tooltip("The time it will wait to perform the actions")]
        [SerializeField] private float m_Delay = 0.0f;

        /// <summary>
        /// If false, it will do the scaling just once, if true it will loop it.
        /// </summary>
        [Tooltip("If false, it will do it just once, if true it will loop it every delay Seconds if > 0")]
        [SerializeField] private bool m_Loop = false;

        /// <summary>
        /// Array of the transform to activate <em>setActive(true)</em>
        /// </summary>
        [Tooltip("Array of the transform to activate setActive(true)")]
        [SerializeField] private Transform[] m_Activate = null;

        /// <summary>
        /// Array of the transform to deactivate <em>setActive(false)</em>
        /// </summary>
        [Tooltip("Array of the transform to deactivate setActive(false)")]
        [SerializeField] private Transform[] m_Deactivate = null;

        /// <summary>
        /// Array of the gameobjects to destroy
        /// </summary>
        [Tooltip("Array of the gameobjects to destroy")]
        [SerializeField] private Transform[] m_Destroy = null;

        /// <summary>
        /// Array of the transform to toogle, if on, then it will become off, and viceversa
        /// </summary>
        [Tooltip("Array of the transform to toogle, if on, then it will become off, and viceversa")]
        [SerializeField] private Transform[] m_Toogle = null;

        // Enumerator proxy, it is activated OnEnable
        protected override IEnumerator Do()
        {
            bool bFirst = true;

            if (!this.m_Loop && this.m_Delay == 0)
            {
                // turn myself off if true
                this.transform.gameObject.SetActive(!this.m_SelfTurnOff);
            }

            while (bFirst || (this.m_Loop && this.m_Delay > 0))
            {
                bFirst = false;

                // wait for delay seconds
                if (this.m_Delay > 0)
                {
                    yield return new WaitForSeconds(this.m_Delay);
                }

                // toogle for the next 
                foreach (Transform child in this.m_Toogle)
                {
                    if (child == null)
                    {
                        this.Logs("this.m_Toogle NULL", ENUM_Verbose.WARNING);
                    }
                    else
                    {
                        child.gameObject.SetActive(!child.gameObject.activeSelf);
                    }
                }

                // activate 
                foreach (Transform child in this.m_Activate)
                {
                    if (child == null)
                    {
                        this.Logs("this.m_Activate NULL", ENUM_Verbose.WARNING);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                    }
                }

                // deactivate
                foreach (Transform child in this.m_Deactivate)
                {
                    if (child == null)
                    {
                        this.Logs("this.m_Deactivate NULL", ENUM_Verbose.WARNING);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                // destroy
                foreach (Transform child in this.m_Destroy)
                {
                    if (child == null)
                    {
                        this.Logs("this.m_Destroy NULL", ENUM_Verbose.WARNING);
                    }
                    else
                    {
                        Object.Destroy(child.gameObject, 0.0f);
                    }
                }

            }

            // turn myself off if true
            this.transform.gameObject.SetActive(!this.m_SelfTurnOff);


            // finish the next frame
            yield return null;
        }

        /// <summary>
        /// Modify the delay to wait
        /// </summary>
        /// <param name="valueLocal">The tine in seconds to delay</param>
        public void SetDelay(float valueLocal)
        {
            this.m_Delay = valueLocal;
        }
    }
}