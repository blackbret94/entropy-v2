using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Vashta.Entropy.Network;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Util;
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
        
        public StartGameArgs CreateRoomOptions(MatchmakingArgs matchmakingArgs)
        {
            StartGameArgs startGameArgs = new StartGameArgs();

            // if (matchmakingArgs.roomName != "")
            // {
                // startGameArgs.SessionName = matchmakingArgs.roomName;
            // }

            startGameArgs.SessionName = HashCodeGenerator.GenerateRandomString(6);
            
            startGameArgs.PlayerCount = matchmakingArgs.maxPlayers;
            startGameArgs.IsVisible = matchmakingArgs.isVisible;
            startGameArgs.GameMode = matchmakingArgs.isMultiplayer ? Fusion.GameMode.Shared : Fusion.GameMode.Single;
            startGameArgs.SceneManager = NetworkSceneManagerDefault;
            
            // custom properties
            // game mode
            Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
            customProperties[RoomKeys.modeKey] = (byte)(int)matchmakingArgs.gameMode;

            // map
            bool mapIsRandom = matchmakingArgs.mapName == "random";
            string mapName = !mapIsRandom ? matchmakingArgs.mapName : "random";
            customProperties[RoomKeys.mapKey] = mapName;
            
            if (matchmakingArgs.roomName != "")
            {
                customProperties[RoomKeys.roomNameKey] = matchmakingArgs.roomName;
            }
            else
            {
                customProperties[RoomKeys.roomNameKey] = GenerateRoomName();
            }
            
            // score
            if (!mapIsRandom)
            {
                customProperties[RoomKeys.maxScoreKey] = matchmakingArgs.maxScore;
            }
            
            // time
            customProperties[RoomKeys.maxTime] = matchmakingArgs.maxTime;
            
            // bot filling
            customProperties[RoomKeys.botFilling] = matchmakingArgs.botFilling;
            
            startGameArgs.SessionProperties = customProperties;

            return startGameArgs;
        }

        public StartGameArgs QuickplayArgs()
        {
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionName = HashCodeGenerator.GenerateRandomString(6);
            startGameArgs.PlayerCount = 12;
            startGameArgs.IsVisible = true;
            startGameArgs.GameMode = Fusion.GameMode.Shared;
            startGameArgs.SceneManager = NetworkSceneManagerDefault;
            
            return startGameArgs;
        }

        // public StartGameArgs QuickplayArgsForMode(GameModeDefinition gameMode)
        // {
        //     StartGameArgs startGameArgs = new StartGameArgs();
        //     startGameArgs.SessionName = HashCodeGenerator.GenerateRandomString(6);
        //     startGameArgs.PlayerCount = 12;
        //     startGameArgs.IsVisible = true;
        //     startGameArgs.GameMode = Fusion.GameMode.Shared;
        //     startGameArgs.SceneManager = NetworkSceneManagerDefault;
        //     
        //     Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
        //     customProperties[RoomKeys.modeKey] = (byte)(int)gameMode.GameMode;
        //     customProperties[RoomKeys.roomNameKey] = GenerateRoomName();
        //     customProperties[RoomKeys.mapKey] = "random";
        //     startGameArgs.SessionProperties = customProperties;
        //     
        //     return startGameArgs;
        // }
        
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