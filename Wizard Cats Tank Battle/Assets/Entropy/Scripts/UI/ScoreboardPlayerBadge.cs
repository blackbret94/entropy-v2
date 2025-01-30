using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class ScoreboardPlayerBadge : GamePanel
    {
        [FormerlySerializedAs("ClassList")] public ClassDirectory classDirectory;
        public TextMeshProUGUI PlayerNamesText;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI DeathsText;
        public Image ClassIcon;

        private TeamStateSnapshot _teamStateSnapshot;

        public void Setup(ScoreboardRowData row)
        {
            if(row.TeamInstance != null)
                PlayerNamesText.color = row.TeamInstance.teamDefinition.Material.color;
            else
                PlayerNamesText.color = Color.gray;
            
            PlayerNamesText.text = row.Name;
            ScoreText.text = row.Kills.ToString();
            DeathsText.text = row.Deaths.ToString();
            ClassIcon.sprite = classDirectory[row.ClassId].classIcon;

            GetComponent<Image>().enabled = row.IsLocalPlayer;
        }
    }
}