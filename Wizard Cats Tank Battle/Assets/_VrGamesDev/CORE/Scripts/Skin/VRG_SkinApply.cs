using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	public class VRG_SkinApply : VRG_Base
	{
		/// <summary>
		/// The data to query from the m_SessionObject
		/// </summary>
		[Tooltip("The data to query from the m_SessionObject")]
		[SerializeField] private string m_SessionData = "Default";
		public string sessionData { get { return this.m_SessionData; }  set { this.m_SessionData = value; }  }

		/// <summary>
		/// If the loaded value is true this GameObject will trigger
		/// </summary>
		[Tooltip("If the loaded value is true this GameObject will trigger")]
		[SerializeField] protected GameObject[] m_WhenApply = null;

		/// <summary>
		/// All the skin data colors
		/// </summary>
		[Tooltip("All the skin data colors")]
		[SerializeField] private VRG_Skin m_Skin = null;



		public VRG_SkinApply()
        {
			this.m_SelfTurnOff = true;
        }


		/// <summary>
		/// <strong><em>Do it's thing: </em></strong>Load from VRG_Session a value
		/// </summary>
		protected override IEnumerator Do()
		{
			// Let's assume everything is configured properly
			yield return VRG_SkinPool.IsValid();
			// is it?
			if (VRG_SkinPool.Instance != null && VRG_Session.GetString("Skin", "Current") == this.m_SessionData)
			{
				string[] sColors = VRG_Session.GetString("Skin", this.m_SessionData).Split('|');

				if (sColors.Length == VRG_SkinPool.skinSessionData)
				{
					string[] sColor;

					int iCont = 0;

					sColor = sColors[++iCont].Split(',');
					this.m_Skin.fontColor = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.fontColorTitle = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.fontColorForeground = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.fontColorForegroundTitle = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));

					sColor = sColors[++iCont].Split(',');
					this.m_Skin.backgroundColor = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.foregroundColor = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.thirdColor = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));

					sColor = sColors[++iCont].Split(',');
					this.m_Skin.iconBackground = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.iconColor = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.iconText = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));

					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonText = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonNormal = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonHighlighted = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonPressed = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonSelected = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
					sColor = sColors[++iCont].Split(',');
					this.m_Skin.buttonDisabled = new Color(float.Parse(sColor[0]), float.Parse(sColor[1]), float.Parse(sColor[2]), float.Parse(sColor[3]));
				}

				VRG_SkinPool.Set(this.m_Skin);


				foreach (GameObject child in this.m_WhenApply)
				{
					if (child != null)
					{
						// activate it
						child.SetActive(true);
					}
					else
					{
						this.Logs(this.name + " | There is a null element in the Apply array");
					}
				}
			}

			yield return null;
		}

	}
}