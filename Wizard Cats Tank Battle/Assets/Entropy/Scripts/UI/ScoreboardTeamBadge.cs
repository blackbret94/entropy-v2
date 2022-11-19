using TMPro;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardTeamBadge : GamePanel
    {
        public TextMeshProUGUI PlayerNamesText;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI DeathsText;
        
        private TeamState _teamState;

        public void Setup(ScoreboardRowData row)
        {
            PlayerNamesText.color = row.Team.material.color;
            PlayerNamesText.text = row.Name;
            ScoreText.text = row.Kills.ToString();
            DeathsText.text = row.Deaths.ToString();
        }
    }
}