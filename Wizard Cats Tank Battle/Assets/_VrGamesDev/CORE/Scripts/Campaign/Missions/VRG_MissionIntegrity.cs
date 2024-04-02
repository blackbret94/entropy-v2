using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Check for the integrity of the missions, this is to stop a possible invalid load,
    /// with more missions than defined or trying to play a mission not unlocked yet
    /// </summary>
    [RequireComponent(typeof(Image))]

    public class VRG_MissionIntegrity : VRG_Base
    {
        /// <summary>
        /// The background Image, a curtain
        /// </summary>
        [Tooltip("The background Image, a curtain while checking for the integrity")]
        [SerializeField]
        private Image m_Image = null;

        /// <summary>
        /// The go to scene object to go to home in case of an invalid mission
        /// </summary>
        [Tooltip("The go to scene object to go to home in case of an invalid mission")]
        [SerializeField]
        private VRG_Delayed m_Delayed = null;

        /// <summary>
        /// The name of the Scene to load
        /// </summary>
        [Tooltip("The name of the Scene to load")]
        [SerializeField]
        [SceneName] private string m_Scene = "Home";



        /// #IGNORE
        public VRG_MissionIntegrity()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = false;
        }

        /// #IGNORE
        private void Awake()
        {
            // find my own image 
            this.m_Image = this.FindMy(this.m_Image);

            if (this.m_Image != null)
            {
                // enable it, we do not know yet if the scene is a valid one
                this.m_Image.enabled = true;
            }

            // in case we do not have any GoTOScene defined
            if (false
                || this.m_Image == null
                || this.m_Delayed == null
                )
            {
                // inform the error in the console
                this.Logs
                (
                    "<b>DESTROYED: </b> VRG_MissionIntegrity is misconfigured, some components are null",
                    "VRG_MissionIntegrity->Awake()",
                    ENUM_Verbose.ERROR
                );

                // destroy the object
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Wait for the <a href="#VrGamesDev.VRG_Campaign">VRG_Campaign</a> isValid() and to the integrity
        /// In case it is false, it will load the Scene defined
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            bool bSentScene = true;

            // Let's assume everything is configured properly
            yield return VRG_Campaign.IsValid();

            if (VRG_Campaign.Instance != null)
            {
                bool bIntegrity = VRG_Campaign.Integrity();

                // Get the integrity value
                if (bIntegrity)
                {
                    bSentScene = false;
                    this.m_Delayed.gameObject.SetActive(true);
                }
            }

            if (bSentScene)
            {
                // Go to scene Home, the scene is invalid
                SceneManager.LoadScene(this.m_Scene, LoadSceneMode.Single);
            }

            // next frame
            yield return null;
        }
    }
}