using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace VrGamesDev
{
    /// <summary>
    /// You need to add this prefab to any scene that will load the core classes
    /// and singletons like <a href="#VrGamesDev.Missions.VRG_Campaign">VRG_Campaign</a> for all the game settings, If the core was
    /// already loaded, this class ignores the loading.
    /// </summary>
    public class VRG_Managers : VRG_Base
    {
        // The main package class
        //[SerializeField]
        private string m_GameManager = "VRG_Campaign";

        /// <summary>
        /// The name of the core Scene to load the singletons and managers
        /// </summary>
        [Tooltip("The name of the core Scene to load the singletons and managers")]
        [SerializeField] [SceneName] private string m_Scene = "VRG_Managers"; //"[RELOAD SCENE]";


        // Use this for initialization
        // para decirle al engine que es una funcion nueva
        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != this.m_Scene)
            {
                // just do it if a manager is defined
                if (this.m_GameManager.Trim() != "")
                {
                    // call scene to know when it is loaded
                    SceneManager.sceneLoaded += OnSceneLoaded;

                    // by default it's not
                    bool bLoadTheCore = false;

                    // try to find the main GameObject to
                    if (GameObject.Find(this.m_GameManager) == null && this.m_Scene != "[RELOAD SCENE]")
                    {
                        bLoadTheCore = true;
                    }

                    // load or destroy this object 
                    if (bLoadTheCore)
                    {
                        SceneManager.LoadScene(this.m_Scene, LoadSceneMode.Additive);
                    }
                    else
                    {
                        if (m_Scene == "[RELOAD SCENE]")
                        {
                            this.Logs("LoadCore need a Scene name, please edit the GameObject's Scene property, [RELOAD SCENE] is not a valid scene", ENUM_Verbose.ERROR);
                        }

                        Destroy(this.gameObject);
                    }
                }
            }
            else
            {
                this.Logs("THE core Scene is the same as the current scene, Infinity Loading attempt stopped", ENUM_Verbose.ERROR);
            }
        }

        // Destroy self on call
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // just in case it is not the core
            if (scene.name == this.m_Scene && this != null)
            {
                Destroy(this.gameObject);
            }
        }

        protected override IEnumerator Do()
        {
            // go to next frame
            yield return null;
        }

    }
}