using System.Collections;

using UnityEngine.SceneManagement;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// Inform the VRG_DDuA <a href="#VrGamesDev.DDuA.VRG_DDuA">VRG_DDuA</a> main object
    /// a new scene was loaded and is an addressable
    /// </summary>
    public class VRG_Scene : VRG_Base
    {
        private void Awake()
        {
            if (UnityEngine.Object.FindObjectsOfType<VRG_DDuA>().Length == 0)
            {
                VRG_Session.SetString("VRG_Loader", "VRG_Scene", SceneManager.GetActiveScene().name);

                bool bForce = true;

#if UNITY_EDITOR_OSX
                if (SceneManager.sceneCountInBuildSettings < 2)
                {
                    bForce = false;
                }
#endif

#if UNITY_EDITOR_WIN
                if (SceneManager.sceneCountInBuildSettings < 2)
                {
                    bForce = false;
                }
#endif

                if (bForce)
                {
                    SceneManager.LoadScene(0);
                }
                else
                {
                    // log and inform i couldn't release the handler
                    this.Logs
                    (
                        "<color=blue><i>" + SceneManager.GetActiveScene().name + "</i></color> | SceneManager has no scenes loaded, please add your VRG_Loader scene to the build settings",
                        "VRG_Scene->Awake()",
                        ENUM_Verbose.ERROR
                    );
                }
            }

            /*
            else
            {
                this.Logs("VRG_DDuA.SceneInit();", "VRG_Scene->Awake()");
            }
            */

            UnityEngine.Object.Destroy(this.gameObject);
        }


        ///#IGNORE
        protected override IEnumerator Do() { yield return null; }
    }
}