using System.Collections.Generic;
using TanksMP;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionPanel : GamePanel
    {
        public ClassSelectionSelector ClassSelectionSelector;
        public ClassSelectionTeamSelector ClassSelectionTeamSelector;

        public void ApplyChanges()
        {
            Player player = GameManager.GetInstance().localPlayer;
            player.SetClass(ClassSelectionSelector.SelectedClassDefinition());
            player.PreferredTeamIndex = ClassSelectionTeamSelector.SelectedTeamIndex();
            
            ClosePanel();
        }

        public void RespawnPlayer()
        {
            ApplyChanges();
            
            GameManager.GetInstance().RespawnLocalPlayer();
        }
    }
}