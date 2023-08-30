using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPanel : GamePanel
    {
        public bool ControlsHUD = true;
        [FormerlySerializedAs("TeamPanel")] public List<ScoreboardPlayerBadge> PlayerRows;

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
            List<ScoreboardRowData> rows = GetOfflinePlayers();
            List<TeamState> teamStates = TeamUtility.GetTeamStates();

            foreach (TeamState teamState in teamStates)
            {
                rows = rows.Concat(teamState.PlayerRows).ToList();
            }

            rows = rows.OrderByDescending(row => row.Kills).ThenBy(row => row.Kills).ToList();

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