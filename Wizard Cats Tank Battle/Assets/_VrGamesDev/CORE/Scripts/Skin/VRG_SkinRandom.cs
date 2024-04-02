using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
	/// <summary>
	/// Load the current skin theme to the VRG_SkinPool
	/// </summary>
	public class VRG_SkinRandom : VRG_Base
    {
        [Header("FROM: Randomize")]
        /// <summary>
        /// The button colors data
        /// </summary>
        [Tooltip("The button colors data")]
        [SerializeField] private bool m_Button = false;

        /// <summary>
        /// The Icon colors data
        /// </summary>
        [Tooltip("The Icon colors data")]
        [SerializeField] private bool m_Icon = false;

        /// <summary>
        /// The Font colors data and type
        /// </summary>
        [Tooltip("The Font colors data and type")]
        [SerializeField] private bool m_Font = false;

        /// <summary>
        /// The background and foregroubd colors data
        /// </summary>
        [Tooltip("The background and foregroubd colors data")]
        [SerializeField] private bool m_Background = false;

        /// <summary>
        /// The background and foregroubd colors data
        /// </summary>
        [Tooltip("The background and foregroubd colors data")]
        [SerializeField] private bool m_Foreground = false;



        // Enumerator proxy, it is activated OnEnable
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_SkinPool.IsValid();

            // is it?
            if (VRG_SkinPool.Instance != null)
            {
                // get all the VRG_UI
                VRG_UI[] vrg_UI = Object.FindObjectsOfType<VRG_UI>();

                // cycle the UI objects
                // randomize the colors
                foreach (VRG_UI child in vrg_UI)
                {
                    if (this.m_Font)
                    {
                        VRG_SkinPool.skin.fontColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.fontColorTitle = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.fontColorForeground = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.fontColorForegroundTitle = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.iconText = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonText = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }

                    // background and foreground
                    if (this.m_Background)
                    {
                        VRG_SkinPool.skin.backgroundColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.thirdColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }

                    // background and foreground
                    if (this.m_Foreground)
                    {
                        VRG_SkinPool.skin.foregroundColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }

                    // icon and its stuff
                    if (this.m_Icon)
                    {
                        VRG_SkinPool.skin.iconBackground = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.iconColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }

                    // the main button
                    if (this.m_Button)
                    {
                        VRG_SkinPool.skin.buttonText = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonNormal = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonHighlighted = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonPressed = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonSelected = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        VRG_SkinPool.skin.buttonDisabled = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }

                    // play the changes
                    child.Play();
                }
            }

            // regreso, equivale a un void
            yield return null;
        }
    }
}