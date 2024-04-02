using System.Collections;

using UnityEngine;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// This class handles all the data of the game missions
	/// and it can be accessed by its public static methods,
    /// VRG_Campaign.Instance works with the singleton pattern.
	/// </summary>
	public class VRG_Campaign : VRG_Base
	{
		/// #IGNORE
		[Tooltip("You can ask for this variable to know if the object is ready or is still querying information")]
		private bool m_IsReady = false;
		/// <summary>
		/// You can ask for this variable to know if the object is ready or is still querying information
		/// </summary>
		public static bool isReady { get { return Instance.m_IsReady; } }

		/// #IGNORE
		[Tooltip("The page container the mission buttons")]
		[SerializeField] private GameObject m_MissionPage = null;
		/// <summary>
		/// The page container the mission buttons
		/// </summary>
		[SerializeField] public static GameObject missionPage { get { return Instance.m_MissionPage; } }

		/// #IGNORE
		[Tooltip("The button prefabs into the mission pages")]
		[SerializeField] private GameObject m_MissionButton = null;
		/// <summary>
		/// The button prefabs into the mission pages
		/// </summary>
		[SerializeField] public static GameObject missionButton { get { return Instance.m_MissionButton; } }

		/// #IGNORE
		[Tooltip("The total missions in your game")]
		[SerializeField] private int m_MissionTotal = 50;
		/// <summary>
		/// The total missions in your game, this data will be used to generate the missions pages
		/// </summary>
		[SerializeField] public static int missionTotal { get { return Instance.m_MissionTotal; } }

		/// #IGNORE
		[Tooltip("The missions columns shown per page, it will used to genereate the mission pages")]
		[SerializeField] private int m_MissionColumn = 3;
		/// <summary>
		/// The missions columns shown per page, it will used to genereate the mission pages
		/// </summary>
		[SerializeField] public static int missionColumn { get { return Instance.m_MissionColumn == 0 ? 1 : Instance.m_MissionColumn; } }

		/// #IGNORE
		[Tooltip("The missions rows shown per page, it will used to genereate the mission pages")]
		[SerializeField] private int m_MissionRow = 4;
		/// <summary>
		/// The missions rows shown per page, it will used to genereate the mission pages
		/// </summary>
		[SerializeField] public static int missionRow { get { return Instance.m_MissionRow == 0 ? 1 : Instance.m_MissionRow; } }



		[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
		
		[Tooltip("The max level of the campaing")]
		//[SerializeField]
		/// #IGNORE
		private int m_CampaingMax = 0;

		[Tooltip("Current level loaded")]
		//[SerializeField]
		/// #IGNORE
		private int m_CampaingCurrent = 0;

		[Tooltip("Total possible")]
		//[SerializeField]
		/// #IGNORE
		private int m_CampaingTotal = 0;

		[Tooltip("The levels with stars")]
		//[SerializeField]
		/// #IGNORE
		private string m_CampaingStars = "";



		/// <summary>
		/// Singleton pattern, Instance property should be the unique class in the scene
		/// </summary>
		public static VRG_Campaign Instance;

		/// #IGNORE
		private void Awake()
		{
			// Checking if I am the first instance
			if (Instance == null)
			{
				// I am the one
				Instance = this;

				// i follow my own rules
				this.transform.SetParent(null);

				// ... and I will live forever
				DontDestroyOnLoad(this);
			}
			else
			{
				// I am not worthy
				Destroy(this.gameObject);
			}
		}

		/// #IGNORE
		protected override IEnumerator Do()
		{
			this.m_IsReady = true;

			yield return null;
		}

		/// #IGNORE
		private void OnDisable()
        {
			this.m_IsReady = false;
		}


		/// <summary>
        /// You can yield this function to wait until it is ready, and instantiated
        /// 
        /// </summary>
        /// <returns></returns>
		public static IEnumerator IsValid()
		{
			yield return VRG_Campaign.IsValid(true);
		}
		/// <summary>
		/// Ask for this ienumerator if the VRG_Campaign.instance is ready
		///
		/// yield return VRG_Campaign.IsValid();
        /// 
        /// Then ask for the Instance object if it is not null
        /// 
        /// if (VRG_Campaign.Instance != null)
        /// 
		/// </summary>
		/// <returns>Null when the VRG_Campaign is ready to be queried</returns>
		public static IEnumerator IsValid(bool valueLocal)
		{
			// Let's assume everything is configured properly
			bool bContinue = true;

			// count 1 second to find the VRG_Campaign
			int i = 0;

			if (VRG_Campaign.Instance == null)
			{
				if (GameObject.FindObjectOfType<VRG_Campaign>() == null)
				{
					bContinue = false;
				}
			}

			// If after 30 frames you can't get the VRG_Campaign object it's probable not configured
			while (VRG_Campaign.Instance == null && bContinue)
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
				while (VRG_Campaign.isReady == false)
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
						"Please be sure a VRG_Campaign prefab is added to the scene",
						"VRG_Campaign->IsValid()",
						ENUM_Verbose.WARNING,
						"Static Method"
					);
				}
			}

			// go to next frame
			yield return null;
		}

		/// #IGNORE
		private void Start()
        {
			// update campaing data
			this.UpdateCampaignSession();
		}

		/// #IGNORE
		// check the session for the data configured
		private void UpdateCampaignSession()
        {
			Instance.m_CampaingMax =		VRG_Session.GetInt("Campaign", "Max");
			Instance.m_CampaingCurrent =	VRG_Session.GetInt("Campaign", "Current");
			Instance.m_CampaingTotal =		VRG_Session.GetInt("Campaign", "Total");
			Instance.m_CampaingStars =		VRG_Session.GetString("Campaign", "Stars");
		}

		/// <summary>
		/// Check for the integrity of the mission loaded, if you try to load an invalid mission, it will return false
		/// </summary>
		/// <returns></returns>
		public static bool Integrity()
		{
			// by default the mission is ok
			bool bReturn = true;

			// update campaing data
			Instance.UpdateCampaignSession();

			// if the current is over total or the max, or it is a negative mission
			if (
					Instance.m_CampaingCurrent > Instance.m_CampaingTotal
					|| Instance.m_CampaingMax > Instance.m_CampaingTotal
					|| Instance.m_CampaingCurrent > Instance.m_CampaingMax + 1
					|| Instance.m_CampaingCurrent < 1
				)
			{
				bReturn = false;
			}

			return bReturn;
		}

		/// <summary>
        /// Pass the current mission, set it as "passed" and save it to session
        /// </summary>
		public static void Pass()
		{
			// update campaing data
			Instance.UpdateCampaignSession();

			// check for the maximum
			if (Instance.m_CampaingCurrent > Instance.m_CampaingTotal || Instance.m_CampaingMax > Instance.m_CampaingTotal)
			{
				// inform the player the problem)
				Instance.Logs("HAK_Scenes.Load(Instance.m_Home); The current mission " + Instance.m_CampaingCurrent + " is invalid.");

				// send to home if invalid
				Debug.Log("VRG_FaderScene.Load(Instance.m_SceneHome);");
			}
			else
			{
				// if it is the latest mission done, update the max
				if (Instance.m_CampaingCurrent > Instance.m_CampaingMax)
				{
					// the max is now the current
					Instance.m_CampaingMax = Instance.m_CampaingCurrent;

					// save to session
					VRG_Session.SetInt("Campaign", "Max", Instance.m_CampaingMax);
				}
			}
		}

		/// <summary>
        /// Save the current mission as a starred mission
        /// </summary>
		public static void Star()
		{
			// update campaing data
			Instance.UpdateCampaignSession();

			// if it is not starred
			if (!Instance.m_CampaingStars.Contains("|" + Instance.m_CampaingCurrent + "|"))
			{
				// add it
				Instance.m_CampaingStars = "|" + Instance.m_CampaingCurrent + "|" + Instance.m_CampaingStars;

				// and save it
				VRG_Session.SetString("Campaign", "Stars", Instance.m_CampaingStars);

				// and get total
				VRG_Session.Add("Campaign", "StarTotal");
			}

			// the mission is passed, since it was starred
			VRG_Campaign.Pass();
		}

		/// <summary>
        /// Move the current to the next mission
        /// </summary>
		public static void Next()
		{
			// update campaing data
			Instance.UpdateCampaignSession();

			// Go to the next mission
			Instance.m_CampaingCurrent = VRG_Session.Add("Campaign", "Current");
		}
	}
}