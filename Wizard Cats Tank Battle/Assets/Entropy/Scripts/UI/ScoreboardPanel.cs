using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPanel : GamePanel
    {
        public bool ControlsHUD = true;
        public List<ScoreboardTeamBadge> TeamPanel;

        private void Start()
        {
            Inflate();
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            
            if(ControlsHUD)
                HUDPanel.Get().ClosePanel();
            
            Refresh();
        }

        public override void ClosePanel()
        {
            if(ControlsHUD)
                HUDPanel.Get().OpenPanel();
            
            base.ClosePanel();
        }

        public override void Refresh()
        {
            base.Refresh();
            
            Inflate();
        }

        // note that "team" badges are a legacy name, these are rows for individual players now.
        private void Inflate()
        {
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            List<TeamState> teamStates = TeamUtility.GetTeamStates();

            foreach (TeamState teamState in teamStates)
            {
                rows = rows.Concat(teamState.PlayerRows).ToList();
            }

            rows = rows.OrderByDescending(row => row.Kills).ThenBy(row => row.Kills).ToList();

            if(rows.Count > TeamPanel.Count)
                Debug.LogWarning($"There are more rows ({teamStates.Count}) than panels ({TeamPanel.Count})");
            
            for (int i = 0; i < TeamPanel.Count; i++)
            {
                ScoreboardTeamBadge teamBadgePanel = TeamPanel[i];

                if (i < rows.Count)
                {
                    ScoreboardRowData data = rows[i];

                    teamBadgePanel.Setup(data);
                    teamBadgePanel.SetActive(true);
                }
                else
                {
                    teamBadgePanel.SetActive(false);
                }
            }
        }
    }
}