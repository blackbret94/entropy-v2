using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;

using Object = UnityEngine.Object;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class is the main loader it loads the VRG_DDuA and the HAK systems
    /// it also waits if there is no internet until a conection is available
    /// </summary>
    public class VRG_Loader : VRG_Base
    {
        /// <summary>
        /// It destroy itself if true after it is created once
        /// </summary>
        [Tooltip("If it will turn off after it's done.")]
        //[SerializeField]
        protected bool m_SelfTurnDestroy = true;

        [Header("FROM Remote:")]
        /// <summary>
        /// The delay wait for seconds to check for internet again
        /// </summary>
        [Range(0.0f, 5.0f)]
        [Tooltip("The delay wait for seconds to check for internet again")]
        [SerializeField] private float m_InternetPing = 1.0f;

        [Tooltip("From Remote: Load any other catalogue in case they are defined")]
        //[SerializeField]//
        private string m_Remote_InternetPing = "VRG_Loader.m_InternetPing";

        [Tooltip("How often we will pull a new address in the background")]
        [SerializeField]
        private List<string> m_CatalogueList = new List<string>();

        [Tooltip("From Remote: Load any other catalogue in case they are defined")]
        //[SerializeField]//
        private string m_Remote_CatalogueList = "VRG_Loader.m_CatalogueList";


        [Tooltip("How often we will pull a new address in the background")]
        [SerializeField]
        private List<AsyncOperationHandle<IResourceLocator>> m_CatalogueHandle = new List<AsyncOperationHandle<IResourceLocator>>();

        [Header("From: UI")]

        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Space(20)]
        /// <summary>
        /// UI component: Box to display error messages
        /// </summary>
        [Tooltip("UI component: Box to display error messages")]
        [SerializeField] private GameObject m_UiDialogueBox = null;

        /// <summary>
        /// UI component: We can't find an active internet conection
        /// </summary>
        [Tooltip("We can't find an active internet conection")]
        [SerializeField] private Text m_UiInternetError = null;

        /// <summary>
        /// UI component: Addressable message, this message displays an error with the addressables
        /// </summary>
        [Tooltip("Addressable message, this message displays an error with the addressables")]
        [SerializeField] private Text m_UiAddressableError = null;

        /// <summary>
        /// UI component: Status message, this display what is happening to the user
        /// </summary>
        [Tooltip("Status message, this display what is happening to the user")]
        [SerializeField] private Text m_UiStatusLabel = null;


        [Header("From: VRG_Addressable")]
        /// <summary>
        /// The VRG_DDuA addressable Object
        /// </summary>
        [Tooltip("The VRG_DDuA addressable Object")]
        //[SerializeField]
        private VRG_AddressableSerializable m_Addressable = new VRG_AddressableSerializable("VRG_DDuA");

        /// <summary>
        /// The Current Scene loaded
        /// </summary>
        [Tooltip("The Current Scene loaded")]
        //[SerializeField]
        private VRG_AddressableScene m_Scene = new VRG_AddressableScene("VRG_DDuA_Proxy");















        public VRG_Loader()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = true;
        }
        
        /// <summary>
        /// Singleton pattern, Instance is the variable that save the data from every class.
        /// </summary>
        public static VRG_Loader Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    // I am not the one the prophecy said ... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
        }


        ///#IGNORE
        protected override IEnumerator Do()
        {
            // init the local variables
            string sReachabilityText = "";
            bool bInternetStatus = false;

            if (Object.FindObjectsOfType<VRG_DDuA>().Length == 0)               
            {
                if
                (
                    this.m_UiDialogueBox == null
                    || this.m_UiInternetError == null
                    || this.m_UiAddressableError == null
                    || this.m_UiStatusLabel == null                    
                )
                {
                    // LOGS: we are starting the VRG_Loader
                    this.Logs
                    (
                        "The version is corrupt, some components are null",
                        "VRG_Loader->Start()",
                        ENUM_Verbose.ERROR
                    );
                }
                else
                {
                    this.m_SelfTurnDestroy = this.m_SelfTurnOff;
                    this.m_SelfTurnOff = false;

                    bool bFirst = true;

                    while (!bInternetStatus)
                    {
                        yield return null;

                        // Check if the device cannot reach the internet
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            //Not Reachable
                            sReachabilityText = "Searching for an active internet conection ...";
                        }

                        // Check if the device can reach the internet via a carrier data network
                        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        {
                            //Reachable via carrier data network.
                            sReachabilityText = "Mobile network Found";
                            bInternetStatus = true;
                        }

                        // Check if the device can reach the internet via a LAN
                        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                        {
                            //Reachable via Local Area Network;
                            sReachabilityText = "Wifi Found";
                            bInternetStatus = true;
                        }

                        // LOGS: now we have internet
                        this.Logs
                        (
                            "<b>" + sReachabilityText + "</b>",
                            "VRG_Loader->CheckInternet()",
                            ENUM_Verbose.LOGS
                        );

                        // if internet is available...
                        if (!bInternetStatus)
                        {
                            if (bFirst)
                            {
                                bFirst = false;

                                // show the dialogue box message
                                this.m_UiDialogueBox.SetActive(true);

                                // show No internet box message
                                this.m_UiInternetError.gameObject.SetActive(true);

                                // hide No internet box message
                                yield return VRG_Remote.IsValid();

                                float fInternetPing = VRG_Remote.GetFloat(this.m_Remote_InternetPing, false);

                                if (fInternetPing > 0)
                                {
                                    this.m_InternetPing = fInternetPing;
                                }
                            }

                            print("this.m_InternetPing = " + this.m_InternetPing);

                            // check every seconds until a conection is found
                            yield return new WaitForSeconds(this.m_InternetPing);
                        }
                    }

                    this.InitAddressables();
                }
            }
            else
            {
                // turn my self off
                this.gameObject.SetActive(false);

                this.Logs(this.m_Addressable.address + " already exists, will not attempt to reload it.", ENUM_Verbose.WARNING);
            }

            yield return null;
        }


        public void InitAddressables()
        {
            //print(Time.frameCount + ") ----------------------------- InitAddressables()");

            AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
            handle.Completed += CheckForCatalogUpdates;
        }

        private void CheckForCatalogUpdates(AsyncOperationHandle<IResourceLocator> obj)
        {
            //print(Time.frameCount + ") ----------------------------- CheckForCatalogUpdates()");

            AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates(false);
            checkForUpdateHandle.Completed += CheckForCatalogUpdates_Completed;
        }

        private void CheckForCatalogUpdates_Completed(AsyncOperationHandle<List<string>> obj)
        {
            /*
            print(Time.frameCount + ") ----------------------------- CheckForCatalogUpdates_Completed()");
            print(Time.frameCount + ") IsValid = " + obj.IsValid());
            print(Time.frameCount + ") Status = " + obj.Status);

            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                print(Time.frameCount + ") Result = " + obj.Result.Count + " | " + obj.Result);
            }
            //*/

            if (obj.IsValid())
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    string sLogs = string.Empty;
                    List<string> catalogsToUpdate = new List<string>();

                    if (obj.Result.Count > 0)
                    {
                        catalogsToUpdate.AddRange(obj.Result);

                        sLogs += "<hr>";
                        foreach (string child in obj.Result)
                        {
                            sLogs += " - " + child + "\n";
                        }
                        sLogs += "\n";

                        base.Logs
                        (
                            "<color=grey><i>Catalogue | Update (" + obj.Result.Count + ")" + sLogs + "</i></color>",
                            "VRG_Loader->CheckForCatalogUpdates_Completed()",
                            ENUM_Verbose.DEBUG
                        );

                        this.m_UiStatusLabel.text = "Catalogue Updated";

                        AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);
                        updateHandle.Completed += UpdateHandle_Completed;
                    }
                    else
                    {
                        StartCoroutine(this.UpdateOtherCatalog());
                    }
                }
                else
                {
                    this.Logs
                    (
                        "<color=grey><i>Catalogue | Status (" + obj.Status + ")</i></color>",
                        "VRG_Loader->CheckForCatalogUpdates_Completed()",
                        ENUM_Verbose.ERROR
                    );
                }
            }
            else
            {
                this.Logs
                (
                    "<color=grey><i>Catalogue | CheckForCatalogUpdates is invalid </i></color>",
                    "VRG_Loader->CheckForCatalogUpdates_Completed()",
                    ENUM_Verbose.ERROR
                );
            }

            Addressables.Release(obj);
        }

        private void UpdateHandle_Completed(AsyncOperationHandle<List<IResourceLocator>> obj)
        {
            //print(Time.frameCount + ") ----------------------------- UpdateHandle_Completed()");

            //print(Time.frameCount + ") IsValid = " + obj.IsValid());
            //print(Time.frameCount + ") Status = " + obj.Status);

            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                //print(Time.frameCount + ") Result = " + obj.Result.Count + " | " + obj.Result);
            }

            Addressables.Release(obj);

            StartCoroutine(this.UpdateOtherCatalog());
        }

        private IEnumerator UpdateOtherCatalog()
        {
            yield return VRG_Remote.IsValid();

            float fInternetPing = VRG_Remote.GetFloat(this.m_Remote_InternetPing, false);

            if (fInternetPing > 0)
            {
                this.m_InternetPing = fInternetPing;
            }

            string[] aCatalog = VRG_Remote.GetString(this.m_Remote_CatalogueList, false).Split('|');

            foreach(string child in aCatalog)
            {
                if (!this.m_CatalogueList.Contains(child) && child.Trim() != string.Empty)
                {
                    this.m_CatalogueList.Add(child);
                }
            }

            foreach(string child in this.m_CatalogueList)
            {
                //*
                AsyncOperationHandle<IResourceLocator> asyncCatalog = Addressables.LoadContentCatalogAsync(child);
                asyncCatalog.Completed += AsyncCatalog_Completed;
                this.m_CatalogueHandle.Add(asyncCatalog);
                /*/
                this.m_CatalogueHandle.Add(Addressables.LoadContentCatalogAsync(child));
                //*/
            }

            bool bContinue = true;

            while (bContinue)
            {
                bContinue = false;
                int i = 0;
                foreach (AsyncOperationHandle<IResourceLocator> child in this.m_CatalogueHandle)
                {
                    if (!child.IsDone)
                    {
                        /*
                        print(Time.frameCount + ") "
                            + child.IsDone + ", "
                            + child.IsValid() + ", "
                            + child.Status + ", "
                            + child.Result + " | "
                            + this.m_CatalogueList[i]);
                        //*/

                        bContinue = true;
                        break;
                    }

                    i++;
                }

                yield return null;
            }

            StartCoroutine(this.Play_VRG_DDuA());

            yield return null;
        }

        //*
        private void AsyncCatalog_Completed(AsyncOperationHandle<IResourceLocator> valueLocal)
        {
            /*
            print("-----------------------------------------");

            print("<color=green>" + Time.frameCount + ") "
                + valueLocal.IsDone + ", "
                + valueLocal.IsValid() + ", "
                + valueLocal.Status + ", "
                + valueLocal.Result + "</color>");

            foreach (object child in valueLocal.Result.Keys)
            {
                print(child);
            }
            /*
            AsyncOperationHandle<IResourceLocator> catalogsToLoad = new AsyncOperationHandle<IResourceLocator>();

            catalogsToLoad = Addressables.LoadContentCatalogAsync(string.Format("{0}catalog_{1}.json", s3Bucket, catalogID));
            catalogsToLoad.Completed += (handle) =>
            {
                AsyncOperationHandle aOperation = new AsyncOperationHandle();
                foreach (object s in handle.Result.Keys)
                {
                    aOperation = Addressables.DownloadDependenciesAsync(s, true);
                }
                aOperation.Completed += (handle2) =>
                {
                    Debug.Log(string.Format("Catalog {0} has been downloaded.", handle.Result.LocatorId));
                    completeCallback(true);
                };
            };
            */
        }
        //*/

        private IEnumerator Play_VRG_DDuA()
        {
            this.m_UiDialogueBox.SetActive(false);

            // hide No internet box message
            yield return VRG_Remote.IsValid();

            // asign the parent for the logs
            this.m_Addressable.objectInScene = VRG.GetSceneGameObject(this.gameObject);

            this.m_Addressable.WhenComplete += VRG_DDuA_Complete;

            this.m_Addressable.Play();

            yield return null;
        }

        // delegated function to load the VRG_DDuA
        private void VRG_DDuA_Complete()
        {
            // unsuscribe 
            this.m_Addressable.WhenComplete -= VRG_DDuA_Complete;

            // in case it was a success
            if (this.m_Addressable.status == ENUM_AddressableStatus.PREFAB)
            {
                // turn my self off
                this.gameObject.SetActive(false);

                if (this.m_SelfTurnDestroy)
                {
                    this.m_Scene.Play();
                }
            }
            else
            {
                // Display the No internet, to the user
                this.m_UiDialogueBox.SetActive(true);

                // hide No internet box message
                this.m_UiAddressableError.gameObject.SetActive(true);
            }
        }




















        // VRG_Logs wrapper
        protected override void Logs(string valueLocal, string sourceLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // if the status was declared 
            if (this.m_UiStatusLabel != null)
            {
                // ... then copy the message to the user
                this.m_UiStatusLabel.text = valueLocal;

                // paint red if error
                if (ENUM_VerboseLocal == ENUM_Verbose.ERROR)
                {
                    this.m_UiStatusLabel.text = "<color=red>" + this.m_UiStatusLabel.text + "</color>";
                }
            }

            // I'm your father
            base.Logs(valueLocal, sourceLocal, ENUM_VerboseLocal);
        }
    }
}







/*
Debug.Log(Time.frameCount + ") =================================");
Debug.Log(Time.frameCount + ") obj = " + obj);
Debug.Log(Time.frameCount + ") IsDone = " + obj.IsDone);
Debug.Log(Time.frameCount + ") IsValid = " + obj.IsValid());
Debug.Log(Time.frameCount + ") PercentComplete = " + obj.PercentComplete);
Debug.Log(Time.frameCount + ") Status = " + obj.Status);
Debug.Log(Time.frameCount + ") Result = " + obj.Result);
Debug.Log(Time.frameCount + ") DebugName = " + obj.DebugName);
Debug.Log(Time.frameCount + ") GetHashCode = " + obj.GetHashCode());
//Debug.Log(Time.frameCount + ") GetType = " + obj.GetType());
/*/