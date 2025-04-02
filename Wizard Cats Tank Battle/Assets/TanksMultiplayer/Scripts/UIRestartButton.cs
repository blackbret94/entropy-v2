/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using Fusion;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.Network;
using Vashta.Entropy.PhotonExtensions;
using SceneNavigator = Vashta.Entropy.SceneNavigation.SceneNavigator;

namespace TanksMP
{
    /// <summary>
    /// This script is attached to a runtime-generated gameobject in the game scene,
    /// taken over to the intro scene to directly request starting a new multiplayer game.
    /// </summary>
    public class UIRestartButton : SimulationBehaviour
    {
        //listen to scene changes
        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        //give the scene some time to initialize
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(EnterPlay());
        }
        
        //call the play button instantly on scene load
        //destroy itself after use
        IEnumerator EnterPlay()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            while(!SceneNavigator.IsMainMenu())
                yield return null;
            
            yield return new WaitForSeconds(.25f);
            
            string matchmakingArgsString = PlayerPrefs.GetString(Vashta.Entropy.SaveLoad.PrefsKeys.matchmakingArgs);
            MatchmakingArgs matchmakingArgs = JsonConvert.DeserializeObject<MatchmakingArgs>(matchmakingArgsString);
        
            UIMain.GetInstance().ToggleLoadingWindow(true);

            RoomOptionsFactory roomOptionsFactory = FindFirstObjectByType<RoomOptionsFactory>();
            StartGameArgs startGameArgs = roomOptionsFactory.CreateRoomOptions(matchmakingArgs);
    
            // create room
            UIMain.GetInstance().roomConnectionController.CreateRoom(startGameArgs);
            
            Destroy(gameObject, .5f);
        }
    }
}
