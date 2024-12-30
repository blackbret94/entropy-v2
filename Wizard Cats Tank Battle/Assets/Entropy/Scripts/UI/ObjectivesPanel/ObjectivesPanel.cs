using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI.ObjectivesPanel
{
    public class ObjectivesPanel : GamePanel
    {
        public TextMeshProUGUI ObjectiveText;

        public List<TeamPlayerRow> TeamPlayerRows;

        private const float RefreshRateS = .33f;
        private float _lastRefreshTime;
        
        private void Start()
        {
            _lastRefreshTime = Time.time;
            Invoke(nameof(Init), .1f);
        }

        private void Update()
        {
            if (Time.time > _lastRefreshTime + RefreshRateS)
            {
                RefreshContent();
            }
        }

        private void Init()
        {
            ObjectiveText.text = GameManager.GetInstance().GameModeDefinition.Description;
        }
        
        // None of this can be cached, as it may change in between refreshes
        private void RefreshContent()
        {
            int teamIndex = PhotonNetwork.LocalPlayer.GetTeam();
            
            // Get team mates
            TeamState teamState = TeamUtility.GetTeamState(teamIndex);
            
            // inflate based on team mates
            for (int i = 0; i < TeamPlayerRows.Count; i++)
            {
                TeamPlayerRow uiRow = TeamPlayerRows[i];
                
                if (i < teamState.Size())
                {
                    // Update row content
                    ScoreboardRowData rowData = teamState.GetRow(i);

                    if (rowData == null)
                    {
                        Debug.LogError("Row data is null!");
                        return;
                    }
                    
                    uiRow.OpenPanel();
                    uiRow.Set(rowData.Name, rowData.ClassId, teamState.Color, rowData.IsAlive());
                }
                else
                {
                    // Hide row
                    uiRow.ClosePanel();
                }
            }
            
            // Update refresh time
            _lastRefreshTime = Time.time;
        }

    }
}