using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class control how many Slides go through the carrousel
    /// The movement, next and loading/instantiate of the VRG_Slide
    /// </summary>
    public class VRG_SlideShow : VRG_Base
    {
        [Header("From: Components")]
        [Tooltip("UI component, it holds all the little buttons to change the slides")]
        [SerializeField] private GameObject m_Menu = null;

        [Tooltip("The slider progress bar")]
        [SerializeField] private Slider m_Progress = null;

        [Tooltip("UI component: Box to display a rotating loading")]
        [SerializeField] private GameObject m_LoadingBox = null;

        [Range(0.0f, 2.0f)]
        [Tooltip("The delay in seconds to wait before the loading appears")]
        [SerializeField] private float m_LoadingBoxDelay = 1.0f;



        [Header("From: Manager")]
        /// <summary>
        /// Does it start at a random Slide or at the first one?
        /// </summary>
        [Tooltip("Does it start at a random Slide or at the first one?")]
        [SerializeField] private int m_Seed = -1;

        /// <summary>
        /// The catalogue (VRG_AddressablesByLabel) addressable Object
        /// </summary>
        [Tooltip("The catalogue (VRG_AddressablesByLabel) addressable Object")]
        [SerializeField] private VRG_AddressablesByLabel m_Data = new VRG_AddressablesByLabel("PlaceHolder");
        public static VRG_AddressablesByLabel data
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.m_Data;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// The catalogue (VRG_AddressablesByLabel) addressable Object
        /// </summary>
        [Tooltip("The catalogue (VRG_AddressablesByLabel) addressable Object")]
        [SerializeField] private VRG_AddressablesByLabel m_DataDefault = new VRG_AddressablesByLabel("SlideShow");

        /// <summary>
        /// The catalogue (VRG_AddressablesByLabel) for every scene, this is temporal and is called by custom scene
        /// </summary>
        [Tooltip("The catalogue (VRG_AddressablesByLabel) for every scene, this is temporal and is called by custom scene")]
        [SerializeField] private VRG_AddressablesByLabel m_DataByScene = new VRG_AddressablesByLabel("SlideShow.Scenes");

        /// <summary>
        /// The current addressable Slide Object
        /// </summary>
        [Tooltip("The current addressable Slide Object")]
        [SerializeField] private VRG_AddressableSerializable m_Addressable = new VRG_AddressableSerializable();







        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        [Tooltip("The catalogue (VRG_AddressablesByLabel) for every scene, this is temporal and is called by custom scene")]
        [SerializeField] private bool m_IsNewScene = false;


        [Tooltip("You can ask for this variable to know if the object is ready or is still querying information")]
        [SerializeField]//
        private bool m_IsDownloading = false;
        /// <summary>
        /// You can ask for this variable to know if the object is ready or is still querying information
        /// </summary>
        public static bool isDownloading { get { return Instance.m_IsDownloading; } }

        [Tooltip("You can ask for this variable to know if the object is ready or is still querying information")]
        [SerializeField]//
        private bool m_IsReady = false;
        /// <summary>
        /// You can ask for this variable to know if the object is ready or is still querying information
        /// </summary>
        public static bool isReady { get { return Instance.m_IsReady; } }

        [Tooltip("From Remote: The remote variable that host the list of the Slide")]
        [SerializeField]//
        private string m_Remote_Labels = "SlideShow";

        [Tooltip("From Remote: The remote variable that host the list of the Slide")]
        [SerializeField]//
        private string m_Remote_Seed = "VRG_SlideShow.m_Seed";



        [Tooltip("The current index of the Loading asset that is running")]
        [SerializeField]//
        private int m_SlideCurrent = -1;

        [Tooltip("The current index of the Loading asset that is running")]
        [SerializeField]//
        private int m_SlideCurrentScene = -1;

        [Tooltip("The delay before the next slide is loaded, 0 for infinity")]
        [SerializeField]//
        private float m_SlideDelay = 0;

        [Tooltip("The delay courutine handler")]
        [SerializeField]//
        private Coroutine m_LoadingIEnumerator;

        [Tooltip("The progress courutine handler")]
        [SerializeField]//
        private Coroutine m_ProgressIEnumerator;






        public VRG_SlideShow()
        {
            this.m_PlayOnEnable = true;
            this.m_SelfTurnOff = false;
            this.m_NextFrame = true;

            this.m_Data.orderBy = true;
            this.m_DataDefault.orderBy = true;
            this.m_DataByScene.orderBy = true;
        }

        // singletoning Pattern
        public static VRG_SlideShow Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                if (this.m_Menu == null)
                {
                    this.m_Menu = this.transform.Find("Menu").gameObject;

                    if (this.m_Menu == null)
                    {
                        this.Logs("Slide Manager can't create without a Menu", ENUM_Verbose.ERROR);

                        // I am not the one... I will walk to the eternal darkness
                        Destroy(this.gameObject);
                    }
                }
                if (this.m_Progress == null)
                {
                    this.m_Progress = this.transform.Find("Progress").GetComponent<Slider>();
                }
                if (this.m_LoadingBox == null)
                {
                    this.m_LoadingBox = this.transform.Find("Loading Box").gameObject;
                }

                if (this.m_Menu == null || this.m_Progress == null || this.m_LoadingBox == null)
                {
                    this.Logs("VRG_SlideShow Object has some UI null elements", ENUM_Verbose.ERROR);

                    // I am not the one... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
            else
            {
                this.Logs("Already exist the VRG_SlideShow Object, Stopped a double try to load it", ENUM_Verbose.ERROR);

                // I am not the one... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        private void OnDisable()
        {
            // cycle the carrousels to create the mini-buttons
            foreach (Transform child in this.m_Menu.transform)
            {
                // ... and its number
                child.GetComponent<VRG_SlideButton>().Selected(-11);

                child.gameObject.SetActive(false);
            }

            this.StopCoroutines();

            this.m_Addressable.Release();

            this.m_DataByScene.ClearLabels();
        }

















        public static IEnumerator IsValid()
        {
            yield return VRG_SlideShow.IsValid(true);
        }
        /// <summary>
        /// Ask for this ienumerator if the VRG_SlideShow is ready
        /// </summary>
        /// <returns>Null when the VRG_SlideShow is ready to be queried</returns>
        public static IEnumerator IsValid(bool valueLocal)
        {
            // Let's assume everything is configured properly
            bool bContinue = true;

            if (VRG_SlideShow.Instance == null)
            {
                if (GameObject.FindObjectOfType<VRG_SlideShow>() == null)
                {
                    bContinue = false;
                }
            }

            // If after 300 frames you can't get the VRG_Campaign object it's probable not configured
            while (VRG_SlideShow.Instance == null && bContinue)
            {
                yield return null;
            }

            if (bContinue)
            {
                // wait until Init is done
                while (VRG_SlideShow.isReady == false)
                {
                    yield return null;
                }
            }
            else
            {
                if (valueLocal)
                {
                    VRG_Bhel.Do
                    (
                        "Please be sure a VRG_SlideShow prefab is added to the scene",
                        "VRG_SlideShow->IsValid()",
                        ENUM_Verbose.WARNING,
                        "Static Method"
                    );
                }
            }

            // go to next frame
            yield return null;
        }

        /// <summary>
        /// Load the next scene, it checks if it has particular sliders or it loads the default
        /// </summary>
        /// <param name="valueLocal">The scene name to load</param>
        public static void SetScene(string valueLocal)
        {
            // the new scene needs to load
            Instance.m_IsNewScene = true;

            // restart the instance
            Instance.m_DataByScene.ClearLabels();

            // add the label list from remote
            Instance.m_DataByScene.AddLabel(valueLocal + ".SlideShow");

            // add the label list from remote
            Instance.m_DataByScene.AddLabel(VRG_Remote.GetString(valueLocal + ".SlideShow", false));

            // update teh current instance
            Instance.m_Data = Instance.m_DataByScene;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Start the timer and activate the current slide
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void Play(int valueLocal)
        {
            if (valueLocal == Instance.m_SlideCurrent)
            {
                Instance.StopCoroutines();

                // start the delay to the download sign
                Instance.m_ProgressIEnumerator = Instance.StartCoroutine(Instance.SetDelay_IEnumerator());
            }
            else
            {
                if (Instance.m_IsReady && !Instance.m_IsDownloading)
                {
                    if (valueLocal >= 0 && valueLocal < Instance.m_Data.keys.Count)
                    {
                        // next Slide
                        Instance.m_SlideCurrent = valueLocal;

                        // play the next one
                        Instance.Play();
                    }
                }
            }
        }

        /// <summary>
        /// The time you need to wait before the next slide, this parameter is defined
        /// every new <a href="#VrGamesDev.DDuA.VRG_Slide">VRG_Slider</a> by the <a href="#VrGamesDev.DDuA.VRG_Slide.m_Delay">m_Delay</a> parameter
        ///
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void SetDelay(float valueLocal)
        {
            if (Instance != null)
            {
                // just do it if a slider is defined
                if (Instance.m_Progress != null)
                {
                    if (Instance.gameObject.activeInHierarchy)
                    {
                        if (valueLocal > 0 && Instance.m_Data.keys.Count > 1)
                        {
                            Instance.m_SlideDelay = valueLocal;

                            // start the delay to the download sign
                            Instance.m_ProgressIEnumerator = Instance.StartCoroutine(Instance.SetDelay_IEnumerator());
                        }
                    }
                }
            }
        }
        // wait some delay seconds and activate the loading box
        private IEnumerator SetDelay_IEnumerator()
        {
            // just do it if a slider is defined
            if (Instance.m_Progress != null)
            {
                // start at the very begginging
                this.m_Progress.value = 0;

                // I will begin at:
                float fInitialTime = Time.realtimeSinceStartup;

                // if it is not done
                while (this.m_Progress.value < 1)
                {
                    // increase it 
                    this.m_Progress.value = (Time.realtimeSinceStartup - fInitialTime) / this.m_SlideDelay;

                    yield return null;
                }

                // turn it off in case the next one is not a Slide
                this.m_Progress.value = 0;

                if (Instance.m_IsReady && !Instance.m_IsDownloading)
                {
                    // next
                    Instance.m_SlideCurrent++;

                    // play the next one
                    Instance.Play();
                }
            }
            yield return null;
        }















        protected override IEnumerator Do()
        {
            // if it is changing.. ignore it
            if (!this.m_IsDownloading)
            {
                // just do it once, then play the Slide 
                if (this.m_IsReady == false)
                {
                    // just do it if a slider is defined
                    if (this.m_Progress != null)
                    {
                        // reset the progress of the slider
                        this.m_Progress.value = 0;
                    }
                    else
                    {
                        this.Logs("Your progress slider is NULL", ENUM_Verbose.WARNING);
                    }

                    // ask and wait for teh remote singleton
                    yield return StartCoroutine(VRG_Remote.IsValid());

                    if (VRG_Remote.ExistInt(this.m_Remote_Seed))
                    {
                        this.m_Seed = VRG_Remote.GetInt(this.m_Remote_Seed);
                    }

                    // we will start to download all
                    this.m_IsDownloading = true;

                    // and be able to change the loaded item
                    this.m_Addressable.isLocked = false;

                    // asign the parent for the logs
                    this.m_Addressable.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                    // asign the parent for the logs
                    this.m_DataByScene.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                    // asign the parent for the logs
                    this.m_DataDefault.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                    // add the label list from remote
                    this.m_DataDefault.AddLabel
                    (
                        VRG_Remote.GetString(this.m_Remote_Labels, false).Split('|')
                    );

                    // by default start with the initial screen
                    this.m_Data = this.m_DataDefault;

                    // we will download the first so it is not needed to register it download in the slider
                    this.m_Data.WhenKeysFound += OnKeysFound;
                    this.m_Data.Play();
                }
                else
                {
                    if (this.m_IsNewScene)
                    {
                        // we will download the first so it is not needed to register it download in the slider
                        this.m_Data.WhenKeysFound += OnKeysFound;
                        this.m_Data.Play();
                    }
                    else
                    {
                        // In case we are above the maximun
                        if (this.m_SlideCurrent >= this.m_Data.keys.Count)
                        {
                            // start with the first one
                            this.m_SlideCurrent = 0;
                        }

                        if (this.m_SlideCurrent >= 0)
                        {
                            // start the delay to the download sign
                            this.m_LoadingIEnumerator = StartCoroutine(this.Loading());

                            // suscribe to the complete event
                            this.m_IsDownloading = true;

                            // suscribe to the complete event
                            this.m_Addressable.WhenComplete += OnComplete;

                            // Instantiate the core class and on completed go to delegated
                            this.m_Addressable.Play(this.m_Data.keys[this.m_SlideCurrent]);
                        }
                        else
                        {
                            // cycle the carrousels to create the mini-buttons
                            foreach (Transform child in this.m_Menu.transform)
                            {
                                // ... and its number
                                child.GetComponent<VRG_SlideButton>().Selected(-11);

                                child.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }

            // return, it is like a void and wait a frame
            yield return null;
        }

        private void OnKeysFound(string[] valueLocal)
        {
            // unsuscribe
            this.m_Data.WhenKeysFound -= OnKeysFound;

            bool bSlideShowDefault = true;

            // just do it if there are keys
            if (this.m_Data.keys.Count > 0)
            {
                if (this.m_IsNewScene)
                {
                    bSlideShowDefault = false;

                    // when it is downloaded, start the app and we are ready to go
                    this.m_Addressable.WhenComplete += OnComplete;

                    this.m_SlideCurrentScene = Random.Range(0, this.m_Data.keys.Count);

                    this.m_Addressable.Play(this.m_Data.keys[this.m_SlideCurrentScene]);
                }
            }
            else
            {
                this.m_Data = this.m_DataDefault;

                this.m_IsReady = true;

                this.m_IsNewScene = false;


                if (this.m_Data.keys.Count == 0)
                {
                    VRG_DDuA.sceneNoSlideShow = true;
                }
            }

            if (bSlideShowDefault && this.m_Data.keys.Count > 0)
            {
                if (this.m_Seed < 0)
                {
                    // select a random Waitins from Remote
                    this.m_SlideCurrent = Random.Range(0, this.m_Data.keys.Count);
                }
                else
                {
                    if (this.m_SlideCurrent < 0)
                    {
                        // select a random Waitins from Remote
                        this.m_SlideCurrent = this.m_Seed;
                    }
                }

                // In case we are above the maximun
                if (this.m_SlideCurrent >= this.m_Data.keys.Count)
                {
                    // Instantiate the core class and on completed go to delegated
                    this.m_SlideCurrent = 0;
                }

                // when it is downloaded, start the app and we are ready to go
                this.m_Addressable.WhenComplete += OnComplete;

                // Instantiate the core class and on completed go to delegated
                this.m_Addressable.Play(this.m_Data.keys[this.m_SlideCurrent]);
            }
            else
            {
                this.m_IsReady = true;

                this.StopCoroutines();

                int iCurrent = this.m_SlideCurrent;

                if (this.m_IsNewScene)
                {
                    iCurrent = this.m_SlideCurrentScene;
                }

                this.m_IsNewScene = false;

                // ok, we are done with this loading, and the UI is filled
                this.m_IsDownloading = false;

                // Turn it on until the Slide is loaded
                this.m_LoadingBox.SetActive(true);
            }
        }

        // when it is completed, dissapear the loading image
        private void OnComplete()
        {
            // unsuscribe
            this.m_Addressable.WhenComplete -= OnComplete;

            this.m_IsReady = true;

            this.StopCoroutines();

            int iCurrent = this.m_SlideCurrent;

            if (this.m_IsNewScene)
            {
                iCurrent = this.m_SlideCurrentScene;
            }

            this.m_IsNewScene = false;

            // ok, we are done with this loading, and the UI is filled
            this.m_IsDownloading = false;

            // if it was completed
            if (this.m_Addressable.status == ENUM_AddressableStatus.PREFAB)
            {
                for (int i = 0; i < this.m_Menu.transform.childCount; i++)
                {
                    if (this.m_Data.keys.Count > i && this.m_Data.keys.Count > 1)
                    {
                        this.m_Menu.transform.GetChild(i).gameObject.SetActive(true);

                        this.m_Menu.transform.GetChild(i).GetComponent<VRG_SlideButton>().Selected(iCurrent);
                    }
                    else
                    {
                        this.m_Menu.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                this.Logs
                (
                    this.m_Addressable.address + " <b>FAILED</b> | Status = " + this.m_Addressable.status,
                    "VRG_SlideShow->OnComplete()",
                    ENUM_Verbose.ERROR
                );
            }
        }

        // wait some delay seconds and activate the loading box
        private IEnumerator Loading()
        {
            // if we have a box...
            if (this.m_LoadingBox != null)
            {
                // ... wait for the delay
                yield return new WaitForSeconds(this.m_LoadingBoxDelay);

                // Turn it on until the Slide is loaded
                this.m_LoadingBox.SetActive(true);
            }

            yield return null;
        }

        // stop all the progress UI and proccess
        private void StopCoroutines()
        {
            // ... and I have a box
            if (this.m_LoadingBox != null)
            {
                if (this.m_LoadingIEnumerator != null)
                {
                    // stop the timer
                    StopCoroutine(this.m_LoadingIEnumerator);
                }

                // Turn it off, the Slide is loaded
                this.m_LoadingBox.SetActive(false);
            }

            if (this.m_Progress != null)
            {
                if (this.m_ProgressIEnumerator != null)
                {
                    // stop the progress bar
                    this.StopCoroutine(this.m_ProgressIEnumerator);

                    this.m_Progress.value = 0;
                }
            }
        }

    }
}