using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Load a scene using the <a href="#VrGamesDev.Missions.VRG_Fader">VRG_Fader</a>, using a <a href="#VrGamesDev.Missions.VRG_Campaign">VRG_Campaign</a> scenes configuration
    /// The <a href="#VrGamesDev.Missions.ENUM_GameScenes">type of ENUM_GameScenes</a> if not ENUM_GameScenes.CUSTOM, the m_Scene data is ignored.
    /// </summary>
    public class VRG_GoToScene : VRG_Base
    {
        /// <summary>
        /// The name of the Scene to load
        /// </summary>
        [Tooltip("The name of the Scene to load")]
        [SerializeField] [SceneName] private string m_Scene = "[RELOAD SCENE]";



        public VRG_GoToScene()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = false;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Send to a new scene using <a href="#VrGamesDev.Missions.VRG_Fader">VRG_Fader</a>
        /// </summary>
        protected override IEnumerator Do()
        {
            if (this.m_Scene != string.Empty)
            {
                if (VRG_FaderScene.Instance == null)
                {
                    this.Logs("VRG_GoToScene needs a VRG_FaderScene prefab to smooth load the scene", ENUM_Verbose.WARNING);
                }

                // load the fader scene
                VRG_FaderScene.Load(this.m_Scene);
            }
            else
            {                
                this.Logs("You are trying to load an empty scene", ENUM_Verbose.ERROR);                
            }

            // regreso, equivale a un void
            yield return null;
        }
    }
}