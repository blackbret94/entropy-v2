using ExitGames.Client.Photon;
using Photon.Realtime;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomOptionsFactory : MonoBehaviour
    {
        public CatNameList CatNameList;

        private const int MaxRoomNameLength = 30;

        /// <summary>
        /// Catches issues in room name, generates a name if empty
        /// </summary>
        public string CreateRoomName(string roomName="")
        {
            // Generate
            if (roomName == "")
            {
                roomName = GenerateRoomName();
            }
            
            // Truncate
            if (roomName.Length >= MaxRoomNameLength)
                roomName = roomName.Substring(0, MaxRoomNameLength);
            
            // Sanitize

            return roomName;
        }

        public string CreateRoomNameFromPlayerNickname(string nickName)
        {
            return CreateRoomName(nickName + "'s Room");
        }
        
        public RoomOptions InitRoomOptions(string roomName, string mapName, int maxPlayers, TanksMP.GameMode gameMode)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayers;
            
            roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomKeys.modeKey, RoomKeys.mapKey, RoomKeys.roomNameKey };
            roomOptions.CustomRoomProperties = new Hashtable()
            {
                { RoomKeys.roomNameKey, roomName},
                { RoomKeys.modeKey, (byte)(int)gameMode}, 
                { RoomKeys.mapKey, mapName}
            };

            roomOptions.MaxPlayers = (byte)maxPlayers;
            roomOptions.CleanupCacheOnLeave = false;
            roomOptions.BroadcastPropsChangeToAll = false;

            return roomOptions;
        }

        private string GenerateRoomName()
        {
            if (CatNameList == null)
            {
                Debug.LogError("Missing cat name list!");
                return "";
            }

            return CatNameList.GetRandomName() + "'s Room";
        }
    }
}