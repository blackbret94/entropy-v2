using System.Collections;

using UnityEngine;

using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Display a number during a duration seconds, it grows slowly with events to catch events
    /// </summary>
    public class VRG_GrowingNumber : VRG_Base
    {
        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        /// <summary>
        /// The number we will star to count from
        /// </summary>
        [Tooltip("The number we will star to count from")]
        [SerializeField] protected Text m_Origin = null;

        /// <summary>
        /// Where we will add the origin data
        /// </summary>
        [Tooltip("Where we will add the origin data")]
        [SerializeField] protected Text m_Target = null;

        /// <summary>
        /// If we want to start from zero or to continue from Target Data
        /// </summary>
        [Tooltip("If we want to start from zero or to continue from Target Data")]
        [SerializeField] protected bool m_ResetToZero = true;

        /// <summary>
        /// The detail of the floating point 
        /// </summary>
        [Tooltip("The detail of the floating point ")]
        [SerializeField] protected int m_Decimals = 2;

        /// <summary>
        /// How fast or slow the duration of the counting
        /// </summary>
        [Tooltip("How fast or slow the duration of the counting")]
        [SerializeField] protected float m_Duration = 0.5f;


        [Header("FROM: Events")]
        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] protected GameObject[] m_WhenBegin = null;
        

        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] protected GameObject[] m_WhenAdd = null;
        

        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] protected GameObject[] m_WhenDone = null;
        

        private void Awake()
        {
            // search for target
            this.m_Target = this.FindMy(this.m_Target);

            // and origin
            if (this.m_Origin == null)
            {
                // if not declared it is an error, just destroy it
                this.Logs("VRG_GrowingNumber need a origin text to read from", ENUM_Verbose.ERROR);
                Object.Destroy(this.gameObject);
            }
        }

        protected override IEnumerator Do()
        {
            // all the temporal floats needed to grow the number
            float fNumberFinal, fNumberCurrent, fNumberNormalized = 0, fNumberPrevious, fSpeed;

            // if no origin or target defined, don't even try it
            if (this.m_Origin != null && this.m_Target != null)
            {
                // by default is zero
                fNumberCurrent = float.Parse((0.000000000f).ToString("F" + this.m_Decimals.ToString()));

                // ... unless
                if (!this.m_ResetToZero)
                {
                    // the number will continue from the previous one
                    fNumberCurrent = float.Parse(this.m_Target.text);
                }

                // make the final number the current plus the origin
                fNumberFinal = fNumberCurrent + float.Parse(this.m_Origin.text);

                // as slow as defined by duration
                fSpeed = fNumberFinal / this.m_Duration;


                foreach (GameObject child in this.m_WhenBegin)
                {
                    if (child != null)
                    {
                        // activate it
                        child.SetActive(true);
                    }
                    else
                    {
                        this.Logs(this.name + " | There is a null element in the Begin array", ENUM_Verbose.WARNING);
                    }
                }

                // do it while we are not at the final number
                while (fNumberCurrent < fNumberFinal)
                {
                    // Normalize the number to play a sound or to inform the number changed
                    fNumberPrevious = fNumberNormalized;

                    // get the new normalized number according to the decimals
                    fNumberNormalized = float.Parse(fNumberCurrent.ToString("F" + this.m_Decimals.ToString()));

                    // set it in the target text
                    this.m_Target.text = fNumberNormalized.ToString();

                    // increase the current number by the speed
                    fNumberCurrent += Time.deltaTime * fSpeed;

                    // if it changed
                    if (fNumberPrevious != fNumberNormalized)
                    {
                        // inform the suscribed objects 
                        foreach (GameObject child in this.m_WhenAdd)
                        {
                            if (child != null)
                            {
                                // activate it
                                child.SetActive(true);
                            }
                            else
                            {
                                this.Logs(this.name + " | There is a null element in the Add array", ENUM_Verbose.WARNING);
                            }
                        }
                    }

                    // next frame
                    yield return null;
                }

                // set it to the total, just in case it is a little above depending on the render speed
                this.m_Target.text = fNumberFinal.ToString("F" + this.m_Decimals.ToString());

                foreach (GameObject child in this.m_WhenDone)
                {
                    if (child != null)
                    {
                        // activate it
                        child.SetActive(true);
                    }
                    else
                    {
                        this.Logs(this.name + " | There is a null element in the Done array", ENUM_Verbose.WARNING);
                    }
                }

            }

            // next frame
            yield return null;
        }


    }
}