using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using VrGamesDev.DDuA;



using Object = UnityEngine.Object;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_DDuA_Examples : VRG_Editor_DDuA
    {
        public static string m_Utils= "Tools/Core/Prefabs/Utils/";



        
        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/Clear Examples Data", false, 120001)]
        public static void Clear_Example_Data()
        {
            /*
            VRG_Editor_DDuA.RemoveGroupFromAddressables(VRG_Editor_DDuA.m_SlideShow);
            VRG_Editor_DDuA.RemoveGroupFromAddressables(VRG_Editor_DDuA.m_OnLaunch);
            VRG_Editor_DDuA.RemoveGroupFromAddressables(VRG_Editor_DDuA.m_PreDownload);
            VRG_Editor_DDuA.RemoveGroupFromAddressables(VRG_Editor_DDuA.m_Scene);            
            //*/

            Clear_Example();
        }
        public static void Clear_Example()
        {
            List<AddressableAssetEntry> assets = new List<AddressableAssetEntry>();

            AddressableAssetSettingsDefaultObject.Settings.GetAllAssets(assets, true);

            foreach (AddressableAssetEntry child in assets)
            {
                if (child.labels.Contains(m_Example))
                {
                    AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(child.guid, true);
                }
            }
        }




        ////////////////////////////////////////////////////////////////////////
        //
        // Examples: VRG_Addressable
        //
        ////////////////////////////////////////////////////////////////////////
        public static void Example_VRG_Addressable(string valueLocal)
        {
            Clear_Example();

            Example_Prefab("VRG_DDuA/PreDownload/Cube");

            Example_Prefab("VRG_DDuA/PreDownload/Capsule");

            LoadScene("DDuA/Examples/Scenes/VRG_Addressable/" + valueLocal);
        }

        // Tools/Vr Games Dev/Examples/DDuA/

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Hello World", false, 121001)]
        public static void Example_121001()
        {
            Clear_Example();

            Example_Prefab("HelloWorld");

            LoadScene("DDuA/Examples/Scenes/VRG_Addressable/01 HelloWorld");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Created By Name", false, 121002)]
        public static void Example_121002()
        {
            Clear_Example();

            Example_Prefab("HelloWorld");

            LoadScene("DDuA/Examples/Scenes/VRG_Addressable/02 By Name");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Play and Destroy", false, 121003)]
        public static void Example_121003() => Example_VRG_Addressable("03 Play and Destroy");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Self Turn Off", false, 121004)]
        public static void Example_121004() => Example_VRG_Addressable("04 Self Turn Off");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Is Locked", false, 121005)]
        public static void Example_121005() => Example_VRG_Addressable("05 IsLocked");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: BHEL", false, 121006)]
        public static void Example_121006() => Example_VRG_Addressable("06 BHEL");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Is Parent", false, 121007)]
        public static void Example_121007() => Example_VRG_Addressable("07 IsParent");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Is Overwrite", false, 121008)]
        public static void Example_121008() => Example_VRG_Addressable("08 IsOverwrite");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: When Complete", false, 121009)]
        public static void Example_121009() => Example_VRG_Addressable("09 When Complete");

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: By Script", false, 121010)]
        public static void Example_121010()
        {
            Example_VRG_Addressable("10 By Script");

            Example_Prefab("HelloWorld");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Addressable: Item Pooling", false, 121011)]
        public static void Example_121011()
        {
            Clear_Example();

            Example_Prefab("VRG_DDuA/PreDownload/Bullet");

            LoadScene("DDuA/Examples/Scenes/VRG_Addressable/" + "11 Item Pooling");
        }








































        ////////////////////////////////////////////////////////////////////////
        //
        // Examples: VRG_Loader
        //
        ////////////////////////////////////////////////////////////////////////
        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: VRG_Loader", false, 122000)]
        public static void VRG_Loader()
        {
            Clear_Example();

            VRG_Editor_DDuA.Add_Addresables_Group(false);

            AddressableAssetEntry AAE_Entry = VRG_Editor_DDuA.AddPrefabToAddressables("VRG_DDuA", VRG_Editor_DDuA.m_OnLaunch, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            AAE_Entry = VRG_Editor_DDuA.AddSceneToAddressables(VRG_DDuA.m_SceneProxy, VRG_Editor_DDuA.m_PreDownload, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            List<string> sceneList = new List<string>();

            sceneList.Add(CalculateInstallationPath() + "DDuA/Examples/Scenes/VRG_Loader.unity");

            foreach (EditorBuildSettingsScene child in EditorBuildSettings.scenes)
            {
                if (!sceneList.Contains(child.path))
                {
                    sceneList.Add(child.path);
                }
            }

            int i = 0;
            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (string child in sceneList)
            {
                if (!string.IsNullOrEmpty(sceneList[i]))
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneList[i], true));
                }
                i++;
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

            LoadScene("DDuA/Examples/Scenes/VRG_Loader");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: HelloWorld", false, 122001)]
        public static void Example_122001()
        {
            VRG_Loader();

            Example_Prefab("HelloWorld");

            Example_Scene("VRG_DDuA/01 HelloWorld/Home");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: VRG_Scene", false, 122002)]
        public static void Example_122002()
        {
            VRG_Loader();

            Example_Prefab("HelloWorld");

            Example_Scene("VRG_DDuA/02 VRG_Scene/Empty");

            LoadScene("DDuA/Examples/Scenes/VRG_DDuA/02 VRG_Scene/Empty");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: VRG_Slide", false, 122003)]
        public static void Example_122003()
        {
            Clear_Example();

            LoadScene("DDuA/Examples/Scenes/VRG_DDuA/03 VRG_Slide");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: SlideShow Label", false, 122004)]
        public static void Example_122004()
        {
            VRG_Loader();

            Example_Folder("Prefabs/Slides");

            Example_Scene("VRG_DDuA/04 SlideShow/Home");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: OnLaunch Label", false, 122005)]
        public static void Example_122005()
        {
            Example_WithFPS();

            Example_Scene("VRG_DDuA/05 OnLaunch/Home");
        }
        public static void Example_WithFPS()
        {
            VRG_Loader();

            Example_Folder("Prefabs/Slides");

            string sAsset = "VRG_FPS";
            AddressableAssetEntry AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(sAsset, VRG_Editor_DDuA.m_OnLaunch, ".prefab", true, CalculateInstallationPath() + m_Utils + sAsset + ".prefab");
            AAE_Entry.SetLabel(m_Example, true, true);
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: PreDownload Label", false, 122006)]
        public static void Example_122006()
        {
            Example_WithFPS();

            Example_Folder("Prefabs/VRG_DDuA");

            Example_Scene("VRG_DDuA/06 PreDownload/Home");

            Example_Prefab("VRG_Scene/04 PreDownload/Cylinder", VRG_Editor_DDuA.m_PreDownload, new string[] { "" });

            VRG_Cache.Clear();
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: Skins", false, 122007)]
        public static void Example_122007()
        {
            VRG_Loader();

            Example_Scene("VRG_DDuA/07 Skins/Home");

            string sSkinPath = CalculateInstallationPath() + "Tools/CORE/Prefabs/Skin/VRG_Skin - "; 
            string[] aAsset = new string[]
            {
                "Elysium",
                "Gaia",
                "Inferno",
                "Limbo",
                "Metropolis",
            };

            AddressableAssetEntry AAE_Entry;

            foreach (string child in aAsset)
            {
                AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(child, VRG_Editor_DDuA.m_OnLaunch, ".prefab", true, sSkinPath + child + ".prefab");
                AAE_Entry.SetLabel(m_Example, true, true);
            }
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_DDuA: Self Download", false, 122008)]
        public static void Example_122008()
        {
            VRG_Loader();

            Example_Scene("VRG_DDuA/08 Self Download/Home");

            string sSkinPath = CalculateInstallationPath() + "Tools/CORE/Prefabs/Skin/VRG_Skin - ";
            string[] aAsset = new string[]
            {
                "Gaia",
                "Inferno",
                "Limbo",
            };

            AddressableAssetEntry AAE_Entry;

            foreach (string child in aAsset)
            {
                AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(child, VRG_Editor_DDuA.m_Scene, ".prefab", false, sSkinPath + child + ".prefab");
                AAE_Entry.SetLabel(m_Example, true, true);
            }
        }





































        ////////////////////////////////////////////////////////////////////////
        //
        // Examples: VRG_Loader
        //
        ////////////////////////////////////////////////////////////////////////
        public static void Example_Empty()
        {
            VRG_Loader();

            Example_Folder("Prefabs/Slides");

            string sAsset = "VRG_FPS";
            AddressableAssetEntry AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(sAsset, VRG_Editor_DDuA.m_OnLaunch, ".prefab", true, CalculateInstallationPath() + m_Utils + sAsset + ".prefab");
            AAE_Entry.SetLabel(m_Example, true, true);

            Example_Folder("Prefabs/VRG_DDuA");

            Example_Scene("Home");
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: OnLaunch", false, 123001)]
        public static void Example_123001()
        {
            Example_Empty();

            Example_Prefab("VRG_Scene/01 OnLaunch/VRG_UI", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeOnLaunch }, false);
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: VRG_GoToScene", false, 123002)]
        public static void Example_123002()
        {
            Example_Empty();

            Example_Scene("Simple");

            Example_Prefab("VRG_Scene/02 VRG_GoToScene_Addressable/VRG_UI", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeOnLaunch, "Simple.OnLaunch" }, false);
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: SlideShow", false, 123003)]
        public static void Example_123003()
        {
            Example_Empty();

            Example_Scene("Simple");

            Example_Prefab("VRG_Scene/03 SlideShow/VRG_UI", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeOnLaunch, "Simple.OnLaunch" }, false);

            Example_Prefab("VRG_Scene/03 SlideShow/Slide - Home", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeSlideShow }, false);

            Example_Prefab("VRG_Scene/03 SlideShow/Slide - Simple", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.SlideShow" }, false);
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: PreDownload", false, 123004)]
        public static void Example_123004()
        {
            Example_123003();

            Example_Prefab("VRG_Scene/04 PreDownload/Cylinder", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.PreDownload" }, false);

            Example_Prefab("VRG_Scene/04 PreDownload/Create a Cylinder", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.OnLaunch" }, false);

            VRG_Cache.Clear();
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: Idle Download", false, 123005)]
        public static void Example_123005()
        {
            string sAsset = "VRG_FPS";
            AddressableAssetEntry AAE_Entry;

            VRG_Loader();

            Example_Scene("Home");
            Example_Scene("Simple");

            Example_Prefab("VRG_Scene/05 Idle Download/VRG_UI", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeOnLaunch, "Simple.OnLaunch" }, false);

            AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(sAsset, VRG_Editor_DDuA.m_OnLaunch, ".prefab", false, CalculateInstallationPath() + m_Utils + sAsset + ".prefab");
            AAE_Entry.SetLabel(m_Example, true, true);

            Example_Prefab("HelloWorld", VRG_Editor_DDuA.m_PreDownload, new string[]{}, false);

            Example_Prefab("VRG_DDuA/PreDownload/Cube", VRG_Editor_DDuA.m_PreDownload, new string[]{}, false);
            Example_Prefab("VRG_DDuA/PreDownload/Capsule", VRG_Editor_DDuA.m_PreDownload, new string[]{}, false);
            Example_Prefab("VRG_Scene/04 PreDownload/Cylinder", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.PreDownload" }, false);

            Example_Prefab("Slides/SlideShow/Slide 0", VRG_Editor_DDuA.m_SlideShow, new string[]{});
            Example_Prefab("Slides/SlideShow/Slide 1", VRG_Editor_DDuA.m_SlideShow, new string[]{}, false);
            Example_Prefab("Slides/SlideShow/Slide 2", VRG_Editor_DDuA.m_SlideShow, new string[]{}, false);

            Example_Prefab("VRG_Scene/03 SlideShow/Slide - Home", VRG_Editor_DDuA.m_SlideShow, new string[] { "" }, false);

            Example_Prefab("VRG_Scene/03 SlideShow/Slide - Simple", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.SlideShow" }, false);

            Example_Prefab("VRG_Scene/04 PreDownload/Create a Cylinder", VRG_Editor_DDuA.m_OnLaunch, new string[] { "" }, false);

            AAE_Entry = VRG_Editor_DDuA.AddPrefabToAddressables("VRG_Addressable", VRG_Editor_DDuA.m_PreDownload, false);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            AAE_Entry = VRG_Editor_DDuA.AddPrefabToAddressables("VRG_GoToScene_Addressable", VRG_Editor_DDuA.m_PreDownload, false);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            AAE_Entry = VRG_Editor_DDuA.AddPrefabToAddressables("VRG_Scene", VRG_Editor_DDuA.m_PreDownload, false);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            AAE_Entry = VRG_Editor_DDuA.AddPrefabToAddressables("VRG_Slide", VRG_Editor_DDuA.m_PreDownload, false);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            AAE_Entry = VRG_Editor_DDuA.AddSceneToAddressables(VRG_DDuA.m_SceneProxy, VRG_Editor_DDuA.m_PreDownload, false);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);
            AAE_Entry.SetLabel(m_Example, true, true);

            VRG_Cache.Clear();
        }

        [MenuItem("Tools/Vr Games Dev/Examples/DDuA/VRG_Scene: Scene OnClick", false, 123006)]
        public static void Example_123006()
        {
            print("VRG_Scene: Multi Scene(Example_123006);");

            VRG_Loader();

            Example_Scene("Home");
            Example_Scene("Simple");
            Example_Scene("VRG_Scene/06 Scene OnClick/Heavy");

            Example_Prefab("VRG_Scene/06 Scene OnClick/VRG_UI", VRG_Editor_DDuA.m_Scene, new string[] { VRG_Editor_DDuA.m_HomeOnLaunch, VRG_Editor_DDuA.m_Example }, false);

            Example_Prefab("VRG_Scene/06 Scene OnClick/VRG_UI - Scene", VRG_Editor_DDuA.m_Scene, new string[] { "Simple.OnLaunch", VRG_Editor_DDuA.m_Example }, false);

        }











































        public static void Example_Prefab(string valueLocal) => Example_Prefab(valueLocal, VRG_Editor_DDuA.m_Default, new string[] { }, true);
        public static void Example_Prefab(string valueLocal, string groupLocal) => Example_Prefab(valueLocal, groupLocal, new string[] { }, true);
        public static void Example_Prefab(string valueLocal, string groupLocal, string[] labelLocal) => Example_Prefab(valueLocal, groupLocal, labelLocal, true);
        public static void Example_Prefab(string valueLocal, string groupLocal, string[] labelLocal, bool addGroupLocal)
        {
            if (valueLocal.Trim() != string.Empty)
            {
                AddressableAssetEntry AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(valueLocal, groupLocal, ".prefab", addGroupLocal, CalculateInstallationPath() + "DDuA/Examples/Prefabs/" + valueLocal + ".prefab");

                if (labelLocal.Length > 0)
                {
                    foreach (string child in labelLocal)
                    {
                        if (child.Trim() != string.Empty)
                        {
                            AAE_Entry.SetLabel(child, true, true);
                        }
                    }
                }

                if (groupLocal.Trim() != string.Empty)
                {
                    AAE_Entry.SetLabel(m_Example, true, true);
                }

            }
        }
        public static void Example_Scene(string valueLocal)
        {
            if (valueLocal.Trim() != string.Empty)
            {
                AddressableAssetEntry AAE_Entry = VRG_Editor_DDuA.AddEntryToAddressables(valueLocal, VRG_Editor_DDuA.m_Scene, ".unity", false, CalculateInstallationPath() + "DDuA/Examples/Scenes/" + valueLocal + ".unity");

                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Scene, true, true);
                AAE_Entry.SetLabel(m_Example, true, true);
                /*
                string[] aSceneSingle = valueLocal.Split('/');

                string sSceneSingle = aSceneSingle[aSceneSingle.Length - 1];

                AAE_Entry.SetLabel(sSceneSingle + ".OnLaunch", true, true);
                AAE_Entry.SetLabel(sSceneSingle + ".PreDownload", true, true);
                AAE_Entry.SetLabel(sSceneSingle + ".SlideShow", true, true);
                
                AAE_Entry.SetLabel(sSceneSingle + ".OnLaunch", false, true);
                AAE_Entry.SetLabel(sSceneSingle + ".PreDownload", false, true);
                AAE_Entry.SetLabel(sSceneSingle + ".SlideShow", false, true);
                */
            }
        }
        public static void Example_Folder(string valueLocal)
        {
            if (valueLocal.Trim() != string.Empty)
            {
                valueLocal = CalculateInstallationPath() + "DDuA/Examples/" + valueLocal + "/";

                valueLocal = valueLocal.Replace("//", "/");

                VRG_Editor_DDuA.AddFolderToAddressables(valueLocal, VRG_Editor_DDuA.m_Default, new string[] { m_Example });

                DirectoryInfo info = new DirectoryInfo(valueLocal);
                DirectoryInfo[] dirInfo = info.GetDirectories();

                foreach (DirectoryInfo child in dirInfo)
                {
                    VRG_Editor_DDuA.AddFolderToAddressables(valueLocal + child.Name + m_OS_Separator, child.Name, new string[] { m_Example });
                }
            }
        }


    }
}