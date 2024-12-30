using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamUtility
    {
        public static List<TeamState> GetTeamStates(bool includeLocalPlayer = true)
        {
            Team[] teams = GameManager.GetInstance().TeamController.teams;
            int[] scores = PhotonNetwork.CurrentRoom.GetScore();

            if (teams.Length != scores.Length)
            {
                Debug.LogWarning($"Team count ({teams.Length}) did not match score count({scores.Length})!");
            }

            List<TeamState> teamStates = new List<TeamState>();
            
            for (int i = 0; i < teams.Length && i < scores.Length; i++)
            {
                TeamState state = new TeamState(teams[i], scores[i], i, includeLocalPlayer);
                teamStates.Add(state);
            }

            return teamStates;
        }

        public static TeamState GetTeamState(int teamIndex, bool includeLocalPlayer = true)
        {
            Team[] teams = GameManager.GetInstance().TeamController.teams;
            int[] scores = PhotonNetwork.CurrentRoom.GetScore();

            if (teams.Length != scores.Length || teamIndex >= teams.Length || teamIndex >= scores.Length)
            {
                Debug.LogWarning($"Team count ({teams.Length}) did not match score count({scores.Length}), or team index {teamIndex} was too high!");
            }

            return new TeamState(teams[teamIndex], scores[teamIndex], teamIndex, includeLocalPlayer);
        }
    }
}