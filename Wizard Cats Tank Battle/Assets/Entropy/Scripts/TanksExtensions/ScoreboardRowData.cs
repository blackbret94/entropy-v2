using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.Scoreboard;

namespace Vashta.Entropy.TanksExtensions
{
    public class ScoreboardRowData
    {
        public string Name { get; }
        public int Kills { get; }
        public int Deaths { get; }
        public TeamInstance TeamInstance { get; }
        public Material Material => TeamInstance.teamDefinition.Material;
        public bool IsLocalPlayer { get; }

        public PlayerController PlayerController { get; }

        public int ClassId { get; }
        public bool PlayerIsOnline { get; }

        public ScoreboardRowData(PlayerController playerController, TeamInstance teamInstance, bool isLocalPlayer)
        {
            PlayerController = playerController;
            Name = playerController.PlayerName;
            Kills = playerController.Kills;
            Deaths = playerController.Deaths;
            TeamInstance = teamInstance;
            IsLocalPlayer = isLocalPlayer;
            ClassId = playerController.ClassController.ClassId;
            PlayerIsOnline = true;
        }

        public ScoreboardRowData(ScoreboardRowDataSerializable serializable)
        {
            Name = serializable.Name;
            Kills = serializable.Kills;
            Deaths = serializable.Deaths;
            ClassId = serializable.ClassId;
            PlayerIsOnline = serializable.PlayerIsOnline;
        }
        
        public ScoreboardRowDataSerializable OfflinePlayerSerializable()
        {
            ScoreboardRowDataSerializable serializable = new ScoreboardRowDataSerializable();
            serializable.Name = Name;
            serializable.Kills = Kills;
            serializable.Deaths = Deaths;
            serializable.ClassId = ClassId;
            serializable.PlayerIsOnline = false;

            return serializable;
        }
        
        public bool IsAlive()
        {
            if (PlayerController != null)
            {
                return PlayerController.IsAlive;
            }
            
            Debug.LogError("Row is not attached to a player!");
            return false;
        }
    }
}