using System.Collections;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// This class checks if the VRG_SkinPool singleton exists, if not, it disable itself
	/// </summary>
	public class VRG_SkinPoolExists : VRG_Base
	{
		public VRG_SkinPoolExists()
		{
			this.m_PlayOnEnable = true;
			this.m_NextFrame = true;
			this.m_SelfTurnOff = false;
		}

		protected override IEnumerator Do()
		{
			// Let's assume everything is configured properly
			yield return VRG_SkinPool.IsValid(false);

			// is it?
			if (VRG_SkinPool.Instance == null || VRG_SkinPool.pool.Count <= 0)
			{
				this.gameObject.SetActive(false);
			}

			yield return null;
		}
	}
}