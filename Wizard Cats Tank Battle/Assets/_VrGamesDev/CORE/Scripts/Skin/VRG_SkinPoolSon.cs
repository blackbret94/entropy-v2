using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_Campaign
	/// </summary>
	public class VRG_SkinPoolSon : VRG_Base
	{
		///#IGNORE
		protected override IEnumerator Do()
		{
			bool bContinue = true;
			if (this.transform.parent != null)
            {
				if (this.transform.parent.GetComponent<VRG_SkinPool>() != null)
                {
					bContinue = false;
				}
            }

			if (bContinue)
            {
				VRG_SkinPool[] aSkins = Object.FindObjectsOfType<VRG_SkinPool>();

				if (aSkins.Length > 0)
				{
					this.transform.SetParent(aSkins[0].transform);

					VRG_SkinPool.UpdateChildList();
				}
			}

			yield return null;
		}
	}
}