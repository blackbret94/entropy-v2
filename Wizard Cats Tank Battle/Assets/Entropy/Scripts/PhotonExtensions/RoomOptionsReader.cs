using ExitGames.Client.Photon;
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
                int gameMode = (int)property;
                return (TanksMP.GameMode) gameMode;
            }
            else
            {
                Debug.Log("retrieved game mode from playerprefs");
                TanksMP.GameMode gameMode = (TanksMP.GameMode)(int)PlayerPrefs.GetInt(PrefsKeys.gameMode);
                return TanksMP.GameMode.CTF;
            }
        }
    }
}