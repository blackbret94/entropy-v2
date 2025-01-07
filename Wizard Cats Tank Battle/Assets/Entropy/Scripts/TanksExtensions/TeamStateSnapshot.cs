using TanksMP;
using UnityEngine;
using System.Collections.Generic;
using Entropy.Scripts.Player;

namespace Vashta.Entropy.TanksExtensions
{
    /// <summary>
    /// This captures the state of a team at one moment in time, to be used in scoreboards etc
    /// </summary>
    public class TeamStateSnapshot
    {
        private GameManager _gameManager;

        public string Name => TeamInstance.teamDefinition.TeamNameDisplay;
        public Material Material => TeamInstance.teamDefinition.Material;
        public Color Color
        {
            get
            {
                if (TeamInstance.teamDefinition.Material == null)
                    return Color.white;
                else
                    return TeamInstance.teamDefinition.Material.color;
            }
        }

        public Transform SpawnArea => TeamInstance.spawnArea;
        public TeamInstance TeamInstance { get; }
        public int Score { get; }
        public int TeamId { get; }
        public List<ScoreboardRowData> PlayerRows { get; }
        public int Size() => PlayerRows.Count;
        
        public TeamStateSnapshot(TeamInstance teamInstance, int score, int teamId, bool includeLocalPlayer)
        {
            TeamInstance = teamInstance;
            Score = score;
            TeamId = teamId;
            PlayerRows = GetPlayerRows(includeLocalPlayer);
            
            _gameManager = GameManager.GetInstance();
        }

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
            List<Player> players = PlayerList.GetAllPlayers;

            foreach (var player in players)
            {
                if (player.GetTeam() == TeamId)
                {
                    if (player.IsLocal && !includeLocalPlayer)
                        continue;
                    
                    ScoreboardRowData row = new ScoreboardRowData(player, TeamInstance, player.IsLocal);
                    data.Add(row);
                }
            }
            
            // bots
            List<PlayerBot> bots = GameManager.GetInstance().BotController.GetBotList();
            foreach (PlayerBot bot in bots)
            {
                if (bot.teamIndex == TeamId)
                {
                    ScoreboardRowData row = new ScoreboardRowData(bot, TeamInstance, false);
                    data.Add(row);
                }
            }
            
            return data;
        }
    }
}