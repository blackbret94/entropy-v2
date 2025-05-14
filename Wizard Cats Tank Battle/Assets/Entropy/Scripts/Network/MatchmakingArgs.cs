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
        public int maxPlayers;
        public string mapName;
        public TanksMP.GameMode gameMode;
        public bool isVisible = true;
        public int maxScore;
        public int maxTime;

        public MatchmakingArgs()
        {
            
        }

        public MatchmakingArgs(bool isMultiplayer, string roomName, int maxPlayers, string mapName,
            TanksMP.GameMode gameMode, bool isVisible, int maxScore, int maxTime)
        {
            this.isMultiplayer = isMultiplayer;
            this.roomName = roomName;
            this.maxPlayers = maxPlayers;
            this.mapName = mapName;
            this.gameMode = gameMode;
            this.isVisible = isVisible;
            this.maxScore = maxScore;
            this.maxTime = maxTime;
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