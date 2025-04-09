using System.Collections.Generic;
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

        public SessionInfo GetSessionInfo()
        {
            return _sessionInfo;
        }

        public StartGameArgs GetStartGameArgs()
        {
            Dictionary<string, SessionProperty> properties = new Dictionary<string, SessionProperty>();

            
            // Get mode
            string modeKey = RoomKeys.modeKey;
            if(_sessionInfo.Properties.TryGetValue(modeKey, out var modeProperty))
                properties[modeKey] =  modeProperty;
            
            // Get map
            string mapKey = RoomKeys.mapKey;
            if(_sessionInfo.Properties.TryGetValue(mapKey, out var mapProperty))
                properties[mapKey] =  mapProperty;
            
            return new StartGameArgs()
            {
                SessionName = _sessionInfo.Name,
                GameMode = Fusion.GameMode.Shared,
                SessionProperties = properties
            };
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
            return _sessionInfo.Name;
            // ReadOnlyDictionary<string, SessionProperty> customProperties = _sessionInfo.Properties;
            //
            // if (customProperties.TryGetValue(RoomKeys.roomNameKey, out var property))
            // {
            //     return (string)property;
            // }
            //
            // return "";
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

            string roomName = (string)_sessionInfo.Name;

            return $"{roomName} | {map} ({_sessionInfo.PlayerCount}/{_sessionInfo.MaxPlayers})";
        }
    }
}