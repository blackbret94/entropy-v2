using System.Collections.Generic;
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

        [Header("Music")] 
        public AudioClip[] MusicTracks;

        public AudioClip VictoryMusic;
        public AudioClip DefeatMusic;

        [Header("Supported Game Modes")]
        public List<TanksMP.GameMode> SupportedGameModes;

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
        
        public TanksMP.GameMode GetRandomGamemode()
        {
            if (SupportedGameModes.Count == 0)
                return TanksMP.GameMode.TDM;

            int rand = Random.Range(0, SupportedGameModes.Count);
            return SupportedGameModes[rand];
        }
    }
}