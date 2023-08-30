using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPlayerBadge : GamePanel
    {
        public ClassList ClassList;
        public TextMeshProUGUI PlayerNamesText;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI DeathsText;
        public Image ClassIcon;

        private TeamState _teamState;

        public void Setup(ScoreboardRowData row)
        {
            if(row.Team != null)
                PlayerNamesText.color = row.Team.material.color;
            else
                PlayerNamesText.color = Color.gray;
            
            PlayerNamesText.text = row.Name;
            ScoreText.text = row.Kills.ToString();
            DeathsText.text = row.Deaths.ToString();
            ClassIcon.sprite = ClassList[row.ClassId].classIcon;

            GetComponent<Image>().enabled = row.IsLocalPlayer;
        }
    }
}