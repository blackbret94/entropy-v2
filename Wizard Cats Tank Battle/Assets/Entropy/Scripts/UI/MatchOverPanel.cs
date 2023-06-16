using System.Collections.Generic;
using System.Linq;
using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MatchOverPanel : GamePanel
    {
        [Tooltip("List of locations to show the victory screen.  Indexes are paired with team indecies.  If the team is higher than the number of views it will default to 0.")]
        public List<MatchOverTeamView> TeamViews;
        public TextMeshProUGUI TeamWinnerText;
        private CanvasGroup _canvasGroup;

        private static MatchOverPanel _instance;

        public static MatchOverPanel Get() => _instance;

        private void Start()
        {
            _instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        public void Inflate(string winningTeamName, int winningTeamIndex, Color winningTeamColor)
        {
            gameObject.SetActive(true);
            HUDPanel.Get().ClosePanel();
            _canvasGroup.alpha = 1;
            TeamWinnerText.text = $"{winningTeamName} Wins";
            TeamWinnerText.color = winningTeamColor;

            List<ScoreboardRowData> topThreePlayers = GetTopThreePlayers();

            ScoreboardRowData player1 = null;
            ScoreboardRowData player2 = null;
            ScoreboardRowData player3 = null;
            
            if(topThreePlayers.Count > 0)
                player1 = topThreePlayers[0];
            
            if(topThreePlayers.Count > 1)
                player2 = topThreePlayers[1];
            
            if(topThreePlayers.Count > 2)
                player3 = topThreePlayers[2];
            
            ChooseTeamView(winningTeamIndex).Activate(player1, player2, player3);
        }

        private MatchOverTeamView ChooseTeamView(int teamIndex)
        {
            if (teamIndex >= TeamViews.Count)
                return TeamViews[0];

            return TeamViews[teamIndex];
        }

        private List<ScoreboardRowData> GetTopThreePlayers()
        {
            int numberOfPlayersToReturn = 3;
            
            List<ScoreboardRowData> rows = new List<ScoreboardRowData>();
            List<TeamState> teamStates = TeamUtility.GetTeamStates();

            foreach (TeamState teamState in teamStates)
            {
                rows = rows.Concat(teamState.PlayerRows).ToList();
            }

            return rows.OrderByDescending(row => row.Kills).ThenBy(row => row.Kills).Take(numberOfPlayersToReturn).ToList();
        }
    }
}