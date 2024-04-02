using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	public class VRG_SkinLoad : VRG_Base
	{
		/// <summary>
		/// The skin name to load
		/// </summary>
		[Tooltip("The skin name to load")]
		[SerializeField]
		protected string m_Value = string.Empty;




		public VRG_SkinLoad()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = true;
        }


		/// <summary>
		/// <strong><em>Do it's thing: </em></strong> Load the Skin
		/// </summary>
		protected override IEnumerator Do()
		{
			// Let's assume everything is configured properly
			yield return VRG_SkinPool.IsValid();

			// is it?
			if (VRG_SkinPool.Instance != null)
			{
				// load the skin value
				VRG_SkinPool.Instance.Play(this.m_Value);
			}
		
			yield return null;
		}

	}
}