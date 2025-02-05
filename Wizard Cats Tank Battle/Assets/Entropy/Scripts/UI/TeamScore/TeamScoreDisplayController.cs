using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Vashta.Entropy.UI.TeamScore
{
    public class TeamScoreDisplayController : MonoBehaviour
    {
        private static TeamScoreDisplayController _instance;
        public List<TeamScoreUnit> Scores = new();

        public static TeamScoreDisplayController GetInstance()
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
                int teamIndex = i;
                
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
                int teamIndex = i;

                foreach (var scoreUnit in Scores)
                {
                    if (teamIndex == scoreUnit.TeamIndex)
                    {
                        scoreUnit.UpdateNumberOfPlayers(sizes[i]);
                    }
                }
            }
        }
    }
}