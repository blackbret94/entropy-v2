using UnityEditor;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_CORE_VRG_Remote : VRG_Editor
    {
        [MenuItem("Tools/Vr Games Dev/VRG_Remote/Add VRG_Remote Prefab", false, 1011)]
        public static void Add_VRG_Remote() => CreatePrefab(m_Prefabs + "VRG_Remote", true);

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/REMOTE_CONFIG_INSTALLED: Add", false, 1031)]
        public static void Add_VRG_Remote_precompiled()
        {
            VRG_DefineSymbols.Add("REMOTE_CONFIG_INSTALLED");
            print("REMOTE_CONFIG_INSTALLED: Added ... Recompiling");
        }

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/REMOTE_CONFIG_INSTALLED: Remove", false, 1032)]
        public static void Remove_VRG_Remote_precompiled()
        {
            VRG_DefineSymbols.Remove("REMOTE_CONFIG_INSTALLED");
            print("REMOTE_CONFIG_INSTALLED: Removed ... Recompiling");
        }

        [MenuItem("Tools/Vr Games Dev/VRG_Remote/VRG_Announcement", false, 1051)]
        public static void Add_VRG_Remote_VRG_Announcement()
        {
            VRG_Remote go_Remote = CreateRemote("VRG_Announcement");

            if (go_Remote != null)
            {
                go_Remote.AddInt("VRG_Announcement.repeat", 0);

                go_Remote.AddString("VRG_Announcement.date", "2000-01-01");
                go_Remote.AddString("VRG_Announcement.title", "Added from Menu");
                go_Remote.AddString("VRG_Announcement.body", "Edit the VRG_Remote object added to customize this local message.<br><br>Remember to create the server version when you publish your game");
            }
        }

    }
}