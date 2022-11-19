using TanksMP;
using UnityEngine;
using Player = Photon.Realtime.Player;
using System.Collections.Generic;
using Photon.Pun;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamState
    {
        public TeamState(Team team, int score, int teamId)
        {
            Team = team;
            Score = score;
            TeamId = teamId;
            PlayerRows = GetPlayerRows();
        }

        public string Name => Team.name;
        public Material Material => Team.material;
        public Transform Spawn => Team.spawn;
        public Team Team { get; }
        public int Score { get; }
        public int TeamId { get; }
        public List<ScoreboardRowData> PlayerRows { get; }

        private List<ScoreboardRowData> GetPlayerRows()
        {
            List<ScoreboardRowData> data = new List<ScoreboardRowData>();

            // players
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            foreach (KeyValuePair<int, Player> kvp in players)
            {
                Player player = kvp.Value;
                if (player.GetTeam() == TeamId)
                {
                    ScoreboardRowData row = new ScoreboardRowData(player, Team, player.IsLocal);
                    data.Add(row);
                    Debug.Log("Adding row: " + row.FormatAsString());
                }
            }
            
            // bots
            List<PlayerBot> bots = GameManager.GetInstance().GetBotList();
            foreach (PlayerBot bot in bots)
            {
                if (bot.teamIndex == TeamId)
                {
                    ScoreboardRowData row = new ScoreboardRowData(bot, Team);
                    data.Add(row);
                    Debug.Log("Adding row: " + row.FormatAsString());
                }
            }
            
            return data;
        }
    }
}