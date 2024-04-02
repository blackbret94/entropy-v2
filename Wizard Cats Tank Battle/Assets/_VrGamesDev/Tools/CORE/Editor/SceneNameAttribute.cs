using System;
using UnityEditor;
using UnityEngine;

namespace VrGamesDev
{
    ///#IGNORE

    /// <summary>
    ///  This class checks the scenes loaded into the Build Settings
    ///  and display them into an array to select for easier typing and 
    /// less prone to mistakes
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        // lets start in -1, in case the build settings are empty
        private int m_SceneIndex = -1;

        // the names of all the scenes that will be displayed
        private GUIContent[] m_SceneNames;

        // the path spliiter take the extension and the path out
        private readonly string[] m_ScenePathSplitters = { "/", ".unity" };

        // it works when the OnGui event 
        public override void OnGUI(Rect positionLocal, SerializedProperty propertyLocal, GUIContent labelLocal)
        {
            // if the index is the one
            if (this.m_SceneIndex == -1)
            {
                // the name of the scene analized
                string sSceneName;

                // local scenes according to the Build settings
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                // copy from the content
                this.m_SceneNames = new GUIContent[scenes.Length + 1];

                // always add [RELOAD SCENE] as an option
                this.m_SceneNames[0] = new GUIContent("[RELOAD SCENE]");

                // cycle through them to 
                for (int i = 0; i < this.m_SceneNames.Length - 1; i++)
                {
                    // get local path to split it into the subject name
                    string path = scenes[i].path;

                    // split it with the m_ScenePathSplitters
                    string[] splitPath = path.Split(this.m_ScenePathSplitters, StringSplitOptions.RemoveEmptyEntries);

                    // if the name exist
                    if (splitPath.Length > 0)
                    {
                        // asign it to the name splited without extension
                        sSceneName = splitPath[splitPath.Length - 1];
                    }
                    else
                    {
                        // it is a deleted scene, 
                        sSceneName = "(Deleted Scene)";
                    }

                    // create into the gui and the names
                    this.m_SceneNames[i + 1] = new GUIContent(sSceneName);
                }


                // if there is a mistake and no scenes are added
                if (this.m_SceneNames.Length == 1)
                {
                    // inform the player that no scenes are into the build settings
                    this.m_SceneNames = new[] { new GUIContent("[No Scenes In Build Settings]") };
                }

                // look for the name searched to select it
                if (!string.IsNullOrEmpty(propertyLocal.stringValue))
                {
                    // was it found?
                    bool sceneNameFound = false;

                    // cycle through the array of scenes
                    for (int i = 0; i < this.m_SceneNames.Length; i++)
                    {
                        // if it is the same
                        if (this.m_SceneNames[i].text == propertyLocal.stringValue)
                        {
                            // select it
                            this.m_SceneIndex = i;

                            // because i found it
                            sceneNameFound = true;

                            // ... and stop the search, target found
                            break;
                        }
                    }

                    // if the scene is not found
                    if (!sceneNameFound)
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
                propertyLocal.stringValue = this.m_SceneNames[this.m_SceneIndex].text;
            }

            // if my current index is the same as the previous
            int oldIndex = this.m_SceneIndex;

            // popup with all the scenes names ordered
            this.m_SceneIndex = EditorGUI.Popup(positionLocal, labelLocal, this.m_SceneIndex, this.m_SceneNames);

            // if it is diferent
            if (oldIndex != this.m_SceneIndex)
            {
                // the current selected
                propertyLocal.stringValue = this.m_SceneNames[this.m_SceneIndex].text;
            }
        }
    }
}