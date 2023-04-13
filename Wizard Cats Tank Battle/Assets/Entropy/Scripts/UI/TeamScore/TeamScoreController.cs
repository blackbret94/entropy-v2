using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.UI.TeamScore
{
    public class TeamScoreController : MonoBehaviour
    {
        private static TeamScoreController _instance;
        public List<TeamScoreUnit> Scores = new();

        public static TeamScoreController GetInstance()
        {
            return _instance;
        }

        private void Start()
        {
            _instance = this;
        }

        public void UpdateScores(int[] scores)
        {
            for (int i = 0; i < scores.Length; i++)
            {
                int teamIndex = i + 1;
                
                foreach (var scoreUnit in Scores)
                {
                    if(teamIndex == scoreUnit.TeamIndex)
                        scoreUnit.UpdateScore(scores[i]);
                }
            }
        }

        public void UpdateTeamSizes(int[] sizes)
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                int teamIndex = i + 1;

                foreach (var scoreUnit in Scores)
                {
                    if(teamIndex == scoreUnit.TeamIndex)
                        scoreUnit.UpdateNumberOfPlayers(sizes[i]);
                }
            }
        }
    }
}