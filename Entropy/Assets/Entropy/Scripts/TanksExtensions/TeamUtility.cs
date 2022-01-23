using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Player = Photon.Realtime.Player;

namespace Vashta.Entropy.TanksExtensions
{
    public class TeamUtility
    {
        public static List<TeamState> GetTeamStates()
        {
            Team[] teams = GameManager.GetInstance().teams;
            int[] scores = PhotonNetwork.CurrentRoom.GetScore();

            if (teams.Length != scores.Length)
            {
                Debug.LogWarning($"Team count ({teams.Length}) did not match score count({scores.Length})!");
            }

            List<TeamState> teamStates = new List<TeamState>();
            
            for (int i = 0; i < teams.Length && i < scores.Length; i++)
            {
                TeamState state = new TeamState(teams[i], scores[i], i);
                teamStates.Add(state);
            }

            return teamStates;
        }
    }
}