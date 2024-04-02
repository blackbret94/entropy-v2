using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Size a gameobject at the farClipPlane of the camera render distance,
    /// and scale it to aspect ratio (portrait / Landscape)
    /// </summary>
    public class VRG_CameraBackground : VRG_Base
    {
        /// <summary>
        /// The camera to affect, by default gets the main camera
        /// </summary>
        [Tooltip("The camera to affect, by default gets the main camera")]
        [SerializeField] private Camera m_Camera = null;

        /// <summary>
        /// Background color, it is the tint value
        /// </summary>
        [Tooltip("Background color, it is the tint value")]
        // [SerializeField] private Color m_Color = new Color(0.251f, 0.608f, 1.0f, 1.0f);
        [SerializeField]
        private Color m_Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color color { get { return this.m_Color; } set { this.m_Color = value; this.Play(); } }



        /// <summary>
        /// The proportion to scale the background on 10:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 10:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Orto_Portrait = new Vector2(14.4f, 24.0f);

        /// <summary>
        /// The proportion to scale the background on 10:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 10:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Orto_Landscape = new Vector2(50.0f, 24.0f);

        /// <summary>
        /// The proportion to scale the background on 9:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 9:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Orto_9_16 = new Vector2(13.5f, 24.0f);

        /// <summary>
        /// The proportion to scale the background on 9:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 9:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Orto_16_9 = new Vector2(42.6f, 24f);





        /// <summary>
        /// The proportion to scale the background on 10:16 perspective camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 10:16 perspective camera ")]
        [SerializeField] private Vector2 m_Pers_Portrait = new Vector2(0.72f, 1.19f);

        /// <summary>
        /// The proportion to scale the background on 9:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 9:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Pers_Landscape = new Vector2(2.37f, 1.16f);

        /// <summary>
        /// The proportion to scale the background on 9:16 perspective camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 9:16 perspective camera ")]
        [SerializeField] private Vector2 m_Pers_9_16 = new Vector2(0.65f, 1.19f);

        /// <summary>
        /// The proportion to scale the background on 10:16 ortogonal camera 
        /// </summary>
        [Tooltip("The proportion to scale the background on 10:16 ortogonal camera ")]
        [SerializeField] private Vector2 m_Pers_16_9 = new Vector2(2.05f, 1.16f);

        

        [Tooltip("The math number of 9:16")]
        [SerializeField]
        private float m_9_16_math = 9f / 16f;//0.5625f;

        [Tooltip("The math number of 9:16")]
        [SerializeField]
        private float m_16_9_math = 16f / 9f;




        private void Awake()
        {
            // make sure you have a camera ready, if not, kill the object
            this.m_Camera = this.FindMy(this.m_Camera);
        }

        /// <summary>
        /// Resize the background, tint it and position it
        /// </summary>
        protected override IEnumerator Do()
        {
            // the new scale vector
            Vector3 v3_LocalScale;

            // the renderer to tint the background
            Renderer rendererBackgroud = this.GetComponent<Renderer>();

            // if it has a renderer
            if (rendererBackgroud != null)
            {
                // tint it
                rendererBackgroud.material.color = this.m_Color;
            }

            // there are 2 flavors, orthographic and not
            if (this.m_Camera.orthographic)
            {
                // if the aspect is portrait
                if (this.m_Camera.aspect < 1)
                {
                    // 9:16 is this.m_9_16_math, math son
                    if (this.m_Camera.aspect == this.m_9_16_math)
                    {
                        v3_LocalScale = new Vector3(this.m_Orto_9_16.x, this.m_Orto_9_16.y, 1);
                    }

                    // if not, by default set it to the 10:16 which is bigger
                    else
                    {
                        v3_LocalScale = new Vector3(this.m_Orto_Portrait.x, this.m_Orto_Portrait.y, 1);
                    }
                }

                // landscape
                else
                {
                    // 9:16 is this.m_9_16_math, math son
                    if (this.m_Camera.aspect == this.m_16_9_math)
                    {
                        v3_LocalScale = new Vector3(this.m_Orto_16_9.x, this.m_Orto_16_9.y, 1);
                    }

                    // if not, by default set it to the 10:16 which is bigger
                    else
                    {
                        v3_LocalScale = new Vector3(this.m_Orto_Landscape.x, this.m_Orto_Landscape.y, 1);
                    }
                }
            }

            // the not, is usually perspective
            else
            {
                // if the aspect is portrait
                if (this.m_Camera.aspect < 1)
                {
                    // 9:16 is this.m_9_16_math, math son
                    if (this.m_Camera.aspect == this.m_9_16_math)
                    {
                        v3_LocalScale = new Vector3(this.m_Pers_9_16.x * this.m_Camera.farClipPlane, m_Pers_9_16.y * this.m_Camera.farClipPlane, 1);
                    }

                    // if not, by default set it to the 10:16 which is bigger
                    else
                    {
                        v3_LocalScale = new Vector3(this.m_Pers_Portrait.x * this.m_Camera.farClipPlane, m_Pers_Portrait.y * this.m_Camera.farClipPlane, 1);
                    }
                }


                // landscape
                else
                {
                    // 9:16 is this.m_9_16_math, math son
                    if (this.m_Camera.aspect == this.m_16_9_math)
                    {
                        v3_LocalScale = new Vector3(this.m_Pers_16_9.x * this.m_Camera.farClipPlane, m_Pers_16_9.y * this.m_Camera.farClipPlane, 1);
                    }

                    // if not, by default set it to the 10:16 which is bigger
                    else
                    {
                        v3_LocalScale = new Vector3(this.m_Pers_Landscape.x * this.m_Camera.farClipPlane, m_Pers_Landscape.y * this.m_Camera.farClipPlane, 1);
                    }
                }
            }

            // scale acoordling to aspect ratio
            this.transform.localScale = v3_LocalScale;

            // and go to the very end the clip plane
            this.transform.localPosition = new Vector3(0, 0, (this.m_Camera.farClipPlane * 0.998f));

            // done, return control
            yield return null;
        }

    }
}