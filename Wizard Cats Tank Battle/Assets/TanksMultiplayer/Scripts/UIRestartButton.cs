/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.PhotonExtensions;

namespace TanksMP
{
    /// <summary>
    /// This script is attached to a runtime-generated gameobject in the game scene,
    /// taken over to the intro scene to directly request starting a new multiplayer game.
    /// </summary>
    public class UIRestartButton : MonoBehaviourPunCallbacks 
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
            NetworkMode networkMode = (NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode);

            if (networkMode == NetworkMode.Online)
            {
                // Handle online
            
                if (!PhotonNetwork.IsConnected)
                {
                    NetworkManagerCustom.GetInstance().Connect(networkMode);

                    // while (!PhotonNetwork.IsConnected)
                    // {
                    //     yield return null;
                    // }
                }
                else
                {
                    OnConnectedToMaster();
                }
            }
            else
            {
                // Handle offline
                
                // Clear connection
                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.Disconnect();
                    while (PhotonNetwork.IsConnected)
                    {
                        yield return null;
                    }
                }
            
                NetworkManagerCustom.GetInstance().Connect(networkMode);
                
                OnConnectedToMaster();
            }
            
            
        }

        /// <summary>
        /// Called after the connection to the master is established.
        /// See the official Photon docs for more details.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            // Right now it goes to random map/mode.  Can save conditions later
            FindObjectOfType<RoomController>().Play();
            
            Destroy(gameObject);
        }
    }
}
