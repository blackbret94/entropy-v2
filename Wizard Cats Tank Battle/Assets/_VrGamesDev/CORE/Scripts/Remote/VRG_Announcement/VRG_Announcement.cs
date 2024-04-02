using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev
{
    /// <summary>
    /// This class retrieve the cloud server Remote config data
    /// and display a full screen pop up announcement to the user
    /// To use it add it from menu -> Tools -> Vr Games Dev -> Announcement
    /// </summary>
    public class VRG_Announcement : VRG_Base
    {
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        [Tooltip("The available status, ask for this property to know if it is available")]
        [SerializeField] protected bool m_IsReady = false;
        public static bool isReady
        {
            get
            {
                bool bReturn = false;

                if (Instance != null)
                {
                    bReturn = Instance.m_IsReady;
                }

                return bReturn;
            }
        }



        /// <summary>
        /// If we have new information, this will be true
        /// </summary>
        [Tooltip("If we have new information, this will be true")]
        [SerializeField]
        private bool m_News = false;
        public static bool news { get { return Instance.m_News; } }


        //[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Header("From: Components")]

        /// <summary>
        /// From UI: The date text container
        /// </summary>
        [Tooltip("From UI: The date text container")]
        [SerializeField] private Text m_Date = null;

        /// <summary>
        /// From UI: The Title text container
        /// </summary>
        [Tooltip("From UI: The Title text container")]
        [SerializeField] private Text m_Title = null;

        /// <summary>
        /// From UI: The Body text container as a RectTransform, the information from server will be displayed here
        /// </summary>
        [Tooltip("From UI: The Body text container as a RectTransform, the information from server will be displayed here")]
        [SerializeField] private RectTransform m_Content = null;

        /// <summary>
        /// From UI: The Body text container
        /// </summary>
        [Tooltip("From UI: The Body text container")]
        [SerializeField] private Text m_Body = null;

        /// <summary>
        /// From UI: The date text container
        /// </summary>
        [Tooltip("From UI: The date text container")]
        [SerializeField] private GameObject m_Loading = null;



        /// <summary>
        /// From UI: The Body text container
        /// </summary>
        [Tooltip("From UI: The Body text container")]
        //[SerializeField]
        private float m_ContentHeight = 500.0f;

        [Header("From: Settings")]
        [Tooltip("From VRG_Remote: How many times will show to the user, 0 for infinite")]
        //[SerializeField]
        private string m_Settings_Repeat = "VRG_Announcement.repeat";

        [Tooltip("From VRG_Remote: The date, this is when the anncouncement was made")]
        //[SerializeField]
        private string m_Settings_Date = "VRG_Announcement.date";

        [Tooltip("From VRG_Remote: The title of the current anncouncement")]
        //[SerializeField]
        private string m_Settings_Title = "VRG_Announcement.title";

        [Tooltip("From VRG_Remote: The body of the message")]
        //[SerializeField]
        private string m_Settings_Body = "VRG_Announcement.body";



        /// <summary>
        /// Singleton pattern, Instance is the unique variable in the scene from this class
        /// </summary>
        public static VRG_Announcement Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // start the loading of the information
                StartCoroutine(this.Awake_IEnumerator());
            }
            else
            {
                // I am not the one the prophecy said ... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }




        public static IEnumerator IsValid()
        {
            yield return VRG_Announcement.IsValid(true);
        }
        /// <summary>
        /// Ask for this ienumerator if the VRG_Announcement is ready
        /// </summary>
        /// <returns>Null when the VRG_Announcement is ready to be queried</returns>
        public static IEnumerator IsValid(bool valueLocal)
        {
            // Let's assume everything is configured properly
            bool bContinue = true;

            // count 1 second to find the VRG_Announcement
            int i = 0;

            if (VRG_Announcement.Instance == null)
            {
                if (GameObject.FindObjectOfType<VRG_Announcement>() == null)
                {
                    bContinue = false;
                }
            }

            // If after 30 frames you can't get the VRG_Announcement object it's probable not configured
            while (VRG_Announcement.Instance == null && bContinue)
            {
                if (i > VRG.IsReady)
                {
                    bContinue = false;
                }
                i++;

                yield return null;
            }

            if (bContinue)
            {
                // wait until Init is done
                while (VRG_Announcement.isReady == false)
                {
                    yield return null;
                }
            }
            else
            {
                if (valueLocal)
                {
                    VRG_Bhel.Do
                    (
                        "Please be sure a VRG_Announcement prefab is added to the scene",
                        "VRG_Announcement->IsValid()",
                        ENUM_Verbose.WARNING,
                        "Static Method"
                    );
                }
            }

            // go to next frame
            yield return null;
        }








        // when activated Do your thing, load the scene
        private IEnumerator Awake_IEnumerator()
        {
            // since we don't know how long it will take to get the remote information, start deacticated
            this.Activate(false);

            // by default there isn't any new information
            this.m_News = false;

            // wait until the remote is ready
            yield return VRG_Remote.IsValid();

            // is it?
            if (VRG_Remote.Instance != null)
            {
                // get the data from server and save it to local values.
                int iRepeat = VRG_Remote.GetInt(this.m_Settings_Repeat);
                //WAS_echo("INSIDE: " + this.m_Settings_Repeat + ") " + iRepeat + " - " + Animator.StringToHash(iRepeat.ToString()));

                string sDate = VRG_Remote.GetString(this.m_Settings_Date).Trim();
                //WAS_echo("INSIDE: " + this.m_Settings_Date + ") " + sDate + " - " + Animator.StringToHash(sDate));

                string sTitle = VRG_Remote.GetString(this.m_Settings_Title).Trim();
                //WAS_echo("INSIDE: " + this.m_Settings_Title + ") " + sTitle + " - " + Animator.StringToHash(sTitle));

                string sBody = VRG_Remote.GetString(this.m_Settings_Body).Trim();
                //WAS_echo("INSIDE: " + this.m_Settings_Body + ") " + sBody + " - " + Animator.StringToHash(sBody));
            
                int iLocalRepeat = VRG_Session.GetInt("VRG_Announcement", "repeat");
                //WAS_echo("INSIDE: " + iRepeat + " > " + iLocalRepeat);

                // hash the content to be sure it is new information
                int iHash = Animator.StringToHash(sDate + sTitle + sBody);
                //WAS_echo("Inside: Hash = " + iHash);

                // compare it with the local saved.
                int sLocalHash = VRG_Session.GetInt("VRG_Announcement", "hash");

                // the anncouncement changed from latest shown
                if (sLocalHash != iHash)
                {
                    // first time this information was displayed
                    iLocalRepeat = 0;
                }

                // display in the UI the new information
                this.m_Date.text = sDate;
                this.m_Title.text = sTitle;
                this.m_Body.text = sBody.Replace("<br>", "\n");


                // just incase it is new and the content is not null
                if (
                    (iRepeat == 0 || iRepeat > iLocalRepeat)
                    && sDate != ""
                    && sTitle != ""
                    && sBody != ""
                    )
                {
                    this.m_News = true;

                    // check if it is 100% new or we need more reminders
                    if (sLocalHash != iHash)
                    {
                        VRG_Session.SetInt("VRG_Announcement", "hash", iHash);
                        VRG_Session.SetInt("VRG_Announcement", "repeat", 1);
                    }

                    // or add one more to the latest
                    else
                    {
                        //WAS_echo("iLocalRepeat = " + iLocalRepeat);
                        VRG_Session.Add("VRG_Announcement", "repeat");
                    }
                }

                this.m_Loading.SetActive(false);
            }

            // adapt to the screen of the client
            this.ResizeContent();

            // Ok, now i am ready
            this.m_IsReady = true;
            
            // ... wait until next frame
            yield return null;
        }

        // Activate the slider and the messages of the UI
        private IEnumerator Activate(bool valueLocal)
        {
            this.Logs
            (
                "<b>Announcement:</b> Activate(" + valueLocal + ")",
                "VRG_Announcement->Activate()",
                ENUM_Verbose.DEBUG
            );

            // if it still exist
            if (this != null)
            {
                // and it is in the scene
                if (this.transform != null && this.isActiveAndEnabled)
                {
                    // cycle all the childrens and activate / deacticate according to valueLocal
                    foreach (Transform child in this.transform)
                    {
                        if (child.name != "Actions")
                        {
                            child.gameObject.SetActive(valueLocal);
                        }
                    }
                }
            }

            yield return null;
        }

        // the function called when there is a remote setting
        private void ResizeContent()
        {
            // resize the body according to the information to display
            this.m_Content.offsetMax = new Vector2(this.m_Content.offsetMax.x, 0.0f);
            float fHeight = this.m_Body.preferredHeight - this.m_ContentHeight;
            if (fHeight < 0.0f)
            {
                fHeight = 0.0f;
            }
            this.m_Content.sizeDelta = new Vector2(this.m_Content.sizeDelta.x, fHeight);
        }

        // when activated Do your thing, load the scene
        protected override IEnumerator Do()
        {
            StartCoroutine(this.Activate(true));

            // ... wait until next frame
            yield return null;
        }

        // stop and close
        public void Stop()
        {
            StartCoroutine(this.Activate(false));
        }
    }
}