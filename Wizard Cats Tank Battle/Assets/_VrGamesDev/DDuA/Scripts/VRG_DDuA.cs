using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;
using UnityEngine.AddressableAssets;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class is the core that controls the whole Addressable system
    /// 
    /// - Download and play the Slide
    /// - Reads the data from VRG_remote and start the download of the singletons modules
    /// - Gets the scenes and their prefab data from the addressable server/cache
    /// - Inform the player with a loading bar what is happening behind the curtain
    /// </summary>
    public class VRG_DDuA : VRG_Base
    {
        [Tooltip("Is this the first time i will enter")]
        //[SerializeField]//
        private bool m_IsInit = false;

        [Header("FROM Remote:")]
        /// <summary>
        /// If true, DDuA will offer the option to download the catalogue while Idle
        /// </summary>
        [Tooltip("If true, DDuA will offer the option to download the catalogue while Idle")]
        [SerializeField]
        private bool m_IsCatalogueIdle = false;
        public bool isCatalogueIdle
        {
            get
            {
                return this.m_IsCatalogueIdle;
            }
            set
            {
                this.m_IsCatalogueIdle = value;

                this.CatalogueDownload();
            }
        }

        [Tooltip("From Remote: The name of the delayer to download in background the catalogue")]
        //[SerializeField]//
        private string m_Remote_CatalogueIdle = "VRG_DDuA.isCatalogueIdle";

        /// <summary>
        /// How often checks for the download progress and update the UI
        /// </summary>
        [Range(0, 2.0f)]
        [Tooltip("How often checks for the download progress and update the UI, 0 is every frame")]
        [SerializeField] private float m_ProgressDelay = 0.250f;

        [Tooltip("From Remote: The time to adjust how often to refresh the slider progress bar")]
        //[SerializeField]//
        private string m_Remote_ProgressDelay = "VRG_DDuA.m_ProgressDelay";

        [Tooltip("The name of the scene to load the next time LoadScene is called")]
        [SerializeField]//
        private string m_SceneName = "Home";
        //[SerializeField]//
        private string m_SceneNamePrevious = "Home";

        [Tooltip("From Remote: The name of the first scene to load, by default is Home")]
        //[SerializeField]//
        private string m_Remote_SceneName = "VRG_DDuA.m_SceneName";




        [Tooltip("From Remote: All the Prefab and singletons you need to get instantiated before anything else")]
        //[SerializeField]//
        private string m_Remote_OnLaunch = "OnLaunch";

        [Tooltip("From Remote: All the download needed 'soon', and you want to be sure it is always predownloaded")]
        //[SerializeField]//
        private string m_Remote_PreDownload = "PreDownload";













        [Header("From: Scenes")]
        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Space(20)]
        /// <summary>
        /// To stop a possible double listener, disable when loading new scene
        /// </summary>
        [Tooltip("To stop a possible double listener, disable when loading new scene")]
        [SerializeField] private AudioListener m_AudioListener = null;

        /// <summary>
        /// Luke, I'm your <a href="index.html#VrGamesDev.VRG_Fader">Fader</a>
        /// </summary>
        [Tooltip("Luke, I'm your Fader")]
        [SerializeField] private VRG_Fader m_VRG_Fader = null;


        [Tooltip("AutoLoad the next scene when ready?")]
        //[SerializeField]//
        private bool m_SceneNoSlideShow = false;
        public static bool sceneNoSlideShow
        {
            get
            {
                return Instance.m_SceneNoSlideShow;
            }
            set
            {
                Instance.m_SceneNoSlideShow = value;
            }
        }

        /// <summary>
        /// The Current Scene loaded
        /// </summary>
        [Tooltip("The Current Scene loaded")]
        //[SerializeField]
        private VRG_AddressableScene m_Scene = new VRG_AddressableScene("VRG_DDuA_Proxy");

        [Tooltip("The Slide of addressables objects")]
        //[SerializeField]
        private VRG_AddressablesByLabel m_SceneOnLaunch = new VRG_AddressablesByLabel();

        [Tooltip("The name of the scene to load the next time LoadScene is called")]
        //[SerializeField]//
        public static string m_SceneProxy = "VRG_DDuA_Proxy";







        [Header("From: Catalogue")]

        [Tooltip("How often we will pull a new address in the background")]
        [SerializeField]
        private GameObject m_UiCatalogue = null;

        [Tooltip("asdasd")]
        [SerializeField]
        private GameObject m_UiCatalogueActivate = null;

        [Tooltip("asdasd")]
        [SerializeField]
        private GameObject m_UiCatalogueDownload = null;

        [Tooltip("asdasd")]
        [SerializeField]
        private Text m_UiCatalogueSize = null;

        [Tooltip("asdasd")]
        [SerializeField]
        private GameObject m_UiCatalogueActions = null;

        [Tooltip("asdasd")]
        //[SerializeField]
        private long m_CatalogueSize = 0;
        public long catalogueSize { get { return this.m_CatalogueSize; } }



        [Tooltip("If true, DDuA already loaded the catalogue in memory")]
        //[SerializeField]
        private bool m_IsCatalogueInit = false;

        [Tooltip("If true, DDuA is downloading the catalogue")]
        //[SerializeField]
        private bool m_IsCatalogueDownload = false;
        public bool isCatalogueDownload
        {
            get
            {
                return this.m_IsCatalogueDownload;
            }
            set
            {
                this.m_IsCatalogueDownload = value;

                VRG_Session.SetBool("VRG_DDuA", "isCatalogueDownload", this.m_IsCatalogueDownload);

                this.CatalogueDownload();
            }
        }

        //[SerializeField] private List<string> m_CatalogueTEMP = new List<string>();
        [Tooltip("All the catalogue list of items to download")]
        //[SerializeField]
        private List<VRG_AddressableSerializable> m_Catalogue = new List<VRG_AddressableSerializable>();
        public List<VRG_AddressableSerializable> catalogue { get { return this.m_Catalogue; } }

        /// <summary>
        /// The addressable list of objects do download and create at launch of the app, It is recommended to be DontDestroyOnLoad objects
        /// </summary>
        [Tooltip("The addressable list of objects do download at launch of the app, It is recommended to be DontDestroyOnLoad objects")]
        //[SerializeField]
        private VRG_AddressablesByLabel m_OnLaunch = new VRG_AddressablesByLabel("OnLaunch");

        /// <summary>
        /// The addressable list of objects do download at launch of the app, this objects will just download to have them cached when needed
        /// </summary>
        [Tooltip("The VRG_DDuA addressable Object")]
        //[SerializeField]
        private VRG_AddressablesByLabel m_PreDownload = new VRG_AddressablesByLabel("PreDownload");





        [Header("From: UI")]
        /// <summary>
        /// Status that inform the player what is going in
        /// </summary>
        [Tooltip("Status that inform the player what is going in")]
        [SerializeField] private Text m_UiStatusLabel = null;

        /// <summary>
        /// Internet network status and tries to get the bytes
        /// </summary>
        [Tooltip("Internet network status and tries to get the bytes")]
        [SerializeField] private Text m_UiInternetNetwork = null;

        /// <summary>
        /// The slider progress bar
        /// </summary>
        [Tooltip("The slider progress bar")]
        [SerializeField] private Slider m_UiSlider = null;

        /// <summary>
        /// The accept button, it allows to go to scene
        /// </summary>
        [Tooltip("The accept button, it allows to go to scene")]
        [SerializeField] private Image m_UiButton = null;

        /// <summary>
        /// The download percentage text
        /// </summary>
        [Tooltip("The download persentage text")]
        [SerializeField] private Text m_UiPercentage = null;

        /// <summary>
        /// Is there a progress in progress? (Pun intended)
        /// </summary>
        [Tooltip("Is there a progress in progress? (Pun intended)")]
        [SerializeField] private string m_UiButtonText = "Start";

        /// <summary>
        /// The VRG_DDuA addressable Object list to download and/or create
        /// </summary>
        [Tooltip("The VRG_DDuA addressable Object list to download and/or create")]
        //[SerializeField]
        private List<VRG_AddressableSerializable> m_Progressables = new List<VRG_AddressableSerializable>();

































































        /// <summary>
        /// constructor,
        /// </summary>
        public VRG_DDuA()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = true;
        }

        // singletoning Pattern
        public static VRG_DDuA Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                if (!(false

                    || this.m_UiCatalogue == null
                    || this.m_UiCatalogueActivate == null
                    || this.m_UiCatalogueDownload == null
                    || this.m_UiCatalogueActions == null                    
                    || this.m_UiCatalogueSize == null

                    || this.m_UiStatusLabel == null
                    || this.m_UiInternetNetwork == null

                    || this.m_UiSlider == null
                    || this.m_UiButton == null
                    || this.m_UiPercentage == null

                    || this.m_VRG_Fader == null
                    || this.m_AudioListener == null
                    ))
                {
                    // ... since i am the one, I declare myself as the one
                    Instance = this;

                    // i follow my own rules
                    this.transform.SetParent(null);

                    // ... and I will live forever
                    DontDestroyOnLoad(this);

                    // Listener to the event OnFadeIn, when the whole package is downloaded
                    this.m_VRG_Fader.WhenFadeIn += OnFadeIn;

                    // Listener to the event OnFadeOut, when the whole package is downloaded
                    this.m_VRG_Fader.WhenFadeOut += OnFadeOut_Scene;

                    // Listener to the event OnFadeOut, when the whole package is downloaded
                    this.m_VRG_Fader.WhenFadeOut += OnFadeOut_Catalogue;

                    //Check if the device cannot reach the internet
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        //Not Reachable
                        this.m_UiInternetNetwork.text = "No internet conection";
                    }

                    //Check if the device can reach the internet via a carrier data network
                    else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                    {
                        //Reachable via carrier data network.
                        this.m_UiInternetNetwork.text = "Mobile network";
                    }

                    //Check if the device can reach the internet via a LAN
                    else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                    {
                        //Reachable via Local Area Network;
                        this.m_UiInternetNetwork.text = "Wifi";
                    }
                }
                else
                {
                    // inform the logs
                    this.Logs("There are some UI components null, recreate the VRG_DDuA Object", "VRG_DDuA->Awake()", ENUM_Verbose.ERROR);

                    // I am not the one... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
            else
            {
                this.Logs("Already exist the VRG_DDuA Object, Stopped a double try to load it", "VRG_DDuA->Awake()", ENUM_Verbose.ERROR);

                // I am not the one... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            this.m_SelfTurnOff = false;

            if (!this.m_IsInit)
            {
                this.m_IsInit = true;

                // ask and wait for teh remote singleton
                yield return VRG_Remote.IsValid();

                // ask and wait for teh remote singleton
                yield return VRG_SlideShow.IsValid();

                //////////////////////////////////////////////////////////////
                //
                // Update local data with the remote configuration
                //
                //////////////////////////////////////////////////////////////

                // If exist, update the delay of the progress bar from the server config
                if (VRG_Remote.ExistFloat(this.m_Remote_ProgressDelay))
                {
                    this.m_ProgressDelay = VRG_Remote.GetFloat(this.m_Remote_ProgressDelay);
                }

                if (VRG_Session.GetString("VRG_Loader", "VRG_Scene") != string.Empty)
                {
                    this.m_SceneName = VRG_Session.GetString("VRG_Loader", "VRG_Scene");

                    VRG_Session.SetString("VRG_Loader", "VRG_Scene", "");
                }
                else
                {
                    // If exist, update the delay of the progress bar from the server config
                    if (VRG_Remote.ExistString(this.m_Remote_SceneName))
                    {
                        string sSceneName = VRG_Remote.GetString(this.m_Remote_SceneName);

                        if (sSceneName != string.Empty)
                        {
                            this.m_SceneName = sSceneName;
                        }
                    }
                }



                //////////////////////////////////////////////////////////////
                //
                // The Scene loader, it will always it is completed
                //
                //////////////////////////////////////////////////////////////
                // configure the new scene to load
                this.m_Scene.isLocked = false;

                this.m_Scene.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                this.m_Scene.WhenComplete += OnSceneCompleted;

                this.m_Scene.WhenComplete += ToggleUI;

                this.m_SceneOnLaunch.objectInScene = VRG.GetSceneGameObject(this.gameObject);



                //////////////////////////////////////////////////////////////
                //
                // The OnLaunch addressables
                //
                //////////////////////////////////////////////////////////////
                // asign the parent for the logs
                this.m_OnLaunch.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                // download everything in this catalogue
                this.m_OnLaunch.create = true;

                // add the label list from remote
                this.m_OnLaunch.AddLabel(VRG_Remote.GetString(this.m_Remote_OnLaunch, false).Split('|'));

                // we will download the first so it is not needed to register it download in the slider
                this.m_OnLaunch.Play();



                //////////////////////////////////////////////////////////////
                //
                // The PreDownload addressables
                //
                //////////////////////////////////////////////////////////////
                // asign the parent for the logs
                this.m_PreDownload.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                // add the label list from remote
                this.m_PreDownload.AddLabel(VRG_Remote.GetString(this.m_Remote_PreDownload, false).Split('|'));



                //////////////////////////////////////////////////////////////
                //
                // The Scenes addressables
                //
                /////////////////////////////////////////////////////////////                
                this.DownloadSceneElements();



                //////////////////////////////////////////////////////////////
                //
                // The Catalogue addressables | get al the current catalogue
                //
                //////////////////////////////////////////////////////////////
                if (VRG_Remote.ExistBool(this.m_Remote_CatalogueIdle))
                {
                    this.m_IsCatalogueIdle = VRG_Remote.GetBool(this.m_Remote_CatalogueIdle);
                }

                if (!this.m_IsCatalogueInit)
                {
                    this.m_IsCatalogueInit = true;

                    string sLogs = "<hr>Catalogue List<hr>";

                    foreach (var loc in Addressables.ResourceLocators)
                    {
                        foreach (var key in loc.Keys)
                        {
                            // just save the MD5 Keys
                            if (loc.Locate(key, typeof(object), out var resourceLocations))
                            {
                                if (Regex.IsMatch(key.ToString(), "^[0-9a-fA-F]{32}$", RegexOptions.Compiled))
                                {
                                    sLogs += "\n<color=green> + " + key.ToString() + "</color>";

                                    VRG_AddressableSerializable aListElement = new VRG_AddressableSerializable(key.ToString());

                                    aListElement.objectInScene = VRG.GetSceneGameObject(this.gameObject);

                                    aListElement.evolution = ENUM_AddressableEvolution.SIZE;

                                    aListElement.Play();

                                    this.m_Catalogue.Add(aListElement);
                                }
                                else
                                {
                                    sLogs += "\n<color=black> - " + key.ToString() + "</color>";
                                }
                            }
                        }
                    }
                    //this.Logs(sLogs, "VRG_DDuA->Do()", ENUM_Verbose.LOGS);

                    bool bContinue = true;

                    while (bContinue)
                    {
                        bContinue = false;

                        List<VRG_AddressableSerializable> aZeroSize = new List<VRG_AddressableSerializable>();

                        foreach (VRG_AddressableSerializable child in this.m_Catalogue)
                        {
                            if (child.status == ENUM_AddressableStatus.SIZED)
                            {
                                if (child.size == 0)
                                {
                                    //print("Deleting: " + child.addressFromPath);
                                    aZeroSize.Add(child);
                                }
                            }
                            else
                            {
                                bContinue = true;
                                break;
                            }
                        }

                        foreach (VRG_AddressableSerializable child in aZeroSize)
                        {
                            child.Destroy();
                            this.m_Catalogue.Remove(child);
                        }

                        yield return null;
                    }

                    this.m_Catalogue.Sort((p1, p2) => p1.size.CompareTo(p2.size));
                }

                this.isCatalogueDownload = VRG_Session.GetBool("VRG_DDuA", "isCatalogueDownload");
            }

            yield return null;
        }

        private void OnDestroy()
        {
            this.m_VRG_Fader.WhenFadeIn -= OnFadeIn;

            this.m_VRG_Fader.WhenFadeOut -= OnFadeOut_Scene;

            this.m_VRG_Fader.WhenFadeOut -= OnFadeOut_Catalogue;


            this.m_Scene.WhenComplete -= OnSceneCompleted;

            this.m_Scene.WhenComplete -= ToggleUI;
        }

        private void ToggleUI()
        {
            bool bDDuA_UI = false;
            if (this.m_Scene.status == ENUM_AddressableStatus.SCENE_LOADED)
            {
                if (this.m_Scene.address == VRG_DDuA.m_SceneProxy)
                {
                    bDDuA_UI = true;
                }

                foreach (Transform child in this.transform)
                {
                    child.gameObject.SetActive(bDDuA_UI);
                }
                bDDuA_UI = true;
            }
            else
            {
                // inform the player what is going on
                this.m_UiStatusLabel.text = "Error loading the " + this.m_SceneName + " scene = " + this.m_Scene.status;
            }

            this.m_VRG_Fader.gameObject.SetActive(bDDuA_UI);

            this.m_UiCatalogueActions.SetActive(true);

            this.m_UiCatalogue.SetActive(false);
            this.m_UiCatalogueActivate.SetActive(false);
            this.m_UiCatalogueDownload.SetActive(false);
        }















































        private void DownloadSceneElements()
        {
            // make sure the first scene will be loaded
            this.m_PreDownload.id = this.m_SceneName;

            // add the label list from remote
            this.m_PreDownload.AddLabel(this.m_SceneName + ".SlideShow");
            this.m_PreDownload.AddLabel(VRG_Remote.GetString(this.m_SceneName + ".SlideShow", false).Split('|'));

            // add the label list from remote
            this.m_PreDownload.AddLabel(this.m_SceneName + "." + this.m_Remote_OnLaunch);
            this.m_PreDownload.AddLabel(VRG_Remote.GetString(this.m_SceneName + "." + this.m_Remote_OnLaunch, false).Split('|'));

            // add the label list from remote
            this.m_PreDownload.AddLabel(this.m_SceneName + "." + this.m_Remote_PreDownload);
            this.m_PreDownload.AddLabel(VRG_Remote.GetString(this.m_SceneName + "." + this.m_Remote_PreDownload, false).Split('|'));

            // when it is done, trigger the progress bar
            this.m_PreDownload.WhenKeysFound += OnKeysFound;

            // we will download the first so it is not needed to register it download in the slider
            this.m_PreDownload.Play();
        }

        private void OnKeysFound(string[] valueLocal)
        {
            this.m_PreDownload.WhenKeysFound -= OnKeysFound;

            // since the download already started, inform the player of the progress
            StartCoroutine(this.Progress());
        }

        public static bool AddProgress(VRG_AddressableSerializable valueLocal)
        {
            bool bRegresa = false;

            // I will check if I singleton exist
            if (Instance != null)
            {
                // is it downloadable?
                if (valueLocal.size > 0)// && Instance.m_UiSlider.isActiveAndEnabled)
                {
                    // just if it is new
                    if (!Instance.m_Progressables.Contains(valueLocal))
                    {
                        // by default is new
                        bool bContinue = true;

                        // but lets be sure 
                        foreach (VRG_AddressableSerializable child in Instance.m_Progressables)
                        {
                            // AM i already downloading this address?
                            if (valueLocal.address == child.address)
                            {
                                // ... then don't add it
                                bContinue = false;
                                break;
                            }
                        }

                        // was it really new?
                        if (bContinue)
                        {
                            // ... then add it
                            Instance.m_Progressables.Add(valueLocal);

                            bRegresa = true;
                        }
                    }
                }
            }

            return bRegresa;
        }

        // Coruotine to inform the player of the progress with a nice loading bar
        private void CleanProgressables()
        {
            List<VRG_AddressableSerializable> a_Addressable = new List<VRG_AddressableSerializable>();

            foreach (VRG_AddressableSerializable child in this.m_Progressables)
            {
                if (child.progressHandle.IsValid())
                {
                    if (true
                        && child.status == ENUM_AddressableStatus.DOWNLOADED
                        )
                    /*
                        && child.status != ENUM_AddressableStatus.SIZED
                        && child.status != ENUM_AddressableStatus.SIZING
                        && child.status == ENUM_AddressableStatus.DOWNLOADING
                        )
                    /*
                (
                    child.progressHandle.GetDownloadStatus().Percent >= 1
                    && child.progressHandle.GetDownloadStatus().DownloadedBytes >= (child.progressHandle.GetDownloadStatus().TotalBytes * 1)
                    && child.status != ENUM_AddressableStatus.DOWNLOADING
                )
                    */
                    {
                        a_Addressable.Add(child);

                        child.ProgressDone();
                    }
                }
                else
                {
                    a_Addressable.Add(child);
                }
            }

            foreach (VRG_AddressableSerializable child in a_Addressable)
            {
                this.m_Progressables.Remove(child);
            }
        }

        // Coruotine to inform the player of the progress with a nice loading bar
        private IEnumerator Progress()
        {
            yield return null;

            long lSize, lSizeTotal;
            string sLogs;
            float sPercentaje;
            string sColor;
            bool bContinue = true;

            this.CleanProgressables();
            {
                // reset the slider
                this.m_UiSlider.value = 0;

                // label its white and downloading
                this.m_UiPercentage.text = "...";

                // you can't load a new game until the scene is loaded, 
                // the button is white because it is not clickable
                this.m_UiButton.color = new Color32(255, 255, 255, 255);

                // ... and the button is not interactable
                this.m_UiButton.GetComponent<Button>().interactable = false;

                // inform the player what is going on
                this.m_UiStatusLabel.text = "Calculating download ...";
            }

            if (this.m_Progressables.Count > 0)
            {
                while (this.m_UiSlider.value < 1 || bContinue)
                {
                    sLogs = "\n";
                    lSizeTotal = lSize = 0;

                    bContinue = false;
                    foreach (VRG_AddressableSerializable child in this.m_Progressables)
                    {
                        lSize += child.progressHandle.GetDownloadStatus().DownloadedBytes;
                        lSizeTotal += child.progressHandle.GetDownloadStatus().TotalBytes;

                        if (child.status == ENUM_AddressableStatus.DOWNLOADING)
                        {
                            bContinue = true;
                        }

                        sColor = "red";
                        if (child.progressHandle.GetDownloadStatus().Percent > 0.4)
                        {
                            sColor = "orange";
                        }
                        if (child.progressHandle.GetDownloadStatus().Percent > 0.8)
                        {
                            sColor = "yellow";
                        }
                        if (child.progressHandle.GetDownloadStatus().Percent >= 1)
                        {
                            sColor = "green";
                        }

                        sLogs += "<color=" + sColor + ">"
                            + " - " + (child.progressHandle.GetDownloadStatus().Percent * 100).ToString("F1") + " %"
                            + " | " + child.address
                            + " = " + child.progressHandle.GetDownloadStatus().DownloadedBytes
                            + " / " + child.progressHandle.GetDownloadStatus().TotalBytes
                            + "</color>\n";
                    }

                    sPercentaje = 1;
                    if (lSizeTotal > 0)
                    {
                        sPercentaje = (float)lSize / (float)lSizeTotal;

                        this.Logs
                        (
                            "<hr><b><center>Progress of " + (sPercentaje * 100).ToString("F2") + " % (" + this.m_Progressables.Count + " downloads) - " + lSize + "/ " + lSizeTotal + " </b></center><hr>" + sLogs + "\n",
                            "DDuA->Progress()",
                            ENUM_Verbose.ALL
                        ); ;
                    }

                    this.m_UiSlider.value = sPercentaje;

                    if (sPercentaje > 0.995f)
                    {
                        sPercentaje = 0.995f;
                    }

                    this.m_UiPercentage.text = (sPercentaje * 100).ToString("F0") + " %";

                    //this.m_UiStatusLabel.text = "Downloading: " + (lSize).ToString("F0") + " / " + (lSizeTotal).ToString("F0");
                    this.m_UiStatusLabel.text = "Downloading: " + VRG.LongToBytes(lSize, false) + " / " + VRG.LongToBytes(lSizeTotal);



                    if (this.m_ProgressDelay == 0 || this.m_UiSlider.value >= 1)
                    {
                        yield return null;
                    }
                    else
                    {
                        yield return new WaitForSeconds(this.m_ProgressDelay);
                    }
                }
            }
            else
            {
                this.Logs
                (
                    "<hr><b>Progress of 100%, all the addressables are CACHED </b><hr>",
                    "DDuA->Progress()",
                    ENUM_Verbose.ALL
                ); ;
            }

            yield return null;



            //////////////////////////////////////////////////////////////
            //
            // The Catalogue addressables | remove from the catalogue what is just loaded
            //
            //////////////////////////////////////////////////////////////
            List<string> myList = new List<string>();

            myList.AddRange(VRG_SlideShow.data.keys);
            myList.AddRange(this.m_OnLaunch.keys);
            myList.AddRange(this.m_PreDownload.keys);

            float fTotal = myList.Count;

            float i = 0;

            float fCurrent = 0.0f;

            foreach (string child in myList)
            {
                fCurrent = (i++ / fTotal);

                this.m_UiSlider.value = 0.995f + (fCurrent * 0.005f);

                this.m_UiPercentage.text = (this.m_UiSlider.value * 100).ToString("F2") + " %";

                this.m_UiStatusLabel.text = "Validating the download: " + (fCurrent * 100).ToString("F1") + " %";

                this.m_Catalogue.Remove(this.m_Catalogue.Find(x => (x.address == child || x.addressFromPath == child)));
            }

            /*
            this.m_CatalogueTEMP.Clear();
            foreach (VRG_AddressableSerializable child in this.m_Catalogue)
            {
                this.m_CatalogueTEMP.Add(child.size.ToString() + " - " + child.addressFromPath);
            }
            */
            this.CatalogueSize();

            yield return null;



            bContinue = true;

            while (bContinue)
            {
                bContinue = false;

                foreach (VRG_AddressableSerializable child in this.m_PreDownload.addressables)
                {
                    if (true
                        && child.status != ENUM_AddressableStatus.DOWNLOADED
                        )
                    {
                        bContinue = true;
                        break;
                    }
                }

                yield return null;
            }

            this.m_PreDownload.ClearLabels();

            yield return null;



            this.CleanProgressables();
            {
                // complete the slider just in case
                this.m_UiSlider.value = 1;

                // label it is ok
                this.m_UiPercentage.text = this.m_UiButtonText;

                // the button is green because its now clickable
                this.m_UiButton.color = new Color32(155, 230, 95, 255);

                // ... and the button is now interactable
                this.m_UiButton.GetComponent<Button>().interactable = true;

                // inform the player what is going on
                this.m_UiStatusLabel.text = "Download complete!";
            }

            yield return null;

            // recover the autoload setting saved by the user
            if (this.m_SceneNoSlideShow || VRG_Session.GetBool("VRG_DDuA", "AutoLoad"))
            {
                this.m_SceneNoSlideShow = false;

                this.FadeStart();
            }

            yield return null;
        }

















































        public void LoadSceneFromButton()
        {
            // start the fade
            Instance.FadeStart();
        }

        public static void LoadScene(string valueLocal)
        {
            // just in case it is declared as a singleeton
            if (Instance != null)
            {
                if (Instance.m_UiSlider.value >= 1
                    ||
                    Instance.m_UiSlider.value == 0
                    )
                {
                    // if the scene is the same as the current loaded scene
                    if (Instance.m_SceneName == valueLocal || valueLocal == "[RELOAD SCENE]")
                    {
                        // well, it is fully downloaded, since it was alreayd running
                        Instance.m_UiSlider.value = 1;

                        // reload scene is the same as the current one
                        valueLocal = Instance.m_SceneName;
                    }

                    // before changin the scene save the current one
                    Instance.m_SceneNamePrevious = Instance.m_SceneName;

                    // set the name
                    Instance.m_SceneName = valueLocal;

                    // start the fade
                    Instance.FadeStart();
                }
                else
                {
                    Instance.Logs
                    (
                        "Scene: <color=blue><i>" + valueLocal + "</i></color> | Is trying to load while a download is in progress (" + (Instance.m_UiSlider.value * 100).ToString("F2") + " %" + ")",
                        "VRG_DDuA->LoadScene(" + valueLocal + ")",
                        ENUM_Verbose.ERROR
                    );
                }
            }

            //print("1) " + Time.frameCount + " | LoadScene(" + valueLocal + ") = " + (Instance.m_UiSlider.value * 100).ToString("F1") + " %";
        }

        private void FadeStart()
        {
            //print("2) " + Time.frameCount + " | FadeStart() | Progress = " + (Instance.m_UiSlider.value * 100).ToString("F1"));

            if (this.m_VRG_Fader.status == ENUM_Fader.NONE
                &&
                (
                    Instance.m_UiSlider.value >= 1
                    ||
                    Instance.m_UiSlider.value == 0
                )
            )
            {
                this.m_VRG_Fader.Play(true);
            }
        }

        // The fading in is finished the alpha is 1.0
        private void OnFadeIn()
        {
            // destroy the previous scene
            this.m_Scene.Release();

            string sScene = VRG_DDuA.m_SceneProxy;

            if (this.m_UiSlider.value >= 1)
            {
                sScene = this.m_SceneName;
            }

            // print("3) " + Time.frameCount + " | OnFadeIn( "+ this.m_SceneName + " )");

            this.m_AudioListener.enabled = false;

            // configure the new scene to load
            this.m_Scene.Play(sScene);

            string sLogs = "Scene: <color=blue><i>" + sScene + "</i></color> | <b>FADING</b> trying to load it ...";

            if (this.m_SceneName == VRG_DDuA.m_SceneProxy)
            {
                sLogs = "Scene: <color=blue><i>" + this.m_SceneNamePrevious + "</i></color> | <b>UNLOADING</b> ...";
            }

            // ... inform the logs
            this.Logs
            (
                sLogs,
                "VRG_DDuA->OnFadeIn()",
                ENUM_Verbose.LOGS
            );
        }

        private void OnSceneCompleted()
        {
            //print("4) " + Time.frameCount + " | SceneInit( "+ Instance.m_SceneName +" ) = " + this.m_Scene.status);

            if (this.m_Scene.status == ENUM_AddressableStatus.SCENE_LOADED)
            {
                if (this.m_SceneName == this.m_Scene.address)
                {
                    if (Instance.m_UiSlider.value >= 1)
                    {
                        // ... inform the logs
                        Instance.Logs
                        (
                            "Scene: <color=blue><i>" + Instance.m_SceneName + "</i></color> | <b> OnLAUNCH: </b>  ...",
                            "VRG_DDuA->OnSceneCompleted()",
                            ENUM_Verbose.LOGS
                        );
                    }

                    this.m_SceneOnLaunch.ClearLabels();

                    this.m_SceneOnLaunch.create = true;

                    this.m_SceneOnLaunch.AddLabel(this.m_SceneName + ".OnLaunch");

                    this.m_SceneOnLaunch.AddLabel(VRG_Remote.GetString(this.m_SceneName + ".OnLaunch", false).ToString().Split('|'));

                    this.m_SceneOnLaunch.WhenKeysFound += OnSceneOnLaunch_WhenKeysFound;

                    this.m_SceneOnLaunch.Play();
                }
                else
                {
                    if (this.m_Scene.address == VRG_DDuA.m_SceneProxy)
                    {
                        this.m_VRG_Fader.Play(false);
                    }
                    else
                    {
                        this.Logs("Scene Loaded = " + this.m_Scene.address + " / " + this.m_SceneName, ENUM_Verbose.ERROR);
                    }
                }
            }
        }

        private void OnSceneOnLaunch_WhenKeysFound(string[] obj)
        {
            this.m_SceneOnLaunch.WhenKeysFound -= OnSceneOnLaunch_WhenKeysFound;

            StartCoroutine(this.OnSceneOnLaunch_IEnumerator());
        }

        private IEnumerator OnSceneOnLaunch_IEnumerator()
        {
            bool bContinue = true;

            if (this.m_SceneOnLaunch.addressables.Count > 0)
            {
                while (bContinue)
                {
                    bContinue = false;

                    foreach (VRG_AddressableSerializable child in this.m_SceneOnLaunch.addressables)
                    {
                        //if (child.status != ENUM_AddressableStatus.PREFAB)
                        if (child.gameObject == null)
                        {
                            bContinue = true;
                            break;
                        }
                    }

                    yield return null;
                }
            }

            this.m_VRG_Fader.Play(false);

            if (this.m_UiSlider.value >= 1)
            {
                // ... inform the logs
                this.Logs
                (
                    "Scene: <color=blue><i>" + this.m_SceneName + "</i></color> | <b> FINISHED: ... fading out</b>",
                    "VRG_DDuA->OnSceneOnLaunch_IEnumerator()",
                    ENUM_Verbose.LOGS
                );
            }

            yield return null;
        }

        // the fading finished the alpha = 0.0f
        private void OnFadeOut_Scene()
        {
            if (this.m_UiSlider.value == 0)
            {
                //print("6) " + Time.frameCount + " | OnFadeOut_Scene: Download new scene");
                this.DownloadSceneElements();
            }
            else
            if (this.m_UiSlider.value >= 1)
            {
                //print("6) " + Time.frameCount + " | OnFadeOut_Scene: VRG_DDuA - Start the regular catalogue download");

                this.m_UiSlider.value = 0;

                this.CleanProgressables();
            }

            this.m_AudioListener.enabled = true;
        }

















































        // the fading finished the alpha = 0.0f
        private void OnFadeOut_Catalogue()
        {
            if (this.isCatalogueIdle)
            {
                StopAllCoroutines();

                this.CatalogueDownload();
            }
        }

        private void CatalogueDownload()
        {
            //print(Time.frameCount + ") CatalogueDownload = " + this.isCatalogueIdle + " | " + this.m_Scene.address);

            if (true
                && this.isCatalogueIdle
                && this.m_Scene.address != string.Empty
                && this.m_Scene.address != VRG_DDuA.m_SceneProxy
                )
            {
                this.m_UiCatalogue.SetActive(true);
                this.m_UiCatalogueActivate.SetActive(!this.isCatalogueDownload);
                this.m_UiCatalogueDownload.SetActive(this.isCatalogueDownload);

                if (this.isCatalogueDownload)
                {
                    if (this.m_Catalogue.Count > 0)
                    {
                        if (this.m_Catalogue[0].evolution == ENUM_AddressableEvolution.SIZE)
                        {
                            this.m_Catalogue[0].evolution = ENUM_AddressableEvolution.DOWNLOAD;

                            this.m_Catalogue[0].WhenComplete += OnCatalogueDownloadWhenComplete;

                            this.m_Catalogue[0].Play();
                        }
                        else
                        {
                            this.m_Catalogue.Remove(this.m_Catalogue[0]);
                            this.CatalogueDownload();
                        }
                    }
                    else
                    {
                        this.m_UiCatalogue.SetActive(false);
                    }
                }

            }
            else
            {
                this.m_UiCatalogue.SetActive(false);
                this.m_UiCatalogueActivate.SetActive(false);
                this.m_UiCatalogueDownload.SetActive(false);
            }
        }

        private void OnCatalogueDownloadWhenComplete()
        {
            this.m_Catalogue[0].WhenComplete -= OnCatalogueDownloadWhenComplete;

            if (!this.m_UiSlider.isActiveAndEnabled)
            {
                this.CleanProgressables();

                this.m_Catalogue.Remove(this.m_Catalogue[0]);

                this.CatalogueSize();
            }

            this.CatalogueDownload();
        }

        public void CatalogueSize()
        {
            this.m_CatalogueSize = 0;

            foreach (VRG_AddressableSerializable child in this.m_Catalogue)
            {
                this.m_CatalogueSize += child.size;
            }

            if (this.m_CatalogueSize > 0)
            {
                this.m_UiCatalogueSize.text = VRG.LongToBytes(this.m_CatalogueSize);
            }
            else
            {
                this.m_UiCatalogueSize.text = "Up to date";
            }
        }
    }
}