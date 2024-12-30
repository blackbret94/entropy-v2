using TanksMP;
using UnityEngine;
using Player = Photon.Realtime.Player;
using System.Collections.Generic;
using Photon.Pun;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamState
    {
        public TeamState(Team team, int score, int teamId, bool includeLocalPlayer)
        {
            Team = team;
            Score = score;
            TeamId = teamId;
            PlayerRows = GetPlayerRows(includeLocalPlayer);
        }

        public string Name => Team.name;
        public Material Material => Team.material;
        public Color Color
        {
            get
            {
                if (Team.material == null)
                    return Color.white;
                else
                    return Team.material.color;
            }
        }

        public Transform Spawn => Team.spawn;
        public Team Team { get; }
        public int Score { get; }
        public int TeamId { get; }
        public List<ScoreboardRowData> PlayerRows { get; }
        public int Size() => PlayerRows.Count;

        public ScoreboardRowData GetRow(int i)
        {
            if (i > Size())
            {
                Debug.LogError("Attempted to get row with higher index than size");
                return null;
            }
            else
            {
                return PlayerRows[i];
            }
        }

        private List<ScoreboardRowData> GetPlayerRows(bool includeLocalPlayer)
        {
            List<ScoreboardRowData> data = new List<ScoreboardRowData>();

            // players
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            foreach (KeyValuePair<int, Player> kvp in players)
            {
                Player player = kvp.Value;
                if (player.GetTeam() == TeamId)
                {
                    if (player.IsLocal && !includeLocalPlayer)
                        continue;
                    
                    ScoreboardRowData row = new ScoreboardRowData(player, Team, player.IsLocal);
                    data.Add(row);
                }
            }
            
            // bots
            List<PlayerBot> bots = GameManager.GetInstance().BotController.GetBotList();
            foreach (PlayerBot bot in bots)
            {
                if (bot.teamIndex == TeamId)
                {
                    ScoreboardRowData row = new ScoreboardRowData(bot, Team);
                    data.Add(row);
                }
            }
            
            return data;
        }
    }
}