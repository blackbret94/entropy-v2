using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Create the pages with the buttons to load the missions
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class VRG_MissionPages : VRG_Base
    {
        [Header("From: Components")]
        [Tooltip("The Transform to modify with the content size")]
        //[SerializeField]
        /// #IGNORE
        private RectTransform m_RectTransform = null;

        [Tooltip("The grid to render with the content from missions")]
        //[SerializeField]
        /// #IGNORE
        private GridLayoutGroup m_GridLayoutGroup = null;



        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        [Tooltip("how far will it be rendered from X")]
        //[SerializeField]
        /// #IGNORE
        private float m_PaddingX = 0.0f;

        [Tooltip("Current mission to activate the buttons")]
        //[SerializeField]
        /// #IGNORE
        private int m_Current = 0;

        [Tooltip("The stars to add into the button")]
        //[SerializeField]
        /// #IGNORE
        private string m_Stars = "";

        [Tooltip("How many pages to factor into the display")]
        //[SerializeField]
        /// #IGNORE
        private float m_Factor = 0.0f;




        // validate everything is well configured
        /// #IGNORE
        private void Awake()
        {
            bool bContinue = true;

            this.m_RectTransform = this.FindMy(this.m_RectTransform, false);
            if (this.m_RectTransform == null)
            {
                bContinue = false;
                this.Logs("Your m_RectTransform group is Undefined", ENUM_Verbose.ERROR);
            }

            this.m_GridLayoutGroup = this.FindMy(this.m_GridLayoutGroup, false);
            if (this.m_GridLayoutGroup == null)
            {
                bContinue = false;
                this.Logs("Your m_GridLayoutGroup group is Undefined", ENUM_Verbose.ERROR);
            }

            if (!bContinue)
            {
                this.Logs("This GameObject is misconfigured, please check the data in the inspector", ENUM_Verbose.ERROR);
                Object.Destroy(this.gameObject);
            }
        }

        /// #IGNORE
        private void OnDisable()
        {
            // since we could be reloaded, recreate all the pages from zero
            // calculate every time this game object is played
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Create and configure the page
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Campaign.IsValid();

            // is it?
            if (VRG_Campaign.Instance != null)
            {
                // to check if the mission was Star-ed
                bool bStarCurrent;

                // the total buttons per page
                int iMissionPerPages = VRG_Campaign.missionColumn * VRG_Campaign.missionRow;

                // just in case a misconfiguration
                if (iMissionPerPages <= 0)
                {
                    iMissionPerPages = 1;
                }

                // how may pages will need to be created
                int iMissionGroupNumber = Mathf.CeilToInt((VRG_Campaign.missionTotal) / (float)iMissionPerPages);

                // by default we are in the mission 1, so page 1
                int iCurrentMission = 0;


                // init all the campaign variables, in case this is the first time
                //VRG_Session.SetInt("Campaign", "Current", 1);
                VRG_Session.SetInt("Campaign", "Total", VRG_Campaign.missionTotal);
                this.m_Current = VRG_Session.GetInt("Campaign", "Max") + 1;

                // validate the total is not greater than allowed
                if (this.m_Current > VRG_Campaign.missionTotal)
                {
                    // if it is, reset it
                    this.m_Current = VRG_Campaign.missionTotal;
                    VRG_Session.SetInt("Campaign", "Max", this.m_Current);
                }

                // get the stars array
                this.m_Stars = VRG_Session.GetString("Campaign", "Stars");

                //Generate iMissionGroupNumber Missions
                for (int i = 0; i < iMissionGroupNumber; i++)
                {
                    // create a new mission page prefab
                    GameObject MissionGroup = Instantiate(VRG_Campaign.missionPage, this.transform) as GameObject;

                    // ... and populate it with buttons
                    for (int j = 0; j < iMissionPerPages; j++)
                    {
                        iCurrentMission++;

                        // if somehow we are done, 
                        if (iCurrentMission > VRG_Campaign.missionTotal)
                        {
                            // ... do not create more
                            break;
                        }

                        // now, create a button
                        GameObject GO_missionButton = Instantiate(VRG_Campaign.missionButton, MissionGroup.transform) as GameObject;

                        // by default it doesn't has a star
                        bStarCurrent = false;
                        if (this.m_Stars.Contains("|" + iCurrentMission + "|"))
                        {
                            // unless it does
                            bStarCurrent = true;
                        }

                        // get the button component
                        VRG_MissionPageButton MyClassButton = GO_missionButton.GetComponent<VRG_MissionPageButton>();

                        // and init the button with the data collected
                        MyClassButton.Init(iCurrentMission, this.m_Current, bStarCurrent);
                    }
                }

                //Adjust this for the last page visited
                this.m_Factor = (iMissionGroupNumber - 1) * this.m_GridLayoutGroup.cellSize.x + (iMissionGroupNumber - 1) * this.m_GridLayoutGroup.spacing.x;

                // increase the canvas
                this.m_RectTransform.sizeDelta = new Vector2(this.m_Factor, 0);

                // and set it into the current page
                this.m_PaddingX = Mathf.Floor((this.m_Current - 1) / iMissionPerPages)
                    * (this.m_GridLayoutGroup.cellSize.x + this.m_GridLayoutGroup.spacing.x) * (-1);

                // fix the size
                this.m_RectTransform.anchoredPosition = new Vector2(this.m_PaddingX, 0);
            }

            // next frame
            yield return null;
        }



    }
}