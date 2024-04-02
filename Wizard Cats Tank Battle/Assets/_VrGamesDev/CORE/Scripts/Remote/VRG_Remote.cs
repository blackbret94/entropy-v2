using System.Collections;
using System.Collections.Generic;


#if REMOTE_CONFIG_INSTALLED
using Unity.RemoteConfig;
#endif

using UnityEngine;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// This class control the remote settings, Local or from the unity cloud server
    /// You can override the remote data if you use the local arrays
    ///
    /// It uses the <a href="https://unity.com/remote-config">Unity Remote Config</a> module
    ///
    /// You need to set the REMOTE_CONFIG_INSTALLED <a href="https://docs.unity3d.com/Manual/PlatformDependentCompilation.html">player defined symbol </a>
    ///
    /// Add it from the Menu <em>Tools->Vr Games Dev->VRG_Remote->REMOTE_CONFIG_INSTALLED: Add</em>
    /// </summary>
    public class VRG_Remote : VRG_Base
    {
        [Header("From: Remote")]

        [Tooltip("Force in editor mode to pick the server data or the local data")]
        [SerializeField] private ENUM_VRG_Remote m_Status = ENUM_VRG_Remote.NONE;
        public static ENUM_VRG_Remote status
        {
            get
            {
                if (Instance == null)
                {
                    return ENUM_VRG_Remote.NONE;
                }
                else
                {
                    return Instance.m_Status;
                }
            }
        }

        /// <summary>
        /// Force in editor mode to pick the server data or the local data
        /// </summary>
        [Tooltip("Force in editor mode to pick the server data or the local data")]
        [SerializeField] private bool m_UseRemote = true;


        [Tooltip("The Id of the data this VRG_Remote contains")]
        [SerializeField] private string m_Id = "All";
        /// <summary>
        /// The Id of the data this VRG_Remote contains
        /// </summary>
        public string id
        {
            get
            {
                return this.m_Id;
            }
            set
            {
                this.m_Id = value;
                this.name = "VRG_Remote - (" + value + ")";
            }
        }

        /// <summary>
        /// LOCAL: the Bool array that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the Bool data that override the remote settings")]
        [SerializeField]
        private List<VRG_RemoteBool> m_Bools = new List<VRG_RemoteBool>();
        //[SerializeField]//
        private List<VRG_RemoteBool> m_BoolsLocal = new List<VRG_RemoteBool>();
        //[SerializeField]//
        private List<VRG_RemoteBool> m_BoolsRemote = new List<VRG_RemoteBool>();

        /// <summary>
        /// LOCAL: the Int data that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the Int data that override the remote settings")]
        [SerializeField] private List<VRG_RemoteInt> m_Ints = new List<VRG_RemoteInt>();
        //[SerializeField]//
        private List<VRG_RemoteInt> m_IntsLocal = new List<VRG_RemoteInt>();
        //[SerializeField]//
        private List<VRG_RemoteInt> m_IntsRemote = new List<VRG_RemoteInt>();

        /// <summary>
        /// LOCAL: the Float data that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the Float data that override the remote settings")]
        [SerializeField] private List<VRG_RemoteFloat> m_Floats = new List<VRG_RemoteFloat>();
        //[SerializeField]//
        private List<VRG_RemoteFloat> m_FloatsLocal = new List<VRG_RemoteFloat>();
        //[SerializeField]//
        private List<VRG_RemoteFloat> m_FloatsRemote = new List<VRG_RemoteFloat>();

        /// <summary>
        /// LOCAL: the String data that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the String data that override the remote settings")]
        [SerializeField] private List<VRG_RemoteString> m_Strings = new List<VRG_RemoteString>();
        //[SerializeField]//
        private List<VRG_RemoteString> m_StringsLocal = new List<VRG_RemoteString>();
        //[SerializeField]//
        private List<VRG_RemoteString> m_StringsRemote = new List<VRG_RemoteString>();




        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        //[SerializeField]//
        private bool m_IsCached = false;
        /// <summary>
        /// You can ask for this variable to know if the information was cached
        /// </summary>
        public static bool isCached
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.m_IsCached;
                }
                else
                {
                    return false;
                }
            }
        }

        private int m_Updating = 0;
        public static int updating
        {
            get
            {
                if (Instance == null)
                {
                    return -1;
                }
                else
                {
                    return Instance.m_Updating;
                }
            }
            set
            {
                if (value > 0)
                {
                    Instance.m_Updating++;
                }

                if (value < 0)
                {
                    Instance.m_Updating--;
                }
            }
        }


        /// <summary>
        /// Singleton pattern, Instance is the unique variable in the scene from this class
        /// </summary>
        public static VRG_Remote Instance; private void Awake()
        {
            bool bForce = true;

#if UNITY_EDITOR_OSX
            bForce = false;
#endif

#if UNITY_EDITOR_WIN
            bForce = false;
#endif

            if (bForce)
            {
                this.m_UseRemote = true;
            }

            // I will check if I am the first singletong
            if (VRG_Remote.Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will live forever
                DontDestroyOnLoad(this);

                this.m_Status = ENUM_VRG_Remote.INIT;

#if REMOTE_CONFIG_INSTALLED
                // Add a listener to apply settings when successfully retrieved: 
                ConfigManager.FetchCompleted += ApplyRemoteSettings;

                // Fetch configuration setting from the remote service: 
                ConfigManager.FetchConfigs<VRG_Remote_UserAttributes, VRG_Remote_AppAttributes>(new VRG_Remote_UserAttributes(), new VRG_Remote_AppAttributes());
#else
                StartCoroutine(this.ReadRemoteServer());
#endif
            }
            else
            {
                StartCoroutine(this.Awake_IEnumerator());
            }
        }

        private IEnumerator Awake_IEnumerator()
        {
            bool bUpdated = false;
            this.m_Status = ENUM_VRG_Remote.WAITING;

            if (VRG_Remote.status != ENUM_VRG_Remote.READY)
            {
                bUpdated = true;
                VRG_Remote.updating = 1;

                while (VRG_Remote.status != ENUM_VRG_Remote.UPDATING)
                {
                    yield return null;
                }
            }

            yield return null;

            string sLogs = string.Empty;
            string sLogsBlock = string.Empty;

            string sLogsTemp = sLogs;
            bool sLoggable = false;

            sLogsBlock = string.Empty;

            // update all my data into the main singleton before i die
            foreach (VRG_RemoteBool child in this.m_Bools)
            {
                sLogsTemp = VRG_Remote.Instance.AddBool(child.name, child.value);
                if (sLogsTemp != string.Empty)
                {
                    sLoggable = true;
                    sLogsBlock += sLogsTemp;
                }
            }
            if (sLoggable)
            {
                if (sLogs != string.Empty)
                {
                    sLogs += "\n";
                }
                sLogs += sLogsBlock;
            }

            sLoggable = false;
            sLogsBlock = string.Empty;
            foreach (VRG_RemoteInt child in this.m_Ints)
            {
                sLogsTemp = VRG_Remote.Instance.AddInt(child.name, child.value);
                if (sLogsTemp != string.Empty)
                {
                    sLoggable = true;
                    sLogsBlock += sLogsTemp;
                }
            }
            if (sLoggable)
            {
                if (sLogs != string.Empty)
                {
                    sLogs += "\n";
                }
                sLogs += sLogsBlock;
            }

            sLoggable = false;
            sLogsBlock = string.Empty;
            foreach (VRG_RemoteFloat child in this.m_Floats)
            {
                sLogsTemp = VRG_Remote.Instance.AddFloat(child.name, child.value);
                if (sLogsTemp != string.Empty)
                {
                    sLoggable = true;
                    sLogsBlock += sLogsTemp;
                }
            }
            if (sLoggable)
            {
                if (sLogs != string.Empty)
                {
                    sLogs += "\n";
                }
                sLogs += sLogsBlock;
            }

            sLoggable = false;
            sLogsBlock = string.Empty;
            foreach (VRG_RemoteString child in this.m_Strings)
            {
                sLogsTemp = VRG_Remote.Instance.AddString(child.name, child.value);
                if (sLogsTemp != string.Empty)
                {
                    sLoggable = true;
                    sLogsBlock += sLogsTemp;
                }
            }
            if (sLoggable)
            {
                if (sLogs != string.Empty)
                {
                    sLogs += "\n";
                }
                sLogs += sLogsBlock;
            }

            if (sLogs != string.Empty)
            {
                // log the values updated
                this.Logs
                (
                    "<hr><center><i><b>VRG_Remote update: </b>" + this.id + "</i></center><hr>" + sLogs + "\n\n",
                    "VRG_Remote->Awake_IEnumerator()",
                    ENUM_Verbose.DEBUG
                );
            }
            else
            {
                // log the values updated
                this.Logs
                (
                    "<color=grey>VRG_Remote | Up to date with ID = " + this.id + "</color>",
                    "VRG_Remote->Awake_IEnumerator()",
                    ENUM_Verbose.DEBUG
                );
            }

            if (this.m_UseRemote)
            {
                VRG_Remote.Instance.m_UseRemote = this.m_UseRemote;
            }

            if (bUpdated)
            {
                VRG_Remote.updating = -1;
            }

            // I am not the one the prophecy said ... I will walk to the eternal darkness
            Destroy(this.gameObject);

            yield return null;
        }





        protected override IEnumerator Do()
        {
            // go to next frame
            yield return null;
        }

        public static IEnumerator IsValid()
        {
            yield return VRG_Remote.IsValid(false);
        }
        /// <summary>
        /// Ask for this ienumerator if the VRG_Remote is ready
        /// </summary>
        /// <returns>Null when the VRG_Remote is ready to be queried</returns>
        public static IEnumerator IsValid(bool valueLocal)
        {
            // Let's assume everything is configured properly
            bool bContinue = true;

            // count 1 second to find the VRG_Campaign
            int i = 0;

            if (VRG_Remote.Instance == null)
            {
                if (GameObject.FindObjectOfType<VRG_Remote>() == null)
                {
                    bContinue = false;
                }
            }

            // If after 30 frames you can't get the VRG_Remote object it's probable not configured
            while (VRG_Remote.Instance == null && bContinue)
            {
                if (i > VRG.IsReady)
                {
                    bContinue = false;
                }
                i++;

                yield return null;
            }

            if (bContinue)
            {
                // wait until Init is done
                while (Instance.m_Status != ENUM_VRG_Remote.READY)
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
                        "Please be sure a VRG_Remote prefab is added to the scene",
                        "VRG_Remote->IsValid()",
                        ENUM_Verbose.WARNING,
                        "Static Method"
                    );
                }
            }

            // go to next frame
            yield return null;
        }


        private void OnValidate()
        {
            if (this.m_Status == ENUM_VRG_Remote.READY || this.m_Status == ENUM_VRG_Remote.UPDATING)
            {
                if (this.m_UseRemote)
                {
                    this.m_Bools = new List<VRG_RemoteBool>(this.m_BoolsRemote);
                    this.m_Ints = new List<VRG_RemoteInt>(this.m_IntsRemote);
                    this.m_Floats = new List<VRG_RemoteFloat>(this.m_FloatsRemote);
                    this.m_Strings = new List<VRG_RemoteString>(this.m_StringsRemote);
                }
                else
                {
                    this.m_Bools = new List<VRG_RemoteBool>(this.m_BoolsLocal);
                    this.m_Ints = new List<VRG_RemoteInt>(this.m_IntsLocal);
                    this.m_Floats = new List<VRG_RemoteFloat>(this.m_FloatsLocal);
                    this.m_Strings = new List<VRG_RemoteString>(this.m_StringsLocal);
                }
            }
        }

#if REMOTE_CONFIG_INSTALLED
        /// <summary>
        /// Delegated virtual function to wait and check for the remote settings and data
        /// </summary>
        /// <param name="configResponse">A regular <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/api/Unity.RemoteConfig.ConfigResponse.html">configResponse</a></param>
        protected virtual void ApplyRemoteSettings(ConfigResponse valueLocal)
        {
            // Remove a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted -= ApplyRemoteSettings;

            // LOGS: In case of debug log detail
            this.Logs
            (
                "Remote Config Data: <b>" + valueLocal.requestOrigin.ToString() + "</b>",
                "VRG_Remote->ApplyRemoteSettings()",
                ENUM_Verbose.ALL
            );

            // Conditionally update settings, depending on the response's origin:
            switch (valueLocal.requestOrigin)
            {
                case ConfigOrigin.Default:
                    break;

                case ConfigOrigin.Cached:
                    this.m_IsCached = true;

                    if (this.m_Status != ENUM_VRG_Remote.READY)
                    {
                        StartCoroutine(this.ReadRemoteServer());
                    }
                    break;

                case ConfigOrigin.Remote:
                    StartCoroutine(this.ReadRemoteServer());
                    break;
            }
        }
#endif

        private IEnumerator ReadRemoteServer()
        {
            string sLogs = string.Empty;
            string sLogsBool = string.Empty;
            string sLogsInt = string.Empty;
            string sLogsFloat = string.Empty;
            string sLogsString = string.Empty;


            foreach (VRG_RemoteBool child in this.m_Bools)
            {
                this.m_BoolsLocal.Add(new VRG_RemoteBool(child.name, child.value));
                this.m_BoolsRemote.Add(new VRG_RemoteBool(child.name, child.value));
            }
            foreach (VRG_RemoteInt child in this.m_Ints)
            {
                this.m_IntsLocal.Add(new VRG_RemoteInt(child.name, child.value));
                this.m_IntsRemote.Add(new VRG_RemoteInt(child.name, child.value));
            }
            foreach (VRG_RemoteFloat child in this.m_Floats)
            {
                this.m_FloatsLocal.Add(new VRG_RemoteFloat(child.name, child.value));
                this.m_FloatsRemote.Add(new VRG_RemoteFloat(child.name, child.value));
            }
            foreach (VRG_RemoteString child in this.m_Strings)
            {
                this.m_StringsLocal.Add(new VRG_RemoteString(child.name, child.value));
                this.m_StringsRemote.Add(new VRG_RemoteString(child.name, child.value));
            }

            sLogs += "<b>From Local: </b>\n";

            foreach (VRG_RemoteBool child in this.m_Bools)
            {
                sLogs += "\n- BOOL | <b>" + child.name + "</b> = " + child.value;
            }
            if (this.m_Bools.Count > 0)
            {
                sLogs += "\n";
            }

            foreach (VRG_RemoteInt child in this.m_Ints)
            {
                sLogs += "\n- INT | <b>" + child.name + "</b> = " + child.value;
            }
            if (this.m_Ints.Count > 0)
            {
                sLogs += "\n";
            }

            foreach (VRG_RemoteFloat child in this.m_Floats)
            {
                sLogs += "\n- FLOAT | <b>" + child.name + "</b> = " + child.value;
            }
            if (this.m_Floats.Count > 0)
            {
                sLogs += "\n";
            }

            foreach (VRG_RemoteString child in this.m_Strings)
            {
                sLogs += "\n- STRING | <b>" + child.name + "</b> = " + child.value;
            }

            sLogs += "<hr><b>From Remote: </b>\n";



#if REMOTE_CONFIG_INSTALLED
            int i = 0;

            // get all the keys availables
            string[] getKeys = ConfigManager.appConfig.GetKeys();

            // ... and check them all
            foreach (string child in getKeys)
            {
                // if it does exist in the arrays
                i = this.m_BoolsRemote.FindIndex((x) => x.name == child);

                // just if it exist and is new
                if (i >= 0)
                {
                    // assign the  value from the remote
                    this.m_BoolsRemote[i].value = ConfigManager.appConfig.GetBool(child);

                    // ... and inform the logs
                    sLogsBool += "\n- BOOL | <b>" + child + "</b> = " + this.m_BoolsRemote[i].value;
                }

                // if it does exist in the arrays
                i = this.m_IntsRemote.FindIndex((x) => x.name == child);
                // just if it exist and is new
                if (i >= 0)
                {
                    // assign the  value from the remote
                    this.m_IntsRemote[i].value = ConfigManager.appConfig.GetInt(child);

                    // ... and inform the logs
                    sLogsInt += "\n- INT | <b>" + child + "</b> = " + this.m_IntsRemote[i].value;
                }

                // if it does exist in the arrays
                i = this.m_FloatsRemote.FindIndex((x) => x.name == child);
                // just if it exist and is new
                if (i >= 0)
                {
                    // assign the  value from the remote
                    this.m_FloatsRemote[i].value = ConfigManager.appConfig.GetFloat(child);

                    // ... and inform the logs
                    sLogsFloat += "\n- FLOAT | <b>" + child + "</b> = " + this.m_FloatsRemote[i].value;
                }

                // if it does exist in the arrays
                i = this.m_StringsRemote.FindIndex((x) => x.name == child);
                // just if it exist and is new
                if (i >= 0)
                {
                    // assign the  value from the remote
                    this.m_StringsRemote[i].value = ConfigManager.appConfig.GetString(child);

                    // ... and inform the logs
                    sLogsString += "\n- STRING | <b>" + child + "</b> = " + this.m_StringsRemote[i].value;
                }
            }
#endif

            if (sLogs != string.Empty)
            {
                sLogs = "<hr><center><i><b>VRG_Remote Setup: </b> (" + this.id + ") </i></center><hr>"
                + sLogs;

                bool bEmpty = true;
                if (sLogsBool != string.Empty)
                {
                    bEmpty = false;
                    sLogs += sLogsBool + "\n";
                }
                if (sLogsInt != string.Empty)
                {
                    bEmpty = false;
                    sLogs += sLogsInt + "\n";
                }
                if (sLogsFloat != string.Empty)
                {
                    bEmpty = false;
                    sLogs += sLogsFloat + "\n";
                }
                if (sLogsString != string.Empty)
                {
                    bEmpty = false;
                    sLogs += sLogsString + "\n";
                }

                if (bEmpty)
                {
                    sLogs += "\n<color=red> - N/A</color>";
                }

                // log the values updated
                this.Logs
                (
                    sLogs + "\n",
                    "VRG_Remote->ReadRemoteServer()",
                    ENUM_Verbose.DEBUG
                );
            }

            // I am the one, i will take it All
            this.id = "All";

            this.m_Status = ENUM_VRG_Remote.UPDATING;

            while (this.m_Updating > 0)
            {
                yield return null;
            }

            this.m_Status = ENUM_VRG_Remote.READY;

            this.OnValidate();
        }





        public static bool GetBool(string valueLocal)
        {
            return GetBool(valueLocal, true);
        }
        /// <summary>
        /// Returns the value of the bool searched, remote or local
        /// </summary>
        /// <param name="valueLocal">The name of the string to search</param>
        /// <returns>The bool value from the string searched</returns>
        public static bool GetBool(string valueLocal, bool bhelLocal)
        {
            // by default is false
            bool bRegresa = false;

            if (Instance != null)
            {

                // The logs detail verbose
                ENUM_Verbose enumVerbose = ENUM_Verbose.WARNING;

                if (valueLocal.Trim() != string.Empty)
                {
                    string sLogs = "<b><color=black>" + valueLocal + "</color></b> | ";

                    // if it does exist in the arrays
                    int i = Instance.m_Bools.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            bRegresa = ConfigManager.appConfig.GetBool(valueLocal);

                            Instance.AddBool(valueLocal, bRegresa);

                            sLogs += "Exist in the Server Remote config = <b><i>" + bRegresa + "</i></b>";
                            enumVerbose = ENUM_Verbose.ALL;
                        }
                        else
#endif
                        {
                            sLogs += "Key Doesn't exist";
                        }

                        if (bhelLocal)
                        {
                            Instance.Logs
                            (
                                sLogs + " | <b>Recomendation:</b> Add a key before using it.",
                                "VRG_Remote->GetBool()",
                                enumVerbose
                            );
                        }
                    }
                    else
                    {
                        bRegresa = Instance.m_Bools[i].value;
                    }
                }
                else
                {
                    if (bhelLocal)
                    {
                        Instance.Logs
                        (
                            "<color=black>BOOL | </color> You tried to retreive an empty Key",
                            "VRG_Remote->GetBool()",
                            enumVerbose
                        );
                    }
                }
            }

            // return the value filled
            return bRegresa;
        }

        public static int GetInt(string valueLocal)
        {
            return GetInt(valueLocal, true);
        }
        /// <summary>
        /// Returns the value of the bool searched, remote or local
        /// </summary>
        /// <param name="valueLocal">The name of the int to search </param>
        /// <returns>The int value from the string searched</returns>
        public static int GetInt(string valueLocal, bool bhelLocal)
        {
            // by default is false
            int iRegresa = 0;

            if (Instance != null)
            {
                // The logs detail verbose
                ENUM_Verbose enumVerbose = ENUM_Verbose.WARNING;

                if (valueLocal.Trim() != string.Empty)
                {
                    string sLogs = "<b><color=black>" + valueLocal + "</color></b> | ";

                    // if it does exist in the arrays
                    int i = Instance.m_Ints.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            iRegresa = ConfigManager.appConfig.GetInt(valueLocal);

                            Instance.AddInt(valueLocal, iRegresa);

                            sLogs += "Exist in the Server Remote config = <b><i>" + iRegresa + "</i></b>";
                            enumVerbose = ENUM_Verbose.ALL;
                        }
                        else
#endif
                        {
                            sLogs += "Key Doesn't exist";
                        }

                        if (bhelLocal)
                        {
                            Instance.Logs
                            (
                                sLogs + " | <b>Recomendation:</b> Add a key before using it.",
                                "VRG_Remote->GetInt()",
                                enumVerbose
                            );
                        }
                    }
                    else
                    {
                        iRegresa = Instance.m_Ints[i].value;
                    }
                }
                else
                {
                    if (bhelLocal)
                    {
                        Instance.Logs
                        (
                            "<color=black>INT | </color> You tried to retreive an empty Key",
                            "VRG_Remote->GetInt()",
                            enumVerbose
                        );
                    }
                }
            }

            // return the value filled
            return iRegresa;
        }

        public static float GetFloat(string valueLocal)
        {
            return GetFloat(valueLocal, true);
        }
        /// <summary>
        /// Returns the value of the float searched, remote or local
        /// </summary>
        /// <param name="valueLocal">The name of the int to search </param>
        /// <returns>The int value from the string searched</returns>
        public static float GetFloat(string valueLocal, bool bhelLocal)
        {
            // by default is false
            float fRegresa = 0;

            if (Instance != null)
            {
                // The logs detail verbose
                ENUM_Verbose enumVerbose = ENUM_Verbose.WARNING;

                if (valueLocal.Trim() != string.Empty)
                {
                    string sLogs = "<b><color=black>" + valueLocal + "</color></b> | ";

                    // if it does exist in the arrays
                    int i = Instance.m_Floats.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            fRegresa = ConfigManager.appConfig.GetFloat(valueLocal);

                            Instance.AddFloat(valueLocal, fRegresa);

                            sLogs += "Exist in the Server Remote config = <b><i>" + fRegresa + "</i></b>";
                            enumVerbose = ENUM_Verbose.ALL;
                        }
                        else
#endif
                        {
                            sLogs += "Key Doesn't exist";
                        }

                        if (bhelLocal)
                        {
                            Instance.Logs
                            (
                                sLogs + " | <b>Recomendation:</b> Add a key before using it.",
                                "VRG_Remote->GetFloat()",
                                enumVerbose
                            );
                        }
                    }
                    else
                    {
                        fRegresa = Instance.m_Floats[i].value;
                    }
                }
                else
                {
                    if (bhelLocal)
                    {
                        Instance.Logs
                        (
                            "<color=black>FLOAT | </color> You tried to retreive an empty Key",
                            "VRG_Remote->GetFloat()",
                            enumVerbose
                        );
                    }
                }
            }

            // return the value filled
            return fRegresa;
        }


        public static string GetString(string valueLocal)
        {
            return GetString(valueLocal, true);
        }
        /// <summary>
        /// Returns the value of the string searched, remote or local
        /// </summary>
        /// <param name="valueLocal">The name of the string to search</param>
        /// <returns>The string value from the string searched</returns>
        public static string GetString(string valueLocal, bool bhelLocal)
        {
            // by default is false
            string sRegresa = "";

            if (Instance != null)
            {
                // The logs detail verbose
                ENUM_Verbose enumVerbose = ENUM_Verbose.WARNING;

                if (valueLocal.Trim() != string.Empty)
                {
                    string sLogs = "<b><color=black>" + valueLocal + "</color></b> | ";

                    // if it does exist in the arrays
                    int i = Instance.m_Strings.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            sRegresa = ConfigManager.appConfig.GetString(valueLocal);

                            Instance.AddString(valueLocal, sRegresa);

                            sLogs += "Exist in the Server Remote config = <b><i>" + sRegresa + "</i></b>";
                            enumVerbose = ENUM_Verbose.ALL;
                        }
                        else
#endif
                        {
                            sLogs += "Key Doesn't exist";
                        }

                        if (bhelLocal)
                        {
                            Instance.Logs
                            (
                                sLogs + " | <b>Recomendation:</b> Add a key before using it.",
                                "VRG_Remote->GetString()",
                                enumVerbose
                            );
                        }
                    }
                    else
                    {
                        sRegresa = Instance.m_Strings[i].value;
                    }
                }
                else
                {
                    if (bhelLocal)
                    {
                        Instance.Logs
                        (
                            "<color=black>STRING | </color> You tried to retreive an empty Key",
                            "VRG_Remote->GetString()",
                            enumVerbose
                        );
                    }
                }
            }

            // return the value filled
            return sRegresa.Trim();
        }





        /// <summary>
        /// It search for the datalocal into the array of Bools to find if it exists
        /// </summary>
        /// <param name="dataLocal">The name of the data to search for</param>
        /// <returns>true if the value exists in the array</returns>
        public static bool ExistBool(string valueLocal)
        {
            // by default is false
            bool bRegresa = false;

            if (Instance != null)
            {
                if (valueLocal.Trim() != string.Empty)
                {
                    // if it does exist in the arrays
                    int i = Instance.m_Bools.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            Instance.AddBool(valueLocal, ConfigManager.appConfig.GetBool(valueLocal));

                            bRegresa = true;
                        }
#endif
                    }
                    else
                    {
                        bRegresa = true;
                    }
                }

                else
                {
                    Instance.Logs
                    (
                        "<color=black>BOOL | </color> You tried to check if Exist an empty Key",
                        ENUM_Verbose.WARNING
                    );
                }
            }

            // return the value filled
            return bRegresa;
        }

        /// <summary>
        /// It search for the datalocal into the array of Ints to find if it exists
        /// </summary>
        /// <param name="dataLocal">The name of the data to search for</param>
        /// <returns>true if the value exists in the array</returns>
        public static bool ExistInt(string valueLocal)
        {
            // by default is false
            bool bRegresa = false;

            if (Instance != null)
            {
                if (valueLocal.Trim() != string.Empty)
                {
                    // if it does exist in the arrays
                    int i = Instance.m_Ints.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            Instance.AddInt(valueLocal, ConfigManager.appConfig.GetInt(valueLocal));

                            bRegresa = true;
                        }
#endif
                    }
                    else
                    {
                        bRegresa = true;
                    }
                }

                else
                {
                    Instance.Logs
                    (
                        "<color=black>INT | </color> You tried to check if Exist an empty Key",
                        ENUM_Verbose.WARNING
                    );
                }
            }

            // return the value filled
            return bRegresa;
        }

        /// <summary>
        /// It search for the datalocal into the array of Floats to find if it exists
        /// </summary>
        /// <param name="dataLocal">The name of the data to search for</param>
        /// <returns>true if the value exists in the array</returns>
        public static bool ExistFloat(string valueLocal)
        {
            // by default is false
            bool bRegresa = false;

            if (Instance != null)
            {
                if (valueLocal.Trim() != string.Empty)
                {
                    // if it does exist in the arrays
                    int i = Instance.m_Floats.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            Instance.AddFloat(valueLocal, ConfigManager.appConfig.GetFloat(valueLocal));

                            bRegresa = true;
                        }
#endif
                    }
                    else
                    {
                        bRegresa = true;
                    }

                }

                else
                {
                    Instance.Logs
                    (
                        "<color=black>FLOAT | </color> You tried to check if Exist an empty Key",
                        ENUM_Verbose.WARNING
                    );
                }
            }

            // return the value filled
            return bRegresa;
        }

        /// <summary>
        /// It search for the datalocal into the array of Strings to find if it exists
        /// </summary>
        /// <param name="dataLocal">The name of the data to search for</param>
        /// <returns>true if the value exists in the array</returns>
        public static bool ExistString(string valueLocal)
        {
            // by default is false
            bool bRegresa = false;

            if (Instance != null)
            {
                if (valueLocal.Trim() != string.Empty)
                {
                    // if it does exist in the arrays
                    int i = Instance.m_Strings.FindIndex((x) => x.name == valueLocal);

                    // if it didn't exist
                    if (i < 0)
                    {
#if REMOTE_CONFIG_INSTALLED
                        // ... and we can use the server data
                        if (ConfigManager.appConfig.HasKey(valueLocal))
                        {
                            Instance.AddString(valueLocal, ConfigManager.appConfig.GetString(valueLocal));

                            bRegresa = true;
                        }
#endif
                    }
                    else
                    {
                        bRegresa = true;
                    }
                }

                else
                {
                    Instance.Logs
                    (
                        "<color=black>STRING | </color> You tried to check if Exist an empty Key",
                        ENUM_Verbose.WARNING
                    );
                }
            }

            // return the value filled
            return bRegresa;
        }





        /// <summary>
        /// Add a bool to the bools array, it adds it to the very last
        /// </summary>
        /// <param name="dataLocal">The name of the data parameter to add to the array</param>
        /// <param name="valueLocal">the value of the data</param>
        /// <param name="forceLocal">(Default = false) When you add this value, if it is local or remote forced</param>
        public string AddBool(string dataLocal, bool valueLocal)
        {
            bool bChanged = false;

            // start the log register
            string sRegresa = "<color=red>N/A</color>";

            // ... meanwhile, the remote is equal to the local
            bool sValueRemote = valueLocal;

#if REMOTE_CONFIG_INSTALLED
            // ... and the key exists
            if (ConfigManager.appConfig.HasKey(dataLocal))
            {
                // update the value
                sValueRemote = ConfigManager.appConfig.GetBool(dataLocal);

                // and update the status
                sRegresa = "" + sValueRemote;
            }
#endif

            // if it does exist in the arrays
            int i = this.m_Bools.FindIndex((x) => x.name == dataLocal);

            // if it didn't exist add them to the arrays
            if (i < 0)
            {
                if (Instance != null)
                {
                    this.m_BoolsLocal.Add(new VRG_RemoteBool(dataLocal, valueLocal));
                    this.m_BoolsRemote.Add(new VRG_RemoteBool(dataLocal, sValueRemote));
                }
                else
                {
                    this.m_Bools.Add(new VRG_RemoteBool(dataLocal, valueLocal));
                }

                bChanged = true;

                // inform what happened
                sRegresa = "BOOL <b><i>add: </i></b>" + dataLocal + " | <b><i>Local: </i></b>" + valueLocal + " | <b><i>Remote: </i></b>" + sRegresa;
            }
            else
            {
                if (this.m_BoolsLocal[i].value != valueLocal || this.m_BoolsRemote[i].value != sValueRemote)
                {
                    bChanged = true;

                    sRegresa = "BOOL <b><i>update: </i></b>" + dataLocal;

                    if (this.m_BoolsLocal[i].value != valueLocal)
                    {
                        sRegresa += " | <b><i>Local: </i></b>" + this.m_BoolsLocal[i].value + " -> " + valueLocal;

                        // assign the value into the arrays
                        this.m_BoolsLocal[i].value = valueLocal;
                    }

                    if (this.m_BoolsRemote[i].value != sValueRemote)
                    {
                        sRegresa += " | <b><i>Remote: </i></b>" + this.m_BoolsRemote[i].value + " -> " + sValueRemote;

                        // assign the value into the arrays
                        this.m_BoolsRemote[i].value = sValueRemote;
                    }
                }
            }

            if (bChanged)
            {
                sRegresa = "\n - " + sRegresa;
                this.OnValidate();
            }
            else
            {
                sRegresa = string.Empty;
            }

            return sRegresa;
        }

        /// <summary>
        /// Add a Int to the Ints array, it adds it to the very last item
        /// </summary>
        /// <param name="dataLocal">The name of the data parameter to add to the array</param>
        /// <param name="valueLocal">the value of the data</param>
        /// <param name="forceLocal">(Default = false) When you add this value, if it is local or remote forced</param>
        public string AddInt(string dataLocal, int valueLocal)
        {
            bool bChanged = false;

            // start the log register
            string sRegresa = "<color=red>N/A</color>";

            // ... meanwhile, the remote is equal to the local
            int sValueRemote = valueLocal;

#if REMOTE_CONFIG_INSTALLED
            // ... and the key exists
            if (ConfigManager.appConfig.HasKey(dataLocal))
            {
                // update the value
                sValueRemote = ConfigManager.appConfig.GetInt(dataLocal);

                // and update the status
                sRegresa = "" + sValueRemote;
            }
#endif

            // if it does exist in the arrays
            int i = this.m_Ints.FindIndex((x) => x.name == dataLocal);

            // if it didn't exist add them to the arrays
            if (i < 0)
            {
                if (Instance != null)
                {
                    this.m_IntsLocal.Add(new VRG_RemoteInt(dataLocal, valueLocal));
                    this.m_IntsRemote.Add(new VRG_RemoteInt(dataLocal, sValueRemote));
                }
                else
                {
                    this.m_Ints.Add(new VRG_RemoteInt(dataLocal, valueLocal));
                }

                bChanged = true;

                // inform what happened
                sRegresa = "INT <b><i>add: </i></b>" + dataLocal + " | <b><i>Local: </i></b>" + valueLocal + " | <b><i>Remote: </i></b>" + sRegresa;
            }
            else
            {
                if (this.m_IntsLocal[i].value != valueLocal || this.m_IntsRemote[i].value != sValueRemote)
                {
                    bChanged = true;

                    sRegresa = "INT <b><i>update: </i></b>" + dataLocal;

                    if (this.m_IntsLocal[i].value != valueLocal)
                    {
                        sRegresa += " | <b><i>Local: </i></b>" + this.m_IntsLocal[i].value + " -> " + valueLocal;

                        // assign the value into the arrays
                        this.m_IntsLocal[i].value = valueLocal;
                    }

                    if (this.m_IntsRemote[i].value != sValueRemote)
                    {
                        sRegresa += " | <b><i>Remote: </i></b>" + this.m_IntsRemote[i].value + " -> " + sValueRemote;

                        // assign the value into the arrays
                        this.m_IntsRemote[i].value = sValueRemote;
                    }
                }
            }

            if (bChanged)
            {
                sRegresa = "\n - " + sRegresa;
                this.OnValidate();
            }
            else
            {
                sRegresa = string.Empty;
            }

            return sRegresa;
        }

        /// <summary>
        /// Add a FLOAT to the strings array, it adds it to the very last item in case of new, or update the data if is already set
        /// </summary>
        /// <param name="dataLocal">The name of the data parameter to add to the array</param>
        /// <param name="valueLocal">the value of the data</param>
        public string AddFloat(string dataLocal, float valueLocal)
        {
            bool bChanged = false;

            // start the log register
            string sRegresa = "<color=red>N/A</color>";

            // ... meanwhile, the remote is equal to the local
            float sValueRemote = valueLocal;

#if REMOTE_CONFIG_INSTALLED
            // ... and the key exists
            if (ConfigManager.appConfig.HasKey(dataLocal))
            {
                // update the value
                sValueRemote = ConfigManager.appConfig.GetFloat(dataLocal);

                // and update the status
                sRegresa = "" + sValueRemote;
            }
#endif

            // if it does exist in the arrays
            int i = this.m_Floats.FindIndex((x) => x.name == dataLocal);

            // if it didn't exist add them to the arrays
            if (i < 0)
            {
                if (Instance != null)
                {
                    this.m_FloatsLocal.Add(new VRG_RemoteFloat(dataLocal, valueLocal));
                    this.m_FloatsRemote.Add(new VRG_RemoteFloat(dataLocal, sValueRemote));
                }
                else
                {
                    this.m_Floats.Add(new VRG_RemoteFloat(dataLocal, valueLocal));
                }

                bChanged = true;

                // inform what happened
                sRegresa = "FLOAT <b><i>add: </i></b>" + dataLocal + " | <b><i>Local: </i></b>" + valueLocal + " | <b><i>Remote: </i></b>" + sRegresa;
            }
            else
            {
                if (this.m_FloatsLocal[i].value != valueLocal || this.m_FloatsRemote[i].value != sValueRemote)
                {
                    bChanged = true;

                    sRegresa = "FLOAT <b><i>update: </i></b>" + dataLocal;

                    if (this.m_FloatsLocal[i].value != valueLocal)
                    {
                        sRegresa += " | <b><i>Local: </i></b>" + this.m_FloatsLocal[i].value + " -> " + valueLocal;

                        // assign the value into the arrays
                        this.m_FloatsLocal[i].value = valueLocal;
                    }

                    if (this.m_FloatsRemote[i].value != sValueRemote)
                    {
                        sRegresa += " | <b><i>Remote: </i></b>" + this.m_FloatsRemote[i].value + " -> " + sValueRemote;

                        // assign the value into the arrays
                        this.m_FloatsRemote[i].value = sValueRemote;
                    }
                }
            }

            if (bChanged)
            {
                sRegresa = "\n - " + sRegresa;
                this.OnValidate();
            }
            else
            {
                sRegresa = string.Empty;
            }

            return sRegresa;
        }

        /// <summary>
        /// Add a string to the strings array, it adds it to the very last item in case of new, or update the data if is already set
        /// </summary>
        /// <param name="dataLocal">The name of the data parameter to add to the array</param>
        /// <param name="valueLocal">the value of the data</param>
        public string AddString(string dataLocal, string valueLocal)
        {
            bool bChanged = false;

            // start the log register
            string sRegresa = "<color=red>N/A</color>";

            // ... meanwhile, the remote is equal to the local
            string sValueRemote = valueLocal;

#if REMOTE_CONFIG_INSTALLED
            // ... and the key exists
            if (ConfigManager.appConfig.HasKey(dataLocal))
            {
                // update the value
                sValueRemote = ConfigManager.appConfig.GetString(dataLocal);

                // and update the status
                sRegresa = "" + sValueRemote;
            }
#endif

            // if it does exist in the arrays
            int i = this.m_Strings.FindIndex((x) => x.name == dataLocal);

            // if it didn't exist add them to the arrays
            if (i < 0)
            {
                if (Instance != null)
                {
                    this.m_StringsLocal.Add(new VRG_RemoteString(dataLocal, valueLocal));
                    this.m_StringsRemote.Add(new VRG_RemoteString(dataLocal, sValueRemote));
                }
                else
                {
                    this.m_Strings.Add(new VRG_RemoteString(dataLocal, valueLocal));
                }

                bChanged = true;

                // inform what happened
                sRegresa = "STRING <b><i>add: </i></b>" + dataLocal + " | <b><i>Local: </i></b>" + valueLocal + " | <b><i>Remote: </i></b>" + sRegresa;
            }
            else
            {
                if (this.m_StringsLocal[i].value != valueLocal || this.m_StringsRemote[i].value != sValueRemote)
                {
                    bChanged = true;

                    sRegresa = "STRING <b><i>update: </i></b>" + dataLocal;

                    if (this.m_StringsLocal[i].value != valueLocal)
                    {
                        sRegresa += " | <b><i>Local: </i></b>" + this.m_StringsLocal[i].value + " -> " + valueLocal;

                        // assign the value into the arrays
                        this.m_StringsLocal[i].value = valueLocal;
                    }

                    if (this.m_StringsRemote[i].value != sValueRemote)
                    {
                        sRegresa += " | <b><i>Remote: </i></b>" + this.m_StringsRemote[i].value + " -> " + sValueRemote;

                        // assign the value into the arrays
                        this.m_StringsRemote[i].value = sValueRemote;
                    }
                }
            }

            if (bChanged)
            {
                sRegresa = "\n - " + sRegresa;
                this.OnValidate();
            }
            else
            {
                sRegresa = string.Empty;
            }

            return sRegresa;
        }






    }
}