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

        public ScoreboardRowData(Player player, Team team)
        {
            Name = player.NickName;
            Kills = player.GetKills();
            Deaths = player.GetDeaths();
            Team = team;
        }

        public ScoreboardRowData(PlayerBot bot, Team team)
        {
            Name = bot.myName;
            Kills = bot.kills;
            Deaths = bot.deaths;
            Team = team;
        }

        public string FormatAsString()
        {
            return $"Name: {Name}, Team: {Team.name}, Kills: {Kills}, Deaths: {Deaths}";
        }
    }
}