using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TanksMP;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomConnectionController : MonoBehaviour
    {
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
                NetworkManagerCustom.JoinRandomRoom();
            }
            else
            {
                // Join offline
                NetworkManagerCustom.GetInstance().JoinRandomRoomOffline(new Hashtable());
            }
            
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void Play(string mapName, int gameMode)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            
            Hashtable expectedCustomRoomProperties = 
                new Hashtable()
                {
                    { RoomKeys.mapKey, mapName},
                    { RoomKeys.modeKey, (byte)gameMode }
                };
            
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            StartCoroutine(HandleTimeout());
        }

        public void PlayOffline(string mapName, int gameMode)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            
            Hashtable expectedCustomRoomProperties = 
                new Hashtable()
                {
                    { RoomKeys.mapKey, mapName},
                    { RoomKeys.modeKey, (byte)gameMode }
                };
            
            NetworkManagerCustom.GetInstance().JoinRandomRoomOffline(expectedCustomRoomProperties);
        }

        public void JoinRoom(string roomName)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            NetworkManagerCustom.JoinRoom(roomName);
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void CreateRoom(RoomOptions roomOptions)
        {
            UIMain.GetInstance().ToggleLoadingWindow(true);
            NetworkManagerCustom.CreateMatch(roomOptions);
            StartCoroutine(HandleTimeout());
        }
        
        //coroutine that waits 10 seconds before cancelling joining a match
        IEnumerator HandleTimeout()
        {
            yield return new WaitForSeconds(10);

            //timeout has passed, we would like to stop joining a game now
            PhotonNetwork.Disconnect();
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