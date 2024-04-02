using System;
using System.Collections;

using UnityEngine;

using UnityEngine.SceneManagement;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class simply allows the user to load new scene using Addressables, including itself
    /// </summary>
    public class VRG_GoToScene_Addressable : VRG_Base
    {
        /// <summary>
        /// The name of the new scene to load, [RELOAD SCENE] = reload Itself
        /// </summary>
        [Tooltip("The name of the new scene to load from the Addressable Group, [RELOAD SCENE] = reload Itself")]
        [SerializeField]
        [AddressableScene]
        private string m_Scene = "[RELOAD SCENE]";


        public VRG_GoToScene_Addressable()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = false;
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            string sScene = this.m_Scene.Split(new string[] { " - " }, StringSplitOptions.None)[0];

            if (sScene == "[RELOAD SCENE]")
            {
                sScene = SceneManager.GetActiveScene().name;
            }

            // set the custom Slide by labels
            VRG_SlideShow.SetScene(sScene);

            // inform the VRG_DDuA of the new scene
            VRG_DDuA.LoadScene(sScene);

            // return, it is like a void and wait a frame
            yield return null;
        }
    }
}