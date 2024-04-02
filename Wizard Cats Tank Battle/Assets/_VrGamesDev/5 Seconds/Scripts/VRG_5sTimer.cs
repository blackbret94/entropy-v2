using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// The timer of 5 seconds:
    /// - It can Lose the game,
    /// - Stop the timer,
    /// - set a Max timer,
    /// - Add or substract time to the timer
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class VRG_5sTimer : VRG_Base
    {
        [Header("FROM: Components")]
        /// <summary>
        /// The text UI component to display the countdown
        /// </summary>
        [Tooltip("The text UI component to display the countdown")]
        [SerializeField] private Text m_Text = null;


        [Header("FROM: Clock")]

        /// <summary>
        /// The limit of the timer, it can't go above this value
        /// </summary>
        [Tooltip("The limit of the timer, it can't go above this value")]
        [SerializeField] private float m_Time5SecondLimit = 5.0f;
        

        /// <summary>
        /// The detail of the floating point
        /// </summary>
        [Tooltip("The detail of the floating point")]
        [SerializeField] private int m_Decimals = 2;

        /// #IGNORE
        [Tooltip("The Maximum time to play, 5 seconds is the default, you know, Five seconds game")]
        [SerializeField] private float m_TimeMax = 5.0f;
        /// <summary>
        /// The Maximum time to play, 5 seconds is the default, you know, Five seconds game
        /// </summary>
        public float timeMax { get { return this.m_TimeMax; } set { this.m_TimeMax = value; } }

        [Tooltip("When it is stopped,trigget the event on Star")]
        //[SerializeField]
        /// #IGNORE
        private float m_Time = 0.0f;
        /// <summary>
        /// The current time of the timer
        /// </summary>
        public float time { get { return this.m_Time; } }


        [Header("FROM: Events")]

        /// #IGNORE
        [Tooltip("The time limit to win")]
        [SerializeField] private float m_WinTime = 1.0f;
        /// <summary>
        /// The time limit to win
        /// </summary>
        public float winTime { get { return this.m_WinTime; } set { this.m_WinTime = value; } }

        /// #IGNORE
        [Tooltip("The time limit to win")]
        [SerializeField] private int m_WinRound = 20;
        /// <summary>
        /// The time limit to win
        /// </summary>
        public int winRound { get { return this.m_WinRound; } set { this.m_WinRound = value; } }

        /// <summary>
        /// When you lose the game
        /// </summary>
        [Tooltip("When you lose the game")]
        [SerializeField] private GameObject[] m_OnFail = null;

        /// <summary>
        /// When it is stopped,trigget the event on Pass
        /// </summary>
        [Tooltip("When it is stopped,trigget the event on Pass")]
        [SerializeField] private GameObject[] m_OnPass = null;

        /// <summary>
        /// When it is stopped,trigget the event on Star
        /// </summary>
        [Tooltip("When it is stopped,trigget the event on Star")]
        [SerializeField] private GameObject[] m_OnStar = null;




        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        [Tooltip("When you lose the game")]
        //[SerializeField]
        /// #IGNORE
        private bool m_IsRunning = false;


        /// <summary>
        /// Find the Text Element, it is RequireComponent, double check it exist and it is assigned
        /// </summary>
        // Use this for initialization use new to get 
        private void Awake()
        {
            // it isn't running by default
            this.m_IsRunning = false;

            // make sure it has a text component
            this.m_Text = this.FindMy(this.m_Text);

            // the current time is the same as the max
            this.m_Time = this.m_TimeMax;

            // display the data into the Text
            this.Display();
        }

        /// <summary>
        /// Update and display the data into the Text
        /// </summary>
        private void Display()
        {
            // take into account the decimals for the floating point display
            this.m_Text.text = this.m_Time.ToString("F" + this.m_Decimals.ToString());
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Run the timer and count backward from the m_TimeMax.
        /// </summary>
        /// <returns></returns>
        // do your thing
        protected override IEnumerator Do()
        {
            // reset the time current 
            this.m_Time = this.m_TimeMax;

            // it will start to run
            this.m_IsRunning = true;

            // do it as long as it is not zero
            while (this.m_Time > 0)
            {
                // substract the deltatime
                this.m_Time -= Time.deltaTime;

                // if it is done
                if (this.m_Time <= 0.0f)
                {
                    // set it to zero
                    this.m_Time = 0.0f;
                }

                // display the new time value
                this.Display();

                // next frame
                yield return null;
            }

            // next frame
            yield return null;

            // if it was running
            if (this.m_IsRunning)
            {
                // cycle the fail listeners
                foreach (GameObject child in this.m_OnFail)
                {
                    // tell everybody it failed
                    child.SetActive(true);
                }
            }

            // it is not running anymore
            this.m_IsRunning = false;

            // next frame
            yield return null;
        }

        /// <summary>
        /// When you lose the game stop everything and call the OnFail listeners
        /// </summary>
        public void Lose()
        {
            // stop running
            this.m_IsRunning = false;

            // stop the routines
            StopAllCoroutines();

            // cycle the Fails listeners
            foreach (GameObject child in this.m_OnFail)
            {
                // activate them
                child.SetActive(true);
            }
        }

        /// <summary>
        /// Stop the timer, and call the listeners depending on the timer
        /// </summary>
        public void Stop()
        {
            // stop running
            this.m_IsRunning = false;

            // stop the routines
            StopAllCoroutines();

            // if the timer is greater than the winning condition
            if (this.m_TimeMax >= this.m_WinTime)
            {
                // cycle the OnPass listeners
                foreach (GameObject child in this.m_OnPass)
                {
                    if (child != null)
                    {
                        // activate it
                        child.SetActive(true);
                    }
                    else
                    {
                        this.Logs("There is an OnPass GameObject NUL", ENUM_Verbose.WARNING);
                    }
                }
            }

            // if you win
            else
            {
                // cycle the OnStar listeners
                foreach (GameObject child in this.m_OnStar)
                {
                    // activate it
                    child.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Add an amount of seconds to the max timer
        /// </summary>
        /// <param name="valueLocal">The value to add to the max timer</param>
        public void Max(float valueLocal)
        {
            // add the value to the max
            this.m_TimeMax += valueLocal;

            if (this.m_TimeMax > this.m_Time5SecondLimit)
            {
                this.m_TimeMax = this.m_Time5SecondLimit;
            }

            // If it is not runnning
            if (!this.m_IsRunning)
            {
                // update the current time
                this.m_Time = this.m_TimeMax;

                // update the display
                this.Display();
            }
        }

        /// <summary>
        /// Add an ammount of seconds to current time
        /// </summary>
        /// <param name="valueLocal">The value to add to the current time</param>
        public void Add(float valueLocal)
        {
            // add teh value
            this.m_Time += valueLocal;

            // update the display
            this.Display();
        }

    }
}