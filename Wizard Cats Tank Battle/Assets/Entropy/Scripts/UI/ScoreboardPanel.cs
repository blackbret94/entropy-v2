using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPanel : GamePanel
    {
        public List<ScoreboardTeamBadge> TeamPanel;

        private void Start()
        {
            Inflate();
        }

        public void Inflate()
        {
            List<TeamState> teamStates = GetTeamStatesSortedByScore();

            if(teamStates.Count > TeamPanel.Count)
                Debug.LogWarning($"There are more teams ({teamStates.Count}) than panels ({TeamPanel.Count})");
            
            for (int i = 0; i < TeamPanel.Count; i++)
            {
                ScoreboardTeamBadge teamBadgePanel = TeamPanel[i];

                if (i < teamStates.Count)
                {
                    TeamState teamState = teamStates[i];

                    teamBadgePanel.Setup(teamState);
                    teamBadgePanel.SetActive(true);
                }
                else
                {
                    teamBadgePanel.SetActive(false);
                }
            }
        }

        private List<TeamState> GetTeamStatesSortedByScore()
        {
            List<TeamState> teamStates = TeamUtility.GetTeamStates();
            return teamStates.OrderByDescending(ts=>ts.Score).ToList();
        }
    }
}