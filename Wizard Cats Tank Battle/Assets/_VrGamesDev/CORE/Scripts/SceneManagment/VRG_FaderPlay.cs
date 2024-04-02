using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

///#IGNORE
namespace VrGamesDev
{
    ///#IGNORE
    public class VRG_FaderPlay : VRG_Base
    {
        /// <summary>
        /// The VRG_Fader prefab
        /// </summary>
        [Tooltip("The VRG_Fader prefab")]
        [SerializeField] private bool m_FadeIn = true;

        /// <summary>
        /// The VRG_Fader prefab
        /// </summary>
        [Tooltip("The VRG_Fader prefab")]
        [SerializeField] private VRG_Fader m_VRG_Fader = null;

        /// <summary>
        /// Constructor: turn it off as soon as it plays
        /// </summary>
        public VRG_FaderPlay()
        {
            m_SelfTurnOff = true;
        }

        private void Awake()
        {
            this.m_VRG_Fader = this.FindMy(this.m_VRG_Fader);
        }

        protected override IEnumerator Do()
        {
            // Fade the fader
            this.m_VRG_Fader.Play(this.m_FadeIn);

            // go to next frame
            yield return null;
        }



        /*
        private void Start()
        {
            // Suscribe to the OnFadeOut
            this.m_VRG_Fader.OnFadeOut += OnFadeOut;
        }
        private void OnDestroy()
        {
            // Unsuscribe to the OnFadeOut
            this.m_VRG_Fader.OnFadeOut -= OnFadeOut;
        }
        private void OnFadeOut()
        {
            // Disable the object
            this.gameObject.SetActive(false);
        }
        */


    }
}