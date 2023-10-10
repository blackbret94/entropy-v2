using System;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomOptionsReader
    {
        private const string GameModeString = "mode";


        public TanksMP.GameMode GetGameMode()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            if (roomProperties.TryGetValue(GameModeString, out var property))
            {
                Debug.Log("retrieved game mode from Room Info");
                int gameMode = (byte)property;
                return (TanksMP.GameMode) gameMode;
            }
            else
            {
                Debug.Log("retrieved game mode from playerprefs");
                TanksMP.GameMode gameMode = (TanksMP.GameMode)PlayerPrefs.GetInt(PrefsKeys.gameMode);
                return gameMode;
            }
        }
    }
}