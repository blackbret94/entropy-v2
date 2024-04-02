using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	[System.Serializable]
	public class VRG_Skin
	{
		[Header("From: Font")]
		/// <summary>
		/// The font used, you can change the font of every text component with this master for the background components
		/// </summary>
		[Tooltip("The font used, you can change the font of every text component with this master")]
		[SerializeField] public Font font = null;

		/// <summary>
		/// The font color used by default
		/// </summary>
		[Tooltip("The font color used by default")]
		[SerializeField] public Color fontColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		/// <summary>
		/// The font color used by default for the titles
		/// </summary>
		[Tooltip("The font color used by default")]
		[SerializeField] public Color fontColorTitle = new Color((226.0f / 255.0f), (198.0f / 255.0f), (86.0f / 255.0f), 1.0f);

		/// <summary>
		/// The font color used by default in the foregrownd layer
		/// </summary>
		[Tooltip("The font color used by default in the foregrownd layer")]
		[SerializeField] public Color fontColorForeground = new Color(0.20f, 0.20f, 0.20f, 1.0f);

		/// <summary>
		/// The font color used by default in the foregrownd layer for the titles
		/// </summary>
		[Tooltip("The font color used by default in the foregrownd layer")]
		[SerializeField] public Color fontColorForegroundTitle = new Color(0.192f, 0.282f, 0.537f, 0.5f);





		[Header("From: Back/Fore grounds")]
		/// <summary>
		/// The background color used by default
		/// </summary>
		[Tooltip("The background color used by default")]
		[SerializeField] public Color backgroundColor = new Color(0.192f, 0.282f, 0.537f, 0.5f);

		[Tooltip("The background color used by default")]
		/// <summary>
		/// The background color used by default
		/// </summary>
		[SerializeField] public Color foregroundColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);

		/// <summary>
		/// The color of the background of the camera it is a renderer
		/// </summary>
		[Tooltip("The color of the background of the camera it is a renderer")]
		[SerializeField] public Color thirdColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); //new Color(0.251f, 0.608f, 1.0f, 1.0f);





		[Header("From: Icon and Label")]
		[Tooltip("In the VRG_Button - Icon and Label prefab, the icon background color")]
		/// <summary>
		/// In the VRG_Button - Icon and Label prefab, the icon background color
		/// </summary>
		[SerializeField] public Color iconBackground = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		[Tooltip("In the VRG_Button - Icon and Label prefab, the icon color itself")]
		/// <summary>
		/// In the VRG_Button - Icon and Label prefab, the icon color itself
		/// </summary>
		[SerializeField] public Color iconColor = new Color(0.192f, 0.282f, 0.537f, 1.0f);

		[Tooltip("In the VRG_Button - Icon and Label prefab, the font color")]
		/// <summary>
		/// In the VRG_Button - Icon and Label prefab, the font color
		/// </summary>
		[SerializeField] public Color iconText = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		[Header("From: Button Basic")]
		[Tooltip("The text of the Unity UI - button element")]
		/// <summary>
		/// The text of the <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Button.html">Unity UI - button element</a>
		/// </summary>
		[SerializeField] public Color buttonText = new Color(0.20f, 0.20f, 0.20f, 1.0f);

		[Tooltip("The button Normal color from ColorBlock")]
		/// <summary>
		/// The button Normal color from <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.ColorBlock.html">ColorBlock</a>
		/// </summary>
		[SerializeField] public Color buttonNormal = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		[Tooltip("The button Highlighted color from ColorBlock")]
		/// <summary>
		/// The button Highlighted color from <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.ColorBlock.html">ColorBlock</a>
		/// </summary>
		[SerializeField] public Color buttonHighlighted = new Color(0.80f, 0.80f, 0.80f, 1.0f);

		[Tooltip("The button Pressed color from ColorBlock")]
		/// <summary>
		/// The button Pressed color from <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.ColorBlock.html">ColorBlock</a>
		/// </summary>
		[SerializeField] public Color buttonPressed = new Color(0.7f, 0.7f, 0.7f, 1.0f);

		[Tooltip("The button Selected color from ColorBlock")]
		/// <summary>
		/// The button Selected color from <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.ColorBlock.html">ColorBlock</a>
		/// </summary>
		[SerializeField] public Color buttonSelected = new Color(0.80f, 0.80f, 0.80f, 1.0f);

		[Tooltip("The button Disabled color from ColorBlock")]
		/// <summary>
		/// The button Disabled color from <a href="https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.ColorBlock.html">ColorBlock</a>
		/// </summary>
		[SerializeField] public Color buttonDisabled = new Color(0.65f, 0.65f, 0.65f, 0.50f);


		/// <summary>
        /// Assign this data to the main game
        /// </summary>
        /// <param name="valueLocal"></param>
		public void Set(VRG_Skin valueLocal)
        {
			this.font = valueLocal.font;
			this.fontColor = valueLocal.fontColor;
			this.fontColorTitle = valueLocal.fontColorTitle;
			this.fontColorForeground = valueLocal.fontColorForeground;
			this.fontColorForegroundTitle = valueLocal.fontColorForegroundTitle;



			this.backgroundColor = valueLocal.backgroundColor;
			this.foregroundColor = valueLocal.foregroundColor;
			this.thirdColor = valueLocal.thirdColor;



			this.iconBackground = valueLocal.iconBackground;
			this.iconColor = valueLocal.iconColor;
			this.iconText = valueLocal.iconText;
			this.buttonText = valueLocal.buttonText;



			this.buttonNormal = valueLocal.buttonNormal;
			this.buttonHighlighted = valueLocal.buttonHighlighted;
			this.buttonPressed = valueLocal.buttonPressed;
			this.buttonSelected = valueLocal.buttonSelected;
			this.buttonDisabled = valueLocal.buttonDisabled;
		}

	}
}