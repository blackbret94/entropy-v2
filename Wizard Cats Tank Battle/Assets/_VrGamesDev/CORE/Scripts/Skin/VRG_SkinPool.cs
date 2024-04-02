using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;



namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	/// 
	public class VRG_SkinPool : VRG_Base
	{
		// The available status, ask for this property to know if it is available to use.
		public static int skinSessionData { get { return 17; } }

		[Tooltip("The available status, ask for this property to know if it is available to use")]
		//[SerializeField]//
		private bool m_IsReady = false;
		/// <summary>
		/// The available status, ask for this property to know if it is available to use
		/// </summary>
		public static bool isReady { get { return Instance.m_IsReady; } }

		/// <summary>
		/// The following objects will not be modified
		/// </summary>
		[Tooltip("The following objects will not be modified")]
		[SerializeField] private bool m_IncludeCameraColor = true;

		[Header("From: Skin")]
		/// <summary>
		/// The current skin
		/// </summary>
		[Tooltip("The current skin")]
		[SerializeField]
		protected int m_Current = -1;

		/// <summary>
		/// All the skin data colors
		/// </summary>
		[Tooltip("All the skin data colors")]
		[SerializeField] private VRG_Skin m_Skin = new VRG_Skin();
		public static VRG_Skin skin
		{
			get
			{
				if (Instance.m_Current < 0)
				{
					return null;
				}
				else
				{
					return Instance.m_Skin;
				}
			}
		}



		/// <summary>
		/// The skin pool
		/// </summary>
		[Tooltip("The  skin pool")]
		[SerializeField] protected List<Transform> m_Pool = new List<Transform>();
		public static List<Transform> pool
		{
			get
			{
				List<Transform> listReturn = null;

				if (Instance != null)
				{
					listReturn = Instance.m_Pool;
				}

				return listReturn;
			}
		}


		public VRG_SkinPool()
		{
			this.m_PlayOnEnable = false;
			this.m_SelfTurnOff = false;
			this.m_NextFrame = false;
		}





		/// <summary>
		/// Singleton pattern, Instance is the unique variable in the scene from this class
		/// </summary>
		public static VRG_SkinPool Instance; private void Awake()
		{
			// I will check if I am the first singletong
			if (Instance == null)
			{
				// ... since i am the one, I declare myself as the one
				Instance = this;

				// i follow my own rules
				this.transform.SetParent(null);

				// ... and I will live forever
				DontDestroyOnLoad(this);

				// check and build the data from pool
				StartCoroutine(this.Awake_IEnumerator());
			}
			else
			{
				// lets do it for every child
				int iTotal = this.transform.childCount;

				// cycle the transform of the skin pool
				for (int i = 0; i < iTotal; i++)
				{
					this.transform.GetChild(0).SetParent(VRG_SkinPool.Instance.gameObject.transform);
				}

				VRG_SkinPool.UpdateChildList();

				// I am not the one the prophecy said ... I will walk to the eternal darkness
				Destroy(this.gameObject);
			}
		}




		public static IEnumerator IsValid()
		{
			yield return VRG_SkinPool.IsValid(true);
		}
		/// <summary>
		/// Ask for this ienumerator if the VRG_SkinPool is ready
		/// </summary>
		/// <returns>Null when the VRG_SkinPool is ready to be queried</returns>
		public static IEnumerator IsValid(bool valueLocal)
		{
			// Let's assume everything is configured properly
			bool bContinue = true;

			// count 1 second to find the VRG_SkinPool
			int i = 0;

			if (VRG_SkinPool.Instance == null)
			{
				if (GameObject.FindObjectOfType<VRG_SkinPool>() == null)
				{
					bContinue = false;
				}
			}

			// If after 30 frames you can't get the VRG_SkinPool object it's probable not configured
			while (VRG_SkinPool.Instance == null && bContinue)
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
				while (VRG_SkinPool.isReady == false)
				{
					yield return null;
				}
				
				if (Instance.m_Pool.Count <= 0)
				{
					bContinue = false;
				}
			}

			if (valueLocal && !bContinue)
			{
				VRG_Bhel.Do
				(
					"Please be sure a VRG_SkinPool prefab is added to the scene (if you want to use skins)",
					"VRG_SkinPool->IsValid()",
					ENUM_Verbose.WARNING,
					"Static Method"
				);
			}

			// go to next frame
			yield return null;
		}

		public static void UpdateChildList()
		{
			if (Instance != null)
            {
				// check and build the data from pool
				Instance.StartCoroutine(Instance.Awake_IEnumerator());
			}
		}

		private IEnumerator Awake_IEnumerator()
		{
			this.m_IsReady = false;

			// search for the saved skin
			string sCurrent = VRG_Session.GetString("Skin", "Current");

			this.m_Pool.Clear();

			// check all the childs for the different profiles
			int i = 0;
			foreach (Transform child in this.transform)
			{
				VRG_SessionData sessionData = child.GetComponentInChildren<VRG_SessionData>(true);
				VRG_SkinApply skinApply = child.GetComponentInChildren<VRG_SkinApply>(true);

				// if it has session data
				if (sessionData != null && skinApply != null)
				{
					sessionData.name = sessionData.value;

					// add it to the pool
					this.m_Pool.Add(sessionData.transform);

					// if we have a skin saved
					if (sCurrent.Trim() != "")
					{
						// check if this session data has 
						if (skinApply.sessionData == sCurrent)
						{
							this.m_Current = i;
						}
					}
				}

				i++;
			}

			this.m_IsReady = true;

			yield return null;
		}

		protected override IEnumerator Do()
		{
			// if there are pool
			if (this.m_Pool.Count > 0)
			{
				// go the next
				this.m_Current++;

				// if the pool is surpased
				if (this.m_Current >= this.m_Pool.Count)
				{
					// restart the skin
					this.m_Current = -1;
					VRG_Session.SetString("Skin", "Current", "");
				}

				if (this.m_Current >= 0)
				{
					// activate the next skin
					this.m_Pool[this.m_Current].gameObject.SetActive(true);
				}
			}

			yield return null;
		}

		public void Play(string valueLocal)
		{
			if (Instance != null)
			{
				if (valueLocal == string.Empty)
                {
					this.m_Current = this.m_Pool.Count;

					VRG_SkinPool.Instance.Play();
				}
				else
                {
					for (int i = 0; i < this.m_Pool.Count; i++)
					{
						if (this.m_Pool[i].name == valueLocal)
						{
							this.m_Current = i - 1;

							VRG_SkinPool.Instance.Play();

							break;
						}
					}
				}
			}
		}

		public static string GetName()
		{
			string sReturn = string.Empty;

			if (Instance != null)
            {
				if (Instance.m_Current >= 0 && Instance.m_Current < Instance.m_Pool.Count)
				{
					sReturn = Instance.m_Pool[Instance.m_Current].name;
				}
			}

			return sReturn.Trim();
		}

		public static string GetNextName()
		{
			string sReturn = string.Empty;

			if (Instance != null)
			{
				int iIndex = Instance.m_Current + 1;

				if (iIndex >= Instance.m_Pool.Count)
				{
					// restart the skin
					iIndex = -1;
				}

				if (iIndex >= 0)
				{
					sReturn = Instance.m_Pool[iIndex].name;
				}
			}

			// if the pool is surpased
			return sReturn.Trim();
		}

		public static void Set(VRG_Skin valueLocal)
		{
			if (valueLocal != null)
			{
				Instance.m_Skin.Set(valueLocal);

				string sSkin = VRG_Session.GetString("Skin", "Current");

				for (int i = 0; i < Instance.m_Pool.Count; i++)
				{
					if (sSkin == Instance.m_Pool[i].name)
					{
						Instance.m_Current = i;
						break;
					}
				}

				// get all the VRG_UI
				VRG_UI[] vrg_UI = Object.FindObjectsOfType<VRG_UI>();

				if (vrg_UI.Length > 0)
				{
					// cycle the UI objects
					foreach (VRG_UI child in vrg_UI)
					{
						child.Play();
					}
				}
				else
				{
					if (Instance.m_IncludeCameraColor)
					{
						VRG_CameraBackground vrg_CameraBackground = Object.FindObjectOfType<VRG_CameraBackground>();

						if (vrg_CameraBackground != null)
						{
							vrg_CameraBackground.color = VRG_SkinPool.skin.thirdColor;
						}
					}
				}
			}
		}

	}
}