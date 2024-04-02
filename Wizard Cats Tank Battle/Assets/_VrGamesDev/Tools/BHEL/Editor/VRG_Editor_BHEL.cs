//*
using UnityEditor;
using UnityEngine;

using VrGamesDev.BHEL;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_BHEL : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/BHEL/Prefabs/";

        [MenuItem("Tools/Vr Games Dev/BHEL", false, 10001)]
        public static void VRG_Bhel_Window()
        {
            // Get existing open window or if none, make a new one:
            VRG_WindowBHEL window = (VRG_WindowBHEL)EditorWindow.GetWindow(typeof(VRG_WindowBHEL), false, "Add a BHEL module", true);

            window.maxSize = new Vector2(325f, 380f);
            window.minSize = window.maxSize;

            window.Show();
        }

        public static void AddVRG_Bhel() => VRG_Editor_BHEL.AddVRG_Bhel((int)ENUM_Verbose.ALL, "BHEL", 0, true, false, true, 0.0f, 6);
        public static void AddVRG_Bhel
        (
            int verboseLocal,
            string directoryLocal,
            int appendLocal,
            bool htmlLocal,
            bool csvLocal,
            bool remoteLocal,
            float uiDelayLocal,
            int floatingPointLocal
        )
        {
            VRG_Bhel inScene_VRG_Bhel = GameObject.FindObjectOfType<VRG_Bhel>();
            if (inScene_VRG_Bhel == null)
            {
                VRG_Editor_BHEL.CreatePrefab(VRG_Editor_BHEL.m_Prefabs + "VRG_Bhel", true);
                inScene_VRG_Bhel = GameObject.FindObjectOfType<VRG_Bhel>();

                inScene_VRG_Bhel.verbose = (ENUM_Verbose)System.Enum.Parse(typeof(ENUM_Verbose), ((ENUM_Verbose)verboseLocal).ToString());
                inScene_VRG_Bhel.SetFile(directoryLocal, appendLocal);
                inScene_VRG_Bhel.SetOutput(htmlLocal, csvLocal);
                inScene_VRG_Bhel.uiDelay = uiDelayLocal;
                inScene_VRG_Bhel.floatingPointDetail = floatingPointLocal;
            }
            else
            {
                Debug.Log("<color=red>ERROR: </color> There is already a VRG_BHEL object in the scene");
            }

            if (remoteLocal)
            {
                VRG_Remote go_Remote = VRG_Editor_BHEL.CreateRemote("VRG_Bhel");

                if (go_Remote != null)
                {
                    go_Remote.AddBool("VRG_Bhel.m_HTML", htmlLocal);
                    go_Remote.AddBool("VRG_Bhel.m_CSV", csvLocal);

                    go_Remote.AddInt("VRG_Bhel.m_Verbose", verboseLocal);
                    go_Remote.AddInt("VRG_Bhel.m_AppendMode", appendLocal);
                    go_Remote.AddInt("VRG_Bhel.m_FloatingPointDetail", floatingPointLocal);

                    go_Remote.AddFloat("VRG_Bhel.m_UIDelay", uiDelayLocal);

                    go_Remote.AddString("VRG_Bhel.m_DirectoryName", directoryLocal);
                }
            }
        }
    }



    public class VRG_WindowBHEL : EditorWindow
    {
        private GUIStyle m_StyleWrap;

        private int m_VerbositySelected = (int) ENUM_Verbose.ALL;

        private int m_AppendMode = 0;

        private string m_DirectoryName = "BHEL";

        private bool m_ToggleHtml = true;
        private bool m_ToggleCsv = true;

        private float m_UIDelay = 0.0f;
        private int m_FloatingPoint = 6;
        

        private bool m_ToggleRemote = false;

        void OnGUI()
        {
            this.m_StyleWrap = new GUIStyle(GUI.skin.label);
            this.m_StyleWrap.wordWrap = true;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Beautiful Html Enhanced Logs, allows you the get well documented logs to track bugs, or follow the flow of your game.", this.m_StyleWrap);

            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.m_VerbositySelected = EditorGUILayout.Popup("Verbosity's level: ", this.m_VerbositySelected, System.Enum.GetNames(typeof(ENUM_Verbose)));


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.m_AppendMode = EditorGUILayout.Popup("Append Mode: ", this.m_AppendMode, System.Enum.GetNames(typeof(ENUM_Append)));


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.m_DirectoryName = EditorGUILayout.TextField("Save Folder", this.m_DirectoryName);


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI Delay:");
            GUILayout.FlexibleSpace();
            this.m_UIDelay = EditorGUILayout.Slider(this.m_UIDelay, 0.0f, 9.9f, GUILayout.Width(200.0f));
            EditorGUILayout.EndHorizontal();
            

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Floating point detail:");
            GUILayout.FlexibleSpace();
            this.m_FloatingPoint = int.Parse(EditorGUILayout.IntSlider(this.m_FloatingPoint, 0, 6, GUILayout.Width(200.0f)).ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Output Settings:");
            GUILayout.FlexibleSpace();
            this.m_ToggleHtml = GUILayout.Toggle(this.m_ToggleHtml, "  HTML ");
            this.m_ToggleCsv = GUILayout.Toggle(this.m_ToggleCsv, " CSV ");
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Do you want to add Remote Config support?", this.m_StyleWrap))
            {
                this.m_ToggleRemote = !this.m_ToggleRemote;
            }

            GUILayout.FlexibleSpace();
            this.m_ToggleRemote = GUILayout.Toggle(this.m_ToggleRemote, "");
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create a BHEL register", GUILayout.Width(300), GUILayout.Height(30)))
            {
                VRG_Editor_BHEL.AddVRG_Bhel
                (
                    this.m_VerbositySelected,
                    this.m_DirectoryName,
                    this.m_AppendMode,
                    this.m_ToggleHtml,
                    this.m_ToggleCsv,
                    this.m_ToggleRemote,
                    this.m_UIDelay,
                    this.m_FloatingPoint
                );


                this.Close();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}
/*/
using UnityEditor;
using UnityEngine;

using VrGamesDev.BHEL;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_BHEL : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/BHEL/Prefabs/";

        [MenuItem("Tools/Vr Games Dev/BHEL", false, 10001)]
        public static void VRG_Bhel() => AddVRG_Bhel();

        public static void AddVRG_Bhel()
        {
            VRG_Bhel inScene_VRG_Bhel = GameObject.FindObjectOfType<VRG_Bhel>();
            if (inScene_VRG_Bhel == null)
            {
                CreatePrefab(m_Prefabs + "VRG_Bhel", true);
            }
            else
            {
                Debug.Log("<color=red>ERROR: </color> There is already a VRG_BHEL object in the scene");
            }
        }

    }
}
//*/