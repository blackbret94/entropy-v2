using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// This class uses a <a href="#VrGamesDev.VRG_GraphicalNumber">VRG_GraphicalNumber</a> and animate it
    /// to fade out scaled counting up to zero, and then display an image, by default is a "Start" label
    /// </summary>
    public class VRG_GraphicalNumber_BackWardCountdown : VRG_Base
    {
        /// <summary>
        /// The initial number to count from, by default start at 3
        /// </summary>
        [Tooltip("The initial number to count from, by default start at 3")]
        [SerializeField] private int m_Number = 3;

        /// <summary>
        /// GETTER: the starting number
        /// </summary>
        public int number { get { return this.m_Number; } }

        /// <summary>
        /// The duration in seconds between each count, by default is 1 second
        /// </summary>
        [Tooltip("The duration in seconds between each count, by default is 1 second")]
        [SerializeField] private float m_Duration = 1.0f;




        [Header("From: Components")]
        /// <summary>
        /// A <a href="#VrGamesDev.VRG_GraphicalNumber">VRG_GraphicalNumber</a> component
        /// </summary>
        [Tooltip("A VRG_GraphicalNumber component")]
        [SerializeField] private VRG_GraphicalNumber m_GraphicalNumber = null;

        /// <summary>
        /// A <a href="#VrGamesDev.VRG_Scale">VRG_Scale</a> component
        /// </summary>
        [Tooltip("A VRG_Scale component")]
        [SerializeField] private VRG_Scale m_Scale = null;




        [Header("From: Events")]
        /// <summary>
        /// When It reaches zero, this gameobject SetActive(true), by default, it display a "Start" Label,
        /// modify the component to custom to your game needs
        /// </summary>
        [Tooltip("When It reaches zero, this gameobject SetActive(true)")]
//        [SerializeField] private GameObject[] m_OnCountdown = null;
        [SerializeField] private GameObject[] m_WhenCountdown = null;

        /// <summary>
        /// When the Start label finish, this component SetActive(true)
        /// </summary>
        [Tooltip("When the Start label finish, this component SetActive(true)")]
  //      [SerializeField] private GameObject[] m_OnFinish = null;
        [SerializeField] private GameObject[] m_WhenFinish = null;



        /// #IGNORE
        public override void Play() => this.Play(this.number);

        /// <summary>
        /// This method starts the countdown, it starts at <strong>valueLocal</strong>,
        /// by default its value is <strong>this.m_Number</strong>
        /// </summary>
        /// <param name="valueLocal">(Optional) The starting number to countdown, by default is this.m_Number</param>
        public void Play(int valueLocal)
        {
            // always update the number to start from
            this.m_Number = valueLocal;

            // init the components 
            this.m_Scale.SetDuration(this.m_Duration);
            this.m_GraphicalNumber.gameObject.SetActive(true);

            // Start next frame, just in case you need at iterator
            StartCoroutine(this.Do());
        }


        // Enumerator proxy
        protected override IEnumerator Do()
        {
            // start the countdown
            int iNumber = this.m_Number;

            // count and wait
            while (iNumber > 0)
            {
                // play the animation scale
                this.m_Scale.Play();

                // 1 less
                this.m_GraphicalNumber.SetNumber(iNumber--);

                // wait m_Duration seconds
                yield return new WaitForSeconds(this.m_Duration);
            }

            // when it is done
            if (iNumber <= 0)
            {
                // play the animation scale
                this.m_Scale.Play();

                // OnEvent countdown
                foreach (GameObject child in this.m_WhenCountdown)
                {
                    if (child != null)
                    {
                        child.SetActive(true);
                    }
                    else
                    {
                        this.Logs(this.name + " | There is a null element in the Countdown array", ENUM_Verbose.WARNING);
                    }
                }


                // stop the numbers
                this.m_GraphicalNumber.gameObject.SetActive(false);

                // wait m_Duration seconds
                yield return new WaitForSeconds(this.m_Duration);
            }

            // it finish and it is over
            // OnEvent countdown
            foreach (GameObject child in this.m_WhenFinish)
            {
                if (child != null)
                {
                    child.SetActive(true);
                }
                else
                {
                    this.Logs(this.name + " | There is a null element in the Finish array", ENUM_Verbose.WARNING);
                }
            }

            // next frame
            yield return null;
        }


    }
}