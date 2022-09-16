using System.Collections.Generic;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamScoreState
    {
        public TeamScoreState(int teamIndex)
        {
            TeamIndex = teamIndex;
            PlayerScores = new List<PlayerScore>();
        }

        public void AddPlayerScore(PlayerScore playerScore)
        {
            PlayerScores.Add(playerScore);
        }

        public int TeamIndex { get; }
        public List<PlayerScore> PlayerScores { get; }
    }
}