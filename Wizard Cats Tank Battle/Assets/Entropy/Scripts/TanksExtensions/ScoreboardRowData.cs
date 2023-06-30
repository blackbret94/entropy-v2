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

        public ScoreboardRowData(Player player, Team team, bool isLocalPlayer)
        {
            Player = player;
            Name = player.NickName;
            Kills = player.GetKills();
            Deaths = player.GetDeaths();
            Team = team;
            IsLocalPlayer = isLocalPlayer;
            ClassId = player.GetClassId();
        }

        public ScoreboardRowData(PlayerBot bot, Team team)
        {
            Name = bot.myName;
            Kills = bot.kills;
            Deaths = bot.deaths;
            Team = team;
            IsLocalPlayer = false;
            ClassId = bot.classId;
        }
    }
}