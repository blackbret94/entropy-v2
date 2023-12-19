using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Scoreboard;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPanel : GamePanel
    {
        public bool ControlsHUD = true;
        [FormerlySerializedAs("TeamPanel")] public List<ScoreboardPlayerBadge> PlayerRows;
        private ScoreboardCalculator _scoreboardCalculator;

        private bool _hasInit = false;
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;

            _scoreboardCalculator = new ScoreboardCalculator();
            Inflate();

            _hasInit = true;
        }

        public override void OpenPanel()
        {
            Init();
            
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
            List<ScoreboardRowData> rows = _scoreboardCalculator.GetScores(true);
            List<TeamState> teamStates = TeamUtility.GetTeamStates();

            if(rows.Count > PlayerRows.Count)
                Debug.LogWarning($"There are more rows ({teamStates.Count}) than panels ({PlayerRows.Count})");
            
            for (int i = 0; i < PlayerRows.Count; i++)
            {
                ScoreboardPlayerBadge playerBadgePanel = PlayerRows[i];

                if (i < rows.Count)
                {
                    ScoreboardRowData data = rows[i];

                    playerBadgePanel.Setup(data);
                    playerBadgePanel.SetActive(true);
                }
                else
                {
                    playerBadgePanel.SetActive(false);
                }
            }
        }
    }
}