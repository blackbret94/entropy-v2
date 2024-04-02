using System.Collections.Generic;

using UnityEditor;

using UnityEngine;


using VrGamesDev.DDuA;


using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;



namespace VrGamesDev
{
    ///#IGNORE

    /// <summary>
    ///  This class checks the scenes loaded into the Build Settings
    ///  and display them into an array to select for easier typing and 
    /// less prone to mistakes
    /// </summary>
    [CustomPropertyDrawer(typeof(AddressableSceneAttribute))]
    public class AddressableSceneDrawer : PropertyDrawer
    {
        // lets start in -1, in case the build settings are empty
        private int m_SceneIndex = -1;

        // the names of all the scenes that will be displayed
        private GUIContent[] m_AddressableScenes;

        // it works when the OnGui event 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if the index is the one
            if (this.m_SceneIndex == -1)
            {
                // call the setup with SerializedProperty

                List<string> scenesTemp = new List<string>();

                List<AddressableAssetEntry> allMyAssets = new List<AddressableAssetEntry>();

                AddressableAssetSettingsDefaultObject.Settings.GetAllAssets(allMyAssets, true);

                foreach (AddressableAssetEntry child in allMyAssets)
                {
                    if (child.IsScene && child.address != VRG_DDuA.m_SceneProxy)
                    {
                        scenesTemp.Add(child.address + " - " + "[From: " + child.parentGroup.ToString().Split('(')[0] + "]");
                    }
                }

                if (scenesTemp.Count > 0)
                {
                    // copy from the content
                    this.m_AddressableScenes = new GUIContent[scenesTemp.Count + 1];

                    // always add [RELOAD SCENE] as an option
                    this.m_AddressableScenes[0] = new GUIContent("[RELOAD SCENE]");

                    for (int i = 1; i <= scenesTemp.Count; i++)
                    {
                        this.m_AddressableScenes[i] = new GUIContent(scenesTemp[i - 1]);
                    }
                }
                else
                {
                    // inform the player that no scenes are into the build settings
                    this.m_AddressableScenes = new[] { new GUIContent("[No Scenes In Addressable Groups]") };
                }


                // look for the name searched to select it
                if (!string.IsNullOrEmpty(property.stringValue))
                {
                    // was it found?
                    bool AddressableSceneFound = false;

                    // cycle through the array of scenes
                    for (int i = 0; i < this.m_AddressableScenes.Length; i++)
                    {
                        // if it is the same
                        if (this.m_AddressableScenes[i].text == property.stringValue)
                        {
                            // select it
                            this.m_SceneIndex = i;

                            // because i found it
                            AddressableSceneFound = true;

                            // ... and stop the search, target found
                            break;
                        }
                    }

                    // if the scene is not found
                    if (!AddressableSceneFound)
                    {
                        // select the first one
                        this.m_SceneIndex = 0;
                    }
                }
                else
                {
                    // select the first one
                    this.m_SceneIndex = 0;
                }

                // assign the string to the value of the scene selected.
                property.stringValue = this.m_AddressableScenes[this.m_SceneIndex].text;
            }

            // if my current index is the same as the previous
            int oldIndex = this.m_SceneIndex;

            // popup with all the scenes names ordered
            this.m_SceneIndex = EditorGUI.Popup(position, label, this.m_SceneIndex, this.m_AddressableScenes);

            // if it is diferent
            if (oldIndex != this.m_SceneIndex)
            {
                // the current selected
                property.stringValue = this.m_AddressableScenes[this.m_SceneIndex].text;
            }
        }
    }
}