using UnityEditor;

using UnityEditor.SceneManagement;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_CORE_Examples : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/CORE/Prefabs/";

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/Clear Build Settings from examples", false, 100001)]
        public static void Example_100001()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            RemoveScenesFromoBuildSettings(new string[]
            {
                "CORE/Examples/Scenes/04 Scene Managment/" + "04 Scenes managment 1",
                "CORE/Examples/Scenes/04 Scene Managment/" + "04 Scenes managment 2"
            });
        }
        
        [MenuItem("Tools/Vr Games Dev/Examples/CORE/00 Demo", false, 100020)]
        public static void Example_100020() => LoadScene("CORE/Examples/Scenes/" + "00 Demo");
        
        
        [MenuItem("Tools/Vr Games Dev/Examples/CORE/01 UI", false, 100021)]
        public static void Example_100021() => LoadScene("CORE/Examples/Scenes/" + "01 UI");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/02 Graphical Numbers", false, 100022)]
        public static void Example_100022() => LoadScene("CORE/Examples/Scenes/" + "02 Graphical Numbers");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/03 Camera And Fader", false, 100023)]
        public static void Example_100023() => LoadScene("CORE/Examples/Scenes/" + "03 Camera And Fader");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/04 Scene Managment", false, 100024)]
        public static void Example_100024()
        {
            LoadScene("CORE/Examples/Scenes/" + "04 Scene Managment");

            AddScenesToBuildSettings(new string[]
            {
                "CORE/Examples/Scenes/04 Scene Managment/" + "04 Scenes managment 1",
                "CORE/Examples/Scenes/04 Scene Managment/" + "04 Scenes managment 2"
            }, false);
        }

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/05 VRG_SessionData", false, 100025)]
        public static void Example_100025() => LoadScene("CORE/Examples/Scenes/" + "05 VRG_SessionData");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/06 VRG_SessionData UI", false, 100026)]
        public static void Example_100026() => LoadScene("CORE/Examples/Scenes/" + "06 VRG_SessionData UI");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/07 VRG_Announcement", false, 100027)]
        public static void Example_100027() => LoadScene("CORE/Examples/Scenes/" + "07 VRG_Announcement");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/08 Sounds and music", false, 100028)]
        public static void Example_100028() => LoadScene("CORE/Examples/Scenes/" + "08 Sounds and music");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/09 PopUp and Exit", false, 100029)]
        public static void Example_100029() => LoadScene("CORE/Examples/Scenes/" + "09 PopUp and Exit");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/10 Skins", false, 100030)]
        public static void Example_100030() => LoadScene("CORE/Examples/Scenes/" + "10 Skins");

        [MenuItem("Tools/Vr Games Dev/Examples/CORE/11 Bhel", false, 100031)]
        public static void Example_100031() => LoadScene("CORE/Examples/Scenes/" + "11 Bhel");
        



        [MenuItem("Tools/Vr Games Dev/Examples/5 Seconds/Add Game Scenes", false, 100002)]
        public static void Example_100002()
        {
            AddScenesToBuildSettings(new string[]
            {
                "5 Seconds/Scenes/" + "Home",
                "5 Seconds/Scenes/" + "VRG_Managers",
                "5 Seconds/Scenes/" + "Campaign"
            });

            LoadScene("5 Seconds/Scenes/Home");
        }
        
        [MenuItem("Tools/Vr Games Dev/Examples/5 Seconds/Remove Game Scenes", false, 100003)]
        public static void Example_100003()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            RemoveScenesFromoBuildSettings(new string[]
            {
                "5 Seconds/Scenes/" + "Home",
                "5 Seconds/Scenes/" + "VRG_Managers",
                "5 Seconds/Scenes/" + "Campaign"
            });
        }
        

    }
}