using System.Collections.Generic;
using System.Linq;
using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.Scoreboard;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MatchOverPanel : GamePanel
    {
        [Tooltip("List of locations to show the victory screen.  Indexes are paired with team indecies.  If the team is higher than the number of views it will default to 0.")]
        public List<MatchOverTeamView> TeamViews;
        public TextMeshProUGUI TeamWinnerText;

        public TextMeshProUGUI MatchVictoryGoldText;
        public GameObject PlacementGoldGO;
        public TextMeshProUGUI PlacementGoldNumericalText; // gold value
        public TextMeshProUGUI PlacementGoldText; // "First Place"
        
        private CanvasGroup _canvasGroup;

        private PlayerCurrencyRewarder _currencyRewarder;
        private ScoreboardCalculator _scoreboardCalculator;
        
        private static MatchOverPanel _instance;

        public static MatchOverPanel Get() => _instance;

        private void Start()
        {
            _instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;

            _currencyRewarder = new PlayerCurrencyRewarder();
            _scoreboardCalculator = new ScoreboardCalculator();
            
            gameObject.SetActive(false);
        }

        public void Inflate(string winningTeamName, int winningTeamIndex, Color winningTeamColor)
        {
            gameObject.SetActive(true);
            HUDPanel.Get().ClosePanel();
            _canvasGroup.alpha = 1;
            TeamWinnerText.text = $"{winningTeamName} Wins";
            TeamWinnerText.color = winningTeamColor;

            List<ScoreboardRowData> topPlayersIncludeOffline = _scoreboardCalculator.GetScores(true);
            
            ScoreboardRowData player1 = null;
            ScoreboardRowData player2 = null;
            ScoreboardRowData player3 = null;
            
            if(topPlayersIncludeOffline.Count > 0)
                player1 = topPlayersIncludeOffline[0];
            
            if(topPlayersIncludeOffline.Count > 1)
                player2 = topPlayersIncludeOffline[1];
            
            if(topPlayersIncludeOffline.Count > 2)
                player3 = topPlayersIncludeOffline[2];
            
            ChooseTeamView(winningTeamIndex).Activate(player1, player2, player3);
            
            List<ScoreboardRowData> topPlayersOnlineOnly = _scoreboardCalculator.GetScores(false);
            RewardCurrencies(topPlayersOnlineOnly);
        }

        private void RewardCurrencies(List<ScoreboardRowData> topPlayers)
        {
            // Reward currency and update currency reward text
            MatchVictoryGoldText.text = _currencyRewarder.RewardForMatchCompleted().ToString();

            int minPlayersToShow = 4;
            
            if (topPlayers.Count >= minPlayersToShow)
            {
                if (topPlayers[0].Player is { IsLocal: true })
                {
                    // player is in first
                    Debug.Log("Player is in first!");
                    int topScoreReward = _currencyRewarder.RewardForFirstPlace();
                    ShowPlacementBonus(topScoreReward, "First Place!");
                    return;
                }
                
                if (topPlayers[1].Player is {IsLocal : true})
                {
                    // player is in second
                    Debug.Log("Player is in second!");
                    int topScoreReward = _currencyRewarder.RewardForSecondPlace();
                    ShowPlacementBonus(topScoreReward, "Second Place!");
                    return;
                }
                
                if (topPlayers[2].Player is {IsLocal : true})
                {
                    // player is in third
                    Debug.Log("Player is in third!");
                    int topScoreReward = _currencyRewarder.RewardForThirdPlace();
                    ShowPlacementBonus(topScoreReward, "Third Place!");
                    return;
                }
            }

            ShowPlacementBonus(0,"");
        }

        private MatchOverTeamView ChooseTeamView(int teamIndex)
        {
            if (teamIndex >= TeamViews.Count)
                return TeamViews[0];

            return TeamViews[teamIndex];
        }

        private void ShowPlacementBonus(int gold, string text)
        {
            if (gold == 0)
            {
                PlacementGoldGO.SetActive(false);
                return;
            }
            
            PlacementGoldGO.SetActive(true);
            PlacementGoldNumericalText.text = gold.ToString();
            PlacementGoldText.text = text;
        }
    }
}