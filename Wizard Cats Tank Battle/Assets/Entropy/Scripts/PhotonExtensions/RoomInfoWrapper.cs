using System.Collections.ObjectModel;
using Fusion;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomInfoWrapper
    {
        private SessionInfo _sessionInfo;

        public RoomInfoWrapper(SessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
        }

        public string GetMapName()
        {
            ReadOnlyDictionary<string, SessionProperty> customProperties = _sessionInfo.Properties;

            if (customProperties.TryGetValue(RoomKeys.mapKey, out var property))
            {
                return (string)property;
            }

            return "";
        }

        public TanksMP.GameMode GetGameMode()
        {
            ReadOnlyDictionary<string, SessionProperty> customProperties = _sessionInfo.Properties;

            if (customProperties.TryGetValue(RoomKeys.modeKey, out var property))
            {
                return (TanksMP.GameMode)(byte)property;
            }

            Debug.LogError("RoomInfo is missing a gamemode!");
            return TanksMP.GameMode.TDM;
        }

        public string GetRoomNameId()
        {
            return _sessionInfo.Name;
        }

        public string GetDisplayRoomName()
        {
            ReadOnlyDictionary<string, SessionProperty> customProperties = _sessionInfo.Properties;

            if (customProperties.TryGetValue(RoomKeys.roomNameKey, out var property))
            {
                return (string)property;
            }

            return "";
        }

        public int GetMaxPlayers()
        {
            return _sessionInfo.MaxPlayers;
        }

        public bool IsOpen()
        {
            return _sessionInfo.IsOpen;
        }

        public bool IsVisible()
        {
            return _sessionInfo.IsVisible;
        }
        
        public string StringifyRoom()
        {
            if (!_sessionInfo.Properties.ContainsKey(RoomKeys.mapKey))
            {
                Debug.LogError("Retrieved lobby that is missing a map!");
                return null;
            }

            string map = (string)_sessionInfo.Properties[RoomKeys.mapKey];
            
            if (!_sessionInfo.Properties.ContainsKey(RoomKeys.modeKey))
            {
                Debug.LogError("Retrieved lobby that is missing a mode!");
                return null;
            }

            if (!_sessionInfo.Properties.ContainsKey(RoomKeys.roomNameKey))
            {
                Debug.LogError("Retrieved lobby that is missing a room name!");
                return null;
            }

            string roomName = (string)_sessionInfo.Properties[RoomKeys.roomNameKey];

            return $"{roomName} | {map} ({_sessionInfo.PlayerCount}/{_sessionInfo.MaxPlayers})";
        }
    }
}