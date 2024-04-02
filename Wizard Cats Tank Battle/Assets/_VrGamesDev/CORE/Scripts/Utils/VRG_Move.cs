 using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Lerp a GameObject from Origin to Target at Duration speed
    /// </summary>
    public class VRG_Move : VRG_Base
    {
        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        /// <summary>
        /// If true, the origin is cloned temporarely to move it as child of this object
        /// </summary>
        [Tooltip("If true, the origin is cloned temporarely to move it as child of this object")]
        [SerializeField] private bool m_Proxy = false;

        /// <summary>
        /// The transform origin to move from
        /// </summary>
        [Tooltip("The transform origin to move from")]
        [SerializeField] private GameObject m_Origin = null;

        /// <summary>
        /// The transform target to move to
        /// </summary>
        [Tooltip("The transform target to move to")]
        [SerializeField] private GameObject m_Target = null;


        /// <summary>
        /// The time in seconds to complete the scaling
        /// </summary>
        [Tooltip("The time in seconds to complete the scaling")]
        [SerializeField] private float m_Duration = 1.0f;

        /// <summary>
        /// The distance to start the lerp movement
        /// </summary>
        [Tooltip("The distance to start the lerp movement")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_Start = 0.0f;

        /// <summary>
        /// The distance to finish the lerp movement
        /// </summary>
        [Tooltip("The distance to finish the lerp movement")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_Finish = 1.0f;

        [Header("FROM: Events")]
        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] private GameObject[] m_WhenDone = null;


        private void Awake()
        {
            if (this.m_Origin == null || this.m_Target == null)
            {
                this.Logs("VRG_Move needs a Origin and a Target", ENUM_Verbose.ERROR);
                Object.Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Lerp move this GameObject from origin to target
        /// </summary>
        protected override IEnumerator Do()
        {
            // Get the start distance proportion
            float fSpeed = this.m_Start;

            GameObject go_Origin = null;
            if (this.m_Proxy)
            {
                go_Origin = Object.Instantiate(this.m_Origin, this.transform);
            }

            // repeat while not reaching the finish
            while (fSpeed < this.m_Finish)
            {
                // lerp your way 
                this.transform.position = Vector3.Lerp(this.m_Origin.transform.position, this.m_Target.transform.position, fSpeed);

                // get the speed of the movement
                fSpeed += Time.deltaTime * (1/this.m_Duration);

                // wait for next frame
                yield return null;
            }

            // set it at the final distance exactly
            this.transform.position = Vector3.Lerp(this.m_Origin.transform.position, this.m_Target.transform.position, this.m_Finish);

            Object.Destroy(go_Origin);

            foreach (GameObject child in this.m_WhenDone)
            {
                if (child != null)
                {
                    // activate it
                    child.SetActive(true);
                }
                else
                {
                    this.Logs(this.name + " | There is a null element in the Done array", ENUM_Verbose.ERROR);
                }
            }

            yield return null;
        }


    }
}