using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.Scoreboard
{
    public class ScoreboardCalculator
    {
        public List<ScoreboardRowData> GetScores(bool includeOfflinePlayers)
        {
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            
            if(includeOfflinePlayers)
                rows = rows.Concat( GetOfflinePlayers()).ToList();
            
            List<TeamState> teamStates = TeamUtility.GetTeamStates();

            foreach (TeamState teamState in teamStates)
            {
                rows = rows.Concat(teamState.PlayerRows).ToList();
            }

            return rows.OrderByDescending(row => row.Kills).ThenBy(row => row.Deaths).ThenBy(row => row.Name).ToList();
        }
        
        private List<ScoreboardRowData> GetOfflinePlayers()
        {
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            ScoreboardRowDataSerializableList rowsSerialized = PhotonNetwork.CurrentRoom.ReadScoreboard();

            foreach (ScoreboardRowDataSerializable serialized in rowsSerialized.list)
            {
                rows.Add(new ScoreboardRowData(serialized));
            }

            return rows;
        }
    }
}