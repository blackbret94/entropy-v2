using System.Collections.Generic;
using System.Linq;
using TanksMP;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.Scoreboard
{
    public class ScoreboardCalculator
    {
        public List<ScoreboardRowData> GetScores(bool includeOfflinePlayers)
        {
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            
            // TODO: Support offline players in scoreboard
            // if(includeOfflinePlayers)
                // rows = rows.Concat( GetOfflinePlayers()).ToList();
            
            List<TeamStateSnapshot> teamStates = GameManager.GetInstance().TeamController.GetTeamStates();

            foreach (TeamStateSnapshot teamState in teamStates)
            {
                rows = rows.Concat(teamState.PlayerRows).ToList();
            }

            return rows.OrderByDescending(row => row.Kills).ThenBy(row => row.Deaths).ThenBy(row => row.Name).ToList();
        }
        
        private List<ScoreboardRowData> GetOfflinePlayers()
        {
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            ScoreboardRowDataSerializableList rowsSerialized = null;//PhotonNetwork.CurrentRoom.ReadScoreboard();

            foreach (ScoreboardRowDataSerializable serialized in rowsSerialized.list)
            {
                rows.Add(new ScoreboardRowData(serialized));
            }

            return rows;
        }
    }
}