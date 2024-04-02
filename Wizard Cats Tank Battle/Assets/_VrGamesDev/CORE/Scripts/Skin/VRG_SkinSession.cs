using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	public class VRG_SkinSession : VRG_Base
    {
		[Header("From: VRG_Session")]
		/// <summary>
		/// The data to query from the m_SessionObject
		/// </summary>
		[Tooltip("The data to query from the m_SessionObject")]
		[SerializeField] private string m_SessionData = "Custom";






		// constructor
		public VRG_SkinSession()
		{
			this.m_SelfTurnOff = true;
		}

		// Enumerator proxy, it is activated OnEnable
		protected override IEnumerator Do()
		{
			// Let's assume everything is configured properly
			yield return VRG_SkinPool.IsValid();

			// is it?
			if (VRG_SkinPool.Instance != null)
			{
				string sValue = VRG_SkinPool.skin.font != null ? VRG_SkinPool.skin.font.name : "none";
				sValue += "|" + VRG_SkinPool.skin.fontColor.r + "," + VRG_SkinPool.skin.fontColor.g + "," + VRG_SkinPool.skin.fontColor.b + "," + VRG_SkinPool.skin.fontColor.a;
				sValue += "|" + VRG_SkinPool.skin.fontColorTitle.r + "," + VRG_SkinPool.skin.fontColorTitle.g + "," + VRG_SkinPool.skin.fontColorTitle.b + "," + VRG_SkinPool.skin.fontColorTitle.a;
				sValue += "|" + VRG_SkinPool.skin.fontColorForeground.r + "," + VRG_SkinPool.skin.fontColorForeground.g + "," + VRG_SkinPool.skin.fontColorForeground.b + "," + VRG_SkinPool.skin.fontColorForeground.a;
				sValue += "|" + VRG_SkinPool.skin.fontColorForegroundTitle.r + "," + VRG_SkinPool.skin.fontColorForegroundTitle.g + "," + VRG_SkinPool.skin.fontColorForegroundTitle.b + "," + VRG_SkinPool.skin.fontColorForegroundTitle.a;



				sValue += "|" + VRG_SkinPool.skin.backgroundColor.r + "," + VRG_SkinPool.skin.backgroundColor.g + "," + VRG_SkinPool.skin.backgroundColor.b + "," + VRG_SkinPool.skin.backgroundColor.a;
				sValue += "|" + VRG_SkinPool.skin.foregroundColor.r + "," + VRG_SkinPool.skin.foregroundColor.g + "," + VRG_SkinPool.skin.foregroundColor.b + "," + VRG_SkinPool.skin.foregroundColor.a;
				sValue += "|" + VRG_SkinPool.skin.thirdColor.r + "," + VRG_SkinPool.skin.thirdColor.g + "," + VRG_SkinPool.skin.thirdColor.b + "," + VRG_SkinPool.skin.thirdColor.a;



				sValue += "|" + VRG_SkinPool.skin.iconBackground.r + "," + VRG_SkinPool.skin.iconBackground.g + "," + VRG_SkinPool.skin.iconBackground.b + "," + VRG_SkinPool.skin.iconBackground.a;
				sValue += "|" + VRG_SkinPool.skin.iconColor.r + "," + VRG_SkinPool.skin.iconColor.g + "," + VRG_SkinPool.skin.iconColor.b + "," + VRG_SkinPool.skin.iconColor.a;
				sValue += "|" + VRG_SkinPool.skin.iconText.r + "," + VRG_SkinPool.skin.iconText.g + "," + VRG_SkinPool.skin.iconText.b + "," + VRG_SkinPool.skin.iconText.a;



				sValue += "|" + VRG_SkinPool.skin.buttonText.r + "," + VRG_SkinPool.skin.buttonText.g + "," + VRG_SkinPool.skin.buttonText.b + "," + VRG_SkinPool.skin.buttonText.a;
				sValue += "|" + VRG_SkinPool.skin.buttonNormal.r + "," + VRG_SkinPool.skin.buttonNormal.g + "," + VRG_SkinPool.skin.buttonNormal.b + "," + VRG_SkinPool.skin.buttonNormal.a;
				sValue += "|" + VRG_SkinPool.skin.buttonHighlighted.r + "," + VRG_SkinPool.skin.buttonHighlighted.g + "," + VRG_SkinPool.skin.buttonHighlighted.b + "," + VRG_SkinPool.skin.buttonHighlighted.a;
				sValue += "|" + VRG_SkinPool.skin.buttonPressed.r + "," + VRG_SkinPool.skin.buttonPressed.g + "," + VRG_SkinPool.skin.buttonPressed.b + "," + VRG_SkinPool.skin.buttonPressed.a;
				sValue += "|" + VRG_SkinPool.skin.buttonSelected.r + "," + VRG_SkinPool.skin.buttonSelected.g + "," + VRG_SkinPool.skin.buttonSelected.b + "," + VRG_SkinPool.skin.buttonSelected.a;
				sValue += "|" + VRG_SkinPool.skin.buttonDisabled.r + "," + VRG_SkinPool.skin.buttonDisabled.g + "," + VRG_SkinPool.skin.buttonDisabled.b + "," + VRG_SkinPool.skin.buttonDisabled.a;

				VRG_Session.SetString("Skin", this.m_SessionData, sValue);
			}

			// regreso, equivale a un void
			yield return null;
        }




	}
}