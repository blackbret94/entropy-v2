using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Scale a GameObject in X,Y,X, simple and effective
    /// It scale from current Scale to the Target Vector3
    /// </summary>
    public class VRG_Scale : VRG_Base
    {
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        [Tooltip("The available status, ask for this property to know if it is available")]
        [SerializeField] protected bool m_IsReady = false;

        /// <summary>
        /// If false, it will do the scaling just once, if true it will loop it.
        /// </summary>
        [Tooltip("If false, it will do the scaling just once, if true it will loop it.")]
        [SerializeField] private bool m_Loop = false;

        /// <summary>
        /// It goes fromt origin -> target -> origin
        /// </summary>
        [Tooltip("It goes fromt origin -> target -> origin")]
        [SerializeField] private bool m_PingPong = false;

        /// <summary>
        /// The time in seconds to complete the scaling
        /// </summary>
        [Tooltip("The time in seconds to complete the scaling")]
        [SerializeField] private float m_Duration = 1.0f;

        /// <summary>
        /// Target Scale to scale
        /// </summary>
        [Tooltip("Target Scale to scale")]
        [SerializeField] private Vector3 m_TargetScale = new Vector3(1.5f, 1.5f, 1.5f);



        [Header("FROM: Events")]
        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] private GameObject[] m_WhenDone = null;



        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("Target Scale to achieve")]
        //[SerializeField]
        private Vector3 m_StartingScale = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("Target Scale to achieve")]
        //[SerializeField]
        private Vector3 m_Origin = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("Target Scale to achieve")]
        //[SerializeField]
        private Vector3 m_Target = new Vector3(1.0f, 1.0f, 1.0f);

        // set in stone the starting scale
        private void Awake()
        {
            this.m_StartingScale = this.transform.localScale;
        }

        private void OnDisable()
        {
            this.m_IsReady = false;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Start the scaling from origin -> target
        /// </summary>
        public override void Play()
        {
            this.m_IsReady = true;

            this.m_Origin = this.m_StartingScale;
            this.m_Target = this.m_TargetScale;

            base.Play();
        }


        // Enumerator proxy
        protected override IEnumerator Do()
        {
            // no ping pong mode
            int iPingPong = 0;

            // ... unless
            if (this.m_PingPong)
            {
                // yes :D
                iPingPong = 1;
            }

            // repeat the pingong times
            while (iPingPong >= 0 && this.m_IsReady)
            {
                // lets begin the progress
                float progress = 0;

                // while the duration is not completed and the class is ready
                while (progress < this.m_Duration && this.m_IsReady)
                {
                    // lerp the scale for the duration seconds
                    this.transform.localScale = Vector3.Lerp(this.m_Origin, this.m_Target, (progress / this.m_Duration));

                    // add the progress
                    progress += Time.deltaTime;

                    // next frame
                    yield return null;
                }

                // adjust the scale to 100% target

                // if it is still ready
                if (this.m_IsReady)
                {
                    // adjust the scale to 100% target
                    transform.localScale = this.m_Target;
                }

                // next frame
                yield return null;

                // if the mode is pingponed
                if (this.m_PingPong)
                {
                    // go backwards
                    this.m_Origin = this.m_TargetScale;
                    this.m_Target = this.m_StartingScale;
                }

                // pingpong done
                iPingPong--;
            }

            if (this.m_IsReady)
            {

                // if looping mode is active, re play it
                if (this.m_Loop)
                {
                    this.Play();
                }
                else
                {
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
                }

            }
        }



        /// <summary>
        /// Modify the duration of the scaling time
        /// </summary>
        /// <param name="valueLocal">The new duration in seconds</param>
        public void SetDuration(float valueLocal)
        {
            this.m_Duration = valueLocal;
        }

    }
}