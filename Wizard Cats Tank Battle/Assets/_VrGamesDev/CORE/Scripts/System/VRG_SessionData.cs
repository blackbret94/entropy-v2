using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Load a value from the persistant data from Object: VRG_Session
    /// </summary>
    public class VRG_SessionData : VRG_Base
    {
        [Header("From: VRG_Session")]
        /// <summary>
        /// Choose the <a href="#VrGamesDev.ENUM_DataType">data type</a> to save into the <a href="#VrGamesDev.VRG_Session">VRG_Session</a>
        /// </summary>
        [Tooltip("Choose the data type to save into the VRG_Session")]
        [SerializeField] protected ENUM_DataType m_DataType = ENUM_DataType.STRING;

        /// <summary>
        /// The value recovered
        /// </summary>
        [Tooltip("It will load the data from the ")]
        [SerializeField] protected bool m_Load = false;


        /// <summary>
        /// If set, the value will be dumped into this Text component
        /// </summary>
        [Tooltip("If set, the value will be taken from this Text component")]
        [SerializeField] protected Text m_LoadText = null;

        /// <summary>
        /// The value recovered
        /// </summary>
        [Tooltip("The value recovered")]
        [SerializeField] protected bool m_Save = false;

        /// <summary>
        /// If set, the value will be dumped into this Text component
        /// </summary>
        [Tooltip("If set, the value will be dumped into this Text component")]
        [SerializeField] protected Text m_SaveText = null;

        /// <summary>
        /// The Object owner of the data in the <a href="#VrGamesDev.VRG_Session">VRG_Session</a>
        /// </summary>
        [Tooltip("The Object owner of the data in the VRG_Session")]
        [SerializeField] protected string m_SessionObject = "";

        /// <summary>
        /// The data to query from the m_SessionObject
        /// </summary>
        [Tooltip("The data to query from the m_SessionObject")]
        [SerializeField] protected string m_SessionData = "";

        /// <summary>
        /// The value recovered
        /// </summary>
        [Tooltip("The value recovered")]
        [SerializeField] protected string m_Value = "";

        /// <summary>
        /// GETTER: the m_Value
        /// </summary>
        public string value
        {
            get
            {
                return this.m_Value;
            }

            set
            {
                this.m_Value = value;

                // update the text if available and save into the session
                this.Save();
            }
        }


        [Header("From: OnLoad")]
        /// <summary>
        /// When loaded, If the value is null do not save or load
        /// </summary>
        [Tooltip("When loaded, If the value is null do not save or load")]
        [SerializeField] protected bool m_IgnoreOnNull = true;

        /// <summary>
        /// When loaded, if the value is the same as this, it will trigger the action object
        /// </summary>
        [Tooltip("When loaded, if the value is the same as this, it will trigger the action object")]
        [SerializeField] protected string m_IfValue = "";
        public string ifValue
        {
            get
            {
                return this.m_IfValue;
            }

            set
            {
                this.m_IfValue = value;
            }
        }

        /// <summary>
        /// Array of the transform to activate <em>setActive(true)</em>
        /// </summary>
        [Tooltip("Array of the transform to activate setActive(true)")]
        [SerializeField] protected Transform[] m_Activate = null;

        /// <summary>
        /// Array of the transform to deactivate <em>setActive(false)</em>
        /// </summary>
        [Tooltip("Array of the transform to deactivate setActive(false)")]
        [SerializeField] protected Transform[] m_Deactivate = null;

        /// <summary>
        /// If the loaded value is true this GameObject will trigger
        /// </summary>
        [Tooltip("If the loaded value is true this GameObject will trigger")]
        [SerializeField] protected Transform[] m_Toogle = null;







        private void Awake()
        {
            if (!this.m_Load && !this.m_Save)
            {
                this.Logs(this.name + " does not load or save, Are you sure you need a SessionData component?", ENUM_Verbose.WARNING);
            }

            // make sure to get the data needed before any happens
            this.Load();
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong>Load from VRG_Session a value
        /// </summary>
        protected override IEnumerator Do()
        {
            // save the data
            this.Save();

            // and load it, even if it was just saved
            this.Load();


            yield return null;
        }

        /// #IGNORE
        public virtual void Load() => this.Load(this.m_Load);

        /// <summary>
        /// Load the data from the session, and update a text data if provided
        /// </summary>
        /// <param name="valueLocal">Force the load even if the object is not allowed</param>
        public virtual void Load(bool valueLocal)
        {
            // just do it if forced or if its preallowed
            if (valueLocal)
            {
                // do not try to get the session data if not provided both configurations
                if (this.m_SessionObject.Trim() != "" && this.m_SessionData.Trim() != "")
                {
                    switch (this.m_DataType)
                    {
                        case ENUM_DataType.BOOL:
                            this.m_Value = VRG_Session.GetBool(this.m_SessionObject, this.m_SessionData).ToString();
                            break;

                        case ENUM_DataType.INT:
                            this.m_Value = VRG_Session.GetInt(this.m_SessionObject, this.m_SessionData).ToString();
                            break;

                        case ENUM_DataType.FLOAT:
                            this.m_Value = VRG_Session.GetFloat(this.m_SessionObject, this.m_SessionData).ToString();
                            break;

                        case ENUM_DataType.STRING:
                            this.m_Value = VRG_Session.GetString(this.m_SessionObject, this.m_SessionData);
                            break;
                    }
                }

                // update the text if available
                this.UpdateText();

                // Check if the value is a match, and trigger the items
                this.OnValue();
            }
        }



        protected void OnValue()
        {
            // inform the Activate, deactivate and toogle GameObjects if the value was meet
            if (
                (this.m_IfValue.Trim() == this.m_Value.Trim()) &&
                (!this.m_IgnoreOnNull || this.m_Value.Trim() != "")
                )
            {
                this.Logs
                (
                    "Session value match = " + this.m_SessionObject + "->" + this.m_SessionData + ": " + this.m_IfValue.Trim() + "==" + this.m_Value.Trim(),
                    "VRG_SessionData->OnValue()",
                    ENUM_Verbose.DEBUG
                );

                foreach (Transform child in this.m_Activate)
                {
                    if (child != null)
                    {
                        child.gameObject.SetActive(true);
                    }
                }

                foreach (Transform child in this.m_Deactivate)
                {
                    if (child != null)
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                foreach (Transform child in this.m_Toogle)
                {
                    if (child != null)
                    {
                        child.gameObject.SetActive(!child.gameObject.activeSelf);
                    }

                }

            }
        }







        private void UpdateText()
        {
            // just do it it has a load text
            if (this.m_LoadText != null)
            {
                // ... and the value is not null, or it ignore the null values
                if (!this.m_IgnoreOnNull || this.m_Value.Trim() != "")
                {
                    // set the text 
                    this.m_LoadText.text = this.m_Value;
                }
            }
        }


        /// #IGNORE
        public virtual void Save() => this.Save(this.m_Save);

        /// <summary>
        /// Save the data into the session, get the data from the text UI if provided
        /// </summary>
        /// <param name="valueLocal">Force the load even if the object is not allowed</param>
        public virtual void Save(bool valueLocal)
        {
            // just do it if forced or if its preallowed
            if (valueLocal)
            {
                // if the text exists
                if (this.m_SaveText != null)
                {
                    // and the data is not null or asked to ignore it
                    if (!this.m_IgnoreOnNull || this.m_SaveText.text.Trim() != "")
                    {
                        // set the value from the text
                        this.m_Value = this.m_SaveText.text.Trim();
                    }
                }

                // do not try to get the session data if not provided both configurations
                if (this.m_SessionObject.Trim() != "" && this.m_SessionData.Trim() != "")
                {
                    switch (this.m_DataType)
                    {
                        case ENUM_DataType.BOOL:
                            VRG_Session.SetBool(this.m_SessionObject, this.m_SessionData, bool.Parse(this.m_Value));
                            break;

                        case ENUM_DataType.INT:
                            VRG_Session.SetInt(this.m_SessionObject, this.m_SessionData, int.Parse(this.m_Value));
                            break;

                        case ENUM_DataType.FLOAT:
                            VRG_Session.SetFloat(this.m_SessionObject, this.m_SessionData, float.Parse(this.m_Value));
                            break;

                        case ENUM_DataType.STRING:
                            VRG_Session.SetString(this.m_SessionObject, this.m_SessionData, this.m_Value);
                            break;
                    }
                }

                // update the text if available
                this.UpdateText();
            }
        }
    }
}