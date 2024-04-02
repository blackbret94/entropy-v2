using System;
using System.Collections;

using UnityEngine.SceneManagement;


// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;
namespace VrGamesDev
{
    /// <summary>
    /// Unload an additive scene, just provide the name of the scene
    /// If custom you need to fill the m_Scene property, this property will
    /// be ignored if m_Type is not custom.
    /// </summary>
    public class VRG_UnloadAdditiveScene : VRG_Base
    {
        public VRG_UnloadAdditiveScene()
        {
            this.m_PlayOnEnable = true;
            this.m_SelfTurnOff = true;
            this.m_NextFrame = true;
        }

        // Enumerator proxy, it is activated OnEnable
        protected override IEnumerator Do()
        {
            if (SceneManager.sceneCount > 1)
            {
                try
                {
                    //print(SceneManager.GetActiveScene().name);
                    // unload
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
                }
                catch (Exception e)
                {
                    this.Logs(e.ToString(), "VRG_UnloadAdditiveScene->Do()", ENUM_Verbose.ERROR);
                }
            }

            // regreso, equivale a un void
            yield return null;
        }
    }
}