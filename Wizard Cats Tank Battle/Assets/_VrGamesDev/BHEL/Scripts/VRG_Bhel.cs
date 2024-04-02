//*
using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using System.IO;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;


using Object = UnityEngine.Object;

namespace VrGamesDev.BHEL
{
    /// <summary>
    /// This class save to a CSV file and HTML format in a folder the information
    /// for easier debug or to further analysis.
    /// It works alone or it could be controlled by the Remote settings to start
    /// it needs the VRG_Remote singleton
    /// </summary>
    public class VRG_Bhel : VRG_Base
    {
        [Tooltip("You can ask for this variable to know if the object is ready or is still querying information")]
        [SerializeField]//
        private bool m_IsReady = false;
        /// <summary>
        /// You can ask for this variable to know if the object is ready or is still querying information
        /// </summary>
        public static bool isReady { get { return Instance.m_IsReady; } }

        /// <summary>
        /// TRUE to send the logs to the Unity console
        /// </summary>
        [Tooltip("TRUE to send the logs to the Unity console")]
        [SerializeField] private bool m_SaveOnUpdate = true;

        /// <summary>
        /// TRUE to send the logs to the Unity console
        /// </summary>
        [Tooltip("TRUE to send the logs to the Unity console")]
        [SerializeField] private bool m_ShowInConsole = false;
        public bool showInConsole
        {
            get
            {
                return Instance.m_IsReady;
            }
            set
            {
                this.m_ShowInConsole = value;
            }
        }

        [Header("From: Settings")]

        /// <summary>
        /// The append mode, overwrite, append or Diferent files
        /// FROM VRG_Remote: The name of the file log is the project name, The append mode is defined as.-
        /// OVERWRITE = It doesn’t append new logs, just one log is saved (the last one).
        /// APPEND = It does append, every run is logged one after another. (it gets bulky after a while)
        /// DIFFERENT_FILES = The name of the file log is the project name with the datetime, so every run is in a diferent file. (you will get a lot of files quick)
        /// </summary>
        [Tooltip("The append mode, overwrite, append or Diferent files")]
        [SerializeField]
        private ENUM_Append m_AppendMode = ENUM_Append.OVERWRITE;

        /// <summary>
        /// This value is filled with m_SettingsDirectory from Remote
        /// </summary>
        [Tooltip("This value is filled with m_SettingsDirectory from Remote")]
        [SerializeField]
        private string m_DirectoryName = "BHEL";

        /// <summary>
        /// TRUE to generate the HTML log
        /// </summary>
        [Tooltip("TRUE to generate the HTML log")]
        [SerializeField] private bool m_HTML = true;
        private bool m_HTMLSetting;

        /// <summary>
        /// TRUE to generate the CSV log
        /// </summary>
        [Tooltip("TRUE to generate the CSV log")]
        [SerializeField] private bool m_CSV = true;
        private bool m_CSVSetting;





        /// <summary>
        /// The delay time the UI screen will wait before dissapearing
        /// </summary>
        [Range(0.0f, 9.9f)]
        [Tooltip("The delay time the UI screen will wait before dissapearing")]
        [SerializeField] private float m_UIDelay = 0.0f;
        public float uiDelay
        {
            get
            {
                return this.m_UIDelay;
            }
            set
            {
                this.m_UIDelay = value;
            }
        }

        /// <summary>
        /// The level of detail of the floating time
        /// </summary>
        [Range(0, 6)]
        [Tooltip("The level of detail of the floating time")]
        [SerializeField] private int m_FloatingPointDetail = 6;
        public int floatingPointDetail
        {
            get
            {
                return this.m_FloatingPointDetail;
            }
            set
            {
                this.m_FloatingPointDetail = value;
            }
        }




        [Header("From: Components")]
        /// <summary>
        /// TRUE to generate the HTML log
        /// </summary>
        [Tooltip("TRUE to generate the HTML log")]
        [SerializeField] private GameObject m_UIText = null;

        [Tooltip("TRUE to generate the HTML log")]
        [SerializeField] private Transform m_VRG_UI = null;

        [Tooltip("TRUE to generate the HTML log")]
        [SerializeField] private Transform m_ViewPort = null;

        /// <summary>
        /// This methods will not be logged, they will be excluded
        /// </summary>
        [Tooltip("This methods will not be logged, they will be excluded")]
        [SerializeField] private string[] m_Excludes = { "Logs", "InvokeMoveNext" };



        [Header("From: HTML")]
        /// <summary>
        /// Font default color
        /// </summary>
        [Tooltip("Font default color")]
        [SerializeField] private Color m_Font = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        //[SerializeField]
        private string m_ColorFont = "#000000";

        /// <summary>
        /// Row default color
        /// </summary>
        [Tooltip("Row default color")]
        [SerializeField] private Color m_RowDefault = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //[SerializeField]
        private string m_ColorRowDefault = "#FFFFFF";

        /// <summary>
        /// Row color to switch on update
        /// </summary>
        [Tooltip("Row color to switch on update")]
        [SerializeField] private Color m_RowSwitch = new Color(0.804f, 0.804f, 0.804f, 1.0f);
        //[SerializeField]
        private string m_ColorRowSwitch = "#CDCDCD";

        /// <summary>
        /// Default carrier return character
        /// </summary>
        [Tooltip("Default carrier return character")]
        private string m_Cr = "\n";

        /// <summary>
        /// CSS to style the table if an internet conection is available
        /// </summary>
        [Tooltip("CSS to style the table if an internet conection is available")]
        //[SerializeField]
        private string m_Css = "https://s3.amazonaws.com/vrgbhel/css/jquery.dataTables.css";
        /// <summary>
        /// CSS to style the table if an internet conection is available
        /// </summary>
        [Tooltip("CSS to style the table if an internet conection is available")]
        //[SerializeField]
        private string m_JQuery = "https://s3.amazonaws.com/vrgbhel/js/jquery-3.6.0.slim.min.js";
        /// <summary>
        /// CSS to style the table if an internet conection is available
        /// </summary>
        [Tooltip("CSS to style the table if an internet conection is available")]
        //[SerializeField]
        private string m_Js = "https://s3.amazonaws.com/vrgbhel/js/jquery.dataTables.js";





        [Header("From: Debug")]
        /// <summary>
        /// CSS to style the table if an internet conection is available
        /// </summary>
        [Tooltip("CSS to style the table if an internet conection is available")]
        [SerializeField]
        private Text m_Text = null;
        /// <summary>
        /// CSS to style the table if an internet conection is available
        /// </summary>
        [Tooltip("CSS to style the table if an internet conection is available")]
        [SerializeField]
        private string m_LastMessage = string.Empty;





        [Header("FROM Remote")]
        //[SerializeField]
        private string m_Remote_Verbose = "VRG_Bhel.m_Verbose";
        [Tooltip("FROM VRG_Remote: This is the folder where the log(s) files will be saved")]
        //[SerializeField]
        private string m_Remote_AppendMode = "VRG_Bhel.m_AppendMode";
        //[SerializeField]
        private string m_Remote_DirectoryName = "VRG_Bhel.m_DirectoryName";
        //[SerializeField]
        private string m_Remote_HTML = "VRG_Bhel.m_HTML";
        //[SerializeField]
        private string m_Remote_CSV = "VRG_Bhel.m_CSV";        
        //[SerializeField]
        private string m_Remote_UIDelay = "VRG_Bhel.m_UIDelay";
        //[SerializeField]
        private string m_Remote_FloatingPointDetail = "VRG_Bhel.m_FloatingPointDetail";





        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("The date time when the log started")]
        //[SerializeField]
        private string m_DateTime = string.Empty;//"2000-01-01 00:00:01";
        private string dateTime
        {
            get
            {
                string sDateTime = m_DateTime;

                if (Instance == null)
                {
                    sDateTime = string.Format
                    (
                        "{0:yyyy-MM-dd H:mm:ss}",
                        DateTime.Now
                    );
                }
                else
                {
                    if (this.m_DateTime == string.Empty)
                    {
                        this.m_DateTime = string.Format
                        (
                            "{0:yyyy-MM-dd H:mm:ss}",
                            DateTime.Now
                        );
                    }

                    sDateTime = this.m_DateTime;
                }

                return sDateTime.Trim();
            }
        }

        [Tooltip("The date time when the log started")]
        //[SerializeField]
        private string m_FileName = string.Empty;

        [Tooltip("The stream buffer that save the log data while it is not inited")]
        //[SerializeField]
        private List<VRG_LogsBuffer> m_BufferHtml = new List<VRG_LogsBuffer>();

        [Tooltip("The stream buffer that save the log data while it is not inited")]
        //[SerializeField]
        private List<VRG_LogsBuffer> m_BufferCsv = new List<VRG_LogsBuffer>();

        [Tooltip("The row of the document")]
        //[SerializeField]
        private int m_CurrentRow = 0;

        [Tooltip("Latest update Time")]
        //[SerializeField]
        private string m_CurrentUpdateTime = "";

        [Tooltip("Latest update Color")]
        //[SerializeField]
        private string m_CurrentUpdateColor = "";



        [Tooltip("The Stream that save the file HTML")]
        //[SerializeField]
        private StreamWriter m_StreamHtml;

        [Tooltip("The Stream that save the file CSV")]
        //[SerializeField]
        private StreamWriter m_StreamCSV;


        public VRG_Bhel()
        {
            this.verbose = ENUM_Verbose.ALL;

            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = false;
        }

        /// <summary>
        /// Singleton pattern, Instance is the variable that save the data from every class.
        /// </summary>
        public static VRG_Bhel Instance;
        private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                if (this.m_VRG_UI != null)
                {
                    // ... since i am the one, I declare myself as the one
                    Instance = this;

                    // i follow my own rules
                    this.transform.SetParent(null);

                    // ... and I will not get destroyed
                    DontDestroyOnLoad(this);

                    // init the settings from the user input
                    this.SetOutput();

                    // set the colors as text
                    this.SetColors();

                    // init the logs
                    StartCoroutine(Init_IEnumerator());
                }
                else
                {
                    // if no UI interfase, report the error and do not singleton this instance
                    this.Logs("VRG_Bhel->m_UIBox is NULL, can't create the Singleton", ENUM_Verbose.ERROR);

                    // I am not the one... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (Instance != this)
                {
                    // I am not the one... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
        }

        public static IEnumerator IsValid()
        {
            yield return VRG_Bhel.IsValid(true);
        }
        /// <summary>
        /// Ask for this ienumerator if the remote is ready
        /// </summary>
        /// <returns>Null when the VRG_Bhel is ready to be queried</returns>
        public static IEnumerator IsValid(bool valueLocal)
        {
            // Let's assume everything is configured properly
            bool bContinue = true;

            // count 1 second to find the VRG_Bhel
            int i = 0;

            if (VRG_Bhel.Instance == null)
            {
                if (GameObject.FindObjectOfType<VRG_Bhel>() == null)
                {
                    bContinue = false;
                }
            }

            // If after 30 frames you can't get the VRG_Bhel object it's probable not configured
            while (VRG_Bhel.Instance == null && bContinue)
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
                while (VRG_Bhel.isReady == false)
                {
                    yield return null;
                }
            }
            else
            {
                if (valueLocal)
                {
                    print("<color=yellow>Please be sure a VRG_Bhel prefab is added to the scene</color>");
                }
            }

            // go to next frame
            yield return null;
        }

        public void SetFile(string valueLocal, int appendLocal)
        {
            this.SetFile(valueLocal, (ENUM_Append)appendLocal);
        }
        public void SetFile(string valueLocal, ENUM_Append appendLocal)
        {
            this.m_DirectoryName = valueLocal;
            this.m_AppendMode = appendLocal;
        }

        private void SetOutput()
        {
            this.SetOutput(this.m_HTML, this.m_CSV);
        }
        public void SetOutput(bool htmlLocal, bool csvLocal)
        {
            this.m_HTML = this.m_HTMLSetting = htmlLocal;
            this.m_CSV = this.m_CSVSetting = csvLocal;
        }

        private void SetColors()
        {
            this.SetColors(this.m_Font, this.m_RowDefault, this.m_RowSwitch);
        }
        public void SetColors(Color fontLocal, Color rowLocal, Color swithLocal)
        {
            this.m_ColorFont = VRG.GetHtmlFromColor(fontLocal);
            this.m_ColorRowDefault = VRG.GetHtmlFromColor(rowLocal);
            this.m_ColorRowSwitch = VRG.GetHtmlFromColor(swithLocal);
        }

        private void OnValidate()
        {
            if (this.m_IsReady)
            {
                this.SetOutput();
            }
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> init the logs
        /// </summary>
        private IEnumerator Init_IEnumerator()
        {
            // if it is uninited, then procced and remove the listener
            if (!this.m_IsReady)
            {
                //////////////////////////////////////////////////////////////
                //
                // Update local data with the remote configuration
                //
                //////////////////////////////////////////////////////////////
                // ask and wait for teh remote singleton
                yield return VRG_Remote.IsValid(false);

                // in case the singleton is real
                if (VRG_Remote.Instance != null)
                {
                    // check for the output settings
                    if (VRG_Remote.ExistBool(this.m_Remote_HTML))
                    {
                        this.m_HTML = VRG_Remote.GetBool(this.m_Remote_HTML);
                    }
                    if (VRG_Remote.ExistBool(this.m_Remote_CSV))
                    {
                        this.m_CSV = VRG_Remote.GetBool(this.m_Remote_CSV);
                    }

                    if (VRG_Remote.ExistInt(this.m_Remote_Verbose))
                    {
                        int iVerbose = VRG_Remote.GetInt(this.m_Remote_Verbose);
                        if (iVerbose >= 0 && iVerbose < System.Enum.GetValues(typeof(ENUM_Verbose)).Length)
                        {
                            // The verbose setting, 0 is muted the higher the number the verboiser
                            this.verbose = (ENUM_Verbose)System.Enum.Parse(typeof(ENUM_Verbose), ((ENUM_Verbose)iVerbose).ToString());
                        }
                        else
                        {
                            this.Logs("(" + iVerbose + ") is outside the range of values of the ENUM_Verbose", ENUM_Verbose.WARNING);
                        }
                    }
                    if (VRG_Remote.ExistInt(this.m_Remote_AppendMode))
                    {
                        int iAppend = VRG_Remote.GetInt(this.m_Remote_AppendMode);
                        if (iAppend >= 0 && iAppend < System.Enum.GetValues(typeof(ENUM_Append)).Length)
                        {
                            // The verbose setting, 0 is muted the higher the number the verboiser
                            this.m_AppendMode = (ENUM_Append)System.Enum.Parse(typeof(ENUM_Append), ((ENUM_Append)iAppend).ToString());
                        }
                        else
                        {
                            this.Logs("(" + iAppend + ") is outside the range of values of the ENUM_Append", ENUM_Verbose.WARNING);
                        }
                    }
                    if (VRG_Remote.ExistInt(this.m_Remote_FloatingPointDetail))
                    {
                        int iFloating = VRG_Remote.GetInt(this.m_Remote_FloatingPointDetail);
                        if (iFloating > 0)
                        {
                            this.m_FloatingPointDetail = iFloating;
                        }
                    }

                    if (VRG_Remote.ExistFloat(this.m_Remote_UIDelay))
                    {
                        float fDelay = VRG_Remote.GetFloat(this.m_Remote_UIDelay);

                        if (fDelay >= 0.0f)
                        {
                            this.m_UIDelay = fDelay;
                        }
                        else
                        {
                            this.m_UIDelay = 0;
                        }
                    }

                    if (VRG_Remote.ExistString(this.m_Remote_DirectoryName))
                    {
                        string sDirectoryName = VRG_Remote.GetString(this.m_Remote_DirectoryName);

                        if (sDirectoryName.Trim() != "")
                        {
                            this.m_DirectoryName = sDirectoryName;
                        }
                    }

                    // init the settings from the user input
                    this.SetOutput();
                }

                yield return null;




                // just init the stream if the remote config sets this to true
                if (this.m_Verbose > ENUM_Verbose.NONE && this.m_DirectoryName.Trim() != "")
                {
                    // create the directory
                    Directory.CreateDirectory(this.m_DirectoryName);

                    bool bAppend = true;
                    string sDateTime = "";

                    if ((int)this.m_AppendMode <= 0)
                    {
                        bAppend = false;
                    }
                    if ((int)this.m_AppendMode >= 2)
                    {
                        sDateTime = " - " + this.dateTime.Replace(":", "-");
                    }

                    this.m_FileName = this.m_DirectoryName + "/" + Application.productName + sDateTime;

                    bool bAddHeaders = true;

                    if (bAppend)
                    {
                        bool bAppendNew = true;
                        StreamReader streamReader = null;
                        try
                        {
                            streamReader = new StreamReader(this.m_FileName + ".html");
                        }
                        catch
                        {
                            bAppendNew = false;
                        }

                        bAddHeaders = !bAppendNew;

                        if (streamReader != null)
                        {
                            streamReader.Close();
                        }
                    }

                    // if there is a TRUE to save the CSV file
                    if (this.m_CSVSetting)
                    {
                        // Open the log file to append the new log to it.
                        this.m_StreamCSV = new StreamWriter
                        (
                            this.m_FileName + ".csv",
                            bAppend
                        );

                        if (bAddHeaders)
                        {
                            // init the CSV headers
                            this.m_StreamCSV.WriteLine
                            (""
                                + "Id,"
                                + "Session,"
                                + "Verbose,"
                                + "Frame,"
                                + "Time,"
                                + "Scene,"
                                + "Class,"
                                + "Message"
                            );
                        }

                        // cycle through the buffer
                        foreach (VRG_LogsBuffer child in this.m_BufferCsv)
                        {
                            // the verbose is the one defined
                            if (child.verbose <= this.m_Verbose)
                            {
                                // write the buffer if some logs where sent to the file before it was ready and inited
                                this.m_StreamCSV.WriteLine(child.value);
                            }
                        }

                        // Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
                        this.m_StreamCSV.Flush();

                        // Clears the buffer
                        this.m_BufferCsv.Clear();
                    }

                    // if there is a TRUE to save the HTML file
                    if (this.m_HTMLSetting)
                    {
                        // Open the log file to append the new log to it.
                        this.m_StreamHtml = new StreamWriter
                        (
                            this.m_FileName + ".html",
                            bAppend
                        );

                        // verbose level in words
                        string sVerboseInWords = ENUM_Verbose.ALL.ToString();

                        if (this.m_Verbose >= 0 && this.m_Verbose < ENUM_Verbose.ALL)
                        {
                            sVerboseInWords = this.m_Verbose.ToString();
                        }

                        if (bAddHeaders)
                        {
                            // init the HTML table
                            this.m_StreamHtml.WriteLine(""

+ "<!DOCTYPE html>" + this.m_Cr
+ "<html>" + this.m_Cr
+ "<head>" + this.m_Cr
+ "    <link rel=\"stylesheet\" type=\"text/css\" href=\""+ this.m_Css +"\">" + this.m_Cr
+ this.m_Cr
+ "    <style>" + this.m_Cr
+ "        div.container" + this.m_Cr
+ "        {" + this.m_Cr
+ "            width: 80%;" + this.m_Cr
+ "        }" + this.m_Cr
+ "    </style>" + this.m_Cr
+ this.m_Cr
+ "    <script src=\""+ this.m_JQuery +"\"></script>" + this.m_Cr
+ this.m_Cr
+ "    <script type=\"text/javascript\" charset=\"utf8\" src=\""+ this.m_Js +"\"></script>" + this.m_Cr
+ this.m_Cr
+ "    <script>" + this.m_Cr
+ "         $(document).ready(function () " + this.m_Cr
+ "         {" + this.m_Cr
+ "             $('#VRG_Bhel').DataTable" + this.m_Cr
+ "             ({" + this.m_Cr
+ "                 \"order\": [[1, \"asc\"], [3, \"asc\"]]" + this.m_Cr
+ "             });" + this.m_Cr
+ "         });" + this.m_Cr
+ "     </script>" + this.m_Cr
+ this.m_Cr
+ "</head>" + this.m_Cr
+ "<body>" + this.m_Cr
+ this.m_Cr
+ "<table id=\"VRG_Bhel\" class=\"display compact\" style=\"width:100%\">" + this.m_Cr
+ "     <thead>" + this.m_Cr
+ "         <tr bgcolor=black style=\"color:white\">" + this.m_Cr
+ "             <td style=\"min-width: 15px;     max-width: 15px;\">" + "<center><b>" + "Id" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 115px;    max-width: 115px;\">" + "<center><b>" + "Session" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 55px;     max-width: 55px;\">" + "<center><b>" + "Verbose" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 40px;     max-width: 40px;\">" + "<center><b>" + "Frame" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 25px;     max-width: 25px;\">" + "<center><b>" + "Time" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 130px;    max-width: 300px;\">" + "<center><b>" + "Scene" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 150px;    max-width: 300px;\">" + "<center><b>" + "Class" + "</b></center></td>" + this.m_Cr
+ "             <td style=\"min-width: 350px;\"><center><b>" + "Message" + "</b></center></td>" + this.m_Cr
+ "         </tr>" + this.m_Cr
+ "     </thead>" + this.m_Cr
+ this.m_Cr
+ "     <tbody>" + this.m_Cr

                        );}


                        // cycle through the buffer
                        foreach (VRG_LogsBuffer child in this.m_BufferHtml)
                        {
                            // the verbose is the one defined
                            if (child.verbose <= this.m_Verbose)
                            {
                                // if the time is new
                                if (child.time != Instance.m_CurrentUpdateTime)
                                {
                                    // save the new time
                                    Instance.m_CurrentUpdateTime = child.time;

                                    // change the color
                                    if (Instance.m_CurrentUpdateColor == Instance.m_ColorRowDefault)
                                    {
                                        // ... switch color
                                        Instance.m_CurrentUpdateColor = Instance.m_ColorRowSwitch;
                                    }
                                    else
                                    {
                                        // ... default color
                                        Instance.m_CurrentUpdateColor = Instance.m_ColorRowDefault;
                                    }
                                }

                                // write the buffer if some logs where sent to the file before it was ready and inited
                                this.m_StreamHtml.WriteLine(
"           <tr bgcolor=" + Instance.m_CurrentUpdateColor + ">" + this.m_Cr + child.value);
                            }
                        }

                        // Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
                        this.m_StreamHtml.Flush();

                        // Clears the buffer
                        this.m_BufferHtml.Clear();
                    }
                }

                // Ok, officially inited
                this.m_IsReady = true;
            }

            yield return null;
        }

        // close the stream and save the file
        private void OnDestroy()
        {
            // If the file was opened
            if (this.m_Verbose > ENUM_Verbose.NONE && this.m_StreamCSV != null && this.m_CSVSetting)
            {
                // flush the stream to release it
                Instance.m_StreamCSV.Flush();

                // close the file
                this.m_StreamCSV.Close();

                // ... and destroy the stream
                this.m_StreamCSV = null;
            }

            // If the file was opened
            if (this.m_Verbose > ENUM_Verbose.NONE && this.m_StreamHtml != null && this.m_HTMLSetting)
            {                
                // close the table
//                this.m_StreamHtml.WriteLine
//                (""
//+ "         </tbody>" + this.m_Cr
//+ "     </table>" + this.m_Cr
//+ "</body>" + this.m_Cr
//+ "</html>" + this.m_Cr
//                );
                
                // flush the stream to release it
                Instance.m_StreamHtml.Flush();

                // close the file
                this.m_StreamHtml.Close();

                // ... and destroy the stream
                this.m_StreamHtml = null;
            }
        }


        ///#IGNORE
        protected override IEnumerator Do() { yield return null; }
        public static void Do(string valueLocal)
        {
            Do(valueLocal, "", ENUM_Verbose.LOGS, "<font color=red><i>N/A</i></font>");
        }
        public static void Do(string valueLocal, string fromWhereLocal)
        {
            Do(valueLocal, fromWhereLocal, ENUM_Verbose.LOGS, "<font color=red><i>N/A</i></font>");
        }
        public static void Do(string valueLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            Do(valueLocal, "", ENUM_VerboseLocal, "<font color=red><i>N/A</i></font>");
        }

        /// <summary>
        /// Do the work you are supposed to do, Save logs to the file, easy and clean 
        /// </summary>
        /// <param name="valueLocal">The string message to send to the log file</param>
        /// <param name="fromWhereLocal">Helps to understand who summon the log</param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, the higher the less likely it will be to be saved</param>
        /// <param name="gameObjectLocal">The object that summons this </param>
//      public static void Do(string valueLocal, string fromWhereLocal, ENUM_Verbose ENUM_VerboseLocal, int iPaddingLocal)
        public static void Do
        (
            string valueLocal,
            string fromWhereLocal,
            ENUM_Verbose ENUM_VerboseLocal,
            string gameObjectLocal
        )
        {
            valueLocal = valueLocal.Trim();

            if (gameObjectLocal.Trim() == string.Empty)
            {
                gameObjectLocal = "<font color=red><i>N/A</i></font>";
            }

            ///////////////////////////////////////////////////////////////////
            //VRG_Bhel.Console
            //(
            //    valueLocal,
            //    fromWhereLocal,
            //    ENUM_VerboseLocal,
            //    gameObjectLocal
            //);
            ///////////////////////////////////////////////////////////////////

            bool bShowInConsole = true;

            string sCr = "\n";

            // if it is not properly inited, do nothing, remember this is a singleton
            if (Instance != null)
            {
                sCr = Instance.m_Cr;
                bShowInConsole = Instance.m_ShowInConsole;

                // Solo desplegarlo si pasa el nivel de verbosing
                if (Instance.m_Verbose < ENUM_VerboseLocal)
                {
                    bShowInConsole = false;
                }

                if (ENUM_VerboseLocal <= ENUM_Verbose.WARNING)
                {
                    bShowInConsole = true;
                }
            }
            else
            {
                // Solo desplegarlo si pasa el nivel de verbosing
                if (ENUM_VerboseLocal > ENUM_Verbose.WARNING)
                {
                    bShowInConsole = false;
                }
            }

#if UNITY_EDITOR
            if (bShowInConsole)
            {
                string sColor = VRG.GetEnumColor(ENUM_VerboseLocal);

                string sLogs = ""
                    + "<color=" + sColor + "><b>" + Time.frameCount + ") " + ENUM_VerboseLocal.ToString() + ": </b></color>" + valueLocal + sCr
                    + sCr
                    + "<color=" + sColor + "><b>Class: </b></color>" + fromWhereLocal + sCr
                    + "<color=" + sColor + "><b>Object: </b></color>" + gameObjectLocal + sCr
                    + sCr
                    + "<color=" + sColor + "><b>Time: </b></color>" + Time.time + sCr
                    ;

                switch (ENUM_VerboseLocal)
                {
                    default:
                        UnityEngine.Debug.Log(sLogs);
                        break;

                    case ENUM_Verbose.WARNING:
                        UnityEngine.Debug.LogWarning(sLogs);
                        break;

                    case ENUM_Verbose.ERROR:
                        UnityEngine.Debug.LogError(sLogs);
                        break;
                }
            }
#endif













            // if it is not properly inited, do nothing, remember this is a singleton
            if (Instance != null)
            {
                // check the verbose local against the Remote setting verbose
                if (Instance.m_Verbose >= ENUM_VerboseLocal || !Instance.m_IsReady)
                {
                    // if you are running this from the editor, it will provide the line and class and file for easier debug
#if UNITY_EDITOR
                    int iPaddingLocal = 0;
                    int iPaddingCurrent = 0;
                    bool bContinue;
                    
                    for (int i = 0; i < 20; i++)
                    {
                        // the stackframe holds the info of how it is running 
                        StackFrame stackFrame = new StackFrame(i, true);

                        if (stackFrame.GetILOffset() != StackFrame.OFFSET_UNKNOWN && stackFrame.GetILOffset() > -1)
                        {                            
                            //print(i + ") -------------------------------------");
                            //print("GetFileName() = " + stackFrame.GetFileName());
                            //print("GetMethod() = " + stackFrame.GetMethod());
                            //print("sMethod = " + stackFrame.GetMethod().Name);
                            //print("sLineMethod = " + stackFrame.GetFileLineNumber().ToString());

                            // get the info from the file, the method and the line number
                            string[] sWholeRoute = stackFrame.GetFileName().Split('/');
                            string[] sPunto = sWholeRoute[sWholeRoute.Length - 1].Split('.');
                            string sMethod = stackFrame.GetMethod().Name;
                            string sLineMethod = stackFrame.GetFileLineNumber().ToString();

                            // by default the first one is the one
                            bContinue = true;

                            // check if this method is excluded
                            foreach (string child in Instance.m_Excludes)
                            {
                                // if it is
                                if (sMethod == child)
                                {
                                    // exclude
                                    bContinue = false;

                                    // since it was found break and try next
                                    break;
                                }
                            }

                            if (sPunto[0] + "->" + sMethod == "VRG_Bhel->Do")
                            {
                                // exclude
                                bContinue = false;
                            }
                            else
                            {
                                if (iPaddingCurrent < iPaddingLocal)
                                {
                                    iPaddingCurrent++;

                                    // exclude
                                    bContinue = false;
                                }
                            }

                            // it was called from ...
                            if (bContinue)
                            {
                                // save the original sent
                                string sWhere = fromWhereLocal;

                                // get the stackframe origin
                                fromWhereLocal = sPunto[0] + "->" + sMethod + "(" + sLineMethod + ")";

                                // if it is different the found that the sent
                                if (sPunto[0] + "->" + sMethod != sWhere.Split('(')[0])
                                {
                                    // If it is from the class base
                                    if (sPunto[0] == "VRG_Base" && sMethod == "ApplyRemoteSettings")
                                    {
                                        // Repleace the namespace for easier reading format
                                        fromWhereLocal = sWhere.Replace("VrGamesDev.", "").Split('(')[0] + "(" + sLineMethod + ")";
                                    }

                                    // not the hak base
                                    else
                                    {
                                        // if it is an iterator
                                        if (sMethod == "MoveNext")
                                        {
                                            if (sWhere.Trim() != "")
                                            {
                                                // use the sent
                                                fromWhereLocal = sWhere.Split('(')[0] + "(" + sLineMethod + ")";
                                            }
                                            
                                            else
                                            {
                                                fromWhereLocal = sPunto[0] + "->" + "IEnumerator" + "(" + sLineMethod + ")";
                                            }
                                            
                                        }
                                        else
                                        {
                                            if (sWhere.Trim() != "")
                                            {
                                                // inform the disparity
                                                fromWhereLocal += "<font color=orange> | " + sWhere + "</font>";
                                            }
                                        }
                                    }
                                }

                                // since i found a class, stop searching
                                break;
                            }
                        }
                        else
                        {
                            // null, so the search is done
                            break;
                        }
                    }
#endif

                    // format the time to compare and format the color rows
                    //string sCurrentTime = Time.fixedUnscaledTime.ToString("F" + Instance.m_FloatingPointDetail.ToString());
                    string sCurrentTime = Time.time.ToString("F" + Instance.m_FloatingPointDetail.ToString());

                    // if the time is new
                    if (sCurrentTime != Instance.m_CurrentUpdateTime)
                    {
                        // save the new time
                        Instance.m_CurrentUpdateTime = sCurrentTime;

                        // change the color
                        if (Instance.m_CurrentUpdateColor == Instance.m_ColorRowDefault)
                        {
                            // ... switch color
                            Instance.m_CurrentUpdateColor = Instance.m_ColorRowSwitch;
                        }
                        else
                        {
                            // ... default color
                            Instance.m_CurrentUpdateColor = Instance.m_ColorRowDefault;
                        }
                    }







                    Instance.m_LastMessage = valueLocal;
                    if (Instance.m_Text != null)
                    {
                        Instance.m_Text.text = Instance.m_LastMessage;
                    }





                    Instance.m_CurrentRow++;

                    if (Instance.m_UIDelay > 0)                            
                    {
                        // from the random Array of this.m_Prefabs
                        GameObject UiText = Object.Instantiate(Instance.m_UIText, Instance.m_ViewPort.transform);

                        UiText.GetComponent<VRG_Delayed>().SetDelay(Instance.m_UIDelay);

                        UiText.GetComponentInChildren<Text>().text = Time.frameCount + ") ";
                        if (valueLocal == string.Empty)
                        {
                            UiText.GetComponentInChildren<Text>().text += fromWhereLocal;
                        }
                        else
                        {
                            UiText.GetComponentInChildren<Text>().text += valueLocal;
                        }
                    }

                    if (Instance.m_HTMLSetting)
                    {
                        // just save the logs if there is a route
                        if (valueLocal != "")
                        {
                            // add the needed <br>
                            valueLocal = valueLocal.Replace(Instance.m_Cr, "<br>");

                            // change the font for easier visualization in the editor
                            valueLocal = valueLocal.Replace("<color", "<font color");
                            valueLocal = valueLocal.Replace("</color>", "</font>");

                            // if it is an error, turn the message red
                            if (ENUM_VerboseLocal == ENUM_Verbose.ERROR)
                            {
                                valueLocal = "<font color=red bgcolor=black>" + valueLocal + "</font>";
                            }

                            // if it is a WARNING, turn the message yellow
                            if (ENUM_VerboseLocal == ENUM_Verbose.WARNING)
                            {
                                valueLocal = "<font color=yellow bgcolor=black>" + valueLocal + "</font>";
                            }
                        }

                        string sStream = ""
+ "             <td bgcolor=black><font color=grey><center>" + Instance.m_CurrentRow + "</center></font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + "><center><i>" + Instance.dateTime + "</i></center></font></td>" + Instance.m_Cr
+ "             <td bgcolor=#AAAAAA><font color=" + VRG.GetEnumColor(ENUM_VerboseLocal) + "><center>" + ENUM_VerboseLocal.ToString() + "</center></font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + "><center><i> " + Time.frameCount + "</i></center></font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + "><center><i> " + Instance.m_CurrentUpdateTime + "</i></center></font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + ">" + gameObjectLocal + "</font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + ">" + fromWhereLocal.Replace("->", " -> ") + "</font></td>" + Instance.m_Cr
+ "             <td><font color=" + Instance.m_ColorFont + ">" + valueLocal + "</font></td>" + Instance.m_Cr
+ "         </tr>" + Instance.m_Cr;

                        // if it is inited properly
                        if (Instance.m_IsReady)
                        {
                            // in case we are saving to the disk from Remote
                            if (Instance.m_Verbose > ENUM_Verbose.NONE)
                            {
                                // crate the row and column HTML and fill with the info provided
                                Instance.m_StreamHtml.WriteLine(""
+ "         <tr bgcolor=" + Instance.m_CurrentUpdateColor + ">" + Instance.m_Cr
+ sStream);
                                if (Instance.m_SaveOnUpdate)
                                {
                                    // ... and flush it
                                    Instance.m_StreamHtml.Flush();
                                }
                            }
                        }

                        // save it to buffer
                        else
                        {
                            // save the data to the Buffer in case we will change our mind or it is not ready
                            Instance.m_BufferHtml.Add(new VRG_LogsBuffer(Instance.m_CurrentUpdateTime, sStream, ENUM_VerboseLocal));
                        }
                    }


                    if (Instance.m_CSVSetting)
                    {
                        valueLocal = Regex.Replace(valueLocal, "<.*?>", String.Empty);

                        string sCsv = ""
                                    + Instance.m_CurrentRow + ","
                                    + Instance.dateTime + ","
                                    + ENUM_VerboseLocal.ToString() + ","
                                    + Time.frameCount + ","
                                    + Instance.m_CurrentUpdateTime + ","
                                    + gameObjectLocal + ","
                                    + String.Format("\"{0}\"", fromWhereLocal.Replace("\"", "\"" + "\"")) + ","
                                    + String.Format("\"{0}\"", valueLocal.Replace("\"", "\"" + "\""));

                        // if it is inited properly
                        if (Instance.m_IsReady)
                        {
                            // in case we are saving to the disk from Remote
                            if (Instance.m_Verbose > ENUM_Verbose.NONE)
                            {
                                // crate the row and column HTML and fill with the info provided
                                Instance.m_StreamCSV.WriteLine(sCsv);

                                if (Instance.m_SaveOnUpdate)
                                {
                                    // ... and flush it
                                    Instance.m_StreamCSV.Flush();
                                }
                            }
                        }

                        // save it to buffer
                        else
                        {
                            // save the data to the Buffer in case we will change our mind or it is not ready
                            Instance.m_BufferCsv.Add(new VRG_LogsBuffer(Instance.m_CurrentUpdateTime, sCsv, ENUM_VerboseLocal));
                        }
                    }
                }
            }
        }



        public void OpenUrl()
        {
            string sWWW = "";

            string sFolderCompile = "Assets";

#if UNITY_STANDALONE_OSX
            sFolderCompile = ".app";
#endif

#if UNITY_STANDALONE_WIN
            sFolderCompile = ".exe";
#endif

            string[] sWWWSlash = Application.dataPath.Split(new string[] { sFolderCompile }, StringSplitOptions.None)[0].Split(new string[] { "/" }, StringSplitOptions.None);

            for (int i = 0; i < sWWWSlash.Length-1; i++)
            {
                sWWW += sWWWSlash[i] + "/";
            }

            sWWW = "file://" + sWWW + this.m_FileName + ".html";

            Application.OpenURL(System.Uri.EscapeUriString(sWWW));
        }


    }
}
/*/
using System.Collections;

using UnityEngine;

/// <summary>
/// VRG_BHEL Namespace, Beautiful Html Enhanced Logs, let get you the most useful
/// and well documented logs to track bugs, or follow the flow of your game.
/// You can custom:<br></br>
/// - The level of verbosity,
/// - Where and how they will be saved,
/// - and the Html visuals,<br></br>
/// By default is in a preconfigured folder. You can find the Logs, in the
/// “[Your-project's-name] / LogsLocal / [Your-project's-name].html"
///
/// You can download it from <a href="https://assetstore.unity.com/packages/slug/186164">here</a>
/// </summary>
namespace VrGamesDev.BHEL
{
    /// <summary>
    /// Colored debug simplified
    /// </summary>
    public class VRG_Bhel : VRG_Base
    {
        public VRG_Bhel()
        {
            this.verbose = ENUM_Verbose.ALL;

            this.m_PlayOnEnable = true;
            this.m_NextFrame = false;
            this.m_SelfTurnOff = false;
        }

        /// <summary>
        /// Singleton pattern, Instance is the variable that save the data from every class.
        /// </summary>
        public static VRG_Bhel Instance;
        private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will not get destroyed
                DontDestroyOnLoad(this);
            }
            else
            {
                if (Instance != this)
                {
                    // I am not the one... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
        }

        public static IEnumerator IsValid() { yield return VRG_Bhel.IsValid(true); }
        public static IEnumerator IsValid(bool valueLocal) { yield return null; }
        public void OpenUrl() { }


        ///#IGNORE
        protected override IEnumerator Do() { yield return null; }
        public static void Do(string valueLocal)
        {
            Do(valueLocal, "", ENUM_Verbose.LOGS, "<font color=red><i>N/A</i></font>");
        }
        public static void Do(string valueLocal, string fromWhereLocal)
        {
            Do(valueLocal, fromWhereLocal, ENUM_Verbose.LOGS, "<font color=red><i>N/A</i></font>");
        }
        public static void Do(string valueLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            Do(valueLocal, "", ENUM_VerboseLocal, "<font color=red><i>N/A</i></font>");
        }

        /// <summary>
        /// Do the work you are supposed to do, send the message to the console, easy and clean 
        /// </summary>
        /// <param name="valueLocal">The string message to send to the log file</param>
        /// <param name="fromWhereLocal">Helps to understand who summon the log</param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, the higher the less likely it will be to be saved</param>
        /// <param name="gameObjectLocal">The object that summons this </param>
//      public static void Do(string valueLocal, string fromWhereLocal, ENUM_Verbose ENUM_VerboseLocal, int iPaddingLocal)
        public static void Do
        (
            string valueLocal,
            string fromWhereLocal,
            ENUM_Verbose ENUM_VerboseLocal,
            string gameObjectLocal
        )
        {
            if (gameObjectLocal.Trim() == string.Empty)
            {
                gameObjectLocal = "<font color=red><i>N/A</i></font>";
            }
            
            bool bShowInConsole = true;

            string sCr = "\n";

            // if it is not properly inited, do nothing, remember this is a singleton
            if (Instance != null)
            {
                // Solo desplegarlo si pasa el nivel de verbosing
                if (Instance.m_Verbose < ENUM_VerboseLocal)
                {
                    bShowInConsole = false;
                }

                if (ENUM_VerboseLocal <= ENUM_Verbose.WARNING)
                {
                    bShowInConsole = true;
                }

            }
            else
            {
                // Solo desplegarlo si pasa el nivel de verbosing
                if (ENUM_VerboseLocal > ENUM_Verbose.WARNING)
                {
                    bShowInConsole = false;
                }
            }

#if UNITY_EDITOR
            if (bShowInConsole)
            {
                string sColor = VRG.GetEnumColor(ENUM_VerboseLocal);

                string sLogs = ""
                    + "<color=" + sColor + "><b>" + Time.frameCount + ") " + ENUM_VerboseLocal.ToString() + ": </b></color>" + valueLocal + sCr + sCr
                    + "<color=" + sColor + "><b>Class: </b></color>" + fromWhereLocal + sCr
                    + "<color=" + sColor + "><b>Object: </b></color>" + gameObjectLocal + sCr
                    ;

                switch (ENUM_VerboseLocal)
                {
                    default:
                        UnityEngine.Debug.Log(sLogs);
                        break;

                    case ENUM_Verbose.WARNING:
                        UnityEngine.Debug.LogWarning(sLogs);
                        break;

                    case ENUM_Verbose.ERROR:
                        UnityEngine.Debug.LogError(sLogs);
                        break;
                }
            }
#endif            
        }
    }
}
//*/





/*
// Open the log file to append the new log to it.
this.m_OutputStream = new StreamWriter
(
    string.Format
    (
        "{0} - {1:yyyy-MM-dd H-mm-ss}.html",
        this.m_DirectoryName + "/" + Application.productName,
        DateTime.Now
    ),
    true
);
//*/