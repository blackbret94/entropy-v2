using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

using VrGamesDev.BHEL;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// Download all the addresables by any Label or Key
    /// </summary>
    [System.Serializable]
    public class VRG_AddressablesByLabel
    {
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

        [Tooltip("The Id of the labels to download, this will be used on the BHEL records")]
        [SerializeField] private string m_Id = string.Empty;
        public string id
        {
            get
            {
                return this.m_Id;
            }
            set
            {
                this.m_Id = value;

                if (!this.m_Labels.Contains(this.m_Id))
                {
                    this.m_Labels.Add(this.m_Id);
                }
            }
        }

        [Tooltip("The Parent name for the BHEL")]
        [SerializeField] private string m_ObjectInScene = string.Empty;
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

        [Tooltip("The Id of the labels to download, this will be used on the BHEL records")]
        [SerializeField] private bool m_Create = false;
        public bool create
        {
            get
            {
                return this.m_Create;
            }
            set
            {
                this.m_Create = value;
            }
        }

        [Tooltip("s")]
        [SerializeField] private bool m_OrderBy = false;
        public bool orderBy
        {
            get
            {
                return this.m_OrderBy;
            }
            set
            {
                this.m_OrderBy = value;
            }
        }

        [Tooltip("The Elements to search, they could be labels or address")]
        [SerializeField] private List<string> m_Labels = new List<string>();
        public List<string> labels { get { return this.m_Labels; } }

        [Tooltip("The Elements to search, they could be labels or address")]
        [SerializeField] private List<string> m_Keys = new List<string>();
        public List<string> keys { get { return this.m_Keys; } }

        [Tooltip("The Elements to search, they could be labels or address")]
        [SerializeField] private List<VRG_AddressableSerializable> m_Addressables = new List<VRG_AddressableSerializable>();
        public List<VRG_AddressableSerializable> addressables { get { return this.m_Addressables; } }

        /// <summary>
        /// Public Event: Trigers WHEN = We found all the keys associated to this labels
        /// </summary>
        public event Action<string[]> WhenKeysFound;


        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_AddressablesByLabel()
        {
        }


        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_AddressablesByLabel(string valueLocal)
        {
            this.m_Id = valueLocal;
        }



        private void EmptyLabels()
        {
            if (this.m_Id != string.Empty)
            {
                if (!this.m_Labels.Contains(this.m_Id))
                {
                    this.m_Labels.Add(this.m_Id);
                }
            }
            else
            {
                /*
                if (this.verbose >= ENUM_Verbose.WARNING)
                {
                    // LOGS:
                    VRG_Bhel.Do
                    (
                        "You have a VRG_AddressablesByLabel object without ID, please set it",
                        "VRG_AddressablesByLabel->EmptyLabels()",
                        ENUM_Verbose.WARNING,
                        this.m_ObjectInScene
                    );
                }
                */
            }
        }
        
        public void ClearLabels()
        {
            this.m_Labels.Clear();

            this.m_Keys.Clear();

            foreach(VRG_AddressableSerializable child in this.m_Addressables)
            {
                child.Release();
            }

            this.m_Addressables.Clear();
        }

        public int AddLabel(string valueLocal)
        {
            int iRegresa = 0;

            this.EmptyLabels();

            // if the label is new?
            if (!this.m_Labels.Contains(valueLocal) && valueLocal.Trim() != string.Empty)
            {
                // ... add it
                this.m_Labels.Add(valueLocal);

                // yea we could add it
                iRegresa++;
            }

            return iRegresa;
        }

        public int AddLabel(string[] valueLocal)
        {
            int iRegresa = 0;

            this.EmptyLabels();

            foreach(string child in valueLocal)
            {
                // if the label is new?
                if (!this.m_Labels.Contains(child) && child.Trim() != string.Empty)
                {
                    // ... add it
                    this.m_Labels.Add(child);

                    // yea we could add it
                    iRegresa++;
                }
            }

            return iRegresa;
        }


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Get all the paths and start the download
        /// </summary>
        public void Play()
        {
            this.EmptyLabels();

            Addressables.LoadResourceLocationsAsync(this.m_Labels, Addressables.MergeMode.Union).Completed += PathsFromLabels;
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        /// #IGNORE
        private void PathsFromLabels(AsyncOperationHandle<IList<IResourceLocation>> obj)
        {
            bool bContinue = true;

            // the logs to save
            string sLogsKeys = string.Empty;
            string sLogsLabels = string.Empty;

            if (obj.Result.Count > 0)
            {
                // save to logs
                sLogsKeys += "\n- Trying " + obj.Result.Count + " childs. <hr>";

                // check and register all the result
                foreach (IResourceLocation child in obj.Result)
                {
                    // just in case is new
                    if (!this.m_Keys.Contains(child.PrimaryKey))
                    {
                        if (child.PrimaryKey.Trim() != String.Empty && child.PrimaryKey != "VRG_DDuA")
                        {
                            // save to logs
                            sLogsKeys += "\n- " + child.PrimaryKey;

                            // ... add it
                            this.m_Keys.Add(child.PrimaryKey);
                        }
                        else
                        {
                            sLogsKeys += "\n- <color=red>" + child.PrimaryKey + "</color>";
                        }
                    }
                    else
                    {
                        sLogsKeys += "\n- <color=red>" + child.PrimaryKey + "</color>";
                    }
                }

                if (this.m_OrderBy)
                {
                    this.m_Keys.Sort();
                }

                foreach(string child in this.m_Keys)
                {
                    // and create the addresable
                    this.m_Addressables.Add(new VRG_AddressableSerializable(child, this.m_Create));

                    // set the parent
                    this.m_Addressables[this.m_Addressables.Count - 1].objectInScene = this.objectInScene;

                    // ... now
                    this.m_Addressables[this.m_Addressables.Count - 1].Play();
                }

                if (this.m_Addressables.Count > 0)
                {
                    sLogsKeys = "<b>(" + this.m_Addressables.Count + ") " + this.m_Id + " Keys: </b>\n " + sLogsKeys;                    
                }
                else
                {
                    sLogsKeys += "<span style=\"background-color:black; display: inline;\"><color=yellow><b>(0) No " + this.m_Id + " Keys found </b></color></span>";
                }
            }
            else
            {
                sLogsKeys += "\n<span style=\"background-color:black; display: inline;\"><color=yellow><b>- (0) No " + this.m_Id + " Keys found </b></color></span>";
            }


            if (this.m_Labels.Count > 0)
            {
                sLogsLabels += "<b>(" + this.m_Labels.Count + ") " + this.m_Id + " Labels: </b>\n";
                foreach (string child in this.m_Labels)
                {
                    // save to logs
                    sLogsLabels += "\n - " + child;

                    if (child == ("VRG_Scene." + VRG_DDuA.m_SceneProxy).ToString() )
                    {
                        bContinue = false;
                    }
                }
            }
            else
            {
                sLogsLabels += "<span style=\"background-color:black; display: inline;\">< color=yellow><b>(0) No " + this.m_Id + " Labels found</b></color></span>";
            }

            // sLogsKeys += "\n\n";
            sLogsLabels += "<hr>";
            
            if (this.verbose >= ENUM_Verbose.LOGS && bContinue)// && this.m_Id != "VRG_DDuA.OnLaunch")
            {
                string sColor = "" + this.m_Create;

                if (this.m_Create)
                {
                    sColor = "<color=green>" + this.m_Create + "</color>";
                }

                // LOGS:
                VRG_Bhel.Do
                (
                    "<hr><center><i>Addressables Details | Instantiate: <b>" + sColor + "</b> </i> </center><hr>"
                    + sLogsLabels
                    + sLogsKeys
                    + "\n\n",
                    "VRG_AddressablesByLabel->PathsFromLabels()",
                    ENUM_Verbose.LOGS,
                    this.m_ObjectInScene
                );
            }

            // inform everybody we are done
            this.WhenKeysFound?.Invoke((string[])this.m_Keys.ToArray());
        }

    }
}
