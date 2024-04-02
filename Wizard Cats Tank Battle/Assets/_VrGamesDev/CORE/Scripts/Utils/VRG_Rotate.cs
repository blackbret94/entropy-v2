using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Rotates a GameObject in X,Y,X, simple and effective
    /// 0 for no rotation
    /// Positive and negative values, will rotate clockwise and anticlockwise
    /// </summary>
    public class VRG_Rotate : VRG_Base
    {
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        [Tooltip("The available status, ask for this property to know if it is available")]
        [SerializeField] protected bool m_IsReady = false;

        /// <summary>
        /// If it needs to start random in X axis
        /// </summary>
        [Tooltip("If it needs to start random in X axis")]
        [SerializeField] private bool m_XRandom = false;

        /// <summary>
        /// If it needs to start random in Y axis
        /// </summary>
        [Tooltip("If it needs to start random in Y axis")]
        [SerializeField] private bool m_YRandom = false;

        /// <summary>
        /// If it needs to start random in Z axis
        /// </summary>
        [Tooltip("If it needs to start random in Z axis")]
        [SerializeField] private bool m_ZRandom = false;



        /*
        /// <summary>
        /// Rotation speed in X axis
        /// </summary>
        [Tooltip("Rotation speed in X axis")]
        [SerializeField] private float m_XSpeed = 100;

        /// <summary>
        /// Rotation speed in Y axis
        /// </summary>
        [Tooltip("Rotation speed in Y axis")]
        [SerializeField] private float m_YSpeed = 100;

        /// <summary>
        /// Rotation speed in Z axis
        /// </summary>
        [Tooltip("Rotation speed in Z axis")]
        [SerializeField] private float m_ZSpeed = 100;
        */


        /// <summary>
        /// The time in seconds to complete the scaling
        /// </summary>
        [Tooltip("The time in seconds to complete the scaling")]
        [SerializeField] private float m_Duration = 0.0f;

        /// <summary>
        /// Rotation speed in all axis
        /// </summary>
        [Tooltip("Rotation speed in all axis")]
        [SerializeField] private Vector3 m_Speed = new Vector3(100.0f, 100.0f, 100.0f);


        [Header("FROM: Events")]
        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] private GameObject[] m_WhenDone = null;



        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("Current coordinate in X axis")]
        //[SerializeField]
        private float m_X = 0.0f;

        [Tooltip("Current coordinate in Y axis")]
        //[SerializeField]
        private float m_Y = 0.0f;

        [Tooltip("Current coordinate in Z axis")]
        //[SerializeField]
        private float m_Z = 0.0f;





        private new void OnEnable()
        {
            // init the current coords
            this.m_X = this.transform.rotation.x;
            this.m_Y = this.transform.rotation.y;
            this.m_Z = this.transform.rotation.z;

            // decide if it has a random rotation start coordinate
            if (this.m_XRandom) this.m_X = Random.Range(0.0f, 359.9f);
            if (this.m_YRandom) this.m_Y = Random.Range(0.0f, 359.9f);
            if (this.m_ZRandom) this.m_Z = Random.Range(0.0f, 359.9f);

            base.OnEnable();
        }

        private void OnDisable()
        {
            this.m_IsReady = false;
        }

        // Enumerator proxy
        protected override IEnumerator Do()
        {
            this.m_IsReady = true;

            float fStartTime = Time.realtimeSinceStartup;

            while (this.m_IsReady)
            {
                // if the X speed is different that 0
                if (this.m_Speed.x != 0)
                {
                    // increment the speed over time
                    this.m_X += (Time.deltaTime * this.m_Speed.x);

                    // if it is over 360
                    if (this.m_X >= 360.0f) this.m_X -= 360.0f;

                    // if it is below 0
                    if (this.m_X <= 0.0f) this.m_X += 360.0f;
                }

                // if the Y speed is different that 0
                if (this.m_Speed.y != 0)
                {
                    // increment the speed over time
                    this.m_Y += (Time.deltaTime * this.m_Speed.y);

                    // if it is over 360
                    if (this.m_Y >= 360.0f) this.m_Y -= 360.0f;

                    // if it is below 0
                    if (this.m_Y <= 0.0f) this.m_Y += 360.0f;
                }

                // if the Z speed is different that 0
                if (this.m_Speed.z != 0)
                {
                    // increment the speed over time
                    this.m_Z += (Time.deltaTime * this.m_Speed.z);

                    // if it is over 360
                    if (this.m_Z >= 360.0f) this.m_Z -= 360.0f;

                    // if it is below 0
                    if (this.m_Z <= 0.0f) this.m_Z += 360.0f;
                }

                // assign the new rotation
                this.transform.localEulerAngles = new Vector3(this.m_X, this.m_Y, this.m_Z); //  eulerAngles

                // wait next round
                yield return null;

                // Sto the rotation when the duration is meet, just do it if duration > 0
                if (this.m_Duration > 0)
                {
                    if (this.m_Duration <= (Time.realtimeSinceStartup - fStartTime))
                    {
                        this.m_IsReady = false;
                    }
                }

            }

            
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