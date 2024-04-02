using System.Collections;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// This class checks if the VRG_Audio singleton exists, if not, it disable itself
	/// </summary>
	public class VRG_AudioExists : VRG_Base
	{
		public VRG_AudioExists()
		{
			this.m_PlayOnEnable = true;
			this.m_NextFrame = false;
			this.m_SelfTurnOff = false;
		}

		protected override IEnumerator Do()
		{
			// Let's assume everything is configured properly
			yield return VRG_Audio.IsValid(false);

			// is it?
			if (VRG_Audio.Instance == null)
			{
				this.Logs
				(
					"You need a VRG_Audio singleton to modify it",
					"VRG_SkinPoolExists->Do()",
					ENUM_Verbose.WARNING
				);

				this.gameObject.SetActive(false);
			}


			yield return null;
		}
	}
}