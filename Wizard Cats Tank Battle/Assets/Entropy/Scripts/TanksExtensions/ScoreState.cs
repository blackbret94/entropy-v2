using System.Collections.Generic;

namespace Vashta.Entropy.TanksExtensions
{
    public class ScoreState
    {
        public ScoreState()
        {
            _teamScores = new Dictionary<int, TeamScoreState>();
        }

        private readonly Dictionary<int, TeamScoreState> _teamScores;

        public void AddTeam(int teamIndex)
        {
            _teamScores.Add(teamIndex, new TeamScoreState(teamIndex));
        }

        public TeamScoreState GetTeamScore(int teamIndex)
        {
            if (_teamScores.ContainsKey(teamIndex))
                return _teamScores[teamIndex];

            throw new KeyNotFoundException();
        }

        public void AddPlayerScoreToTeam(int teamIndex, PlayerScore playerScore)
        {
            TeamScoreState team = GetTeamScore(teamIndex);
            team.AddPlayerScore(playerScore);
        }
    }
}