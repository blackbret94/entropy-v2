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
        public TanksMP.GameMode GetGameMode()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            if (roomProperties.TryGetValue(RoomKeys.modeKey, out var property))
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

        public string GetRoomName()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProperties.TryGetValue(RoomKeys.roomNameKey, out var property))
            {
                return (string)property;
            }
            else
            {
                return "";
            }
        }

        public string GetMapName()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProperties.TryGetValue(RoomKeys.roomNameKey, out var property))
            {
                return (string)property;
            }
            else
            {
                return "";
            }
        }
    }
}