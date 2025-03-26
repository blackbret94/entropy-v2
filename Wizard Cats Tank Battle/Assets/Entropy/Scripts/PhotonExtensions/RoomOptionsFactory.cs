using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using WebSocketSharp;

namespace Vashta.Entropy.PhotonExtensions
{
    [RequireComponent(typeof(NetworkSceneManagerDefault))]
    public class RoomOptionsFactory : MonoBehaviour
    {
        public CatNameList CatNameList;
        public NetworkSceneManagerDefault NetworkSceneManagerDefault;

        private const int MaxRoomNameLength = 30;

        private void Awake()
        {
            NetworkSceneManagerDefault = GetComponent<NetworkSceneManagerDefault>();
        }
        
        /// <summary>
        /// Catches issues in room name, generates a name if empty
        /// </summary>
        public string CreateRoomName(string roomName="")
        {
            // Generate
            if (roomName == "")
            {
                return roomName;
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
        
        public StartGameArgs CreateRoomOptions(bool isMultiplayer, string roomName, string mapName, int maxPlayers, TanksMP.GameMode gameMode, string password, bool isVisible = true)
        {
            StartGameArgs startGameArgs = new StartGameArgs();

            if (roomName != "")
            {
                startGameArgs.SessionName = roomName;
            }
            
            startGameArgs.PlayerCount = maxPlayers;
            startGameArgs.IsVisible = isVisible;
            startGameArgs.GameMode = isMultiplayer ? Fusion.GameMode.Shared : Fusion.GameMode.Single;
            startGameArgs.SceneManager = NetworkSceneManagerDefault;
            
            // custom properties
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = (byte)(int)gameMode;

            if (mapName != "random")
            {
                customProperties[RoomKeys.mapKey] = mapName;
            }

            if (!password.IsNullOrEmpty())
            {
                customProperties[RoomKeys.password] = password;
            }
            
            startGameArgs.SessionProperties = customProperties;

            return startGameArgs;
        }

        public StartGameArgs CreateRoomOptions(string mapName, byte gameMode)
        {
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = gameMode;
            
            if (mapName != "random")
            {
                customProperties[RoomKeys.mapKey] = mapName;
            }
            
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionProperties = customProperties;
            startGameArgs.GameMode = Fusion.GameMode.Shared;
            startGameArgs.SceneManager = NetworkSceneManagerDefault;

            return startGameArgs;
        }

        public StartGameArgs CreateRoomOptionsGameMode(byte gameMode)
        {
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey]= gameMode;
            
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionProperties = customProperties;
            startGameArgs.GameMode = Fusion.GameMode.Shared;
            startGameArgs.SceneManager = NetworkSceneManagerDefault;

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