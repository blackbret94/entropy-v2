using System;
using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// This class modify the alpha value of an element <a href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/class-CanvasGroup.html">Canvas group</a>
    /// It can fade in and fade out, it has events on every step to inform the other objects 
    /// </summary>
    public class VRG_Fader : VRG_Base
    {
        [Tooltip("Flag that inform the class if it should fade out after the fade in is finished.")]
        [SerializeField] private bool m_AutoFadeOut = true;
        /// <summary>
        /// Public Getter: from m_AutoFadeOut - Flag that inform the class if it should fade out after the fade in is finished
        /// </summary>
        public bool autoFadeOut
        {
            get
            {
                return this.m_AutoFadeOut;
            }
            set
            {
                this.m_AutoFadeOut = value;
            }
        }

        /// <summary>
        /// Array that can activate some gameobjects, When it starts the fading
        /// </summary>
        [Tooltip("The items to turn on when the fade occurs")]
        [SerializeField] private GameObject[] m_ObjectsToFade = null;

        /// <summary>
        /// Flag that inform what the class is doing.
        /// </summary>
        [Tooltip("Flag that inform what the class is doing")]
        [SerializeField] private ENUM_Fader m_Status = ENUM_Fader.NONE;
        public ENUM_Fader status { get { return this.m_Status; } }



        [Header("From: Fade In")]
        [Tooltip("How long will wait before it apply the fade in")]
        [SerializeField] private float m_FadeInDelay = 0.0f;
        /// <summary>
        /// How long will wait before it apply the fade in
        /// </summary>
        public float fadeInDelay
        {
            get
            {
                return this.m_FadeInDelay;
            }
            set
            {
                this.m_FadeInDelay = value;
            }
        }

        [Tooltip("The duration of the Fade In from 0% to 100%")]
        [SerializeField] private float m_FadeInDuration = 0.50f;
        /// <summary>
        /// The duration in seconds for how long will it takes to apply the fade to alpha 100%
        /// </summary>
        public float fadeInDuration
        {
            get
            {
                return this.m_FadeInDuration;
            }
            set
            {
                this.m_FadeInDuration = value;
            }
        }

        [Header("From: Fade Out")]
        [Tooltip("How long will wait before it apply the fade out")]
        [SerializeField] private float m_FadeOutDelay = 0.10f;
        /// <summary>
        /// How long will wait before it apply the fade out
        /// </summary>
        public float fadeOutDelay
        {
            get
            {
                return this.m_FadeOutDelay;
            }
            set
            {
                this.m_FadeOutDelay = value;
            }
        }

        [Tooltip("How long will take to apply the fade")]
        [SerializeField] private float m_FadeOutDuration = 0.50f;
        /// <summary>
        /// The duration in seconds for how long will it takes to apply the fade to alpha 0%
        /// </summary>
        public float fadeOutDuration
        {
            get
            {
                return this.m_FadeOutDuration;
            }
            set
            {
                this.m_FadeOutDuration = value;
            }
        }


        [Header("FROM Components")]
        /// <summary>
        /// Canvas group that has the alpha property
        /// </summary>
        [Tooltip("Canvas group that has the alpha property")]
        //[SerializeField]
        private CanvasGroup m_CanvasGroup = null;


        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("This save the next fade on play")]
        //[SerializeField]
        private bool m_PlayFadeInOrOut = true;


        /// <summary>
        /// Public Event: OnFadeIn = When the fader finished the fade in, this event is triggered 
        /// </summary>
        public event Action WhenFadeIn;

        /// <summary>
        /// Public Event: OnFadeOut = When the fader finished the fade OUT, this event is triggered 
        /// </summary>
        public event Action WhenFadeOut;

        // make sure you have a groun canvas
        private void Awake()
        {
            // if null == search for the group canvas in my own ppl
            if (this.m_CanvasGroup == null)
            {
                this.m_CanvasGroup = this.GetComponent<CanvasGroup>();
            }
        }

        // be sure that all events are unsubscribed when this is destroyed.
        private void OnDestroy()
        {
            // two events, two destroys
            this.WhenFadeIn = null;
            this.WhenFadeOut = null;
        }





        /// <summary>
        /// Public method, to start the fading process
        /// </summary>
        /// <param name="valueLocal">
        /// <strong>TRUE</strong> = Fade in<br></br>
        /// <strong>FALSE</strong> = Fade out
        /// </param>
        public void Play(bool valueLocal)
        {
            this.m_PlayFadeInOrOut = valueLocal;

            base.Play();
        }


        protected override IEnumerator Do()
        {
            // if canvas group is still valid fade in or out from ValueLocal
            if (this.m_CanvasGroup != null)
            {
                if (this.m_PlayFadeInOrOut)
                {
                    yield return StartCoroutine(this.FadeIn());
                }
                else
                {
                    yield return StartCoroutine(this.FadeOut());
                }
            }

            yield return null;
        }

        // modify the alpha value up to 1, solid group canvas
        private IEnumerator FadeIn()
        {
            // update the status
            this.m_Status = ENUM_Fader.FADE_IN;

            // the duration start from 0 up to the duration
            float fCurrent = 0.0f;

            // start from fully transparent
            this.m_CanvasGroup.alpha = 0.0f;

            // if there is a delay
            if (this.m_FadeInDelay > 0)
            {
                // wait the delay
                yield return new WaitForSeconds(m_FadeInDelay);
            }

            // make all the childs visible
            foreach (GameObject child in this.m_ObjectsToFade)
            {
                child.SetActive(true);
            }

            while (this.m_CanvasGroup.alpha < 1.0f)
            {
                fCurrent += Time.deltaTime;

                this.m_CanvasGroup.alpha = fCurrent / this.m_FadeInDuration;

                yield return null;
            }

            // when it is done, make it fully visible
            this.m_CanvasGroup.alpha = 1.0f;

            // invoke the events delegateds
            this.WhenFadeIn?.Invoke();

            // update the status
            this.m_Status = ENUM_Fader.FADED;

            // in case is an autocycle
            if (this.autoFadeOut)
            {
                // fade it out
                this.Play(false);
            }

            yield return null;
        }


        // modify the alpha value up to 0, solid group canvas
        private IEnumerator FadeOut()
        {
            // update the status
            this.m_Status = ENUM_Fader.FADE_OUT;

            // start the time at the maximum 
            float fCurrent = this.m_FadeOutDuration;

            // start from fully visible
            this.m_CanvasGroup.alpha = 1.0f;

            // wait in case 
            if (this.m_FadeOutDelay > 0)
            {
                //... there is a delay
                yield return new WaitForSeconds(m_FadeOutDelay);
            }

            while (this.m_CanvasGroup.alpha > 0.0f)
            {
                fCurrent -= Time.deltaTime;

                this.m_CanvasGroup.alpha = fCurrent / this.m_FadeOutDuration;

                yield return null;
            }

            // finish with alpha fully invisible
            this.m_CanvasGroup.alpha = 0.0f;

            // cycle all childs and deactivate them
            foreach (GameObject child in this.m_ObjectsToFade)
            {
                child.SetActive(false);
            }

            // invoke the events delegateds
            this.WhenFadeOut?.Invoke();

            // update the status
            this.m_Status = ENUM_Fader.NONE;

            // by default restart the on enable 
            this.m_PlayFadeInOrOut = true;

            yield return null;
        }


    }
}