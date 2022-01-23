using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardTeamBadge : GamePanel
    {
        public TextMeshProUGUI TeamNameText;
        public Image TeamIcon;
        public TextMeshProUGUI PlayerNamesText;
        public TextMeshProUGUI ScoreText;
        
        private TeamState _teamState;

        public void Setup(TeamState teamState)
        {
            _teamState = teamState;
            SetNameText();
            SetTeamIcon();
            SetPlayerNamesText();
            SetScoreText();
        }

        private void SetNameText()
        {
            TeamNameText.text = _teamState.Name;
        }

        private void SetTeamIcon()
        {
            TeamIcon.color = _teamState.Material.color;
        }

        private void SetPlayerNamesText()
        {
            string playersText = "";
            List<string> names = _teamState.PlayerNames;

            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    playersText += ", ";

                playersText += names[i];
            }

            PlayerNamesText.text = playersText;
        }

        private void SetScoreText()
        {
            ScoreText.text = "Score: " + _teamState.Score;
        }
    }
}