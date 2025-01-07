using System.Collections.Generic;
using Fusion;
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
        
        public StartGameArgs CreateRoomOptions(string roomName, string mapName, int maxPlayers, TanksMP.GameMode gameMode, bool isVisible = true)
        {
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionName = roomName;
            startGameArgs.PlayerCount = maxPlayers;
            startGameArgs.IsVisible = isVisible;
            startGameArgs.GameMode = Fusion.GameMode.Shared;
            
            // custom properties
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = (byte)(int)gameMode;
            customProperties[RoomKeys.mapKey] = mapName;
            startGameArgs.SessionProperties = customProperties;

            return startGameArgs;
        }

        public StartGameArgs CreateRoomOptions(string mapName, byte gameMode)
        {
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = gameMode;
            customProperties[RoomKeys.mapKey] = mapName;
            
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionProperties = customProperties;
            startGameArgs.GameMode = Fusion.GameMode.Shared;

            return startGameArgs;
        }

        public StartGameArgs CreateRoomOptionsGameMode(byte gameMode)
        {
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = gameMode;
            
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionProperties = customProperties;
            startGameArgs.GameMode = Fusion.GameMode.Shared;

            return startGameArgs;
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