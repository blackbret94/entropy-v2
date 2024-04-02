using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// The main class from <a href="#VrGamesDev.Missions">package Missions</a>, this class handles all the data of the game
	/// and can be accessed by its public static methods, VRG_Audio.Instance as a singleton design.
	/// </summary>
	public class VRG_Audio : VRG_Base
	{
		private bool m_IsReady = false;
		/// <summary>
		/// You can ask for this variable to know if the object is ready or is still querying information
		/// </summary>
		public static bool isReady { get { return Instance.m_IsReady; } }

		[Header("From: Audio")]
		/// <summary>
		/// The base <a href="https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Audio.AudioMixer.html">AudioMixer</a>, included in the <a href="#VrGamesDev.Missions">Missions </a>package
		/// </summary>
		[Tooltip("The base AudioMixer")]
		[SerializeField] private AudioMixer m_AudioMixer = null;

		[Tooltip("The AudioEnum List")]
		//[SerializeField]
		private float m_VolumeMute = -80.0f;

		[Tooltip("The AudioEnum List")]
		//[SerializeField]
		private string[] m_ENUM_AudioMixer;

		[Tooltip("The music group volume array")]
		[Range(-80.0f, 20.0f)]
		//[SerializeField]
		private List<float> m_AudioVolumes = new List<float>();


//		[Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]



		/// <summary>
		/// Singleton pattern, Instance property should be the unique class in the scene
		/// </summary>
		public static VRG_Audio Instance; void Awake()
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

				// we need the audiomixer
				if (this.m_AudioMixer == null)
                {
					this.Logs("VRG_Audio needs an AudioMixer", ENUM_Verbose.ERROR);
                }
                else
				{
					// the volume from each group from the audiomixer
					float fVolume;

					// get the audiomixers from the database missions
					this.m_ENUM_AudioMixer = System.Enum.GetNames(typeof(ENUM_AudioMixer));

					// cycle thourgh the enum
					for (int i = 0; i < this.m_ENUM_AudioMixer.Length; i++)
					{
						// get the float volume
						this.m_AudioMixer.GetFloat(this.m_ENUM_AudioMixer[i], out fVolume);

						// save it as Max volume
						this.m_AudioVolumes.Add(fVolume);
					}
				}
			}
			else
			{
				// I am not worthy
				Destroy(this.gameObject);
			}
		}

		protected override IEnumerator Do()
		{
			this.m_IsReady = true;

			yield return null;
		}

        private void OnDisable()
        {
			this.m_IsReady = false;
		}














		public static IEnumerator IsValid()
		{
			yield return VRG_Audio.IsValid(true);
		}
		/// <summary>
		/// Ask for this ienumerator if the VRG_Audio is ready
		/// </summary>
		/// <returns>Null when the VRG_Remote is ready to be queried</returns>
		public static IEnumerator IsValid(bool valueLocal)
		{
			// Let's assume everything is configured properly
			bool bContinue = true;

			// count 1 second to find the VRG_Audio
			int i = 0;

			if (VRG_Audio.Instance == null)
			{
				if (GameObject.FindObjectOfType<VRG_Audio>() == null)
				{
					bContinue = false;
				}
			}

			// If after 30 frames you can't get the VRG_Audio object it's probable not configured
			while (VRG_Audio.Instance == null && bContinue)
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
				while (VRG_Audio.isReady == false)
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
						"Please be sure a VRG_Audio prefab is added to the scene",
						"VRG_Audio->IsValid()",
						ENUM_Verbose.WARNING,
						"Static Method"
					);
				}
			}

			// go to next frame
			yield return null;
		}



		private void Start()
        {
			Mute();
		}



		/// <summary>
		/// Mute a group from the <a href="https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Audio.AudioMixer.html">AudioMixer</a> and save it into the game session
		/// </summary>
		/// <param name="audioLocal">The <a href="https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Audio.AudioMixer.html">AudioMixer</a> group as <a href="#VrGamesDev.Missions.ENUM_AudioMixer">ENUM_AudioMixer</a> or as the int place in the enum</param>
		/// <param name="valueLocal"><strong><em>(Optional)</em></strong> The mute status, by default is true</param>
		public static void Mute(ENUM_AudioMixer audioLocal, bool valueLocal) => VRG_Audio.Mute((int)audioLocal, valueLocal);
		public static void Mute(ENUM_AudioMixer audioLocal) => VRG_Audio.Mute((int)audioLocal, true);
		public static void Mute(int audioLocal) => VRG_Audio.Mute(audioLocal, true);
		public static void Mute(int audioLocal, bool valueLocal)
		{
			// get the volume defined
			float fVolume = Instance.m_AudioVolumes[audioLocal];

			// unless it is muted
			if (valueLocal)
            {
				// muted volume
				fVolume = Instance.m_VolumeMute;
			}

			// set it to the volume
			Instance.m_AudioMixer.SetFloat(Instance.m_ENUM_AudioMixer[audioLocal], fVolume);

			// ... and save it to the session
			VRG_Session.SetBool("Mute", Instance.m_ENUM_AudioMixer[audioLocal], valueLocal);
		}
		public static void Mute()
		{
			// cycle the enum audio mixer
			for (int i = 0; i < Instance.m_ENUM_AudioMixer.Length; i++)
			{
				// get the mute from the 
				bool bMute = VRG_Session.GetBool("Mute", Instance.m_ENUM_AudioMixer[i], false);

				// ... and mute the session save data
				VRG_Audio.Mute(i, bMute);
			}
		}

	}
}