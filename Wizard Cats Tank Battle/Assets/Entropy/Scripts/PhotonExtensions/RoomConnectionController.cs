using System.Collections;
using Fusion;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomConnectionController : SimulationBehaviour
    {
        public RoomOptionsFactory RoomOptionsFactory;
        private NetworkManagerCustom _networkManager;

        private void Start()
        {
            _networkManager = NetworkManagerCustom.GetInstance();
        }
        
        /// <summary>
        /// Tries to enter the game scene. Sets the loading screen active while connecting to the
        /// Matchmaker and starts the timeout coroutine at the same time.
        /// </summary>
        public void Play()
        {
            NetworkMode networkMode = (NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode);
            
            UIMain.GetInstance().ToggleLoadingWindow(true);

            if (networkMode == NetworkMode.Online)
            {
                // Join online
                _networkManager.JoinRandomRoom();
            }
            else
            {
                // Join offline
                _networkManager.JoinRandomRoomOffline(new StartGameArgs());
            }
            
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void Play(string mapName, int gameMode)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);

            StartGameArgs startGameArgs = RoomOptionsFactory.CreateRoomOptions(mapName, (byte)gameMode);
            Runner.StartGame(startGameArgs);
            StartCoroutine(HandleTimeout());
        }

        public void PlayOffline(string mapName, int gameMode)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            
            StartGameArgs startGameArgs = RoomOptionsFactory.CreateRoomOptions(mapName, (byte)gameMode);
            NetworkManagerCustom.GetInstance().JoinRandomRoomOffline(startGameArgs);
        }

        public void JoinRoom(string roomName)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            NetworkManagerCustom.GetInstance().JoinRoom(roomName);
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void CreateRoom(StartGameArgs startGameArgs)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            NetworkManagerCustom.GetInstance().CreateMatch(startGameArgs);
            StartCoroutine(HandleTimeout());
        }
        
        //coroutine that waits 10 seconds before cancelling joining a match
        IEnumerator HandleTimeout()
        {
            yield return new WaitForSeconds(10);

            //timeout has passed, we would like to stop joining a game now
            Runner.Shutdown();
            //display connection issue window
            OnConnectionError();
        }


        //activates the connection error window to be visible
        public void OnConnectionError()
        {
            //game shut down completely
            if (this == null)
                return;

            Debug.LogError("Connection error");
            
            StopAllCoroutines();
            
            UIMain.GetInstance().ShowConnectionErrorWindow();
        }
    }
}