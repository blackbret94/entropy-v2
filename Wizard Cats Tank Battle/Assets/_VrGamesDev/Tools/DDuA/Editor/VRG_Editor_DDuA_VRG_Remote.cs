using UnityEditor;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_DDuA_VRG_Remote : VRG_Editor
    {

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/VRG_Loader", false, 20002)]
        public static void Add_VRG_Remote_VRG_Loader()
        {
            VRG_Remote go_Remote = VRG_Editor_BHEL.CreateRemote("VRG_Loader");

            if (go_Remote != null)
            {                
                go_Remote.AddFloat("VRG_Loader.m_InternetPing", 1.0f);

                go_Remote.AddString("VRG_Loader.m_CatalogueList", "");
            }
        }

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/VRG_DDuA", false, 20003)]
        public static void Add_VRG_Remote_VRG_DDuA()
        {
            VRG_Remote go_Remote = VRG_Editor_BHEL.CreateRemote("VRG_DDuA");

            if (go_Remote != null)
            {
                go_Remote.AddBool("VRG_DDuA.isCatalogueIdle", false);

                go_Remote.AddFloat("VRG_DDuA.m_ProgressDelay", 0.25f);

                go_Remote.AddString("VRG_DDuA.m_SceneName", "");

                go_Remote.AddString("OnLaunch", "");
                go_Remote.AddString("PreDownload", "");
            }
        }

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/VRG_SlideShow", false, 20004)]
        public static void Add_VRG_Remote_VRG_SlideShow()
        {
            VRG_Remote go_Remote = VRG_Editor_BHEL.CreateRemote("VRG_SlideShow");

            if (go_Remote != null)
            {
                go_Remote.AddInt("SlideShow.m_Seed", -1);

                go_Remote.AddString("SlideShow", "");
            }
        }

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/Scene: Home", false, 20005)]
        public static void Add_VRG_Remote_Scene__Home()
        {
            string sSceneName = "Home";

            VRG_Remote go_Remote = VRG_Editor_BHEL.CreateRemote("VRG_Scene." + sSceneName);

            if (go_Remote != null)
            {
                go_Remote.AddString(sSceneName + ".OnLaunch", "");
                go_Remote.AddString(sSceneName + ".PreDownload", "");
                go_Remote.AddString(sSceneName + ".SlideShow", "");
            }
        }
    }
}