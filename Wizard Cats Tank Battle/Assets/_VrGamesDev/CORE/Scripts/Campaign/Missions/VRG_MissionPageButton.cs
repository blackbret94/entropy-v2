using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// The button in the page, it is just a button with a "go to the mission" funcionality
    /// It is configured by the pages with the Id of the mission, if you want to use a string
    /// name, you can configure it in the VRG_Remote using the following syntax
    /// 
    /// "VRG_MissionPageButton." + this.m_Id
    ///
    /// As a string, if there are an image with that name, it will use it as the button label
    /// 
    /// </summary>
    public class VRG_MissionPageButton : VRG_Base
    {

        /// <summary>
        /// My Own Id, it is the mission i will try to load
        /// </summary>
        [Tooltip("My Own Id, it is the mission i will try to load")]
        [SerializeField] private int m_Id = 0;

        /// <summary>
        /// To know if i need to draw a star
        /// </summary>
        [Tooltip("aTo know if i need to draw a starsdasd")]
        [SerializeField] private bool m_Star = false;

        /// <summary>
        /// If i am still locked, i won't be pushable
        /// </summary>
        [Tooltip("If i am still locked, i won't be pushable")]
        [SerializeField] private bool m_Interactable = false;

        [Header("From: Components")]
        /// <summary>
        /// The star game object
        /// </summary>
        [Tooltip("The star game object")]
        [SerializeField] private Transform m_GoStar = null;

        /// <summary>
        /// My text Game object
        /// </summary>
        [Tooltip("My text Game object")]
        [SerializeField] private Text m_GoText = null;



        /// #IGNORE
        protected override IEnumerator Do() { yield return null; }

        /// <summary>
        /// Init the button, and set itself into a mission button
        /// </summary>
        /// <param name="valueLocal">My Id, the mission i will load</param>
        /// <param name="currentLocal">The latest mission active, if am i Higher i am locked</param>
        /// <param name="starLocal">If i was star-ed, draw it</param>
        public void Init(int valueLocal, int currentLocal, bool starLocal)
        {            
            // asignar el texto al centro del boton
            this.m_Id = valueLocal;

            // set my id as my own text
            this.m_GoText.text = this.m_Id.ToString();

            // get the button compoonet
            Button button = this.GetComponent<Button>();

            // by default is locked
            this.m_Interactable = false;

            // unless i am already unlocked
            if (this.m_Id <= currentLocal)
            {
                // ... so i am Pushable
                this.m_Interactable = true;
            }

            // set it
            button.interactable = this.m_Interactable;

            // if it was stared, 
            this.m_Star = (starLocal && button.interactable);

            // ... display it
            this.m_GoStar.gameObject.SetActive(this.m_Star);

            string sName = string.Empty;

            // check if there is a remote configuration custom
            if (VRG_Remote.ExistString("VRG_MissionPageButton." + this.m_Id))
            {
                sName = VRG_Remote.GetString("VRG_MissionPageButton."+this.m_Id);
            }

            if ( !string.IsNullOrWhiteSpace(sName) )
            {
                // assign the name
                this.m_GoText.text = sName;

                print("VRG_MissionPageButtonTitles.Instance = " + VRG_MissionPageButtonTitles.Instance);

                // search for the image in case
                if (VRG_MissionPageButtonTitles.Instance)
                {
                    // search for the image
                    Image tImage = VRG_MissionPageButtonTitles.Instance.Get(this.m_GoText.text);

                    // if the image is found
                    if (tImage != null)
                    {
                        // deactivate the text
                        this.m_GoText.gameObject.SetActive(false);

                        // Activate the image
                        tImage.gameObject.SetActive(true);

                        // and make it my son
                        tImage.transform.SetParent(this.transform, false);

                        // set the same color as its button
                        if (!button.interactable)
                        {
                            tImage.color = button.colors.disabledColor;
                        }

                    }
                }
            }
        }




        /// <summary>
        /// I was clicked, load the stage and mission
        /// </summary>
        //public override void OnClick()
        public void OnClick()
        {
            this.Logs("OnClick: Campaign->Current = " + this.m_Id, "VRG_MissionPageButton->OnClick()", ENUM_Verbose.LOGS);

            // save the current mission loaded, this data is for the mission scene
            VRG_Session.SetInt("Campaign", "Current", this.m_Id);
        }



    }
}