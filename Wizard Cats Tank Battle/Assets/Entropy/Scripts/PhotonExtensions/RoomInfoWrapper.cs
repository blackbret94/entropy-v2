using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomInfoWrapper
    {
        private RoomInfo _roomInfo;

        public RoomInfoWrapper(RoomInfo roomInfo)
        {
            _roomInfo = roomInfo;
        }

        public string GetMapName()
        {
            Hashtable customProperties = _roomInfo.CustomProperties;

            if (customProperties.TryGetValue(RoomKeys.mapKey, out var property))
            {
                return (string)property;
            }

            return "";
        }

        public TanksMP.GameMode GetGameMode()
        {
            Hashtable customProperties = _roomInfo.CustomProperties;

            if (customProperties.TryGetValue(RoomKeys.modeKey, out var property))
            {
                return (TanksMP.GameMode)(byte)property;
            }

            Debug.LogError("RoomInfo is missing a gamemode!");
            return TanksMP.GameMode.TDM;
        }

        public string GetRoomNameId()
        {
            return _roomInfo.Name;
        }

        public string GetDisplayRoomName()
        {
            Hashtable customProperties = _roomInfo.CustomProperties;

            if (customProperties.TryGetValue(RoomKeys.roomNameKey, out var property))
            {
                return (string)property;
            }

            return "";
        }

        public int GetMaxPlayers()
        {
            return _roomInfo.MaxPlayers;
        }

        public bool IsOpen()
        {
            return _roomInfo.IsOpen;
        }

        public bool IsVisible()
        {
            return _roomInfo.IsVisible;
        }
        
        public string StringifyRoom()
        {
            if (!_roomInfo.CustomProperties.ContainsKey("map"))
            {
                Debug.LogError("Retrieved lobby that is missing a map!");
                return null;
            }

            string map = (string)_roomInfo.CustomProperties["map"];
            
            if (!_roomInfo.CustomProperties.ContainsKey("mode"))
            {
                Debug.LogError("Retrieved lobby that is missing a mode!");
                return null;
            }

            if (!_roomInfo.CustomProperties.ContainsKey("roomName"))
            {
                Debug.LogError("Retrieved lobby that is missing a room name!");
                return null;
            }

            string roomName = (string)_roomInfo.CustomProperties["roomName"];

            return $"{roomName} | {map} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
        }
    }
}