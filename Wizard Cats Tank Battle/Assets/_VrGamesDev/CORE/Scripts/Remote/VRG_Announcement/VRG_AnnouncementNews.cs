using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// This class retrieve the cloud server Remote config data
    /// and display a full screen pop up announcement to the user
    /// To use it add it from menu -> Tools -> Vr Games Dev -> Announcement
    /// </summary>
    public class VRG_AnnouncementNews : VRG_Base
    {
        /// <summary>
        /// The gameobjects to affect based on the news update
        /// </summary>
        [Tooltip("The button owner if this news icon")]
        [SerializeField] private GameObject m_Button = null;

        /// <summary>
        /// The gameobjects to affect based on the news update
        /// </summary>
        [Tooltip("The gameobjects to affect based on the news update")]
        [SerializeField] private GameObject[] m_GameObjects = null;



        // when activated Do your thing, load the scene
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Announcement.IsValid(false);

            // is it?
            if (VRG_Announcement.Instance != null)
            {
                foreach (GameObject child in this.m_GameObjects)
                {
                    child.SetActive(VRG_Announcement.news);
                }
            }
            else
            {
                /*
                this.Logs
                (
                    "Please be sure a VRG_Announcement prefab is added to the scene",
                    "VRG_AnnouncementNews->Do()",
                    ENUM_Verbose.WARNING
                );
                */

                if (this.m_Button != null)
                {
                    this.m_Button.SetActive(false);
                }
            }

            // ... wait until next frame
            yield return null;
        }
    }
}