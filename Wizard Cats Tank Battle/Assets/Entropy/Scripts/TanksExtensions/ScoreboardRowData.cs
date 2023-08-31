using TanksMP;
using UnityEngine;
using Player = Photon.Realtime.Player;

namespace Vashta.Entropy.TanksExtensions
{
    public class ScoreboardRowData
    {
        public string Name { get; }
        public int Kills { get; }
        public int Deaths { get; }
        public Team Team { get; }
        public Material Material => Team.material;
        public bool IsLocalPlayer { get; }
        public Player Player { get; }
        public int ClassId { get; }
        public bool PlayerIsOnline { get; }

        public ScoreboardRowData(Player player, Team team, bool isLocalPlayer)
        {
            Player = player;
            Name = player.NickName;
            Kills = player.GetKills();
            Deaths = player.GetDeaths();
            Team = team;
            IsLocalPlayer = isLocalPlayer;
            ClassId = player.GetClassId();
            PlayerIsOnline = true;
        }

        public ScoreboardRowData(PlayerBot bot, Team team)
        {
            Name = bot.myName;
            Kills = bot.kills;
            Deaths = bot.deaths;
            Team = team;
            IsLocalPlayer = false;
            ClassId = bot.classId;
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
    }
}