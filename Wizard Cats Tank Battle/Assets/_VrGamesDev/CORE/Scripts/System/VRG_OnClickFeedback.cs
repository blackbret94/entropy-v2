using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// On click, it spawns particles as feedback to the player and plays a sound
    /// </summary>
    public class VRG_OnClickFeedback : VRG_Base
    {
        /// <summary>
        /// The camera to spawn the feedback
        /// </summary>
        [Tooltip("The camera to spawn the feedback")]
        [SerializeField] private Camera m_Camera;

        /// <summary>
        /// The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx
        /// </summary>
        [Tooltip("The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx")]
        [SerializeField] private GameObject m_SFx = null;

        /// <summary>
        /// The Ortographic particles prefab to spawn
        /// </summary>
        [Tooltip("The Ortographic particles prefab to spawn")]
        [SerializeField] private GameObject m_ParticlesOrtographic = null;

        /// <summary>
        /// The Perspective particles prefab to spawn
        /// </summary>
        [Tooltip("The Perspective particles prefab to spawn")]
        [SerializeField] private GameObject m_ParticlesPerspective = null;


        private void Awake()
        {
            this.m_Camera = this.FindMy(this.m_Camera);
        }

        protected override IEnumerator Do()
        {
            // go to next frame
            yield return null;
        }

        private void Update()
        {
            // check for the click
            if (Input.GetMouseButtonDown(0))
            {
                // get the position of the click
                this.transform.position = this.m_Camera.ScreenToWorldPoint(new Vector3
                (
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    this.m_Camera.nearClipPlane
                ));

                // what prefab will be spawned ... the perspective
                GameObject go_Particles = this.m_ParticlesPerspective;

                // ... or the orthographic
                if (this.m_Camera.orthographic)
                {
                    // it depends on the camera settings
                    go_Particles = this.m_ParticlesOrtographic;
                }

                // if a particles was declared
                if (go_Particles != null)
                {
                    // ...vcreate the particles at the position clicked
                    Object.Instantiate(go_Particles, this.transform);
                }

                // if there is a sound
                if (this.m_SFx != null)
                {
                    // ... play it
                    Object.Instantiate(this.m_SFx, this.transform);
                }
            }
        }
    }
}