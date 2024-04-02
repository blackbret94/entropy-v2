using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev.DDuA
{
    public class VRG_Addressable : VRG_Base
    {
        [Tooltip("m_Bhel")]
        [SerializeField]
        protected bool m_Bhel = false;
        public bool bhel
        {
            get
            {
                return this.m_Bhel;
            }
            set
            {
                this.m_Bhel = value;

                this.WhenBhelSuscribe();
            }
        }

        [Tooltip("m_IsParent")]
        [SerializeField]
        protected bool m_IsParent = true;


        [Header("From: Pooling")]
        [Space(10)]
        [Tooltip("If the pool is used")]
        [SerializeField]
        protected bool m_IsPoolOverflow = false;
        /// <summary>
        /// If it is pooling, it will reuse the same objects
        /// </summary>
        public bool isPoolOverflow
        {
            get
            {
                return this.m_IsPoolOverflow;
            }
            set
            {
                this.m_IsPoolOverflow = value;
            }
        }

        [Tooltip("If it is pooling, it will reuse the same objects")]
        [SerializeField]
        protected int m_Pool = 0;
        /// <summary>
        /// If it is pooling, it will reuse the same objects
        /// </summary>
        public int pool
        {
            get
            {
                return this.m_Pool;
            }
            set
            {
                this.m_Pool = value;
            }
        }

        [Tooltip("The Elements spawned")]
        [SerializeField] private List<GameObject> m_Spawns = new List<GameObject>();
        public List<GameObject> spawns { get { return this.m_Spawns; } }


        [Header("From: Serializable")]
        [Space(10)]
        [Tooltip("VRG_AddressableSerializable")]
        [SerializeField]
        protected VRG_AddressableSerializable m_Data = new VRG_AddressableSerializable();
        public VRG_AddressableSerializable data { get { return this.m_Data; } }



        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        /// <summary>
        /// It destroy itself if true after it is created once
        /// </summary>
        [Tooltip("If it will turn off after it's done.")]
        //[SerializeField]
        protected bool m_SelfTurnDestroy = false;



        /// <summary>
        /// Public Event: Trigers WHEN = Bhel data is updated
        /// </summary>
        public event Action<string> WhenBHEL;

        /// <summary>
        /// Public Event: Trigers WHEN = A set of Addressables is finished and fully downloaded
        /// </summary>
        public event Action<GameObject> WhenCreated;




        public VRG_Addressable()
        {
            this.verbose = ENUM_Verbose.ALL;

            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = false;
        }


        protected void Awake()
        {
            this.SetName(this.data.address);

            this.data.WhenCreated += data_WhenCreated;

            this.OnValidate();
        }
        private void OnValidate()
        {
            this.data.verbose = this.verbose;
            this.WhenBhelSuscribe();
        }
        private void WhenBhelSuscribe()
        {
            this.data.WhenBHEL -= data_OnBHEL;

            if (this.m_Bhel)
            {
                this.data.WhenBHEL += data_OnBHEL;
            }
        }

        protected virtual void SetName(string valueLocal)
        {
            if (this.name.Contains("VRG_Addressable") && valueLocal != string.Empty)
            {
                this.name = "VRG_Addressable - (" + valueLocal + ")";
            }
        }

        public void SetAddress(string valueLocal)
        {
            this.data.address = valueLocal;
        }

        protected override IEnumerator Do()
        {
            yield return null;

            if (this.data.address == string.Empty && !this.name.Contains("VRG_Addressable"))
            {
                this.data.address = this.name;
            }

            this.Play(this.data.address);

            // return, it is like a void and wait a frame
            yield return null;
        }

        public void Play(string valueLocal)
        {
            this.m_SelfTurnDestroy = this.m_SelfTurnOff;
            this.m_SelfTurnOff = false;

            this.data.objectInScene = VRG.GetSceneGameObject(this.gameObject);

            if (valueLocal != string.Empty)
            {
                if (this.pool > 0)
                {
                    this.m_IsParent = true;
                    this.data.isLocked = true;
                    this.data.isOverWrite = false;
                    this.data.evolution = ENUM_AddressableEvolution.CREATE;

                    if (this.transform.childCount < this.pool)
                    {
                        this.data.Play(valueLocal);
                    }
                }
                else
                {
                    this.data.Play(valueLocal);
                }
            }
        }

        public GameObject Get()
        {
            GameObject go_Return = null;

            foreach (Transform child in this.transform)
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    go_Return = child.gameObject;
                    go_Return.transform.SetParent(null);
                    break;
                }
            }

            if (this.isPoolOverflow)
            {
                int i = 0;
                foreach (Transform child in this.transform)
                {
                    if (!child.gameObject.activeInHierarchy)
                    {
                        i++;
                    }
                }

                if (i < this.pool)
                {
                    this.data.Play(this.data.address);
                }
            }
            else
            {
                if (go_Return == null)
                {
                    go_Return = this.transform.GetChild(0).gameObject;
                    go_Return.transform.SetParent(null);
                }
            }

            if (go_Return != null)
            {
                go_Return.transform.SetParent(this.transform);
                go_Return.SetActive(true);
            }
            else
            {
                this.Logs("Can't pool the next gameObject", ENUM_Verbose.WARNING);
            }

            return go_Return;
        }


        protected virtual void data_WhenCreated(GameObject valueLocal)
        {
            this.SetName(this.data.address);

            if (this.m_IsParent)
            {
                valueLocal.transform.SetParent(this.transform, false);
            }

            if (this.pool > 0)
            {
                valueLocal.SetActive(false);
            }

            this.m_Spawns.Add(valueLocal);

            StartCoroutine(this.CleanSpawnsNulls());

            StartCoroutine(this.SelfDestroy());

            // invoke the events delegateds
            this.WhenCreated?.Invoke(valueLocal);

            if (this.pool > 0)
            {
                /*
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
                */

                int i = 0;
                foreach (Transform child in this.transform)
                {
                    if (!child.gameObject.activeInHierarchy)
                    {
                        i++;
                    }
                }


                if (i < this.pool)
                {
                    this.data.Play(this.data.address);
                }
            }

        }

        protected virtual IEnumerator CleanSpawnsNulls()
        {
            yield return null;

            int iTotal = this.m_Spawns.Count - 1;
            
            for (int i = iTotal; i >= 0; i--)
            {
                if (this.m_Spawns[i] == null)
                {
                    this.m_Spawns.RemoveAt(i);
                }
            }

            yield return null;
        }

        protected virtual IEnumerator SelfDestroy()
        {
            yield return null;

            if (this.m_SelfTurnDestroy)
            {
                foreach(GameObject child in this.m_Spawns)
                {
                    child.transform.SetParent(this.transform.parent);
                }

                UnityEngine.Object.Destroy(this.gameObject);
            }

            yield return null;
        }


        public void ToogleLocked() => ToogleLocked(!this.data.isLocked);
        public void ToogleLocked(bool valueLocal)
        {
            this.data.isLocked = valueLocal;

            string sLogs = this.data.bhelName + " | Toogle IsLocked to <b>" + this.data.isLocked + "</b>";

            this.Logs(sLogs, "VRG_Addressable->ToogleLocked()");

            this.data_OnBHEL(sLogs);            
        }

        public void ToogleParent() => ToogleParent(!this.m_IsParent);
        public void ToogleParent(bool valueLocal)
        {
            this.m_IsParent = valueLocal;

            string sLogs = this.data.bhelName + " | Toogle IsParent to <b>" + this.m_IsParent + "</b>";

            this.Logs(sLogs, "VRG_Addressable->ToogleParent()");

            this.data_OnBHEL(sLogs);

        }

        public void ToogleOverwrite() => ToogleOverwrite(!this.data.isOverWrite);
        public void ToogleOverwrite(bool valueLocal)
        {
            this.data.isOverWrite = valueLocal;

            string sLogs = this.data.bhelName + " | Toogle IsOverWrite to <b>" + this.data.isOverWrite + "</b>";

            this.Logs(sLogs, "VRG_Addressable->ToogleOverwrite()");

            this.data_OnBHEL(sLogs);
        }

        public void TooglePoolOverflow() => TooglePoolOverflow(!this.isPoolOverflow);
        public void TooglePoolOverflow(bool valueLocal)
        {
            this.isPoolOverflow = valueLocal;

            string sLogs = this.data.bhelName + " | Toogle isPoolOverflow to <b>" + this.isPoolOverflow + "</b>";

            this.Logs(sLogs, "VRG_Addressable->TooglePoolOverflow()");

            this.data_OnBHEL(sLogs);
        }

        protected void data_OnBHEL(string valueLocal)
        {
            if (this.m_Bhel)
            {
                // invoke the events delegateds
                this.WhenBHEL?.Invoke(valueLocal);
            }
        }

        public void Destroy()
        {
            if (this.pool > 0)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                if (this.m_Spawns.Count > 0)
                {
                    string sLogs = "Destroying " + this.m_Spawns.Count + " spawn";

                    if (this.m_Spawns.Count > 1)
                    {
                        sLogs += "s";
                    }
                    sLogs += "<hr>";
            
                    this.data.Destroy();

                    foreach (GameObject child in this.spawns)
                    {
                        if (child != null)
                        {
                            sLogs += " - " + child.name + "\n";
                            UnityEngine.Object.Destroy(child);
                        }
                    }

                    this.spawns.Clear();

                    this.Logs(sLogs, "VRG_Addressable->Destroy()", ENUM_Verbose.ALL);

                    this.data_OnBHEL(sLogs);
                }
            }
        }

        protected void OnDestroy()
        {
            this.data.WhenCreated -= data_WhenCreated;

            if (this.m_Bhel)
            {
                this.data.WhenBHEL -= data_OnBHEL;
            }

        }
    }
}