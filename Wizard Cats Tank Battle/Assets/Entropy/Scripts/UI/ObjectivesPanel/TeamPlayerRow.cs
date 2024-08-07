using Entropy.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ObjectivesPanel
{
    public class TeamPlayerRow : GamePanel
    {
        public ClassList ClassList;
        
        public TextMeshProUGUI PlayerNameText;
        public Color PlayerIsDeadColor = Color.gray;
        public Image ClassIcon;


        public void Set(string playerName, int classId, Color color, bool isAlive)
        {
            
            // Update text
            PlayerNameText.text = playerName;
            PlayerNameText.color = isAlive ? color : PlayerIsDeadColor;
            
            RefreshContent();

            // set class icon
            ClassDefinition classDefinition = ClassList[classId];

            if(classDefinition != null)
                ClassIcon.sprite = classDefinition.classIcon;
        }

        public void RefreshContent()
        {
            
        }
    }
}