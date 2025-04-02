using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Network
{
    [Serializable]
    public class MatchmakingArgs
    {
        public bool isMultiplayer;
        public string roomName;
        public string password;
        public int maxPlayers;
        public string mapName;
        public TanksMP.GameMode gameMode;
        public bool isVisible = true;

        public MatchmakingArgs()
        {
            
        }

        public MatchmakingArgs(bool isMultiplayer, string roomName, string password, int maxPlayers, string mapName,
            TanksMP.GameMode gameMode, bool isVisible=true)
        {
            this.isMultiplayer = isMultiplayer;
            this.roomName = roomName;
            this.password = password;
            this.maxPlayers = maxPlayers;
            this.mapName = mapName;
            this.gameMode = gameMode;
            this.isVisible = isVisible;
        }
        
        public string Encrypt()
        {
            return Encryptor.Encrypt(ToJson());
        }
        
        private string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}