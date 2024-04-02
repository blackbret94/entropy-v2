using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

using Object = UnityEngine.Object;



///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor : VRG_Base
    {
        public static string m_InstallationPath = "Assets/_VrGamesDev/";

        public static string m_Prefabs = "Tools/CORE/Prefabs/";

#if UNITY_EDITOR_OSX
        public static string m_OS_Separator = "/";
#else
        public static string m_OS_Separator = "\\";
#endif

        [MenuItem("Tools/Vr Games Dev/Installation path", false, 1)]
        public static void Installation_path() => print("The installation path of Vr Games Dev Packages is: " + CalculateInstallationPath());


        [MenuItem("Tools/Vr Games Dev/Examples/Instructions to use examples", false, 99900)]
        public static void Example_99900()
        {
            if (EditorUtility.DisplayDialog("Vr Games Dev Examples", "May edit or add scenes to the build settings, and/or move some assets into the addressables groups.\n\n Save your current work before loading any example.", "Yes, I got it", "No"))
            {
            }
        }

        [MenuItem("Tools/Vr Games Dev/About", false, 999999)]
        public static void About()
        {
            // Get existing open window or if none, make a new one:
            VRG_WindowStatus window = (VRG_WindowStatus)EditorWindow.GetWindow(typeof(VRG_WindowStatus), false, "About VrGamesDev");

            /*
            window.maxSize = new Vector2(325f, 450f);
            window.minSize = window.maxSize;
            */

            window.Show();
        }



        public static string CalculateInstallationPath()
        {
            string[] stringSeparators = new string[] { "/Assets/" };

            // the stackframe holds the info of how it is running 
            StackFrame stackFrame = new StackFrame(0, true);

            string[] result = stackFrame.GetFileName().Split(stringSeparators, StringSplitOptions.None);

            if (result.Length == 2)
            {
                stringSeparators = new string[] { "_VrGamesDev/" };

                result = result[1].Split(stringSeparators, StringSplitOptions.None);

                if (result.Length == 2)
                {
                    m_InstallationPath = "Assets/" + result[0] + "_VrGamesDev/";
                }
            }

            return m_InstallationPath;
        }



        public static GameObject CreatePrefab(string valueLocal)
        {
            return CreatePrefab(valueLocal, false, null);
        }
        public static GameObject CreatePrefab(string valueLocal, bool forceUnparent)
        {
            return CreatePrefab(valueLocal, forceUnparent, null);
        }
        public static GameObject CreatePrefab(string valueLocal, Transform parentLocal)
        {
            return CreatePrefab(valueLocal, false, parentLocal);
        }
        public static GameObject CreatePrefab(string valueLocal, bool forceUnparent, Transform parentLocal)
        {
            GameObject go_Return = null;
            if (valueLocal.Trim() != "")
            {
                Object object_Undo;
                string sPath = CalculateInstallationPath() + valueLocal + ".prefab";

                Object prefab = AssetDatabase.LoadAssetAtPath(sPath, typeof(GameObject));

                if (prefab != null)
                {
                    if (parentLocal == null)
                    {
                        if (Selection.activeTransform == null || forceUnparent)
                        {
                            object_Undo = PrefabUtility.InstantiatePrefab(prefab, null);
                        }
                        else
                        {
                            object_Undo = PrefabUtility.InstantiatePrefab(prefab, Selection.activeTransform.gameObject.transform);
                        }
                    }
                    else
                    {
                        object_Undo = PrefabUtility.InstantiatePrefab(prefab, parentLocal);
                    }

                    go_Return = object_Undo as GameObject;

                    string[] sFinalSplit = valueLocal.Split('/');

                    Undo.RegisterCreatedObjectUndo(object_Undo, sFinalSplit[sFinalSplit.Length - 1]);

                    print("Adding " + sFinalSplit[sFinalSplit.Length - 1]);
                }
                else
                {
                    print("<color=red>ERROR: " + sPath + " NOT FOUND</color>, please check " + CalculateInstallationPath() + " installation.");
                }
            }
            else
            {
                print("<color=red>ERROR: No m_PrefabToLoad defined</color>");
            }

            return go_Return;
        }



        public static void CreateVRG_EventSystem()
        {
            if (Object.FindObjectsOfType<VRG_EventSystem>().Length == 0)
            {
                CreatePrefab(m_Prefabs + "VRG_EventSystem", true);
            }
        }

        public static void CreateVRG_SkinPool()
        {
            if (Object.FindObjectsOfType<VRG_SkinPool>().Length == 0)
            {
                CreatePrefab(m_Prefabs + "VRG_SkinPool", true);
            }
        }

        protected static GameObject CreatePrefabInCanvas(string valueLocal) => CreatePrefabInCanvas(valueLocal, true);
        protected static GameObject CreatePrefabInCanvas(string valueLocal, bool extrasLocal)
        {
            bool bReturn = false;
            bool bContinue = true;

            GameObject go_Return = null;

            if (Selection.activeTransform != null)
            {
                Canvas myCanvas;
                GameObject go_Canvas = Selection.activeTransform.gameObject;

                while (bContinue && go_Canvas != null)
                {
                    myCanvas = go_Canvas.transform.GetComponent<Canvas>();

                    if (myCanvas != null)
                    { 
                        bReturn = true;
                        bContinue = false;
                    }
                    else
                    {
                        if (go_Canvas.transform.parent != null)
                        {
                            go_Canvas = go_Canvas.transform.parent.gameObject;
                        }
                        else
                        {
                            bContinue = false;
                        }
                    }
                }
            }

            if (bReturn)
            {
                CreatePrefab(valueLocal);
            }
            else
            {
                GameObject go_UI = CreatePrefab(m_Prefabs + "VRG_UI", true);

                CreatePrefab(valueLocal, go_UI.transform);
            }

            if (extrasLocal)
            {
                if (Object.FindObjectsOfType<VRG_Managers>().Length == 0)
                {
                    CreateVRG_EventSystem();

                    CreateVRG_SkinPool();
                }
            }

            return go_Return;
        }



        public static VRG_Remote CreateRemote(string valueLocal)
        {
            bool IsNew = true;
            VRG_Remote go_Return = null;
            VRG_Remote[] go_Returns = GameObject.FindObjectsOfType<VRG_Remote>();

            foreach (VRG_Remote child in go_Returns)
            {
                if (child.id == valueLocal)
                {
                    IsNew = false;
                    break;
                }
            }

            if (IsNew)
            {
                GameObject go_InScene = CreatePrefab(m_Prefabs + "VRG_Remote", true);
                go_InScene.GetComponent<VRG_Remote>().id = valueLocal;

                go_Returns = GameObject.FindObjectsOfType<VRG_Remote>();
                foreach (VRG_Remote child in go_Returns)
                {
                    if (child.id == valueLocal)
                    {
                        go_Return = child;
                        break;
                    }
                }
            }
            else
            {
                print("<color=red>ERROR: </color> There is already a VRG_Remote - (" + valueLocal + ") object in the scene");
            }

            return go_Return;
        }


        public static void LoadScene(string valueLocal)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(CalculateInstallationPath() + valueLocal + ".unity");
            }
        }

        public static void ClearBuildSettings()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
           
            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        public static void AddScenesToBuildSettings(string[] valueLocal) => AddScenesToBuildSettings(valueLocal, true);
        public static void AddScenesToBuildSettings(string[] valueLocal, bool bFirstLocal)
        {
            if (valueLocal.Length > 0)
            {
                List<string> sceneList = new List<string>();

                if (bFirstLocal)
                {
                    for (int i = 0; i < valueLocal.Length; i++)
                    {
                        if (valueLocal[i].Trim() != string.Empty)
                        {
                            if (!sceneList.Contains(CalculateInstallationPath() + valueLocal[i] + ".unity"))
                            {
                                sceneList.Add(CalculateInstallationPath() + valueLocal[i] + ".unity");
                            }
                        }
                    }
                }

                foreach (EditorBuildSettingsScene child in EditorBuildSettings.scenes)
                {
                    if (!sceneList.Contains(child.path))
                    {
                        sceneList.Add(child.path);
                    }
                }


                if (!bFirstLocal)
                {
                    for (int i = 0; i < valueLocal.Length; i++)
                    {
                        if (valueLocal[i].Trim() != string.Empty)
                        {
                            if (!sceneList.Contains(CalculateInstallationPath() + valueLocal[i] + ".unity"))
                            {
                                sceneList.Add(CalculateInstallationPath() + valueLocal[i] + ".unity");
                            }
                        }
                    }
                }

                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                for (int i = 0; i < sceneList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(sceneList[i]))
                    {
                        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneList[i], true));
                    }
                }

                // Set the Build Settings window Scene list
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }
        }

        public static void RemoveScenesFromoBuildSettings(string[] valueLocal)
        {
            if (valueLocal.Length > 0)
            {
                List<string> sceneList = new List<string>();

                Array.Sort(valueLocal);

                foreach (EditorBuildSettingsScene child in EditorBuildSettings.scenes)
                {
                    if (Array.BinarySearch(valueLocal, ((child.path.Replace(".unity", "")).Replace(CalculateInstallationPath(), "")).Trim()) < 0)
                    {
                        sceneList.Add(child.path);
                    }
                }

                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                for (int i = 0; i < sceneList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(sceneList[i]))
                    {
                        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneList[i], true));
                    }
                }

                // Set the Build Settings window Scene list
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }
        }
    }



    public class VRG_WindowStatus : EditorWindow
    {
        private static float m_Width;
        private static float m_Height;
        private static Texture2D m_Logo;
        private static GUIStyle m_StyleWrap;
        private static GUIStyle m_StylePadding;
        private static GUIStyle m_StyleLink;

        void Awake()
        {
            m_Logo = (Texture2D)AssetDatabase.LoadAssetAtPath(VRG_Editor.CalculateInstallationPath() + "Tools/CORE/Sprites/VrGamesDev.png", typeof(Texture2D));

            m_Width = 1200.0f;
            m_Height = 800.0f;

            position = new Rect(0, 00, 325, 500);
        }


        void OnGUI()
        {
            m_StyleWrap = new GUIStyle(GUI.skin.label);
            m_StyleWrap.wordWrap = true;

            m_StylePadding = new GUIStyle(GUI.skin.label);
            m_StylePadding.padding = new RectOffset(0, 0, 150, 0);

            m_StyleLink = new GUIStyle(EditorStyles.label);
            m_StyleLink.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            m_StyleLink.stretchWidth = false;


            GUI.DrawTexture(new Rect((this.position.width / 2) - (m_Width / 12), 10, (m_Width / 6), (m_Height / 6)), m_Logo);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("... because the future is in VR", m_StylePadding);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("We are a couple that make games since 1996, we have made 20+ games, currently creating a mobile Idle RPG, and a Virtual Reality FPS exploration, we love games, technology and to improve and enhance tools. \nFeel free to talk with us about anything, and enjoy our assets.", m_StyleWrap);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Support: ", m_StyleWrap);
            if (GUILayout.Button("unity.support@vrgamesdev.com", m_StyleLink))
            {
                Application.OpenURL("mailto:unity.support@vrgamesdev.com");
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Website: ", m_StyleWrap);
            if (GUILayout.Button("https://www.vrgamesdev.com", m_StyleLink))
            {
                Application.OpenURL("https://www.vrgamesdev.com");
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Enjoy our other assets from the asset store.", m_StyleWrap);
            if (GUILayout.Button("https://assetstore.unity.com/publishers/49114", m_StyleLink))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/49114");
            }
            GUILayout.FlexibleSpace();
        }
    }
}