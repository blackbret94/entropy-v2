using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

using UnityEditor.SceneManagement;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using VrGamesDev.DDuA;



using Object = UnityEngine.Object;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_DDuA : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/DDuA/Prefabs/";

        public static AddressableAssetGroup groupDefault = AddressableAssetSettingsDefaultObject.Settings.DefaultGroup;

        public static string m_Default = "Default";

        public static string m_Example = "Example";

        public static string m_SlideShow = "SlideShow";
        public static string m_OnLaunch = "OnLaunch";
        public static string m_PreDownload = "PreDownload";
        public static string m_Scene = "Scene";

        public static string m_HomeSlideShow = "Home.SlideShow";
        public static string m_HomeOnLaunch = "Home.OnLaunch";
        public static string m_HomePreDownload = "Home.PreDownload";


        //[MenuItem("Tools/Create New DDuA Project", false, 1)]
        [MenuItem("Tools/Vr Games Dev/DDuA/Create New DDuA Project", false, 20001)]
        public static void CreateNew()
        {
            // Get existing open window or if none, make a new one:
            VRG_WindowCreateNewDDuA window = (VRG_WindowCreateNewDDuA)EditorWindow.GetWindow(typeof(VRG_WindowCreateNewDDuA), false, "Create a new DDuA Project", true);

            VRG_Editor_DDuA_Examples.Clear_Example_Data();

            window.maxSize = new Vector2(325f, 440f);
            window.minSize = window.maxSize;

            window.Show();
        }





        [MenuItem("Tools/Vr Games Dev/DDuA/VRG_Addressable", false, 20031)]
        public static void VRG_Addressable() => CreatePrefab(m_Prefabs + "VRG_Addressable");

        [MenuItem("Tools/Vr Games Dev/DDuA/VRG_Scene", false, 20032)]
        public static void VRG_Scene() => CreatePrefab(m_Prefabs + "VRG_Scene", true);

        [MenuItem("Tools/Vr Games Dev/DDuA/VRG_GoToScene_Addressable", false, 20033)]
        public static void VRG_GoToAddressableScene() => CreatePrefab(m_Prefabs + "VRG_GoToScene_Addressable");

        [MenuItem("Tools/Vr Games Dev/DDuA/VRG_Slide", false, 20045)]
        public static void VRG_Slide() => CreatePrefab(m_Prefabs + "VRG_Slide");















        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/Addressables Groups", false, 20061)]
        public static void Add_Addresables_Group() => Add_Addresables_Group(false);
        public static void Add_Addresables_Group(bool valueLocal)
        {
            AddGroupToAddressables(VRG_Editor_DDuA.m_SlideShow);
            AddGroupToAddressables(VRG_Editor_DDuA.m_OnLaunch);
            AddGroupToAddressables(VRG_Editor_DDuA.m_PreDownload);
            AddGroupToAddressables(VRG_Editor_DDuA.m_Scene);

            AddressableAssetEntry AAE_Entry = AddPrefabToAddressables("VRG_DDuA", VRG_Editor_DDuA.m_OnLaunch, false);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_SlideShow, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_SlideShow, false, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_OnLaunch, true, true);
            //AAE_Entry.SetLabel(VRG_Editor_DDuA.m_OnLaunch, false, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, false, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeSlideShow, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeSlideShow, false, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeOnLaunch, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeOnLaunch, false, true);

            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomePreDownload, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomePreDownload, false, true);


            if (!valueLocal)
            {
                RemoveEntryFromAddressables("Tools/DDuA/Prefabs/VRG_DDuA.prefab");
            }
            else
            {
                VRG_Editor_DDuA_Examples.Example_Scene("Home");

                AAE_Entry = AddSceneToAddressables(VRG_DDuA.m_SceneProxy, VRG_Editor_DDuA.m_PreDownload, true);
                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);
            }
        }

        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/VRG_Loader", false, 20062)]
        public static void Add_VRG_Loader()
        {
            Add_Addresables_Group(true);

            if (Object.FindObjectsOfType<VRG_Loader>().Length == 0)
            {
                CreatePrefab(m_Prefabs + "VRG_Loader", true);
            }
            else
            {
                print("<color=red>ERROR: </color> There is already a VRG_Loader object in the scene");
            }
        }

        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/SlideShow", false, 20081)]
        public static void Setup_VRG_DDuA()
        {
            Add_Addresables_Group(true);

            VRG_Editor_DDuA_Examples.Example_Prefab("Slides/SlideShow/Slide 0", VRG_Editor_DDuA.m_SlideShow);

            VRG_Editor_DDuA_Examples.Example_Scene("Home");
        }

        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/PreDownloads", false, 20082)]
        public static void Add_PreDownloads()
        {
            Add_Addresables_Group(true);

            AddressableAssetEntry

            AAE_Entry = AddPrefabToAddressables("VRG_Addressable", VRG_Editor_DDuA.m_PreDownload);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);

            AAE_Entry = AddPrefabToAddressables("VRG_GoToScene_Addressable", VRG_Editor_DDuA.m_PreDownload);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);

            AAE_Entry = AddPrefabToAddressables("VRG_Scene", VRG_Editor_DDuA.m_PreDownload);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);

            AAE_Entry = AddPrefabToAddressables("VRG_Slide", VRG_Editor_DDuA.m_PreDownload);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);
        }

        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/Download Catalogue Idle", false, 20083)]
        public static void Setup_Download_Catalogue_Idle()
        {
            Add_Addresables_Group(true);

            string sNewPath;
            AddressableAssetEntry AAE_Entry;
            AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_OnLaunch);

            sNewPath = VRG_Editor_CORE.CalculateInstallationPath() + "Tools/DDuA/CreateNew/VRG_DDuA_CatalogueIdle.prefab";
            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
            (
                AssetDatabase.AssetPathToGUID(sNewPath),
                group,
                false,
                true
            );
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_OnLaunch, true, true);
            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);
            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
        }

        


        [MenuItem("Tools/Vr Games Dev/DDuA/SetUp/Scene: Home.OnLaunch", false, 20101)]
        public static void Setup_Addresables_Home()
        {
            Add_Addresables_Group(true);

            string sNewPath;
            AddressableAssetEntry AAE_Entry;
            AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_Scene);

            if (group)
            {
                sNewPath = VRG_Editor_CORE.CalculateInstallationPath() + "CORE/Prefabs/Campaign/World - Home.prefab";
                AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                (
                    AssetDatabase.AssetPathToGUID(sNewPath),
                    group,
                    false,
                    true
                );
                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeOnLaunch, true, true);
                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);
                AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));

                sNewPath = VRG_Editor_CORE.CalculateInstallationPath() + "Tools/DDuA/CreateNew/VRG_UI - Other.prefab";
                AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                (
                    AssetDatabase.AssetPathToGUID(sNewPath),
                    group,
                    false,
                    true
                );
                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeOnLaunch, true, true);
                AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Example, true, true);
                AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
            }
        }










        [MenuItem("Tools/Vr Games Dev/DDuA/Clear Cache", false, 20081)]
        public static void Clear_Cache() => VRG_Cache.Clear();









































        public static void AddFolderToAddressables(string valueLocal, string groupLocal, string[] labelLocal)
        {
            // get the scenes just created
            DirectoryInfo info = new DirectoryInfo(valueLocal);
            FileInfo[] fileInfo = info.GetFiles();

            foreach (FileInfo child in fileInfo)
            {
                if (!child.Name.Contains(".meta"))
                {
                    AddressableAssetEntry AAE_Entry = AddEntryToAddressables(child.Name.Split('.')[0], groupLocal, child.Name.Split('.')[1], true, valueLocal + child.Name);

                    if (labelLocal.Length > 0)
                    {
                        foreach (string childLabel in labelLocal)
                        {
                            if (childLabel.Trim() != string.Empty)
                            {
                                AAE_Entry.SetLabel(childLabel, true, true);
                            }
                        }
                    }
                }
            }
        }

        public static void RemoveGroupFromAddressables(string valueLocal)
        {
            AddressableAssetGroup group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(valueLocal);

            if (group)
            {
                AddressableAssetSettingsDefaultObject.Settings.RemoveGroup(group);
            }
        }

        public static AddressableAssetGroup AddGroupToAddressables(string valueLocal)
        {
            AddressableAssetGroup group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(valueLocal);

            if (!group)
            {
                if (valueLocal != m_Default)
                {
                    group = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(valueLocal, false, false, false, groupDefault.Schemas);
                }
                else
                {
                    group = AddressableAssetSettingsDefaultObject.Settings.DefaultGroup;
                }

                if (!group)
                {
                    throw new Exception($"Addressable : can't find group {valueLocal}");
                }
                else
                {
                    if (valueLocal != m_Default)
                    {
                        print("Creating addressable group: " + valueLocal);
                    }
                }
            }

            return group;
        }

        public static AddressableAssetEntry AddPrefabToAddressables(string valueLocal, string groupLocal)
        {
            return AddEntryToAddressables(valueLocal, groupLocal, ".prefab", true, string.Empty);
        }
        public static AddressableAssetEntry AddPrefabToAddressables(string valueLocal, string groupLocal, bool addLabelLocal)
        {
            return AddEntryToAddressables(valueLocal, groupLocal, ".prefab", addLabelLocal, string.Empty);
        }

        public static AddressableAssetEntry AddSceneToAddressables(string valueLocal, string groupLocal)
        {
            return AddEntryToAddressables(valueLocal, groupLocal, ".unity", true, string.Empty);
        }
        public static AddressableAssetEntry AddSceneToAddressables(string valueLocal, string groupLocal, bool addLabelLocal)
        {
            return AddEntryToAddressables(valueLocal, groupLocal, ".unity", addLabelLocal, string.Empty);
        }

        public static AddressableAssetEntry AddEntryToAddressables(string valueLocal, string groupLocal)
        {
            return AddEntryToAddressables(valueLocal.Split('.')[0], groupLocal, "", true, valueLocal);
        }

        public static AddressableAssetEntry AddEntryToAddressables(string valueLocal, string groupLocal, string extentionLocal, bool addLabelLocal, string routeLocal)
        {
            AddressableAssetEntry AAE_Entry = null;

            string sPath = CalculateInstallationPath() + m_Prefabs;

            string sAddress = routeLocal;

            if (sAddress == string.Empty)
            {
                sAddress = sPath + valueLocal + extentionLocal;
            }

            AddressableAssetGroup group = AddGroupToAddressables(groupLocal);

            if (group)
            {
                AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(sAddress), group, false, true);

                if (AAE_Entry != null)
                {
                    AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(valueLocal));

                    if (addLabelLocal && groupLocal.Trim() != m_Default)
                    {
                        AAE_Entry.SetLabel(groupLocal, true, true);
                    }

                    //TO DO:
                    //print("Adding " + valueLocal + " addressable to group " + groupLocal);
                }
                else
                {
                    throw new Exception($"Addressable : can't add {sAddress} to group {groupLocal}");
                }
            }

            return AAE_Entry;
        }

        public static void RemoveEntryFromAddressables(string valueLocal)
        {
            if (valueLocal != string.Empty)
            {
                valueLocal = CalculateInstallationPath() + valueLocal;

                if (AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(valueLocal)))
                {
                    //print("Removed: "+ valueLocal);
                }
                else
                {
                    //print("<color=red>CANT REMOVE: " + valueLocal + "</color>");
                }
            }
        }

        public static string SimplifyName(string valueLocal)
        {
            string[] aSimplifiedName = valueLocal.Split('/');

            return aSimplifiedName[aSimplifiedName.Length - 1].Split('.')[0];
        }

    }












































    public class VRG_WindowCreateNewDDuA : EditorWindow
    {
        private GUIStyle m_StyleWrap;
        private GUIStyle m_StyleLabel;

        private string m_FolderName = "_DDuA Game";

        private bool m_AddSlideShow = true;
        private bool m_IdleDownload = true;

        private bool m_Audio = true;
        private bool m_Campaign = true;

        private bool m_Settings = true;
        private bool m_Announcement = true;
        private bool m_Skins = true;

        void OnGUI()
        {
            int i = 0;

            this.m_StyleWrap = new GUIStyle(GUI.skin.label);
            this.m_StyleWrap.wordWrap = true;

            this.m_StyleLabel = new GUIStyle(GUI.skin.label);
            this.m_StyleLabel.wordWrap = true;
            this.m_StyleLabel.fixedWidth = 100;

            this.m_StyleLabel = new GUIStyle(GUI.skin.label);
            this.m_StyleLabel.wordWrap = true;
            this.m_StyleLabel.fixedWidth = 130;


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Get a new Addressble project with all the scenes and prefabs needed, with the folder name you provide.", this.m_StyleWrap);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(this.m_StyleLabel);
                {
                    GUILayout.Label("Project Name:");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                {
                    this.m_FolderName = EditorGUILayout.TextField("", this.m_FolderName);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("From DDuA:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("box");
            { 
                EditorGUILayout.BeginVertical(this.m_StyleLabel);
                {
                    GUILayout.Label("SlideShow:");
                    EditorGUILayout.Space();
                    GUILayout.Label("Idle download:");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                {
                    this.m_AddSlideShow = GUILayout.Toggle(this.m_AddSlideShow, " Add 2 Slides");
                    EditorGUILayout.Space();
                    this.m_IdleDownload = GUILayout.Toggle(this.m_IdleDownload, " Add CatalogueIdle");
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("From Managers:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.BeginVertical(this.m_StyleLabel);
                {
                    GUILayout.Label("Audio:");
                    EditorGUILayout.Space();
                    GUILayout.Label("Campaign:");
                    EditorGUILayout.Space();
                    GUILayout.Label("Skins:");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                {
                    this.m_Audio = GUILayout.Toggle(this.m_Audio, " VRG_Audio Manager ");
                    EditorGUILayout.Space();
                    this.m_Campaign = GUILayout.Toggle(this.m_Campaign, " Add VRG_Campaign");
                    EditorGUILayout.Space();
                    this.m_Skins = GUILayout.Toggle(this.m_Skins, " 5 Skin Package ");
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("From Home:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.BeginVertical(this.m_StyleLabel);
                {
                    GUILayout.Label("VRG_UI:");
                    EditorGUILayout.Space();
                    GUILayout.Label("Announcement:");
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                {
                    this.m_Settings = GUILayout.Toggle(this.m_Settings, " UI settings ");
                    EditorGUILayout.Space();
                    this.m_Announcement = GUILayout.Toggle(this.m_Announcement, " PopUp & Remote ");
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create new", GUILayout.Width(300), GUILayout.Height(30)))
            {
                string sNewPath;
                AddressableAssetEntry AAE_Entry;
                
                VRG_Editor_DDuA.Add_Addresables_Group();

                List<string> aScene = new List<string>();
                List<string> aSlideShow = new List<string>();
                List<string> aOnLaunch = new List<string>();
                List<string> aPreDownload = new List<string>();
                List<string> aHomeOnLaunch = new List<string>();

                List<string> sceneList = new List<string>();

                string sFolder = "Assets/" + this.m_FolderName + "/";
                string sFolderPrefabs = sFolder + "Prefabs/";
                string sFolderSlideShow = sFolderPrefabs + "SlideShow/";
                string sFolderOnLaunch = sFolderPrefabs + "OnLaunch/";
                string sFolderScenes = sFolder + "Scenes/";
                string sFolderPreDownload = sFolder + "PreDownload/";
                string sFolderHomeOnLaunch = sFolder + "Home/";

                Directory.CreateDirectory(sFolder);
                Directory.CreateDirectory(sFolderPrefabs);
                Directory.CreateDirectory(sFolderSlideShow);
                Directory.CreateDirectory(sFolderOnLaunch);
                Directory.CreateDirectory(sFolderScenes);
                Directory.CreateDirectory(sFolderPreDownload);
                Directory.CreateDirectory(sFolderHomeOnLaunch);


                { 
                    aPreDownload.Add("Tools/DDuA/Prefabs/" + VRG_DDuA.m_SceneProxy + ".unity");

                    // Scene
                    aScene.Add("DDuA/Examples/Scenes/Home.unity");

                    // Slideshows
                    if (this.m_AddSlideShow)
                    {
                        aSlideShow.Add("DDuA/Examples/Prefabs/Slides/SlideShow/Slide 0.prefab");
                        aSlideShow.Add("DDuA/Examples/Prefabs/Slides/SlideShow/Slide 2.prefab");
                    }

                    // OnLaunch
                    aOnLaunch.Add("Tools/DDuA/Prefabs/VRG_DDuA.prefab");
                    if (this.m_IdleDownload)
                    {
                        aOnLaunch.Add("Tools/DDuA/CreateNew/VRG_DDuA_CatalogueIdle.prefab");
                    }
                    if (this.m_Audio)
                    {
                        aOnLaunch.Add("CORE/Prefabs/Audio/VRG_Audio.prefab");
                        aHomeOnLaunch.Add("CORE/Prefabs/Audio/Music - Home.prefab");
                    }
                    if (this.m_Campaign)
                    {
                        aOnLaunch.Add("Tools/DDuA/CreateNew/VRG_Campaign.prefab");
                        aScene.Add("Tools/DDuA/Scenes/Campaign.unity");
                        aScene.Add("Tools/DDuA/Scenes/Survival.unity");
                    }

                    // Home.OnLaunch
                    aHomeOnLaunch.Add("CORE/Prefabs/Campaign/World - Home.prefab");

                    string sUiToLoad = "Other";
                    if (this.m_Settings && this.m_Campaign)
                    {
                        sUiToLoad = "All";
                    }
                    else
                    {
                        if (this.m_Settings)
                        {
                            sUiToLoad = "Settings";
                        }
                        if (this.m_Campaign)
                        {
                            sUiToLoad = "Campaign";
                        }
                    }

                    aHomeOnLaunch.Add("Tools/DDuA/CreateNew/VRG_UI - " + sUiToLoad + ".prefab");

                    if (this.m_Announcement)
                    {
                        aOnLaunch.Add("Tools/DDuA/CreateNew/VRG_Remote - (VRG_Announcement).prefab");

                        aHomeOnLaunch.Add("Tools/CORE/Prefabs/Announcement/VRG_Announcement.prefab");
                    }

                    if (this.m_Skins)
                    {
                        aOnLaunch.Add("Tools/CORE/Prefabs/Skin/VRG_Skin - Elysium.prefab");
                        aOnLaunch.Add("Tools/CORE/Prefabs/Skin/VRG_Skin - Gaia.prefab");
                        aOnLaunch.Add("Tools/CORE/Prefabs/Skin/VRG_Skin - Inferno.prefab");
                        aOnLaunch.Add("Tools/CORE/Prefabs/Skin/VRG_Skin - Limbo.prefab");
                        aOnLaunch.Add("Tools/CORE/Prefabs/Skin/VRG_Skin - Metropolis.prefab");
                    }
                }


                foreach (string child in aPreDownload)
                {
                    sNewPath = sFolderPreDownload + child.Split('/')[child.Split('/').Length - 1];

                    try
                    {
                        FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + child, sNewPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    finally
                    {
                        AssetDatabase.Refresh();
                        AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_PreDownload);

                        if (group)
                        {
                            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                            (
                                AssetDatabase.AssetPathToGUID(sNewPath),
                                group,
                                false,
                                true
                            );

                            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_PreDownload, true, true);

                            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
                        }
                    }
                }

                foreach (string child in aSlideShow)
                {
                    sNewPath = sFolderSlideShow + child.Split('/')[child.Split('/').Length - 1];

                    try
                    {
                        FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + child, sNewPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    finally
                    {
                        AssetDatabase.Refresh();
                        AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_SlideShow);

                        if (group)
                        {
                            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                            (
                                AssetDatabase.AssetPathToGUID(sNewPath),
                                group,
                                false,
                                true
                            );

                            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_SlideShow, true, true);                         

                            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));                            
                        }
                    }
                }

                foreach (string child in aOnLaunch)
                {
                    sNewPath = sFolderOnLaunch + child.Split('/')[child.Split('/').Length - 1];

                    try
                    {
                        FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + child, sNewPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    finally
                    {
                        AssetDatabase.Refresh();
                        AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_OnLaunch);

                        if (group)
                        {
                            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                            (
                                AssetDatabase.AssetPathToGUID(sNewPath),
                                group,
                                false,
                                true
                            );

                            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_OnLaunch, true, true);

                            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
                        }
                    }
                }

                foreach (string child in aScene)
                {
                    sNewPath = sFolderScenes + child.Split('/')[child.Split('/').Length - 1];

                    try
                    {
                        FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + child, sNewPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    finally
                    {
                        AssetDatabase.Refresh();
                        AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_Scene);

                        if (group)
                        {
                            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                            (
                                AssetDatabase.AssetPathToGUID(sNewPath),
                                group,
                                false,
                                true
                            );

                            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_Scene, true, true);

                            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
                        }
                    }
                }

                foreach (string child in aHomeOnLaunch)
                {
                    sNewPath = sFolderHomeOnLaunch + child.Split('/')[child.Split('/').Length - 1];

                    try
                    {
                        FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + child, sNewPath);
                    }
                    catch (IOException ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    finally
                    {
                        AssetDatabase.Refresh();
                        AddressableAssetGroup group = VRG_Editor_DDuA.AddGroupToAddressables(VRG_Editor_DDuA.m_Scene);

                        if (group)
                        {
                            AAE_Entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry
                            (
                                AssetDatabase.AssetPathToGUID(sNewPath),
                                group,
                                false,
                                true
                            );

                            AAE_Entry.SetLabel(VRG_Editor_DDuA.m_HomeOnLaunch, true, true);

                            AAE_Entry.SetAddress(VRG_Editor_DDuA.SimplifyName(sNewPath));
                        }
                    }
                }


                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

                try
                {
                    FileUtil.CopyFileOrDirectory(VRG_Editor_CORE.CalculateInstallationPath() + "DDuA/Examples/Scenes/VRG_Loader.unity", sFolderScenes + "VRG_Loader.unity");
                }
                catch (IOException ex)
                {
                    Debug.Log(ex.Message);
                }
                finally
                {
                    sceneList.Add(sFolderScenes + "VRG_Loader.unity");
                }
                AssetDatabase.Refresh();

                foreach (EditorBuildSettingsScene child in EditorBuildSettings.scenes)
                {
                    if (!sceneList.Contains(child.path))
                    {
                        sceneList.Add(child.path);
                    }
                }

                i = 0;
                foreach (string child in sceneList)
                {
                    if (!string.IsNullOrEmpty(sceneList[i]) && !sceneList[i].Contains(VRG_Editor_DDuA.m_Example))
                    {
                        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneList[i], true));
                    }
                    i++;
                }

                // Set the Build Settings window Scene list
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(sceneList[0]);
                    Debug.Log("Opening Scene: " + sceneList[0]);
                }

                this.Close();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }



}