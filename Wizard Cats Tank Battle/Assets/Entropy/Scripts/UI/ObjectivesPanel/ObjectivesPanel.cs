using System.Collections;
using System.Collections.Generic;
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
        private GameManager _gameManager;
        
        private void Start()
        {
            _gameManager = GameManager.GetInstance();
            _lastRefreshTime = Time.time;
            StartCoroutine(Init());
        }

        private void Update()
        {
            if (Time.time > _lastRefreshTime + RefreshRateS)
            {
                RefreshContent();
            }
        }

        private IEnumerator Init()
        {
            GameManager gameManager = GameManager.GetInstance();
            
            while (!gameManager.HasSpawned)
                yield return null;
            
            ObjectiveText.text = gameManager.GameModeDefinition.Description;
        }
        
        // None of this can be cached, as it may change in between refreshes
        private void RefreshContent()
        {
            int teamIndex = _gameManager.localPlayerController.TeamIndex;
            
            // Get team mates
            TeamStateSnapshot teamStateSnapshot = _gameManager.TeamController.GetTeamState(teamIndex);
            
            // inflate based on team mates
            for (int i = 0; i < TeamPlayerRows.Count; i++)
            {
                TeamPlayerRow uiRow = TeamPlayerRows[i];
                
                if (i < teamStateSnapshot.Size())
                {
                    // Update row content
                    ScoreboardRowData rowData = teamStateSnapshot.GetRow(i);

                    if (rowData == null)
                    {
                        Debug.LogError("Row data is null!");
                        return;
                    }
                    
                    uiRow.OpenPanel();
                    uiRow.Set(rowData.Name, rowData.ClassId, teamStateSnapshot.Color, rowData.IsAlive());
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