using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    [RequireComponent(typeof(Collider))]
    /// <summary>
    /// Add this component to a 3d object to react OnMouse enter down and exit
    /// </summary>
    public class VRG_OnMouse : VRG_Base
    {
        /// <summary>
        /// Toogle the onclick
        /// </summary>
        [Tooltip("Toogle the onclick")]
        [SerializeField] private GameObject[] m_WhenMouseDown = null;

        /// <summary>
        /// Toogle when entered the mouse
        /// </summary>
        [Tooltip("Toogle when entered the mouse")]
        [SerializeField] private GameObject[] m_WhenMouseEnter = null;

        /// <summary>
        /// Toogle when exited the mouse
        /// </summary>
        [Tooltip("Toogle when exited the mouse")]
        [SerializeField] private GameObject[] m_WhenMouseExit = null;

        protected override IEnumerator Do() { yield return null; }


        /// <summary>
        /// Activate all the objects added to the m_WhenMouseDown Array
        /// </summary>
        private void OnMouseDown()
        {
            // this object was clicked - do something
            foreach (GameObject child in this.m_WhenMouseDown)
            {
                if (child == null)
                {
                    this.Logs(this.name + " has a null OnMouseDown", ENUM_Verbose.ERROR);
                }
                else
                {
                    child.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Activate all the objects added to the OnMouseEnter Array
        /// </summary>
        private void OnMouseEnter()
        {
            foreach (GameObject child in this.m_WhenMouseEnter)
            {
                if (child == null)
                {
                    this.Logs(this.name + " has a null OnMouseEnter", ENUM_Verbose.ERROR);
                }
                else
                {
                    child.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Activate all the objects added to the OnMouseExit Array
        /// </summary>
        private void OnMouseExit()
        {
            foreach (GameObject child in this.m_WhenMouseExit)
            {
                if (child == null)
                {
                    this.Logs(this.name + " has a null OnMouseExit", ENUM_Verbose.ERROR);
                }
                else
                {
                    child.SetActive(true);
                }
            }
        }


    }
}