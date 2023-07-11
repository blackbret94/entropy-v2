using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.MapSelection
{
    [CreateAssetMenu(fileName = "Map Definition", menuName = "Entropy/Map Definition", order = 1)]
    public class MapDefinition : ScriptableObjectWithID
    {
        public string Title;
        public string Description;
        public Sprite MapPreviewImage;
        public string SceneName;
        public short PlayerCount;
        public short TeamCount;

        [Header("Supported Game Modes")]
        public bool TeamDeathmatch;
        public bool CaptureTheFlag;
        public bool KingOfTheHill;

        public int SceneIndex()
        {
            string sceneName = SceneName;
            
            int sceneIndex = -1;
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string[] scenePath = SceneUtility.GetScenePathByBuildIndex(i).Split('/');
                if (scenePath[scenePath.Length - 1].StartsWith(sceneName))
                {
                    sceneIndex = i;
                    break;
                }
            }
			
            //check that your scene begins with the game mode abbreviation
            if(sceneIndex == -1)
            {
                Debug.LogWarning("No Scene for selected name " + sceneName + "found in Build Settings!");
            }

            return sceneIndex;
        }
    }
}