using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooldhands;

namespace Cooldhands.UICursor.Example
{
	/// <summary>
	/// You can use other input system 
	/// </summary>
	public class UICursorCustomInputSystem : UICursorStandaloneInputModule
	{
		/// <summary>
		/// Gets the horizontal axis to move cursor
		/// </summary>
		/// <returns>The horizontal axis.</returns>
		protected override float GetHorizontalAxis ()
		{
			return Input.GetAxis(base.horizontalAxis);
		}

		/// <summary>
		/// Gets the vertical axis to move cursor
		/// </summary>
		/// <returns>The horizontal axis.</returns>
		protected override float GetVerticalAxis ()
		{
			return Input.GetAxis (base.verticalAxis);
		}

		/// <summary>
		/// Gets the name of the axis for scroll
		/// </summary>
		/// <returns>The scroll axis.</returns>
		protected override float GetScrollAxis ()
		{
			return Input.GetAxisRaw (base.scrollAxis);
		}

		/// <summary>
		/// Gets the button down for click
		/// </summary>
		/// <returns>True or false.</returns>
		protected override bool GetClickButtonDown ()
		{
			return Input.GetButtonDown (base.clickButton) || Input.GetButtonDown (base.submitButton);
		}

		/// <summary>
		/// Gets the button up for click
		/// </summary>
		/// <returns>True or false.</returns>
		protected override bool GetClickButtonUp ()
		{
			return Input.GetButtonUp (base.clickButton) || Input.GetButtonUp (base.submitButton);
		}
	}
}