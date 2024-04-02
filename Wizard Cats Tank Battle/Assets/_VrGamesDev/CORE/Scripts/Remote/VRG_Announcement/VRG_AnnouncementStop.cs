using System.Collections;

namespace VrGamesDev
{
    /// <summary>
    /// This class retrieve the cloud server Remote config data
    /// and display a full screen pop up announcement to the user
    /// To use it add it from menu -> Tools -> Vr Games Dev -> Announcement
    /// </summary>
    public class VRG_AnnouncementStop : VRG_Base
    {
        /// <summary>
        /// Constructor to have all true
        /// </summary>
        public VRG_AnnouncementStop()
        {
            m_PlayOnEnable = true;
            m_SelfTurnOff = true;
            m_NextFrame = true;
        }

        // when activated Do your thing, load the scene
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Announcement.IsValid();

            // is it?
            if (VRG_Announcement.Instance != null)
            {
                if (this.m_SelfTurnOff)
                {
                    this.gameObject.SetActive(false);
                }

                // stop it
                VRG_Announcement.Instance.Stop();
            }

            // ... wait until next frame
            yield return null;
        }
    }
}