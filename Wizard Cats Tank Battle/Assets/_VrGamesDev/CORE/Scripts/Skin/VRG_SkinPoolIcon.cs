using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;



namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme into the VRG_SkinPool
	/// </summary>
	/// 
	public class VRG_SkinPoolIcon : VRG_Base
	{
		[SerializeField]
		private Vector3 m_UpdateBoxStartingPosition = new Vector3();

		/// <summary>
		/// The Update component, it appears 2 seconds and dissapear
		/// </summary>
		[Tooltip("The Update component, it appears 2 seconds and dissapear")]
		[SerializeField]
		private GameObject m_UpdateBox = null;

		/// <summary>
		/// The text component, it display the current skin uploaded
		/// </summary>
		[Tooltip("The text component, it display the current skin uploaded")]
		[SerializeField]
		private Text m_UpdateText = null;

		[SerializeField]
		private Canvas m_Canvas = null;

		[SerializeField]
		private RectTransform m_RectTransform = null;

		private void Awake()
        {
			this.m_Canvas = this.FindMy(this.m_Canvas, false);

			if (this.m_Canvas == null)
            {
				this.Logs("<color=red><b>DESTROYED: </b></color> this.m_Canvas = " + this.transform.parent.name, ENUM_Verbose.ERROR);

				Object.Destroy(this.transform.parent.gameObject);
            }

			this.m_RectTransform = this.m_Canvas.gameObject.GetComponent<RectTransform>();

			this.m_UpdateBoxStartingPosition = this.m_UpdateBox.transform.localPosition;
		}

		protected override IEnumerator Do()
		{
			this.m_UpdateBox.transform.localPosition = this.m_UpdateBoxStartingPosition;

			Vector3 v3_NewPosition = new Vector3
			(
				this.m_UpdateBoxStartingPosition.x,
				this.m_UpdateBoxStartingPosition.y,
				this.m_UpdateBoxStartingPosition.z
			);

			Vector3[] canvasCorners = new Vector3[4];
			Vector3[] labelCorners = new Vector3[4];

			this.m_RectTransform.GetWorldCorners(canvasCorners);

			this.m_UpdateBox.GetComponent<RectTransform>().GetWorldCorners(labelCorners);

			if (this.m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				if ((labelCorners[0].x - canvasCorners[0].x) < 0)
				{
					v3_NewPosition.x = (labelCorners[0].x - canvasCorners[0].x) * (-1);
				}
				else if ((labelCorners[3].x - canvasCorners[3].x) > 0)
				{
					v3_NewPosition.x = (labelCorners[3].x - canvasCorners[3].x) * (-1);
				}

				if ((labelCorners[1].y - canvasCorners[1].y) > 0)
				{
					v3_NewPosition.y = (labelCorners[1].y - canvasCorners[1].y) * (-1);
				}
			}
			else
			{
				if ((labelCorners[0].x - canvasCorners[0].x) < 0)
				{
					v3_NewPosition.x = this.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
				}
				else if ((labelCorners[3].x - canvasCorners[3].x) > 0)
				{
					v3_NewPosition.x = this.transform.parent.GetComponent<RectTransform>().sizeDelta.x * (-1);
				}

				if ((labelCorners[1].y - canvasCorners[1].y) > 0)
				{
					v3_NewPosition.y = this.transform.parent.GetComponent<RectTransform>().sizeDelta.y * (-1);
				}
			}

			this.m_UpdateBox.transform.localPosition = v3_NewPosition;

			// Let's assume everything is configured properly
			yield return VRG_SkinPool.IsValid();

			// is it?
			if (VRG_SkinPool.Instance != null)
			{
				// get the name of the next skin
				string sNextSkin = VRG_SkinPool.GetNextName();
				if (sNextSkin != string.Empty)
				{
					// ... then play it
//					VRG_SkinPool.Instance.Play();
				}
				else
				{
					// or there arent any skin avaliable
					sNextSkin = "N/A";
				}

				// ... then play it
				VRG_SkinPool.Instance.Play();

				// if there is a way to inform the player
				if (this.m_UpdateBox != null && this.m_UpdateText != null)
				{
					// turn it off
					this.m_UpdateBox.SetActive(false);

					// wait
					yield return null;

					// and display the updated name
					this.m_UpdateBox.SetActive(true);
					this.m_UpdateText.text = "Skin: " + sNextSkin;
				}
			}

			yield return null;
		}
	}
}