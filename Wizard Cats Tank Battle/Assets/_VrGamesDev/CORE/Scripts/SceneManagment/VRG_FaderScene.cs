using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Use a <a href="#VrGamesDev.VRG_Fader">VRG_Fader</a> to sync with the scene loading and unloading, for easier and smooth transition
    /// </summary>
    public class VRG_FaderScene : VRG_Base
    {
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        [Tooltip("The available status, ask for this property to know if it is available")]
        [SerializeField] protected bool m_IsReady = false;
        public static bool isReady
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.m_IsReady;
                }
                else
                {
                    return false;
                }
            }
        }





        /// <summary>
        /// Luke, I'm your <a href="index.html#VrGamesDev.VRG_Fader">Fader</a>
        /// </summary>
        [Tooltip("Luke, I'm your Fader")]
        [SerializeField] private VRG_Fader m_VRG_Fader = null;

        /// <summary>
        /// The name of the Scene to unload
        /// </summary>
        [Tooltip("The name of the Scene to unload")]
        [SerializeField] [SceneName] private string m_Scene = "[RELOAD SCENE]";

        /// <summary>
        /// The scene currently loaded
        /// </summary>
        public static string current { get { return SceneManager.GetActiveScene().name; } }

        /// <summary>
        /// Singleton pattern, Instance is the unique variable in the scene from this class
        /// </summary>
        public static VRG_FaderScene Instance; void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will live forever
                DontDestroyOnLoad(this);

                // if the VRG_Fader is null
                if (Instance.m_VRG_Fader == null)
                {
                    Instance.m_VRG_Fader = this.transform.GetComponentInChildren<VRG_Fader>();
                }

                // it means no VRG_FADER, it is an error
                if (Instance.m_VRG_Fader == null)
                {
                    this.Logs("You need a VRG_FADER class as a child for VRG_FaderScene", ENUM_Verbose.ERROR);

                    // destroy this object, since it is a very illegal item
                    Destroy(this.gameObject);
                }
                else
                {
                    // set the fade status
                    this.m_VRG_Fader.autoFadeOut = false;

                    // set the fade status
                    this.m_VRG_Fader.WhenFadeIn += M_VRG_Fader_WhenFadeIn;

                    // set the fade status
                    this.m_VRG_Fader.WhenFadeOut += M_VRG_Fader_WhenFadeOut;

                    // I need to know when a new scene is loaded
                    SceneManager.sceneLoaded += OnSceneLoaded;
                }


                // Now it is ready to listen to other scene
                this.m_IsReady = true;
            }
            else 
            {
                // I am not the one the prophecy said ... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            yield return null;
        }

        private void OnDestroy()
        {
            // set the fade status
            this.m_VRG_Fader.WhenFadeIn -= M_VRG_Fader_WhenFadeIn;

            // set the fade status
            this.m_VRG_Fader.WhenFadeOut -= M_VRG_Fader_WhenFadeOut;

            // release the listener to scenes
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Load a scene as a public static VRG_FaderScene.Load("Scene To load", "delay in seconds");
        /// This class is called by the script: "VRG_GoToScene.cs"
        /// </summary>
        /// <param name="valueLocal">The name of the scene to load </param>
        /// <param name="delayLocal">(Optional) The delay to start the loading, by default is 0 for an instant scene loading</param>
        public static void Load(string valueLocal)
        {
            if (string.IsNullOrEmpty(valueLocal) || valueLocal == "[RELOAD SCENE]")
            {
                valueLocal = VRG_FaderScene.current;
            }

            if (Instance != null)
            {
                // asigno la nueva escena que quiero
                Instance.m_Scene = valueLocal;

                // can't load another scene while this one is loading
                Instance.m_IsReady = false;

                // activate the box object that hold the fader
                Instance.m_VRG_Fader.Play(true);
            }
            else
            {
                SceneManager.LoadScene(valueLocal);
            }
        }

        private void M_VRG_Fader_WhenFadeIn()
        {
            // la cargo
            SceneManager.LoadSceneAsync(this.m_Scene);
        }

        // called second
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // ... activate it
            SceneManager.SetActiveScene(scene);

            // activate the box object that hold the fader
            this.m_VRG_Fader.Play(false);
        }

        private void M_VRG_Fader_WhenFadeOut()
        {
            // Now it is ready to listen to other scene
            this.m_IsReady = true;
        }

    }
}