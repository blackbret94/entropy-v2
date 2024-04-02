using System.IO;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using UnityEditor.SceneManagement;


///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    /// <summary>
    ///  It just add menu items under tools/VR Games Dev and calculate some 
    ///  data shared among all the classes
    ///  
    ///  0000 Data and information
    ///  1000 UI
    ///  2000 System and General
    ///  3000 Remote
    ///  
    /// </summary>

    public class VRG_Editor_CORE : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/CORE/Prefabs/";

        public static string m_Scenes = "Tools/CORE/Scenes/";






        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/Vr Games Dev/CORE/Game: Create new ", false, 1001)]
        static void Create_new_game()
        {
            // Get existing open window or if none, make a new one:
            VRG_WindowCreateNewGame window = (VRG_WindowCreateNewGame)EditorWindow.GetWindow(typeof(VRG_WindowCreateNewGame), false, "Create a new Game: Campaign", true);

            window.maxSize = new Vector2(325f, 180f);
            window.minSize = window.maxSize;

            window.Show();
        }
        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/Vr Games Dev/CORE/Scene: Create new", false, 1002)]
        static void Create_new_scene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

                bool bManagers = VRG_Managers(false, false);

                if (bManagers)
                {
                    VRG_Managers(true, false);

                    Html(!bManagers);

                    Main_Camera___Feedback();
                }
                else
                {
                    Main_Camera___Feedback();

                    Html(!bManagers);
                }


            }

        }





        [MenuItem("Tools/Vr Games Dev/CORE/UI/Main Camera: Feedback", false, 2001)]
        public static void Main_Camera___Feedback() => CreatePrefab(m_Prefabs + "UI/Main Camera - Feedback", true);

        [MenuItem("Tools/Vr Games Dev/CORE/UI/Html: (Header, Body, Footer)", false, 2102)]
        public static void Html() => Html(true);
        public static void Html(bool valueLocal) => CreatePrefabInCanvas(m_Prefabs + "UI/html", valueLocal);

        [MenuItem("Tools/Vr Games Dev/CORE/UI/PopUp: Default", false, 2103)]
        public static void PopUp___Neutral() => CreatePrefabInCanvas(m_Prefabs + "UI/PopUp - Default");



        [MenuItem("Tools/Vr Games Dev/CORE/UI/Button: Basic", false, 2201)]
        public static void Basic_Button() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_Button - Basic");

        [MenuItem("Tools/Vr Games Dev/CORE/UI/Button: Icon", false, 2202)]
        public static void Icon_Button() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_Button - Icon");

        [MenuItem("Tools/Vr Games Dev/CORE/UI/Button: Label and Icon", false, 2203)]
        public static void Label_Icon_And_Button() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_Button - Label and Icon");



        [MenuItem("Tools/Vr Games Dev/CORE/UI/Graphical Number: Basic", false, 2301)]
        public static void Basic_Graphical_Numbe() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_GraphicalNumber");

        [MenuItem("Tools/Vr Games Dev/CORE/UI/Graphical Number: Animated", false, 2302)]
        public static void Animated_Number() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_GraphicalNumber - Animated");

        [MenuItem("Tools/Vr Games Dev/CORE/UI/Graphical Number: BackWard Countdown", false, 2303)]
        public static void BackWard_Countdown_Number() => CreatePrefabInCanvas(m_Prefabs + "UI/" + "VRG_GraphicalNumber - BackWardCountdown");
















        [MenuItem("Tools/Vr Games Dev/CORE/Scene Managment/VRG_GoToScene", false, 3001)]
        public static void VRG_GoToScene() => CreatePrefab(m_Prefabs + "Scene Managment/VRG_GoToScene");

        [MenuItem("Tools/Vr Games Dev/CORE/Scene Managment/VRG_Managers", false, 3002)]
        public static bool VRG_Managers()
        {
            return VRG_Managers(true, true);
        }
        public static bool VRG_Managers(bool valueLocal, bool printLocal)
        {
            bool bContinue = false;

            if (Object.FindObjectsOfType<VRG_Managers>().Length == 0)
            {
                foreach (EditorBuildSettingsScene child in EditorBuildSettings.scenes)
                {
                    if (child.path.Contains("VRG_Managers"))
                    {
                        bContinue = true;
                        break;
                    }
                }

                if (bContinue)
                {
                    if (valueLocal)
                    {
                        CreatePrefab(m_Prefabs + "Scene Managment/VRG_Managers", true);
                    }
                }
                else
                {
                    if (printLocal)
                    {
                        print("<color=red>ERROR: </color>There aren't any VRG_Managers Scene in the build settings, please add scene");
                    }
                }
            }
            else
            {
                if (printLocal)
                {
                    print("<color=red>ERROR: </color>You already have a VRG_Managers object in the scene");
                }
            }

            return bContinue;
        }




        [MenuItem("Tools/Vr Games Dev/CORE/Announcement/UI Icon: VRG_Announcement", false, 3011)]
        public static void Announcement_News() => CreatePrefabInCanvas(m_Prefabs + "Announcement/" + "VRG_Announcement - Icon");

        [MenuItem("Tools/Vr Games Dev/CORE/Announcement/PopUp: VRG_Announcement", false, 3012)]
        public static void VRG_Announcement()
        {
            CreateVRG_EventSystem();

            CreateVRG_SkinPool();

            VRG_Announcement inScene_VRG_Announcement = GameObject.FindObjectOfType<VRG_Announcement>();

            if (inScene_VRG_Announcement == null)
            {
                CreatePrefab(m_Prefabs + "Announcement/" + "VRG_Announcement", true);
                inScene_VRG_Announcement = GameObject.FindObjectOfType<VRG_Announcement>();
            }
            else
            {
                Debug.Log("<color=red>ERROR: </color> There is already a VRG_Announcement object in the scene");
            }

            VRG_Editor_CORE_VRG_Remote.Add_VRG_Remote_VRG_Announcement();
        }





        [MenuItem("Tools/Vr Games Dev/CORE/Skins/UI Icon: VRG_SkinPool", false, 3021)]
        static void Create_VRG_SkinPool___Icon() => CreatePrefabInCanvas(m_Prefabs + "Skin/" + "VRG_SkinPool - Icon");

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Custom", false, 3201)]
        static void Create_VRG_Skin()
        {
            // Get existing open window or if none, make a new one:
            VRG_WindowVRG_Skin window = (VRG_WindowVRG_Skin)EditorWindow.GetWindow(typeof(VRG_WindowVRG_Skin), false, "Create a custom VRG_Skin");

            window.maxSize = new Vector2(325f, 180f);
            window.minSize = window.maxSize;

            window.Show();
        }

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Elysium", false, 3301)]
        static void Elysium() => AddSkinToPool(m_Prefabs + "Skin/" + "VRG_Skin - Elysium");

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Gaia", false, 3302)]
        static void Gaia() => AddSkinToPool(m_Prefabs + "Skin/" + "VRG_Skin - Gaia");

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Inferno", false, 3302)]
        static void Inferno() => AddSkinToPool(m_Prefabs + "Skin/" + "VRG_Skin - Inferno");

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Limbo", false, 3304)]
        static void Limbo() => AddSkinToPool(m_Prefabs + "Skin/" + "VRG_Skin - Limbo");

        [MenuItem("Tools/Vr Games Dev/CORE/Skins/Add skin: Metropolis", false, 3305)]
        static void Metropolis() => AddSkinToPool(m_Prefabs + "Skin/" + "VRG_Skin - Metropolis");


















        [MenuItem("Tools/Vr Games Dev/CORE/Utils//VRG_SessionData", false, 4001)]
        public static void VRG_SessionData() => CreatePrefab(m_Prefabs + "Utils/VRG_SessionData");

        [MenuItem("Tools/Vr Games Dev/CORE/Utils/VRG_FPS: Frames Per Second", false, 4002)]
        public static void VRG_FPS() => CreatePrefab(m_Prefabs + "Utils/VRG_FPS", true);

        [MenuItem("Tools/Vr Games Dev/CORE/Utils//VRG_OpenUrl", false, 4003)]
        public static void VRG_OpenUrl() => CreatePrefab(m_Prefabs + "Utils/VRG_OpenUrl");












































        protected static GameObject AddSkinToPool(string valueLocal)
        {
            GameObject go_Return = null;

            CreateVRG_SkinPool();

            VRG_SkinPool[] go_SkinPool = GameObject.FindObjectsOfType<VRG_SkinPool>();
            if (go_SkinPool.Length == 1)
            {
                CreatePrefab(valueLocal, go_SkinPool[0].transform);
            }
            else
            {
                print("<color=red>ERROR: </color> Can't create a VRG_SkinPool prefab");
            }

            return go_Return;
        }
    }












    public class VRG_WindowVRG_Skin : EditorWindow
    {
        private GUIStyle m_StyleWrap;

        private string m_SkinName = "Custom";

        void OnGUI()
        {
            this.m_StyleWrap = new GUIStyle(GUI.skin.label);
            this.m_StyleWrap.wordWrap = true;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("You will add a custom skin with the name below, If there isn't a skinpool, It will add one.", this.m_StyleWrap);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //GUILayout.Label("VRG_Skin settings", EditorStyles.boldLabel);
            this.m_SkinName = EditorGUILayout.TextField("Skin name", this.m_SkinName);

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create new", GUILayout.Width(300), GUILayout.Height(30)))
            {
                VRG_Editor_CORE.CreateVRG_SkinPool();

                VRG_SkinPool[] go_SkinPool = GameObject.FindObjectsOfType<VRG_SkinPool>();
                if (go_SkinPool.Length > 0)
                {
                    GameObject go_VRG_Skin = VRG_Editor_CORE.CreatePrefab(VRG_Editor_CORE.m_Prefabs + "Skin/" + "VRG_Skin", go_SkinPool[0].transform);

                    go_VRG_Skin.name = "VRG_Skin - " + this.m_SkinName;

                    foreach (Transform child in go_VRG_Skin.transform)
                    {
                        VRG_SessionData childSession = child.GetComponent<VRG_SessionData>();
                        if (childSession != null)
                        {
                            childSession.value = this.m_SkinName;
                        }

                        VRG_SkinApply childApply = child.GetComponent<VRG_SkinApply>();
                        if (childApply != null)
                        {
                            childApply.sessionData = this.m_SkinName;
                        }
                    }
                }

                this.Close();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }












    public class VRG_WindowCreateNewGame : EditorWindow
    {
        private GUIStyle m_StyleWrap;

        private string m_FolderName = "_Game";

        void OnGUI()
        {
            int i = 0;

            this.m_StyleWrap = new GUIStyle(GUI.skin.label);
            this.m_StyleWrap.wordWrap = true;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("You will get a folder with all the scenes needed to create a new game, with the folder name you provide.", this.m_StyleWrap);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            this.m_FolderName = EditorGUILayout.TextField("Game name", this.m_FolderName);

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            if (GUILayout.Button("Create new", GUILayout.Width(300), GUILayout.Height(30)))
            {
                string sNewGamePath = "Assets/" + this.m_FolderName;
                Directory.CreateDirectory(sNewGamePath);

                sNewGamePath = sNewGamePath + "/Scenes/";
                Directory.CreateDirectory(sNewGamePath);


                string sOldPath = VRG_Editor_CORE.CalculateInstallationPath() + "Tools/CORE/Scenes/";

                List<string> aScene = new List<string>();

                // Scenes
                aScene.Add("Campaign.unity");
                aScene.Add("Home.unity");
                aScene.Add("Survival.unity");
                aScene.Add("VRG_Managers.unity");

                string sMessage;
                foreach (string child in aScene)
                {
                    sMessage = "Copying " + child;
                    try
                    {
                        FileUtil.CopyFileOrDirectory(sOldPath + child, sNewGamePath + child);
                    }
                    catch (IOException ex)
                    {
                        sMessage = ex.Message;
                    }

                    //Debug.Log(sMessage);
                }

                AssetDatabase.Refresh();

                List<string> sceneList = new List<string>();

                sceneList.Add(sNewGamePath + "Home.unity");
                

                // get the scenes just created
                DirectoryInfo info = new DirectoryInfo(sNewGamePath);
                FileInfo[] fileInfo = info.GetFiles();
                foreach (FileInfo child in fileInfo)
                {
                    if (child.Name.Contains(".unity") && !child.Name.Contains(".meta"))
                    {
                        string[] both = child.DirectoryName.Split(new string[] { VRG_Editor.m_OS_Separator + "Assets" }, System.StringSplitOptions.None);

                        if (!sceneList.Contains("Assets" + both[1] + VRG_Editor.m_OS_Separator + child.Name))
                        {
                            sceneList.Add("Assets" + both[1] + VRG_Editor.m_OS_Separator + child.Name);
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

                i = 0;
                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                foreach (string child in sceneList)
                {
                    if (!string.IsNullOrEmpty(sceneList[i]))
                    {
                        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneList[i], true));
                    }
                    Debug.Log("Adding Scene: " + i + ") " + sceneList[i]);
                    i++;
                }

                // Set the Build Settings window Scene list
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(sceneList[0]);
                    Debug.Log("Opening Home Scene");
                }

                this.Close();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }

}