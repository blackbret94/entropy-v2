using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class is a serializable to be able to watch it in inspector
    /// it saves and define the Async data and status of each of the Addresables
    /// It can instantiate it and release the memory
    /// </summary>
    [System.Serializable]
    public class VRG_AddressableSerializable
    {
        [Tooltip("The level of commitment with this addressable")]
        [SerializeField] protected ENUM_AddressableEvolution m_Evolution = ENUM_AddressableEvolution.CREATE;
        /// <summary>
        /// The level of commitment with this addressable
        /// </summary>
        public ENUM_AddressableEvolution evolution
        {
            get
            {
                return this.m_Evolution;
            }
            set
            {
                this.m_Evolution = value;
            }
        }

        [Tooltip("The name of the addressble to load it should be the same as in the Addressables Group Panel")]
        [SerializeField] private string m_Address = string.Empty;
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        public string address
        {
            get
            {
                return this.m_Address.Trim();
            }
            set
            {
                if (!this.isLocked || this.m_Address == string.Empty)
                {
                    this.m_Address = value;

                    this.m_Address = this.m_Address.Trim();
                }

                this.bhelName = this.m_Address;
            }
        }

        [Tooltip("If it will just be loaded or Instantiated")]
        [SerializeField] protected bool m_IsLocked = true;
        /// <summary>
        /// If it will just be loaded or Instantiated
        /// </summary>
        public bool isLocked
        {
            get
            {
                return this.m_IsLocked;
            }
            set
            {
                this.m_IsLocked = value;
            }
        }

        [Tooltip("If it will just be loaded or Instantiated")]
        [SerializeField] protected bool m_IsOverWrite = true;
        /// <summary>
        /// If it will just be loaded or Instantiated
        /// </summary>
        public bool isOverWrite
        {
            get
            {
                return this.m_IsOverWrite;
            }
            set
            {
                this.m_IsOverWrite = value;
            }
        }

        [Header(" - Download data - ")]

        [Header("---  DEBUG: DO NOT EDIT below  ---")]

        [Tooltip("The available status, ask for this property to know if it is available to use")]
        [SerializeField]//
        //[HideInInspector]//
        protected ENUM_AddressableStatus m_Status = ENUM_AddressableStatus.NONE;
        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        public ENUM_AddressableStatus status { get { return this.m_Status; } }

        [Tooltip("The prefab of the gameObject downloaded")]
        [SerializeField]//
        //[HideInInspector]//
        protected GameObject m_Prefab = null;
        /// <summary>
        /// The instance of the gameObject downloaded
        /// </summary>
        public GameObject prefab { get { return this.m_Prefab; } }

        [Tooltip("The gameobject instanted of the gameObject downloaded")]
        [SerializeField]//
        //[HideInInspector]//
        protected GameObject m_GameObject = null;
        /// <summary>
        /// The instance of the gameObject downloaded
        /// </summary>
        public GameObject gameObject { get { return this.m_GameObject; } }

        [Tooltip("Async Data of the Handler from Size")]
        [SerializeField]//
        //[HideInInspector]//
        protected long m_Size = 0;
        /// <summary>
        /// Async Data of the Handler from Size
        /// </summary>
        public long size { get { return this.m_Size; } }

        [Tooltip("The path of the addressable to download")]
        [SerializeField]//
        //[HideInInspector]//
        protected string m_Path = string.Empty;
        /// <summary>
        /// The path of the addressable to download
        /// </summary>
        public string path { get { return this.m_Path; } }

        [SerializeField]//
        //[HideInInspector]//
        protected string m_AddressFromPath = string.Empty;
        public string addressFromPath { get { return this.m_AddressFromPath; } }








        [Header(" - Internal data - ")]
        [SerializeField]//
        //[HideInInspector]//
        private bool m_MemoryPrefab = false;

        [SerializeField]//
        //[HideInInspector]//
        private bool m_MemoryProgress = false;

        [Tooltip("If you are going to add it to the download progress bar")]
        [SerializeField]//
        //[HideInInspector]//
        private bool m_IsProgress = false;

        [Tooltip("If it is already creating something, do not attemtp to do it again")]
        [SerializeField]//
        //[HideInInspector]//
        protected bool m_IsBusy = false;

        [Tooltip("If this adress is an scene")]
        [SerializeField]//
        //[HideInInspector]//
        protected bool m_IsScene = false;






        [Header(" - BHEL data - ")]

        /// <summary>
        /// <p>Level of verbose, there are 6 level of verbose from silence up to full debug</p>
        /// <p>ENUM_Verbose.NONE = Silence, NONE is sent to the editor log window</p>
        /// <p>ENUM_Verbose.<font color=red>ERROR</font> =  When you need to show an error to the user and record a posible failure</p>
        /// <p>ENUM_Verbose.<font color=yellow>WARNING</font> = Something was cheesy, if something is wacky, strange, or unexpected</p>
        /// <p>ENUM_Verbose.<font color=green>LOGS</font> =To track some basic information, or to send logs to BHEL</p>
        /// <p>ENUM_Verbose.<font color=blue>DEBUG</font> = More information, ins and outs, flow of the game, useful for debug using asynchronous activities</p>
        /// <p>ENUM_Verbose.<font color=cyan>ALL</font> = The most verbose, it basically shows EVERYTHING!!!</p>
        /// </summary>
        [Tooltip("Level of verbose, NONE for silence")]
        [SerializeField] protected ENUM_Verbose m_Verbose = ENUM_Verbose.ALL;
        public ENUM_Verbose verbose { get { return this.m_Verbose; } set { this.m_Verbose = value; } }

        [Tooltip("The Parent game object in the scene, for the BHEL records")]
        [SerializeField]//
        //[HideInInspector]//
        protected string m_ObjectInScene = string.Empty;
        /// <summary>
        /// The Parent game object in the scene, for the BHEL records
        /// </summary>
        public string objectInScene
        {
            get
            {
                return this.m_ObjectInScene;
            }
            set
            {
                this.m_ObjectInScene = value;
            }
        }

        [SerializeField]//
        //[HideInInspector]//
        protected string m_BHELName = string.Empty;
        public string bhelName
        {
            get
            {
                return this.m_BHELName;
            }
            protected set
            {
                this.m_BHELName = string.Empty;

                string sValue = value;

                sValue = sValue.Trim();

                if (sValue != string.Empty)
                {
                    this.m_BHELName = "<color=green>" + value + "</color>";
                }

            }
        }

        [SerializeField]//
        //[HideInInspector]//
        protected string m_BHEL = string.Empty;
        protected string bhel
        {
            get
            {
                return this.m_BHEL;
            }
            set
            {
                this.m_BHEL = value;

                // invoke the events delegateds
                this.WhenBHEL?.Invoke(this.m_BHEL);
            }
        }



        [Header(" - NON_Serializable data - ")]
        [Tooltip("Async Data of the Handler from the download progress")]
        [SerializeField]//
        protected AsyncOperationHandle m_ProgressHandle = new AsyncOperationHandle();
        public AsyncOperationHandle progressHandle { get { return this.m_ProgressHandle; } }


        [Tooltip("Async Data of the Instantiate object")]
        [SerializeField]
        protected AsyncOperationHandle<GameObject> m_PrefabHandle = new AsyncOperationHandle<GameObject>();

        /// <summary>
        /// Public Event: Trigers WHEN = Bhel data is updated
        /// </summary>
        public event Action<string> WhenBHEL;

        /// <summary>
        /// Public Event: Trigers WHEN = A set of Addressables is finished and fully downloaded
        /// </summary>
        public event Action WhenComplete;

        /// <summary>
        /// Public Event: Trigers WHEN all the handlers are released
        /// </summary>
        public event Action WhenRelease;

        /// <summary>
        /// Public Event: Trigers WHEN = A set of Addressables is finished and fully downloaded
        /// </summary>
        public event Action<GameObject> WhenCreated;














        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_AddressableSerializable()
        {
            this.Init();
        }

        /// <summary>
        /// VRG_Addressable Creator, everything is true and zero but the 
        /// string nameLocal = the name that will be loaded and/or instantiated
        /// </summary>
        public VRG_AddressableSerializable(string nameLocal)
        {
            this.Init();

            // assign the new name
            this.address = nameLocal;
        }

        /// <summary>
        /// VRG_Addressable Creator, everything is true and zero but the 
        /// string nameLocal = the name that will be loaded and/or instantiated
        /// bool createLocal = the status, if false, it will not be instantiated
        /// </summary>
        public VRG_AddressableSerializable(string nameLocal, bool createLocal)
        {
            this.Init();

            // assign the new name
            this.address = nameLocal;

            // design the create status, usually set to true
            this.evolution = ENUM_AddressableEvolution.DOWNLOAD;

            if (createLocal)
            {
                this.evolution = ENUM_AddressableEvolution.CREATE;
            }
        }



        // Just reset everything to null, "", false, or default
        protected virtual void Init()
        {
            this.evolution = ENUM_AddressableEvolution.CREATE;

            this.address = string.Empty;

            this.isLocked = true;

            this.isOverWrite = true;

            this.m_IsProgress = false;

            this.m_IsScene = false;

            this.verbose = ENUM_Verbose.ALL;

            this.m_ObjectInScene = string.Empty;

            this.m_BHELName = string.Empty;

            this.m_BHEL = string.Empty;

            if (this.m_MemoryProgress)
            {
                this.m_MemoryPrefab = false;
            }

            if (this.m_MemoryPrefab)
            {
                this.m_MemoryProgress = false;
            }

            // Back to new
            this.Reset();
        }

        // Just reset everything to null, "", false, or default
        protected void Reset()
        {
            this.m_Status = ENUM_AddressableStatus.NONE;

            this.m_Prefab = null;

            this.m_Size = 0;

            this.m_Path = string.Empty;

            this.m_AddressFromPath = string.Empty;


            this.m_IsBusy = false;

            // reset the progress handle memory
            this.m_ProgressHandle = new AsyncOperationHandle();
            this.m_MemoryProgress = false;

            // release the handle memory
            this.m_PrefabHandle = new AsyncOperationHandle<GameObject>();
            this.m_MemoryPrefab = false;
        }



        public void Play() => this.Play(this.address);

        /// <summary>
        /// You will download the addressable and will instantiate it according to the m_IsCreate variable
        /// </summary>
        public void Play(string valueLocal)
        {
            if (!this.m_IsBusy)
            {
                if (valueLocal.Trim() != string.Empty)
                {
                    bool bContinue = false;

                    if (this.prefab != null)
                    {
                        if (this.address == valueLocal && this.evolution == ENUM_AddressableEvolution.CREATE)
                        {
                            bContinue = true;
                        }
                    }

                    if (bContinue)
                    {
                        this.Instantiate();
                    }
                    else
                    {
                        bContinue = !this.isLocked;

                        if (this.prefab == null)
                        {
                            bContinue = true;
                        }

                        if (this.address == string.Empty)
                        {
                            bContinue = true;
                        }

                        if (bContinue)
                        {
                            // let's start clean
                            this.Release();

                            // save if we are gonna inform the progress of this asset
                            this.address = valueLocal;

                            // we will start the download, time... unknown.
                            this.m_IsBusy = true;

                            // we will begin to size the download, update the status 
                            this.m_Status = ENUM_AddressableStatus.PATHING;

                            // search for the path
                            Addressables.LoadResourceLocationsAsync(this.address).Completed += GetPath;
                        }
                        else
                        {
                            if (this.verbose >= ENUM_Verbose.WARNING)
                            {
                                // log and inform the size of the asset
                                this.bhel = this.m_BHELName + " has Locked(true), <color=grey><i>" + valueLocal + "</i></color> tried to recreate it";
                                VRG_Bhel.Do
                                (
                                    this.bhel,
                                    "VRG_AddressableSerializable->Play()",
                                    ENUM_Verbose.WARNING,
                                    this.m_ObjectInScene
                                );
                            }

                            this.InvokeEvent_WhenComplete();
                        }
                    }
                }
                else
                {
                    if (this.verbose >= ENUM_Verbose.WARNING)
                    {
                        // log and inform the size of the asset
                        this.bhel = this.m_BHELName + " | Is trying to create an empty address";
                        VRG_Bhel.Do
                        (
                            this.bhel,
                            "VRG_AddressableSerializable->Play()",
                            ENUM_Verbose.WARNING,
                            this.m_ObjectInScene
                        );
                    }
                }
            }
            else
            {
                if (this.verbose >= ENUM_Verbose.WARNING)
                {
                    this.bhel = this.m_BHELName + " | Is busy (" + this.status + "), <color=grey><i>" + valueLocal + "</i></color> tried to recreate it";

                    // log and inform the size of the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->Play()",
                        ENUM_Verbose.WARNING,
                        this.m_ObjectInScene
                    );
                }
            }
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        protected virtual void GetPath(AsyncOperationHandle<IList<IResourceLocation>> obj)
        {
            bool bDuplicatedKey = false;
            bool bContinue = true;

            string sLogs = string.Empty;
            string sPathTemporal = string.Empty;

            List<string> sAllPaths = new List<string>();

            foreach (IResourceLocation child in obj.Result)
            {
                if (!sAllPaths.Contains(child.ToString()))
                {
                    bContinue = true;

                    if (this.evolution == ENUM_AddressableEvolution.CREATE)
                    {
                        if (this.m_IsScene)
                        {
                            if (!child.ToString().Contains(".unity"))
                            {
                                //Debug.LogError("chale si entre = " + this.address);
                                bContinue = false;
                            }
                        }
                        else
                        {
                            if (child.ToString().Contains(".unity"))
                            {
                                //Debug.LogError("chale si entre = " + this.address);
                                bContinue = false;
                            }
                        }
                    }

                    if (bContinue)
                    {
                        sLogs += "- " + child.ToString() + "\n";

                        sAllPaths.Add(child.ToString());

                        if (sPathTemporal == string.Empty)
                        {
                            sPathTemporal = child.ToString();
                        }
                        else
                        {
                            bDuplicatedKey = true;
                        }
                    }
                }
            }

            if (bDuplicatedKey && sAllPaths.Count > 1)
            {
                // we will begin to size the download, update the status 
                this.m_Status = ENUM_AddressableStatus.MORE_THAN_ONE_PATH;

                if (this.verbose >= ENUM_Verbose.WARNING)
                {
                    this.bhel = "<hr>" + this.m_BHELName + " | <b>has " + sAllPaths.Count + "</b> possible paths, check your Addressable Group<hr>" + sLogs + "\n";

                    // log and inform the size of the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->GetPath()",
                        ENUM_Verbose.WARNING,
                        this.m_ObjectInScene
                    );
                }
            }

            // if the path is found
            if (sPathTemporal != string.Empty)
            {
                // save the path
                this.m_Path = sPathTemporal;

                // we will begin to size the download, update the status 
                this.m_Status = ENUM_AddressableStatus.SIZING;

                this.m_AddressFromPath = this.m_Path.Split('/')[this.m_Path.Split('/').Length - 1].Split('.')[0];

                if (this.address != this.m_AddressFromPath)
                {
                    this.bhelName = this.m_AddressFromPath;
                }

                // get the download size
                Addressables.GetDownloadSizeAsync(this.address).Completed += GetSize;
            }

            // if not
            else
            {
                this.m_Path = string.Empty;
                // we will begin to size the download, update the status 
                this.m_Status = ENUM_AddressableStatus.PATH_NOT_FOUND;

                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                    this.bhel = this.m_BHELName + " | <b>Path not FOUND</b>, check your Addressable Group";

                    // log and inform the size of the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->GetPath()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
                }

                this.InvokeEvent_WhenComplete();
            }

            // release the handle
            Addressables.Release(obj);
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        protected virtual void GetSize(AsyncOperationHandle<long> obj)
        {
            // if the operation was valid
            if (obj.IsValid())
            {
                // ... and it was a success
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    // we will begin to size the download, update the status 
                    this.m_Status = ENUM_AddressableStatus.SIZED;

                    // save the size result
                    this.m_Size = obj.Result;

                    if (this.evolution != ENUM_AddressableEvolution.SIZE)
                    {
                        // we will begin to download, update the status 
                        this.m_Status = ENUM_AddressableStatus.DOWNLOADING;

                        // If the download size is greater than 0, download all the dependencies.
                        if (this.m_Size > 0)
                        {
                            if (this.verbose >= ENUM_Verbose.LOGS && !(this.address == VRG_DDuA.m_SceneProxy && this.m_IsScene))
                            {
                                this.bhel = this.m_BHELName + " | <b>DOWNLOADING: </b> " + this.m_Size + " bytes";
                                // log and inform the size of the asset
                                VRG_Bhel.Do
                                (
                                    this.m_BHEL,
                                    "VRG_AddressableSerializable->GetSize()",
                                    ENUM_Verbose.LOGS,
                                    this.m_ObjectInScene
                                );
                            }
                        }
                        else
                        {
                            if (this.verbose >= ENUM_Verbose.DEBUG && this.address == this.m_AddressFromPath)
                            {
                                this.bhel = this.m_BHELName + " | <b>FROM CACHE</b>";
                                // log and inform the size of the asset
                                VRG_Bhel.Do
                                (
                                    this.m_BHEL,
                                    "VRG_AddressableSerializable->GetSize()",
                                    ENUM_Verbose.DEBUG,
                                    this.m_ObjectInScene
                                );
                            }
                        }

                        // now try to download
                        this.m_ProgressHandle = Addressables.DownloadDependenciesAsync(this.address);
                        this.m_MemoryProgress = true;

                        // when it is over, log it and release this handler
                        this.m_ProgressHandle.Completed += GetDownload;

                        if (this.size > 0)
                        {
                            this.m_IsProgress = VRG_DDuA.AddProgress(this);
                        }
                    }
                    else
                    {
                        // invoke the events delegateds
                        this.InvokeEvent_WhenComplete();
                    }
                }

                // ... it wasn't a success, something goes wrong, log it
                else
                {
                    if (this.verbose >= ENUM_Verbose.ERROR)
                    {
                        this.bhel = this.m_BHELName + " | It's not a valid Address, check your CDN (" + obj.Status + ")";
                        // log and inform i couldn't load the asset
                        VRG_Bhel.Do
                        (
                            this.m_BHEL,
                            "VRG_AddressableSerializable->GetSize()",
                            ENUM_Verbose.ERROR,
                            this.m_ObjectInScene
                        );
                    }

                    // invoke the events delegateds
                    this.InvokeEvent_WhenComplete();
                }
            }

            // ... if is not valid, something goes wrong
            else
            {
                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                    this.bhel = this.m_BHELName + " | " + " <b>Size: NOT valid</b>";
                    // log and inform i couldn't load the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->GetSize()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
                }

                // invoke the events delegateds
                this.InvokeEvent_WhenComplete();
            }

            Addressables.Release(obj);
        }

      
        // delegated function to load the Addressable
        public virtual float CurrentDownloadPercentage()
        {
            float fReturn = 0.0f;

            if (this.m_ProgressHandle.IsValid())
            {
                fReturn = this.progressHandle.PercentComplete;
            }

            return fReturn;
        }

        // delegated function to load the Addressable
        public virtual string CurrentDownload()
        {
            string sReturn = VRG.LongToBytes(((long)this.CurrentDownloadPercentage() * this.size), false) + " / " + VRG.LongToBytes(this.size);

            if (this.m_ProgressHandle.IsValid())
            {
                sReturn += " (" + (this.progressHandle.PercentComplete * 100).ToString("F1") + "% )";
            }

            return sReturn;
        }


        // delegated function to load the Addressable
        protected virtual void GetDownload(AsyncOperationHandle obj)
        {
            bool bInvokeEvent_WhenComplete = true;

            if (this.m_ProgressHandle.IsValid())
            {
                if (this.m_ProgressHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (this.m_Size > 0)
                    {
                        if (this.verbose >= ENUM_Verbose.DEBUG)
                        {
                            this.bhel = this.m_BHELName + " | <b>DOWNLOADED: </b>\n" + this.path;
                            // log and inform i am done
                            VRG_Bhel.Do
                            (
                                this.m_BHEL,
                                "VRG_AddressableSerializable->GetDownload()",
                                ENUM_Verbose.DEBUG,
                                this.m_ObjectInScene
                            );
                        }
                    }

                    // It is already downloaded, update the status 
                    this.m_Status = ENUM_AddressableStatus.DOWNLOADED;

                    // if needs to be created
                    if (this.evolution == ENUM_AddressableEvolution.READY || this.evolution == ENUM_AddressableEvolution.CREATE)
                    {
                        if (this.m_Path.Contains(".prefab"))
                        {
                            // It is already downloaded, update the status 
                            this.m_Status = ENUM_AddressableStatus.LOADING_MEMORY;

                            // Instantiate the core class and on completed go to delegated
                            this.m_PrefabHandle = Addressables.LoadAssetAsync<GameObject>(this.address);
                            this.m_MemoryPrefab = true;

                            this.m_PrefabHandle.Completed += GetPrefab;

                            bInvokeEvent_WhenComplete = false;
                        }
                        else
                        {
                            if (this.verbose >= ENUM_Verbose.WARNING)
                            {
                                string[] asType = this.m_Path.ToString().Split('.');

                                string sType = "N/A";

                                if (asType.Length > 0)
                                {
                                    sType = asType[asType.Length - 1];

                                    if (sType == "unity")
                                    {
                                        sType = "Scene";
                                    }
                                }

                                this.bhel = this.m_BHELName + " | <color=black>Type: <b><i>" + sType + "</i></b></color> - You can't instantiate a non-prefab";
                                // log and inform i am done
                                VRG_Bhel.Do
                                (
                                    this.m_BHEL,
                                    "VRG_AddressableSerializable->GetDownload()",
                                    ENUM_Verbose.WARNING,
                                    this.m_ObjectInScene
                                );
                            }
                        }
                    }
                }

                // if failed
                else
                {
                    // It is already downloaded, update the status 
                    this.m_Status = ENUM_AddressableStatus.FAILED;

                    if (this.verbose >= ENUM_Verbose.ERROR)
                    {
                        this.bhel = this.m_BHELName + " | " + "<b> DIDN'T</b> download = " + this.m_ProgressHandle.Status;
                        // log and inform i couldn't load the asset
                        VRG_Bhel.Do
                        (
                            this.m_BHEL,
                            "VRG_AddressableSerializable->GetDownload()",
                            ENUM_Verbose.ERROR,
                            this.m_ObjectInScene
                        );
                    }
                }
            }

            else
            {
                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                    this.bhel = this.m_BHELName + " | " + " The Progress Handle <b>WAS Destroyed</b> " + this.m_Status;
                    // log and inform i couldn't load the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->GetDownload()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
                }

                // It is already downloaded, update the status 
                this.m_Status = ENUM_AddressableStatus.FAILED;
            }

            if (bInvokeEvent_WhenComplete)
            {
                // invoke the events delegateds
                this.InvokeEvent_WhenComplete();
            }
        }

        protected virtual void GetPrefab(AsyncOperationHandle<GameObject> obj)
        {
            bool bFail = false;
            string sLogs = string.Empty;

            // if it was created successfully
            if (this.m_PrefabHandle.IsValid())
            {
                // if the addressable was found and is a success
                if (this.m_PrefabHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.m_Status = ENUM_AddressableStatus.PREFAB;

                    // Save the game object by the result
                    this.m_Prefab = this.m_PrefabHandle.Result;

                    if (this.evolution == ENUM_AddressableEvolution.CREATE)
                    {
                        this.Instantiate();
                    }
                }
                else
                {
                    // this failed couldn't create it
                    bFail = true;
                    sLogs = this.m_BHELName + " | <b> Prefab: " + this.m_PrefabHandle.Status + "</b>";
                }
            }

            // ... if is not valid, something goes wrong
            else
            {
                bFail = true;
                sLogs = this.m_BHELName + " | " + " <b> Prefab: NOT valid</b>";
            }

            if (bFail)
            {
                // update it failed
                this.m_Status = ENUM_AddressableStatus.FAILED;

                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                    this.bhel = sLogs;
                    // log and inform i could load the asset
                    VRG_Bhel.Do
                    (
                        this.m_BHEL,
                        "VRG_AddressableSerializable->Completed()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
                }
            }

            // invoke the events delegateds
            this.InvokeEvent_WhenComplete();
        }



        public virtual void Instantiate()
        {
            if (this.m_Prefab == null)
            {
                this.Play();
            }
            else
            {
                if (this.isOverWrite)
                {
                    this.Destroy();
                }

                GameObject goTemp = UnityEngine.Object.Instantiate(this.m_Prefab);

                this.m_GameObject = goTemp;

                // delete the clone extention from the name
                this.m_GameObject.name = this.m_GameObject.name.ToString().Replace("(Clone)", "");

                if (this.verbose >= ENUM_Verbose.ALL)
                {
                    // log and inform the size of the asset
                    this.bhel = this.m_BHELName + " | <b>INSTANTIATED</b>";
                    VRG_Bhel.Do
                    (
                        this.bhel,
                        "VRG_AddressableSerializable->Instantiate()",
                        ENUM_Verbose.ALL,
                        this.m_ObjectInScene
                    );
                }

                this.InvokeEvent_WhenCreated(this.m_GameObject);                
            }
        }

        public virtual void Destroy()
        {
            if (this.m_GameObject != null)
            {
                UnityEngine.Object.Destroy(this.m_GameObject);

                this.m_GameObject = null;
            }
        }

        public void ProgressDone()
        {
            // if it is used?
            if (this.m_ProgressHandle.IsValid())
            {
                try
                {
                    // release the handler
                    Addressables.Release(this.m_ProgressHandle);
                }
                catch (Exception e)
                {
                    if (this.verbose >= ENUM_Verbose.ERROR)
                    {
                        this.bhel = this.m_BHELName + " | " + "(this.m_ProgressHandle) | " + this.status + ") = " + e.ToString();
                        // log and inform i couldn't release the handler
                        VRG_Bhel.Do
                        (
                            this.m_BHEL,
                            "VRG_AddressableSerializable->Destroy()",
                            ENUM_Verbose.ERROR,
                            this.m_ObjectInScene
                        );
                    }
                }
            }

            // reset it
            this.m_ProgressHandle = new AsyncOperationHandle();
            this.m_MemoryProgress = false;
        }

        // delegated function to load the Addressable
        public void Release() => this.Release(true);
        public void Release(bool valueLocal)
        {
            // release the memory
           this.ProgressDone();

            // if it was created successfully
            if (this.m_PrefabHandle.IsValid())
            {
                try
                {
                    // release the handler
                    Addressables.Release(this.m_PrefabHandle);
                }
                catch (Exception e)
                {
                    if (this.verbose >= ENUM_Verbose.ERROR)
                    {
                        this.bhel = this.m_BHELName + " | " + "(this.m_PrefabHandle) = " + e.ToString();
                        // log and inform the error
                        VRG_Bhel.Do
                        (
                            this.m_BHEL,
                            "VRG_AddressableSerializable->Destroy()",
                            ENUM_Verbose.ERROR,
                            this.m_ObjectInScene
                        );
                    }
                }
                
                finally
                {
                    if ((!(this.address == VRG_DDuA.m_SceneProxy && this.m_IsScene)) && valueLocal && this.verbose >= ENUM_Verbose.ALL)
                    {
                        this.bhel = this.m_BHELName + " | " + "<b>RELEASE</b> memory";
                        // log and inform i could load the asset
                        VRG_Bhel.Do
                        (
                            this.m_BHEL,
                            "VRG_AddressableSerializable->Release()",
                            ENUM_Verbose.ALL,
                            this.m_ObjectInScene
                        );
                    }
                }
            }

            this.Reset();
        }





        // the Addressable has a size assigned, try to get it from Remote or Cache
        protected virtual void InvokeEvent_WhenComplete()
        {
            this.m_IsBusy = false;

            if (!this.m_IsProgress)
            {
                this.ProgressDone();
            }

            // invoke the events delegateds
            this.WhenComplete?.Invoke();
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        protected virtual void InvokeEvent_WhenRelease()
        {
            // invoke the events delegateds
            this.WhenRelease?.Invoke();
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        protected virtual void InvokeEvent_WhenCreated(GameObject valueLocal)
        {
            this.m_IsBusy = false;

            // invoke the events delegateds
            this.WhenCreated?.Invoke(valueLocal);
        }

        

    }
}